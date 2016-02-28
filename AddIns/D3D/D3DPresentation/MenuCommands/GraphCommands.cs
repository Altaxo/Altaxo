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

using Altaxo.Collections;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph3D.Viewing;
using Altaxo.Gui.Scripting;
using Altaxo.Main;
using Altaxo.Scripting;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Altaxo.Graph.Graph3D.Commands
{
	/// <summary>
	/// Provides a abstract class for issuing commands that apply to worksheet controllers.
	/// </summary>
	public abstract class AbstractGraph3DControllerCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Determines the currently active worksheet and issues the command to that worksheet by calling
		/// Run with the worksheet as a parameter.
		/// </summary>
		public override void Run()
		{
			Altaxo.Gui.SharpDevelop.SDGraph3DViewContent ctrl
					= Current.Workbench.ActiveViewContent
					as Altaxo.Gui.SharpDevelop.SDGraph3DViewContent;

			if (null != ctrl)
				Run((Altaxo.Gui.Graph3D.Viewing.Graph3DController)ctrl.MVCController);
		}

		/// <summary>
		/// Override this function for adding own worksheet commands. You will get
		/// the worksheet controller in the parameter.
		/// </summary>
		/// <param name="ctrl">The worksheet controller this command is applied to.</param>
		public abstract void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl);
	}

	public class ViewFront : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewFront();
		}
	}

	public class ViewRight : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewRight();
		}
	}

	public class ViewBack : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewBack();
		}
	}

	public class ViewLeft : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewLeft();
		}
	}

	public class ViewTop : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewTop();
		}
	}

	public class ViewBottom : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewBottom();
		}
	}

	public class ViewIsometricStandard : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewIsometricStandard();
		}
	}

	public class ViewIsometricLeftTop : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewIsometricLeftTop();
		}
	}

	public class AddSphere : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.AddSphere();
		}
	}

	public class SetCopyPageOptions : AbstractMenuCommand
	{
		public override void Run()
		{
			object resultobj = Gdi.ClipboardRenderingOptions.CopyPageOptions;
			if (Current.Gui.ShowDialog(ref resultobj, "Set copy page options"))
			{
				Gdi.ClipboardRenderingOptions.CopyPageOptions = (Gdi.ClipboardRenderingOptions)resultobj;
			}
		}
	}

	public class Export3D : AbstractGraph3DControllerCommand
	{
		private static Altaxo.Graph.Gdi.GraphExportOptions _graphExportOptionsToFile = new Graph.Gdi.GraphExportOptions();

		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ShowFileExportSpecificDialog(ctrl.Doc);
		}

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
					new Altaxo.Gui.Graph3D.Common.D3D10BitmapExporter().ExportAsImageToStream(doc, graphExportOptions, myStream);
					myStream.Close();
				} // end openfile ok
			} // end dlgresult ok
		}

		private static IList<KeyValuePair<string, string>> GetFileFilterString(ImageFormat fmt)
		{
			List<KeyValuePair<string, string>> filter = new List<KeyValuePair<string, string>>();

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
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			Altaxo.Graph.Commands.SaveAsMiniProjectBase.Run(ctrl.Doc);
		}
	}

	public class SaveGraphAsTemplate : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			System.IO.Stream myStream;
			var saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();

			saveFileDialog1.Filter = "Altaxo graph files (*.axogrp)|*.axogrp|All files (*.*)|*.*";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;

			if (true == saveFileDialog1.ShowDialog((System.Windows.Window)Current.Workbench.ViewObject))
			{
				if ((myStream = saveFileDialog1.OpenFile()) != null)
				{
					Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
					info.BeginWriting(myStream);
					info.AddValue("Graph", ctrl.Doc);
					info.EndWriting();
					myStream.Close();
				}
			}
		}
	}

	#region Camera

	/// <summary>
	/// Provides a abstract class for issuing commands that apply to worksheet controllers.
	/// </summary>
	public abstract class AbstractCheckableGraphControllerCommand : AbstractCheckableMenuCommand, System.ComponentModel.INotifyPropertyChanged
	{
		public Altaxo.Gui.Graph3D.Viewing.Graph3DController Controller
		{
			get
			{
				if (null != Current.Workbench && null != Current.Workbench.ActiveViewContent)
				{
					var ct = Current.Workbench.ActiveViewContent as Altaxo.Gui.SharpDevelop.SDGraph3DViewContent;
					return ct == null ? null : ct.Controller;
				}
				else
					return null;
			}
		}

		/// <summary>
		/// This function is never be called, since this is a CheckableMenuCommand.
		/// </summary>
		public override void Run()
		{
			base.Run();
		}

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

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (null != PropertyChanged)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// This class is intented to be used for commands into the graph tools toolbar. Commands derived
	/// from it will update the toolbar whenever its state changed.
	/// </summary>
	public abstract class AbstractCameraCommand : AbstractCheckableGraphControllerCommand
	{
		protected Altaxo.Gui.Graph3D.Viewing.Graph3DController _currentGraphController;
		protected Type _cameraTypeForThisCommand;

		protected AbstractCameraCommand(Type cameraTypeForThisCommand)
		{
			_cameraTypeForThisCommand = cameraTypeForThisCommand;
			if (null != Current.Workbench)
			{
				Current.Workbench.ActiveWorkbenchWindowChanged += new EventHandler(this.EhWorkbenchContentChanged);
				this.EhWorkbenchContentChanged(this, EventArgs.Empty);
			}
		}

		protected void EhWorkbenchContentChanged(object o, System.EventArgs e)
		{
			if (!object.ReferenceEquals(Controller, _currentGraphController))
			{
				if (null != _currentGraphController)
				{
					lock (this)
					{
						this._currentGraphController.Doc.Changed -= new EventHandler(this.EhDocumentChanged);
						this._currentGraphController = null;
					}
				}
				if (Controller != null)
				{
					lock (this)
					{
						this._currentGraphController = this.Controller;
						this._currentGraphController.Doc.Changed += new EventHandler(this.EhDocumentChanged);
					}
				}
				OnPropertyChanged("IsChecked");
			}
		}

		protected void EhDocumentChanged(object o, EventArgs e)
		{
			if (e is Altaxo.Graph.Graph3D.Camera.CameraChangedEventArgs)
				OnPropertyChanged("IsChecked");
		}

		protected abstract void InstallCamera();

		public override bool IsChecked
		{
			get
			{
				return null == Controller ? false : _cameraTypeForThisCommand == Controller.Doc.Camera.GetType();
			}
			set
			{
				if (value == true && Controller != null && Controller.Doc.Camera.GetType() != _cameraTypeForThisCommand)
				{
					InstallCamera();
				}
				OnPropertyChanged("IsChecked");
			}
		}
	}

	public class CameraOrtho : AbstractCameraCommand
	{
		public CameraOrtho()
				: base(typeof(Altaxo.Graph.Graph3D.Camera.OrthographicCamera))
		{
		}

		protected override void InstallCamera()
		{
			var oldCamera = _currentGraphController.Doc.Camera;
			double newZNear = oldCamera.ZNear;
			double newWidthAtZNear = oldCamera.WidthAtTargetDistance;
			var newCamera = new Altaxo.Graph.Graph3D.Camera.OrthographicCamera(oldCamera.UpVector, oldCamera.EyePosition, oldCamera.TargetPosition, oldCamera.ZNear, oldCamera.ZFar, newWidthAtZNear);
			_currentGraphController.Doc.Camera = newCamera;
		}
	}

	public class CameraPerspective : AbstractCameraCommand
	{
		public CameraPerspective()
				: base(typeof(Altaxo.Graph.Graph3D.Camera.PerspectiveCamera))
		{
		}

		protected override void InstallCamera()
		{
			var oldCamera = _currentGraphController.Doc.Camera;

			double relViewWidth = 0.93; // 2*tan(viewAngle/2) mit viewAngle = 50 deg
			double newDistance = oldCamera.WidthAtTargetDistance / relViewWidth;
			var newEyePosition = oldCamera.TargetPosition + newDistance * oldCamera.NormalizedEyeVector;
			var newCamera = new Altaxo.Graph.Graph3D.Camera.PerspectiveCamera(oldCamera.UpVector, newEyePosition, oldCamera.TargetPosition, oldCamera.ZNear, oldCamera.ZFar, relViewWidth * oldCamera.ZNear);
			_currentGraphController.Doc.Camera = newCamera;
		}
	}

	#endregion Camera
}