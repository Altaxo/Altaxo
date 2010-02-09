#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;

using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Gui;
using Altaxo.Collections;


namespace Altaxo.Gui.Graph
{
	#region Interfaces

	/// <summary>
	/// Interface that must be implemented by Gui classes that allow to select a color provider.
	/// </summary>
	public interface IColorProviderView
	{
		/// <summary>
		/// Set the list of available color providers.
		/// </summary>
		/// <param name="names">List of items.</param>
		void InitializeAvailableClasses(SelectableListNodeList names);

		/// <summary>
		/// Sets the detailed view for the instance of the color provider.
		/// </summary>
		/// <param name="guiobject"></param>
		void SetDetailView(object guiobject);

		/// <summary>
		/// Set the preview bitmap to be shown in the view.
		/// </summary>
		/// <param name="bitmap">Bitmap to show.</param>
		void SetPreviewBitmap(System.Drawing.Bitmap bitmap);

		/// <summary>
		/// Fired when the selected color provider changed.
		/// </summary>
		event Action ColorProviderChanged;
	}




	#endregion

	/// <summary>
	/// Summary description for AxisScaleController.
	/// </summary>
	[ExpectedTypeOfView(typeof(IDensityScaleView))]
	[UserControllerForObject(typeof(NumericalScale), 101)]
	public class ColorProviderController : IMVCANController
	{
		protected IColorProviderView _view;

		// Cached values
		protected IColorProvider _originalDoc;

		protected IColorProvider _doc;


		protected IMVCAController _detailController;

		protected SelectableListNodeList _availableClasses;


		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is IColorProvider))
				return false;

			_originalDoc = (IColorProvider)args[0];
			_doc = (IColorProvider)_originalDoc.Clone();
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}


		public void Initialize(bool initData)
		{
			InitClassTypes(initData);
			InitDetailController(initData);
			CreateAndSetPreviewBitmap();
		}

		public void InitClassTypes(bool bInit)
		{
			if (bInit)
			{
				_availableClasses = new SelectableListNodeList();
				Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IColorProvider));
				for (int i = 0; i < classes.Length; i++)
				{
					if (classes[i] == typeof(LinkedScale))
						continue;
					SelectableListNode node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.GetType() == classes[i]);
					_availableClasses.Add(node);
				}
			}

			if (null != _view)
				_view.InitializeAvailableClasses(_availableClasses);
		}

		public void InitDetailController(bool bInit)
		{
			if (bInit)
			{
				object providerObject = _doc;
				_detailController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { providerObject }, typeof(IMVCAController));
			}
			if (null != _view)
			{
				_view.SetDetailView(null == _detailController ? null : _detailController.ViewObject);
			}
		}

		void EhColorProviderSelectionChanged()
		{
			Type chosenType = (Type)_availableClasses.FirstSelectedNode.Item;

			try
			{
				if (chosenType != _doc.GetType())
				{
					// replace the current axis by a new axis of the type axistype
					var oldDoc = _doc;
					_doc = (IColorProvider)System.Activator.CreateInstance(chosenType);

					// Try to set the same org and end as the axis before
					// this will fail for instance if we switch from linear to logarithmic with negative bounds
					try
					{
						_doc.CopyFrom(oldDoc);
					}
					catch (Exception)
					{
					}

					InitDetailController(true);
					CreateAndSetPreviewBitmap();
				}
			}
			catch (Exception)
			{
			}
		}

		Bitmap _previewBitmap;
		void CreateAndSetPreviewBitmap()
		{
			const int previewWidth=128;
			const int previewHeight = 16;
			if (null != _view)
			{
				if (null == _previewBitmap)
					_previewBitmap = new Bitmap(previewWidth, previewHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				for (int i = 0; i < previewWidth; i++)
				{
					double relVal = i / (double)(previewWidth - 1);
					Color c = _doc.GetColor(relVal);
					for (int j = 0; j < previewHeight; j++)
						_previewBitmap.SetPixel(i, j, c);
				}

				_view.SetPreviewBitmap(_previewBitmap);
			}
		}


		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.ColorProviderChanged -= EhColorProviderSelectionChanged;
				}

				_view = value as IColorProviderView;

				if (null != _view)
				{
					Initialize(false);
					_view.ColorProviderChanged += EhColorProviderSelectionChanged;
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			if (null != _detailController)
			{
				if (!_detailController.Apply())
					return false;
			}

			return true;
		}

		#endregion
	}
}
