using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;

namespace frida_windows_package_manager.Models
{
    public class Package
    {
        public PackageManifest Manifest { get; private set; }
        public string ScriptContent { get; private set; }
        public string UserLibraryPath { get; private set; }
        public string PackagePath { get; private set; }

        public string MergedScriptPath { get { return Path.Combine(RuntimeRoot, "merged.js"); } }
        public string PackageRoot { get { return Path.GetDirectoryName(PackagePath); } }
        public string RuntimeRoot { get { return Path.Combine(PackageRoot, "Runtime", Process.GetCurrentProcess().Id.ToString()); } }
        public bool IsExe { get { return !string.IsNullOrWhiteSpace(Manifest.Exe); } }
        public string ExtendsFriendlyName { get { return IsExe ? Manifest.Exe : Manifest.AppxPackageFamilyName; } }

        void BuildUserLibrary()
        {
            UserLibraryPath = "";
            if (!string.IsNullOrWhiteSpace(Manifest.csc_commandline))
            {
                var dir = Directory.CreateDirectory(RuntimeRoot);
                Environment.CurrentDirectory = dir.FullName;

                foreach (var file in Manifest.cs_Files)
                {
                    File.Copy(Path.Combine(PackageRoot, file), Path.Combine(RuntimeRoot, file), true);
                }

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "csc.exe");;
                p.StartInfo.Arguments = Manifest.csc_commandline;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                if (output.Trim().Length > 0)
                {
                    MessageBox.Show("Failed to compile " + PackagePath + ":\n" + output);
                    Environment.Exit(0);
                    return;
                }

                UserLibraryPath = dir.GetFiles().FirstOrDefault(f => f.Extension == ".dll").FullName;
            }
        }

        [DataContract]
        class FileEntry
        {
            [DataMember]public string Name;
            [DataMember]public string Content;
        }

        string GetEncodedFiles()
        {
            var jsc = new DataContractJsonSerializer(typeof(FileEntry[]));
            using (var ms = new MemoryStream())
            {
                var files = Manifest.js_Files.Skip(1).Select(fileName => new FileEntry { Name = fileName, Content = File.ReadAllText(Path.Combine(PackageRoot, fileName)) }).ToArray();
                jsc.WriteObject(ms, files);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }

        string GetHostInfo()
        {
            var hostInfo = new HostInfo
            {
                PackageRoot = PackageRoot,
                RuntimeRoot = RuntimeRoot,
                UserLibraryPath = UserLibraryPath.Replace("\\", "/"),
                ProxyLibraryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClrBridge.dll").Replace("\\", "/"),
            };
            var jsc = new DataContractJsonSerializer(typeof(HostInfo));
            using (var ms = new MemoryStream())
            {
                jsc.WriteObject(ms, hostInfo);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }

        public void Compile()
        {
            BuildUserLibrary();

            ScriptContent = "var HostInfo = " + GetHostInfo() + ";" + Environment.NewLine +
                @"function require(fileName) { 
                    var ScriptFiles = " + GetEncodedFiles() + "; " + Environment.NewLine + @"
                    for (var i = 0; i < ScriptFiles.length; ++i) {
                        if (ScriptFiles[i].Name == fileName) {
                            var exports = new function(){};
                            eval(ScriptFiles[i].Content);
                            return exports;
                        }
                    }
                    throw Error(""Can't find file "" + fileName);
                }" + Environment.NewLine +
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    @"frida_windows_package_manager.js.platform.js")).ReadToEnd() + Environment.NewLine +
                    File.ReadAllText(Path.Combine(PackageRoot, Manifest.js_Files.Take(1).First()));

            try
            {
                Directory.CreateDirectory(RuntimeRoot);
                File.WriteAllText(MergedScriptPath, ScriptContent);
            }
            catch { }
        }

        public static Package Load(string filePath)
        {
            var jsc = new DataContractJsonSerializer(typeof(PackageManifest));
            return new Package
            {
                PackagePath = filePath,
                Manifest = (PackageManifest)jsc.ReadObject(File.OpenRead(filePath))
            };
        }

        internal static bool CanLoad(string packagePath)
        {
            try
            {
                Load(packagePath);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return false;
            }
        }

        public void RuntimeCleanup()
        {
            try
            {
                Directory.Delete(RuntimeRoot);
            }
            catch { }
        }
    }
}
