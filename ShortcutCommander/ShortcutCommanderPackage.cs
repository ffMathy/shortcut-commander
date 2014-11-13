using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using EnvDTE;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

using Debugger = System.Diagnostics.Debugger;
using System.Windows;

namespace Techmatic.ShortcutCommander
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidShortcutCommanderPkgString)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    public sealed class ShortcutCommanderPackage : Package
    {
        [ContextStatic]
        private static CommandEvents commandEvents;

        private static HotkeyWindow window;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ShortcutCommanderPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            var m_objDTE = (DTE)GetService(typeof(DTE));
            commandEvents = m_objDTE.Events.CommandEvents;

            commandEvents.BeforeExecute += delegate (string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
            {
                var objCommand = m_objDTE.Commands.Item(Guid, ID);
                if (objCommand != null)
                {
                    var bindings = objCommand.Bindings as object[];
                    if (bindings != null && bindings.Any())
                    {
                        var shortcuts = GetBindings(bindings);
                        if (shortcuts.Any())
                        {

                            lock (typeof(ShortcutCommanderPackage))
                            {

                                var shortcutActive = false;
                                foreach (var shortcut in shortcuts)
                                {
                                    var sequenceActive = false;

                                    var shortcutSequences = shortcut.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var shortcutSequence in shortcutSequences)
                                    {

                                        var keyActive = true;
                                        var shortcutKeys = shortcutSequence.Split('+');

                                        foreach (var shortcutKey in shortcutKeys)
                                        {
                                            Key key;
                                            switch (shortcutKey)
                                            {
                                                case "Ctrl":
                                                    key = Key.LeftCtrl;
                                                    break;

                                                case "Alt":
                                                    key = Key.LeftAlt;
                                                    break;

                                                case "Shift":
                                                    key = Key.LeftShift;
                                                    break;

                                                case "Bkspce":
                                                    key = Key.Back;
                                                    break;

                                                case "Del":
                                                    key = Key.Delete;
                                                    break;

                                                case "Ins":
                                                    key = Key.Insert;
                                                    break;

                                                case "PgDn":
                                                    key = Key.PageDown;
                                                    break;

                                                case "PgUp":
                                                    key = Key.PageUp;
                                                    break;

                                                case "Down Arrow":
                                                    key = Key.Down;
                                                    break;

                                                case "Up Arrow":
                                                    key = Key.Up;
                                                    break;

                                                case "Left Arrow":
                                                    key = Key.Left;
                                                    break;

                                                case "Right Arrow":
                                                    key = Key.Right;
                                                    break;

                                                default:
                                                    if (!Enum.TryParse(shortcutKey, out key))
                                                    {
                                                        if (Debugger.IsAttached)
                                                        {
                                                            Debugger.Break();
                                                        }
                                                        continue;
                                                    }
                                                    break;
                                            }

                                            if (!Keyboard.IsKeyDown(key))
                                            {
                                                keyActive = false;
                                                break;
                                            }

                                        }

                                        if (keyActive)
                                        {
                                            sequenceActive = true;
                                            break;
                                        }
                                    }

                                    if (sequenceActive)
                                    {
                                        shortcutActive = true;
                                        break;
                                    }
                                }

                                if (!shortcutActive)
                                {

                                    if (window != null)
                                    {
                                        window.Close();
                                        window = null;
                                    }

                                    window = new HotkeyWindow();

                                    var contentBlock = window.ContentBlock;
                                    var contentInlines = contentBlock.Inlines;

                                    contentBlock.Text = null;
                                    contentInlines.Clear();

                                    var space = " " + Convert.ToChar(160) + " ";
                                    for (var i = 0; i < shortcuts.Length; i++)
                                    {
                                        if (i > 0)
                                        {
                                            contentInlines.Add(new Run(space + " or " + space));
                                        }
                                        contentInlines.Add(new Run(shortcuts[i]) { Foreground = Brushes.White });
                                    }

                                    window.Show();

                                }

                            }

                        }
                    }
                }
            };

        }

        #endregion

        private static string[] GetBindings(IEnumerable<object> bindings)
        {
            var result = bindings.Select(binding => binding.ToString().IndexOf("::") >= 0
                ? binding.ToString().Substring(binding.ToString().IndexOf("::") + 2)
                : binding.ToString()).Distinct();

            return result.ToArray();
        }

    }
}
