using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;

namespace Altaxo.Gui.Workbench
{
    /// <summary>
    /// Main window of the workbench and therefore of the application.
    /// </summary>
    public partial class MainWorkbenchWindow : FullScreenEnabledWindow
    {
        public MainWorkbenchWindow()
        {
            InitializeComponent();
            this.Loaded += EhLoaded;
        }

        private void EhLoaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Input.CommandManager.RequerySuggested += EhCommandManager_RequerySuggested;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            WorkbenchStateObserver.UpdateWorkbenchStateFromMainWindow(this);
            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
            {
                DockingLayoutStringObserver.SerializeLayoutAndUpdateLayoutString(_dockManager);
                WorkbenchStateObserver.UpdateWorkbenchStateFromMainWindow(this);
                var shutdownService = Current.GetService<IShutdownService>() ?? new ShutdownService();
                shutdownService.OnClosing(e);
            }
        }

        #region Drag/Drop handling

        protected override void OnDragEnter(DragEventArgs e)
        {
            try
            {
                base.OnDragEnter(e);
                if (!e.Handled)
                {
                    e.Effects = GetEffect(e.Data);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Current.MessageService.ShowException(ex);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            try
            {
                base.OnDragOver(e);
                if (!e.Handled)
                {
                    e.Effects = GetEffect(e.Data);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Current.MessageService.ShowException(ex);
            }
        }

        private DragDropEffects GetEffect(IDataObject data)
        {
            try
            {
                if (data != null && data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])data.GetData(DataFormats.FileDrop);
                    if (files != null)
                    {
                        foreach (string file in files)
                        {
                            if (System.IO.File.Exists(file))
                            {
                                return DragDropEffects.Link;
                            }
                        }
                    }
                }
            }
            catch (COMException)
            {
                // Ignore errors getting the data (e.g. happens when dragging attachments out of Thunderbird)
            }
            return DragDropEffects.None;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            try
            {
                base.OnDrop(e);
                if (!e.Handled && e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Handled = true;
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files == null)
                        return;
                    // Handle opening the files outside the drop event, so that the drag source doesn't think
                    // the operation is still in progress while we're showing a "file cannot be opened" error message.
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<string[]>(HandleDrop), files);
                }
            }
            catch (Exception ex)
            {
                Current.MessageService.ShowException(ex);
            }
        }

        private void HandleDrop(string[] files)
        {
            foreach (string file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    var fileName = FileName.Create(file);

                    if (Current.IProjectService.IsProjectFileExtension(System.IO.Path.GetExtension(file)))
                    {
                        Current.IProjectService.OpenProject(file, false);
                    }
                    else
                    {
                        Altaxo.Current.GetRequiredService<IFileService>().OpenFile(fileName);
                    }
                }
            }
        }

        #endregion Drag/Drop handling

        #region Status of Menu and Toolbar

        private void EhCommandManager_RequerySuggested(object sender, EventArgs e)
        {
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            AddInItems.MenuService.UpdateStatus(mainMenu.ItemsSource);
            foreach (var tb in _guiToolBarTray.ToolBars)
            {
                AddInItems.ToolBarService.UpdateStatus(tb.ItemsSource);
            }

            AddInItems.MenuService.UpdateStatus(_dockManager.DocumentContextMenu?.ItemsSource);
        }

        #endregion Status of Menu and Toolbar
    }
}