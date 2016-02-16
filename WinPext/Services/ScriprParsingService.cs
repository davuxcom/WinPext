using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace frida_windows_package_manager.Models
{
    public class ScriptResolver
    {
        readonly string sysPrefix = "windows/";
        readonly string[] system_resources = new string[] {
            "windows/platform",
        };

        Package _package;

        public ScriptResolver(Package package)
        {
            _package = package;
        }

        public string GetMerged()
        {
            string script = "";
            foreach (var file in ResolveDependencies())
            {
                script += GetScriptContent(file) + Environment.NewLine;
            }
            return script;
        }

        public IEnumerable<string> ResolveDependencies()
        {
            // resolve all
            List<string> addedFiles = new List<string>();
            foreach (var file in _package.Manifest.js_Files.Take(1))
            {
                var imports = GetImportsForFile(file);
                addedFiles.AddRange(imports);
                addedFiles.Add(file);
            }

            // clear duplicates from the bottom up.
            for (int i = addedFiles.Count - 1; i > 0; i--)
            {
                if (addedFiles.Take(i - 1).Contains(addedFiles[i]))
                {
                    addedFiles[i] = null;
                }
            }

            return addedFiles.Where(x => x != null);
        }

        public IEnumerable<string> GetImportsForFile(string scriptName)
        {
            var ret = new List<string>();

            var scriptContent = GetScriptContent(scriptName);
            const string importPrefix = "// @import ";
            foreach (var line in scriptContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.ToLower().StartsWith(importPrefix.ToLower()))
                {
                    var newFile = line.Remove(0, importPrefix.Length).ToLower();
                    // Recurse to resolve imports completely
                    ret.AddRange(GetImportsForFile(newFile));
                    ret.Add(newFile);
                }
            }
            return ret;
        }

        public string GetScriptContent(string scriptName)
        {
            scriptName = scriptName.ToLower();
            if (system_resources.Contains(scriptName))
            {
                // var list = Assembly.GetExecutingAssembly().GetManifestResourceNames(); // debugging
                return new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    @"frida_windows_package_manager.js." + scriptName.Remove(0,sysPrefix.Length) + ".js")).ReadToEnd();
            }
            else
            {
                return File.ReadAllText(Path.Combine(_package.PackageRoot, scriptName));
            }
        }
    }
}
