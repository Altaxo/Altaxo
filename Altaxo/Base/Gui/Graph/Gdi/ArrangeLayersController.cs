#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi;
using Altaxo.Serialization;
using System;

namespace Altaxo.Gui.Graph.Gdi
{
	#region interfaces

	public interface IArrangeLayersView
	{
		IArrangeLayersViewEventSink Controller { get; set; }

		void InitializeRowsColumns(int numRows, int numColumns);

		void InitializeSpacing(double rowSpacing, double columnSpacing);

		void InitializeMargins(double top, double left, double bottom, double right);

		void InitializeSuperfluosLayersQuestion(Altaxo.Collections.SelectableListNodeList list);

		void InitializeEnableConditions(bool rowSpacingEnabled, bool columnSpacingEnabled, bool superfluousEnabled);
	}

	public interface IArrangeLayersViewEventSink
	{
		bool EhNumberOfRowsChanged(int val);

		bool EhNumberOfColumnsChanged(int val);

		bool EhRowSpacingChanged(string val);

		bool EhColumnSpacingChanged(string val);

		bool EhTopMarginChanged(string val);

		bool EhLeftMarginChanged(string val);

		bool EhBottomMarginChanged(string val);

		bool EhRightMarginChanged(string val);

		void EhSuperfluousLayersActionChanged(Altaxo.Collections.SelectableListNode node);
	}

	#endregion interfaces

	/// <summary>
	/// Controller for the <see cref="ArrangeLayersDocument" />.
	/// </summary>
	[UserControllerForObject(typeof(ArrangeLayersDocument))]
	[ExpectedTypeOfView(typeof(IArrangeLayersView))]
	public class ArrangeLayersController : IArrangeLayersViewEventSink, IMVCAController
	{
		private ArrangeLayersDocument _doc;
		private ArrangeLayersDocument _tempDoc;
		private IArrangeLayersView _view;

		public ArrangeLayersController(ArrangeLayersDocument doc)
		{
			_doc = doc;
			_tempDoc = new ArrangeLayersDocument();
			_tempDoc.CopyFrom(doc);

			Initialize();
		}

		private void Initialize()
		{
			if (_view != null)
			{
				_view.InitializeRowsColumns(_tempDoc.NumberOfRows, _tempDoc.NumberOfColumns);
				_view.InitializeSpacing(_tempDoc.RowSpacing * 100, _tempDoc.ColumnSpacing * 100);
				_view.InitializeMargins(_tempDoc.TopMargin * 100, _tempDoc.LeftMargin * 100, _tempDoc.BottomMargin * 100, _tempDoc.RightMargin * 100);
				_view.InitializeSuperfluosLayersQuestion(Altaxo.Serialization.GUIConversion.GetListOfChoices(_tempDoc.SuperfluousLayersAction));
				SetEnableConditions();
			}
		}

		private void SetEnableConditions()
		{
			if (_view != null)
			{
				// Note: the concept was not acceptable since the user can not hopp with the mouse
				// into the ColumnSpacing or RowSpacing edit boxes because they are disabled
				_view.InitializeEnableConditions(
					true, // _tempDoc.NumberOfRows >= 2,
					true, // _tempDoc.NumberOfColumns >= 2,
					true
				);
			}
		}

		#region IArrangeLayersViewEventSink Members

		public bool EhNumberOfRowsChanged(int val)
		{
			if (val < 1)
			{
				Current.Gui.ErrorMessageBox("Please provide a value >0 here");
				return true;
			}
			_tempDoc.NumberOfRows = val;
			SetEnableConditions();
			return false;
		}

		public bool EhNumberOfColumnsChanged(int val)
		{
			if (val < 1)
			{
				Current.Gui.ErrorMessageBox("Please provide a value >0 here");
				return true;
			}

			_tempDoc.NumberOfColumns = val;
			SetEnableConditions();
			return false;
		}

		public bool EhRowSpacingChanged(string val)
		{
			double v;
			if (!GUIConversion.IsDouble(val, out v))
			{
				Current.Gui.ErrorMessageBox("You have to provide an numeric value here");
				return true;
			}
			if (v < 0)
			{
				Current.Gui.ErrorMessageBox("Please provide a value >=0 here");
				return true;
			}

			_tempDoc.RowSpacing = v / 100.0;
			return false;
		}

		public bool EhColumnSpacingChanged(string val)
		{
			double v;
			if (!GUIConversion.IsDouble(val, out v))
			{
				Current.Gui.ErrorMessageBox("You have to provide an numeric value here");
				return true;
			}
			if (v < 0)
			{
				Current.Gui.ErrorMessageBox("Please provide a value >=0 here");
				return true;
			}

			_tempDoc.ColumnSpacing = v / 100.0;
			return false;
		}

		public bool EhTopMarginChanged(string val)
		{
			double v;
			if (!GUIConversion.IsDouble(val, out v))
			{
				Current.Gui.ErrorMessageBox("You have to provide an numeric value here");
				return true;
			}
			_tempDoc.TopMargin = v / 100.0;
			return false;
		}

		public bool EhLeftMarginChanged(string val)
		{
			double v;
			if (!GUIConversion.IsDouble(val, out v))
			{
				Current.Gui.ErrorMessageBox("You have to provide an numeric value here");
				return true;
			}
			_tempDoc.LeftMargin = v / 100.0;
			return false;
		}

		public bool EhBottomMarginChanged(string val)
		{
			double v;
			if (!GUIConversion.IsDouble(val, out v))
			{
				Current.Gui.ErrorMessageBox("You have to provide an numeric value here");
				return true;
			}
			_tempDoc.BottomMargin = v / 100.0;
			return false;
		}

		public bool EhRightMarginChanged(string val)
		{
			double v;
			if (!GUIConversion.IsDouble(val, out v))
			{
				Current.Gui.ErrorMessageBox("You have to provide an numeric value here");
				return true;
			}
			_tempDoc.RightMargin = v / 100.0;
			return false;
		}

		public void EhSuperfluousLayersActionChanged(Altaxo.Collections.SelectableListNode node)
		{
			_tempDoc.SuperfluousLayersAction = (SuperfluousLayersAction)node.Tag;
		}

		#endregion IArrangeLayersViewEventSink Members

		#region IMVCController Members

		public object ViewObject
		{
			get { return _view; }
			set
			{
				if (_view != null)
					_view.Controller = null;

				_view = value as IArrangeLayersView;

				Initialize();

				if (_view != null)
					_view.Controller = this;
			}
		}

		public object ModelObject
		{
			get
			{
				return _doc;
			}
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			_doc.CopyFrom(_tempDoc);
			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}