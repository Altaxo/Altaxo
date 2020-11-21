#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Altaxo.Main.Services;

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
      Loaded += EhLoaded;
      DataContextChanged += EhDataContextChanged;
    }

    private void EhDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      {
        if (e.OldValue is AltaxoWorkbench wb)
        {
          wb.PropertyChanged -= EhWorkbenchPropertyChanged;
        }
      }
      {
        if (e.NewValue is AltaxoWorkbench wb)
        {
          wb.PropertyChanged += EhWorkbenchPropertyChanged;
        }
      }
    }

    private void EhWorkbenchPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(AltaxoWorkbench.IsLayoutSerializationRequired))
      {
        DockingLayoutStringObserver.SerializeLayoutAndUpdateLayoutString(_dockManager);
      }
    }

    private void EhLoaded(object sender, RoutedEventArgs e)
    {
      System.Windows.Input.CommandManager.RequerySuggested += EhCommandManager_RequerySuggested;

      // set a timer with low priority that is called __after__ all bindings have been processed.
      var dispatcherTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(1), DispatcherPriority.Background, EhAfterLoaded, Dispatcher);
    }

    /// <summary>
    /// Is called after this workbench is loaded and all bindings (this is important!) have been processed.
    /// We use this to fix a problem when an project is loaded during startup, but
    /// ActiveViewContent and ActiveContent is still null. The fixup determines the selected document and
    /// sets the two properties accordingly.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void EhAfterLoaded(object? sender, EventArgs e)
    {
      ((DispatcherTimer?)sender)?.Stop();

      if (DataContext is AltaxoWorkbench wb)
      {
        wb.FixViewContentIsNullWhenThereAreDocumentsAvailable();
      }
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
        if (data is not null && data.GetDataPresent(DataFormats.FileDrop))
        {
          string[] files = (string[])data.GetData(DataFormats.FileDrop);
          if (files is not null)
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
        if (!e.Handled && e.Data is not null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
          e.Handled = true;
          string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
          if (files is null)
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
      var alreadyProcessedFiles = new HashSet<string>();

      // handle project files (hopefully, it is only one), with the highest priority
      foreach (string file in files)
      {
        var fileName = FileName.Create(file);
        if (!alreadyProcessedFiles.Contains(file) && System.IO.File.Exists(file))
        {
          if (Current.IProjectService.IsProjectFileExtension(System.IO.Path.GetExtension(file)))
          {
            Current.IProjectService.OpenProject(new FileName(file), showUserInteraction: true);
            alreadyProcessedFiles.Add(file);
          }
        }
      }

      // now handle other files that maybe are part of the project
      foreach (string file in files)
      {
        if (!alreadyProcessedFiles.Contains(file) && System.IO.File.Exists(file))
        {
          if (Current.IProjectService.TryOpenProjectItemFile(new FileName(file), forceTrialRegardlessOfExtension: false))
          {
            alreadyProcessedFiles.Add(file);
          }
        }
      }

      // and finally, we maybe are able to open the file independent of the project
      foreach (string file in files)
      {
        if (!alreadyProcessedFiles.Contains(file) && System.IO.File.Exists(file))
        {
          var fileService = Altaxo.Current.GetService<IFileService>();
          if (fileService is not null)
          {
            alreadyProcessedFiles.Add(file);
            fileService.OpenFile(FileName.Create(file));
          }
        }
      }
    }

    #endregion Drag/Drop handling

    #region Status of Menu and Toolbar

    private void EhCommandManager_RequerySuggested(object? sender, EventArgs e)
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

      if (_dockManager.DocumentContextMenu?.ItemsSource is { } documentContextMenu)
      {
        AddInItems.MenuService.UpdateStatus(documentContextMenu);
      }
    }

    #endregion Status of Menu and Toolbar


  }
}
