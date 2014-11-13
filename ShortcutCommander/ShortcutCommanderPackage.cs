using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using EnvDTE;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows.Media;

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

                            lock(typeof(ShortcutCommanderPackage))
                            {

                                foreach (var shortcut in shortcuts)
                                {
                                    var shortcutActive = true;
                                    var shortcutKeys = shortcut.Split('+');

                                    foreach (var shortcutKey in shortcutKeys)
                                    {
                                        Keys keys;
                                        switch (shortcutKey)
                                        {
                                            case "Bkspce":
                                                keys = Keys.Back;
                                                break;

                                            case "Del":
                                                keys = Keys.Delete;
                                                break;

                                            case "Ins":
                                                keys = Keys.Insert;
                                                break;

                                            case "PgDn":
                                                keys = Keys.PageDown;
                                                break;

                                            case "PgUp":
                                                keys = Keys.PageUp;
                                                break;

                                            default:
                                                if (!Enum.TryParse(shortcut, out keys))
                                                {
                                                    continue;
                                                }
                                                break;
                                        }
                                    }
                                }

                                if (window != null)
                                {
                                    window.Close();
                                    window = null;
                                }

                                window = new HotkeyWindow();
                                window.NameBlock.Text = objCommand.Name;

                                var contentBlock = window.ContentBlock;
                                var contentInlines = contentBlock.Inlines;

                                contentBlock.Text = null;
                                contentInlines.Clear();

                                var space = " " + Convert.ToChar(160) + " ";
                                for (var i = 0; i < shortcuts.Length; i++)
                                {
                                    if (i == shortcuts.Length - 1 && i > 0)
                                    {
                                        contentInlines.Add(new Run("or " + space));
                                    }
                                    contentInlines.Add(new Run(shortcuts[i]) { Foreground = Brushes.White });
                                    if (i > 0 && i <= shortcuts.Length - 2)
                                    {
                                        contentInlines.Add(new Run("," + space));
                                    } else
                                    {
                                        contentInlines.Add(new Run(space));
                                    }
                                }

                                window.Show();

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
