using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
    /// <summary>
    /// Static entry point for retrieving Workbench services.
    /// </summary>
    public static class WorkbenchServices
    {
        /// <summary>
        /// Equivalent to <code>SD.Workbench.ActiveViewContent.GetService(type)</code>,
        /// but does not throw a NullReferenceException when ActiveViewContent is null.
        /// (instead, null is returned).
        /// </summary>
        public static object GetActiveViewContentService(Type type)
        {
            var workbench = Altaxo.Current.GetRequiredService<IWorkbench>();
            if (workbench != null)
            {
                var activeViewContent = workbench.ActiveViewContent;
                if (activeViewContent != null)
                {
                    return activeViewContent.GetService(type);
                }
            }
            return null;
        }

        /// <inheritdoc see="IWorkbench"/>
        public static IWorkbench Workbench
        {
            get { return Altaxo.Current.GetRequiredService<IWorkbench>(); }
        }

        public static System.Windows.Window MainWindow
        {
            get { return (System.Windows.Window)Altaxo.Current.GetRequiredService<IWorkbench>().ViewObject; }
        }

        /// <summary>
        /// Gets the <see cref="IDispatcherMessageLoopWpf"/> representing the main UI thread.
        /// </summary>
        public static IDispatcherMessageLoopWpf MainThread
        {
            get { return Altaxo.Current.GetRequiredService<IDispatcherMessageLoopWpf>(); }
        }

        /// <inheritdoc see="IWinFormsService"/>
        public static IWinFormsService WinForms
        {
            get { return Altaxo.Current.GetRequiredService<IWinFormsService>(); }
        }

        /// <inheritdoc see="IStatusBarService"/>
        public static IStatusBarService StatusBar
        {
            get { return Altaxo.Current.GetRequiredService<IStatusBarService>(); }
        }

        public static event Action WorkbenchCreated;

        public static void OnWorkbenchCreated()
        {
            WorkbenchCreated?.Invoke();
        }
    }
}