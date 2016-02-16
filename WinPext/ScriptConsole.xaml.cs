using Frida;
using frida_windows_package_manager.Models;
using frida_windows_package_manager.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace frida_windows_package_manager
{
    public partial class ScriptConsole : Window
    {
        public ObservableCollection<ScriptMessage> Messages { get; private set; }
        public string TitleText { get; private set; }
        int _pid;
        Script _script;
        Session _session;
        Package _package;

        class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        }

        public ScriptConsole(string exeName, string[] args, Session session, Package package, Script script)
        {
            _pid = (int)session.Pid;
            _script = script;
            _session = session;
            _package = package;

            TitleText = string.Format("{0}:{1} Debug", System.IO.Path.GetFileNameWithoutExtension(exeName),_pid);
            script.Message += Script_Message;
            session.Detached += Session_Detached;

            InitializeComponent();
            DataContext = this;

            Messages = new ObservableCollection<ScriptMessage>();
            Messages.Add(ScriptMessage.CreateInfo(string.Format("Attached to {0}:{1} {2}",
                session.Pid,
                exeName,
                string.Join(" ", args))));
            Messages.Add(ScriptMessage.CreateInfo("Loaded " + package.PackagePath));
        }

        private void Session_Detached(object sender, EventArgs e)
        {
            Messages.Add(ScriptMessage.CreateError("Session detatched (target process quit)."));
        }

        private void Script_Message(object sender, ScriptMessageEventArgs e)
        {
            Messages.Add(new ScriptMessage(e.Message));
            ScrollViewDown();
        }

        private void ScrollViewDown()
        {
            // Scroll to last message
            MessageList.ScrollIntoView(MessageList.Items[MessageList.Items.Count - 1]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // We only want to quit when the child process detaches and tears down the process explicitly.
            e.Cancel = true;
            Hide();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Messages.Clear();
        }

        private void Focus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NativeMethods.SetForegroundWindow(System.Diagnostics.Process.GetProcessById(_pid).MainWindowHandle);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void Detach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _script.Unload();
                Hide();
                // process quit will tear us down
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", "\"" + _package.MergedScriptPath + "\"");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _script.Unload();
                _package.Compile();
                _script = _session.CreateScript("merged", _package.ScriptContent);
                _script.Message += Script_Message;
                _script.Load();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Messages.Add(ScriptMessage.CreateError(ex.Message));
                ScrollViewDown();
            }
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.GetProcessById(_pid).Kill();
                // process quit will tear us down
            }
            catch (Exception ex)
            {
                // child isn't alive, kill ourselves.
                Environment.Exit(0);
                Trace.WriteLine(ex.Message);
            }
            Hide();
        }

        private void MessageList_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+C - Copy
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Clipboard.SetText(string.Join("\r\n", MessageList.SelectedItems.OfType<ScriptMessage>().Select(m => m.Content)));
                e.Handled = true;
            }
        }

        private void DebugEnable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _session.EnableDebugger();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Messages.Add(ScriptMessage.CreateError(ex.Message));
                ScrollViewDown();
            }
            Debugger_Enable.IsChecked = false;
            Debugger_Disable.IsChecked = true;
        }

        private void DebugDisable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _session.DisableDebugger();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Messages.Add(ScriptMessage.CreateError(ex.Message));
                ScrollViewDown();
            }
            Debugger_Enable.IsChecked = true;
            Debugger_Disable.IsChecked = false;
        }
    }
}
