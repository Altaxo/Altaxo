#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Drawing.Imaging;
using System.IO;
using Altaxo.Collections;
using Altaxo.Gui;
using Altaxo.Gui.Graph.Graph3D.Common;
using Altaxo.Gui.Graph.Graph3D.Viewing;
using Altaxo.Gui.Workbench;

namespace Altaxo.Graph.Graph3D.Commands
{
  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractGraph3DControllerCommand : SimpleCommand
  {
    /// <summary>
    /// Determines the currently active worksheet and issues the command to that worksheet by calling
    /// Run with the worksheet as a parameter.
    /// </summary>
    public override void Execute(object? parameter)
    {
      var activeViewContent = parameter as IViewContent ?? Current.Workbench.ActiveViewContent;
      if (activeViewContent is Graph3DController ctrl)
        Run(ctrl);
    }

    /// <summary>
    /// Override this function for adding own worksheet commands. You will get
    /// the worksheet controller in the parameter.
    /// </summary>
    /// <param name="ctrl">The worksheet controller this command is applied to.</param>
    public abstract void Run(Graph3DController ctrl);
  }

  #region Arrange commands

  /// <summary>
  /// Groups selected graph objects.
  /// </summary>
  public class GroupSelectedObjects : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.GroupSelectedObjects();
    }
  }

  /// <summary>
  /// Ungroups selected graph objects.
  /// </summary>
  public class UngroupSelectedObjects : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.UngroupSelectedObjects();
    }
  }

  /// <summary>
  /// Aligns selected objects to the top.
  /// </summary>
  public class ArrangeTop : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeTopToTop();
    }
  }

  /// <summary>
  /// Aligns selected objects to the bottom.
  /// </summary>
  public class ArrangeBottom : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeBottomToBottom();
    }
  }

  /// <summary>
  /// Aligns object tops to object bottoms.
  /// </summary>
  public class ArrangeTopToBottom : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeTopToBottom();
    }
  }

  /// <summary>
  /// Aligns object bottoms to object tops.
  /// </summary>
  public class ArrangeBottomToTop : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeBottomToTop();
    }
  }

  /// <summary>
  /// Aligns selected objects to the left.
  /// </summary>
  public class ArrangeLeft : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeLeftToLeft();
    }
  }

  /// <summary>
  /// Aligns selected objects to the right.
  /// </summary>
  public class ArrangeRight : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeRightToRight();
    }
  }

  /// <summary>
  /// Aligns object left edges to right edges.
  /// </summary>
  public class ArrangeLeftToRight : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeLeftToRight();
    }
  }

  /// <summary>
  /// Aligns object right edges to left edges.
  /// </summary>
  public class ArrangeRightToLeft : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeRightToLeft();
    }
  }

  /// <summary>
  /// Distributes selected objects horizontally.
  /// </summary>
  public class ArrangeHorizontal : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeHorizontal();
    }
  }

  /// <summary>
  /// Distributes selected objects vertically.
  /// </summary>
  public class ArrangeVertical : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeVertical();
    }
  }

  /// <summary>
  /// Arranges selected objects in a horizontal table layout.
  /// </summary>
  public class ArrangeHorizontalTable : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeHorizontalTable();
    }
  }

  /// <summary>
  /// Arranges selected objects in a vertical table layout.
  /// </summary>
  public class ArrangeVerticalTable : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeVerticalTable();
    }
  }

  /// <summary>
  /// Sets selected objects to equal width.
  /// </summary>
  public class ArrangeSameHorizontalSize : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeSameHorizontalSize();
    }
  }

  /// <summary>
  /// Sets selected objects to equal height.
  /// </summary>
  public class ArrangeSameVerticalSize : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ArrangeSameVerticalSize();
    }
  }

  #endregion Arrange commands

  /// <summary>
  /// Sets front camera view.
  /// </summary>
  public class ViewFront : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewFront();
    }
  }

  /// <summary>
  /// Sets right camera view.
  /// </summary>
  public class ViewRight : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewRight();
    }
  }

  /// <summary>
  /// Sets back camera view.
  /// </summary>
  public class ViewBack : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewBack();
    }
  }

  /// <summary>
  /// Sets left camera view.
  /// </summary>
  public class ViewLeft : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewLeft();
    }
  }

  /// <summary>
  /// Sets top camera view.
  /// </summary>
  public class ViewTop : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewTop();
    }
  }

  /// <summary>
  /// Sets bottom camera view.
  /// </summary>
  public class ViewBottom : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewBottom();
    }
  }

  /// <summary>
  /// Sets standard isometric camera view.
  /// </summary>
  public class ViewIsometricStandard : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewIsometricStandard();
    }
  }

  /// <summary>
  /// Sets left-top isometric camera view.
  /// </summary>
  public class ViewIsometricLeftTop : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.ViewIsometricLeftTop();
    }
  }

  /// <summary>
  /// Opens options for copy-page settings.
  /// </summary>
  public class SetCopyPageOptions : SimpleCommand
  {
    public override void Execute(object? parameter)
    {
      object resultobj = Gdi.ClipboardRenderingOptions.CopyPageOptions;
      if (Current.Gui.ShowDialog(ref resultobj, "Set copy page options"))
      {
        Gdi.ClipboardRenderingOptions.CopyPageOptions = (Gdi.ClipboardRenderingOptions)resultobj;
      }
    }
  }

  /// <summary>
  /// Exports a 3D graph to an image file.
  /// </summary>
  public class Export3D : AbstractGraph3DControllerCommand
  {
    /// <summary>
    /// Cached export options for file export.
    /// </summary>
    private static Altaxo.Graph.Gdi.GraphExportOptions _graphExportOptionsToFile = new Graph.Gdi.GraphExportOptions();

    public override void Run(Graph3DController ctrl)
    {
      ShowFileExportSpecificDialog(ctrl.Doc);
    }

    /// <summary>
    /// Shows export options specific for file export.
    /// </summary>
    public static void ShowFileExportSpecificDialog(GraphDocument doc)
    {
      object resopt = _graphExportOptionsToFile;
      if (Current.Gui.ShowDialog(ref resopt, "Choose export options"))
      {
        _graphExportOptionsToFile = (Graph.Gdi.GraphExportOptions)resopt;
      }
      else
      {
        return;
      }
      ShowFileExportDialog(doc, _graphExportOptionsToFile);
    }

    /// <summary>
    /// Shows file export dialog and writes the exported image.
    /// </summary>
    public static void ShowFileExportDialog(GraphDocument doc, Altaxo.Graph.Gdi.GraphExportOptions graphExportOptions)
    {
      var saveOptions = new Altaxo.Gui.SaveFileOptions();
      var list = GetFileFilterString(graphExportOptions.ImageFormat);
      foreach (var entry in list)
        saveOptions.AddFilter(entry.Key, entry.Value);
      saveOptions.FilterIndex = 0;
      saveOptions.RestoreDirectory = true;

      if (Current.Gui.ShowSaveFileDialog(saveOptions))
      {
        using (Stream myStream = new FileStream(saveOptions.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
          if (DirectXVersionAvailability.IsDirectX12Available)
            new Gui.Graph.Graph3D.Common.D3D12BitmapExporter().ExportAsImageToStream(doc, graphExportOptions, myStream);
          else
            new Gui.Graph.Graph3D.Common.D3D11BitmapExporter().ExportAsImageToStream(doc, graphExportOptions, myStream);

          myStream.Close();
        } // end openfile ok
      } // end dlgresult ok
    }

    /// <summary>
    /// Gets the file filter list for the selected image format.
    /// </summary>
    private static IList<KeyValuePair<string, string>> GetFileFilterString(ImageFormat fmt)
    {
      var filter = new List<KeyValuePair<string, string>>();

      if (fmt == ImageFormat.Bmp)
        filter.Add(new KeyValuePair<string, string>("*.bmp", "Bitmap files (*.bmp)"));
      else if (ImageFormat.Gif == fmt)
        filter.Add(new KeyValuePair<string, string>("*.gif", "Gif files (*.gif)"));
      else if (ImageFormat.Jpeg == fmt)
        filter.Add(new KeyValuePair<string, string>("*.jpg", "Jpeg files (*.jpg)"));
      else if (ImageFormat.Png == fmt)
        filter.Add(new KeyValuePair<string, string>("*.png", "Png files (*.png)"));
      else if (ImageFormat.Tiff == fmt)
        filter.Add(new KeyValuePair<string, string>("*.tif", "Tiff files (*.tif)"));

      filter.Add(new KeyValuePair<string, string>("*.*", "All files (*.*)"));

      return filter;
    }
  }

  /// <summary>
  /// Handler for the menu item "Graph" - "New layer legend.
  /// </summary>
  public class SaveAsMiniProject : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      Altaxo.Graph.Commands.SaveAsMiniProjectBase.Run(ctrl.Doc);
    }
  }

  /// <summary>
  /// Saves a graph as template file.
  /// </summary>
  public class SaveGraphAsTemplate : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      System.IO.Stream myStream;
      var saveFileDialog1 = new Microsoft.Win32.SaveFileDialog
      {
        Filter = "Altaxo graph files (*.axogrp)|*.axogrp|All files (*.*)|*.*",
        FilterIndex = 1,
        RestoreDirectory = true
      };

      if (true == saveFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject))
      {
        if ((myStream = saveFileDialog1.OpenFile()) is not null)
        {
          var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
          info.BeginWriting(myStream);
          info.AddValue("Graph", ctrl.Doc);
          info.EndWriting();
          myStream.Close();
        }
      }
    }
  }

  /// <summary>
  /// Renames the current graph.
  /// </summary>
  public class GraphRename : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.Doc.ShowRenameDialog();
    }
  }

  /// <summary>
  /// Moves the current graph to another project folder.
  /// </summary>
  public class GraphMoveToFolder : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      Altaxo.Gui.Pads.ProjectBrowser.ProjectBrowserExtensions.MoveDocuments(new[] { ctrl.Doc });
    }
  }

  /// <summary>
  /// Duplicates the Graph and the Graph view to a new one.
  /// </summary>
  public class DuplicateGraph : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      var newDoc = new GraphDocument(ctrl.Doc);
      string newnamebase = Altaxo.Main.ProjectFolder.CreateFullName(ctrl.Doc.Name, Current.Project.Graph3DDocumentCollection.ItemBaseName);
      newDoc.Name = Current.Project.GraphDocumentCollection.FindNewItemName(newnamebase);
      Current.Project.Graph3DDocumentCollection.Add(newDoc);
      Current.ProjectService.CreateNewGraph3D(newDoc);
    }
  }

  /// <summary>
  /// Shows graph properties dialog.
  /// </summary>
  public class GraphShowProperties : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.Doc.ShowPropertyDialog();
    }
  }

  /// <summary>
  /// Handler for the toolbar item Rescale axes.
  /// </summary>
  public class RescaleAxes : AbstractGraph3DControllerCommand
  {
    public override void Run(Graph3DController ctrl)
    {
      ctrl.Doc.RootLayer.IsValidIndex(ctrl.CurrentLayerNumber, out var layer);

      if (layer is XYZPlotLayer xyzLayer)
      {
        xyzLayer.OnUserRescaledAxes();
      }
    }
  }

  #region Camera

  /// <summary>
  /// Provides a abstract class for issuing commands that apply to worksheet controllers.
  /// </summary>
  public abstract class AbstractCheckableGraphControllerCommand : SimpleCheckableCommand, System.ComponentModel.INotifyPropertyChanged
  {
    /// <summary>
    /// Gets the active graph controller.
    /// </summary>
    public Graph3DController? Controller
    {
      get
      {
        return Current.Workbench.ActiveViewContent as Graph3DController;
      }
    }

    /// <summary>
    /// This function is never be called, since this is a CheckableMenuCommand.
    /// </summary>
    public override void Execute(object? parameter)
    {
      base.Execute(parameter);
    }

    /// <summary>
    /// Gets or sets the checked state.
    /// </summary>
    public override bool IsChecked
    {
      get
      {
        return base.IsChecked;
      }
      set
      {
        var oldValue = base.IsChecked;
        base.IsChecked = value;

        if (value != oldValue)
          OnPropertyChanged("IsChecked");
      }
    }

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged is not null)
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
  }

  /// <summary>
  /// This class is intented to be used for commands into the graph tools toolbar. Commands derived
  /// from it will update the toolbar whenever its state changed.
  /// </summary>
  public abstract class AbstractCameraCommand : AbstractCheckableGraphControllerCommand
  {
    /// <summary>
    /// Cached graph controller currently observed for camera changes.
    /// </summary>
    protected Graph3DController? _currentGraphController;
    /// <summary>
    /// Camera type represented by this command.
    /// </summary>
    protected Type _cameraTypeForThisCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractCameraCommand"/> class.
    /// </summary>
    protected AbstractCameraCommand(Type cameraTypeForThisCommand)
    {
      _cameraTypeForThisCommand = cameraTypeForThisCommand;
      if (Current.Workbench is not null)
      {
        Current.Workbench.ActiveViewContentChanged += new WeakEventHandler(EhWorkbenchContentChanged, Current.Workbench, nameof(Current.Workbench.ActiveViewContentChanged));
        EhWorkbenchContentChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Handles active-view-content changes.
    /// </summary>
    protected void EhWorkbenchContentChanged(object? o, System.EventArgs e)
    {
      if (!object.ReferenceEquals(Controller, _currentGraphController))
      {
        if (_currentGraphController is not null)
        {
          lock (this)
          {
            _currentGraphController.Doc.Changed -= EhDocumentChanged;
            _currentGraphController = null;
          }
        }
        if (Controller is not null)
        {
          lock (this)
          {
            _currentGraphController = Controller;
            _currentGraphController.Doc.Changed += EhDocumentChanged;
          }
        }
        OnPropertyChanged("IsChecked");
      }
    }

    /// <summary>
    /// Handles document camera-change notifications.
    /// </summary>
    protected void EhDocumentChanged(object? o, EventArgs e)
    {
      if (e is Altaxo.Graph.Graph3D.Camera.CameraChangedEventArgs)
        OnPropertyChanged(nameof(IsChecked));
    }

    /// <summary>
    /// Installs the camera represented by this command.
    /// </summary>
    protected abstract void InstallCamera();

    /// <summary>
    /// Gets or sets whether this camera command is checked.
    /// </summary>
    public override bool IsChecked
    {
      get
      {
        return Controller is null ? false : _cameraTypeForThisCommand == Controller.Doc.Camera.GetType();
      }
      set
      {
        if (value == true && Controller is not null && Controller.Doc.Camera.GetType() != _cameraTypeForThisCommand)
        {
          InstallCamera();
        }
        OnPropertyChanged(nameof(IsChecked));
      }
    }
  }

  /// <summary>
  /// Selects orthographic camera mode.
  /// </summary>
  public class CameraOrtho : AbstractCameraCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CameraOrtho"/> class.
    /// </summary>
    public CameraOrtho()
        : base(typeof(Altaxo.Graph.Graph3D.Camera.OrthographicCamera))
    {
    }

    /// <summary>
    /// Installs orthographic camera settings.
    /// </summary>
    protected override void InstallCamera()
    {
      if (_currentGraphController is not null)
      {
        var oldCamera = _currentGraphController.Doc.Camera;
        double newZNear = oldCamera.ZNear;
        double newWidthAtZNear = oldCamera.WidthAtTargetDistance;
        var newCamera = new Altaxo.Graph.Graph3D.Camera.OrthographicCamera(oldCamera.UpVector, oldCamera.EyePosition, oldCamera.TargetPosition, oldCamera.ZNear, oldCamera.ZFar, newWidthAtZNear);
        _currentGraphController.Doc.Camera = _currentGraphController.AdjustZNearZFar(newCamera);
      }
    }
  }

  /// <summary>
  /// Selects perspective camera mode.
  /// </summary>
  public class CameraPerspective : AbstractCameraCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CameraPerspective"/> class.
    /// </summary>
    public CameraPerspective()
        : base(typeof(Altaxo.Graph.Graph3D.Camera.PerspectiveCamera))
    {
    }

    /// <summary>
    /// Installs perspective camera settings.
    /// </summary>
    protected override void InstallCamera()
    {
      if (_currentGraphController is not null)
      {
        var oldCamera = _currentGraphController.Doc.Camera;
        double relViewWidth = 0.93; // 2*tan(viewAngle/2) mit viewAngle = 50 deg
        double newDistance = oldCamera.WidthAtTargetDistance / relViewWidth;
        var newEyePosition = oldCamera.TargetPosition + newDistance * oldCamera.TargetToEyeVectorNormalized;
        var newCamera = new Altaxo.Graph.Graph3D.Camera.PerspectiveCamera(oldCamera.UpVector, newEyePosition, oldCamera.TargetPosition, oldCamera.ZNear, oldCamera.ZFar, relViewWidth * oldCamera.ZNear);
        _currentGraphController.Doc.Camera = _currentGraphController.AdjustZNearZFar(newCamera);
      }
    }
  }

  #endregion Camera
}
