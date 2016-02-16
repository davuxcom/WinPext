using Frida;
using frida_windows_package_manager.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace frida_windows_package_manager
{
    public class FridaService
    {
		private static readonly Device _local;

		static FridaService()
		{
			_local = new DeviceManager(App.Current.Dispatcher).EnumerateDevices().First(dev => dev.Type == DeviceType.Local);
		}

		public static void RunProcessWithPackage(string fileName, string[] args, bool attachConsole, Package package)
		{
			var pid = StartProcessWithFrida(fileName, args);
            AttachToProcessWithPackage(pid, fileName, attachConsole, package);
		}

		public static void AttachToProcessWithPackage(uint pid, string fileName, bool attachConsole, Package package)
		{
			Trace.WriteLine("AttachToProcessWithPackage Creating session for " + pid);
			var session = CreateSession(pid, package);

            if (!attachConsole)
            {
                session.Detached += (_, __) => Environment.Exit(0);
            }

			Trace.WriteLine("AttachToProcessWithPackage LoadPackage " + pid);
			var script = LoadPackage(session, package);

            if (package.IsExe)
            {
                Trace.WriteLine("AttachToProcessWithPackage Resuming " + pid);
                _local.Resume(pid);
            }

			if (attachConsole)
			{
				var console = new ScriptConsole(package.IsExe ? fileName : package.Manifest.AppxPackageFamilyName, new string[0], session, package, script);
				console.Show();
			}
		}

		private static IEnumerable<string> GetEnvironmentVariables()
        {
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                yield return string.Format("{0}={1}", entry.Key, entry.Value);
            }
            yield break;
        }

		private static Script LoadPackage(Session session, Package package)
		{
            package.Compile();

            Script script = null;
            try
            {
                script = session.CreateScript("merged", package.ScriptContent);

                script.Message += (_, e) => Trace.WriteLine(e.Message);

                script.Load();
            }
            catch (Exception ex)
            {
                 MessageBox.Show("Failed to load script: \n" + ex.Message, package.MergedScriptPath);
            }
			return script;
		}

		private static Session CreateSession(uint pid, Package package)
		{
			var session = _local.Attach(pid);

            // session.DisableJit();

			return session;
		}

		private static uint StartProcessWithFrida(string fileName, string[] args)
		{
			var argsWithProcess = args.ToList();
			argsWithProcess.Insert(0, fileName);

			var pid = _local.Spawn(fileName, argsWithProcess.ToArray(), GetEnvironmentVariables().ToArray());
            Trace.WriteLine("StartProcessWithFrida Created " + pid);

			return pid;
		}
    }
}
