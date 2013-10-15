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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Gui.Graph.Shapes
{
	#region Interfaces

	public interface ITextGraphicView
	{
		void BeginUpdate();

		void EndUpdate();

		ITextGraphicViewEventSink Controller { set; }

		IBackgroundStyle SelectedBackground { get; set; }

		string EditText { get; set; }

		PointD2D SelectedPosition { get; set; }

		double SelectedRotation { get; set; }

		double SelectedShear { get; set; }

		double SelectedScaleX { get; set; }

		double SelectedScaleY { get; set; }

		System.Drawing.FontFamily SelectedFontFamily { get; set; }

		double SelectedFontSize { get; set; }

		double SelectedLineSpacing { get; set; }

		BrushX SelectedFontBrush { get; set; }

		void InitializePivot(RADouble pivotX, RADouble pivotY, PointD2D sizeOfTextGraphic);

		RADouble XAnchor { get; }

		RADouble YAnchor { get; }

		void InitializeParentReferencePoint(RADouble pivotX, RADouble pivotY, PointD2D sizeOfTextGraphic);

		RADouble ParentReferenceX { get; }

		RADouble ParentReferenceY { get; }

		void InsertBeforeAndAfterSelectedText(string insbefore, string insafter);

		void RevertToNormal();

		void InvalidatePreviewPanel();
	}

	public interface ITextGraphicViewEventSink
	{
		void EhView_BoldClick();

		void EhView_ItalicClick();

		void EhView_UnderlineClick();

		void EhView_SupIndexClick();

		void EhView_SubIndexClick();

		void EhView_GreekClick();

		void EhView_NormalClick();

		void EhView_StrikeoutClick();

		void EhView_PreviewPanelPaint(System.Drawing.Graphics g);

		void EhView_EditTextChanged();

		void EhView_BackgroundStyleChanged();

		void EhView_FontFamilyChanged();

		void EhView_FontSizeChanged();

		void EhView_TextFillBrushChanged();

		void EhView_LineSpacingChanged();
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(Altaxo.Graph.Gdi.Shapes.TextGraphic))]
	[ExpectedTypeOfView(typeof(ITextGraphicView))]
	internal class TextGraphicController : ITextGraphicViewEventSink, IMVCANController
	{
		private UseDocument _useDocumentCopy;
		private TextGraphic _doc, _originalDoc;
		private ITextGraphicView _view;

		private XYPlotLayer m_Layer;

		public TextGraphicController()
		{
		}

		private void Initialize(bool initDocument)
		{
			if (initDocument)
			{
				m_Layer = DocumentPath.GetRootNodeImplementing<XYPlotLayer>(_originalDoc);
			}

			if (_view != null)
			{
				_view.BeginUpdate();

				_view.SelectedBackground = _doc.Background;

				_view.EditText = _doc.Text;

				_view.SelectedPosition = _doc.Position;
				_view.SelectedRotation = _doc.Rotation;
				_view.SelectedShear = _doc.Shear;
				_view.SelectedScaleX = _doc.ScaleX;
				_view.SelectedScaleY = _doc.ScaleY;

				// fill the font name combobox with all fonts
				_view.SelectedFontFamily = _doc.Font.GdiFontFamily();

				_view.SelectedFontSize = _doc.Font.Size;
				_view.SelectedLineSpacing = _doc.LineSpacing;

				_view.InitializePivot(_doc.Location.PivotX, _doc.Location.PivotY, _doc.Size);
				_view.InitializeParentReferencePoint(_doc.Location.ReferenceX, _doc.Location.ReferenceY, _doc.ParentSize);

				// fill the font size combobox with reasonable values
				//this.m_cbFontSize.Items.AddRange(new string[] { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" });
				//this.m_cbFontSize.Text = m_TextObject.Font.Size.ToString();

				// fill the color dialog box
				_view.SelectedFontBrush = this._doc.TextFillBrush;

				_view.EndUpdate();
			}
		}

		#region ITextGraphicViewEventSink Members

		public void EhView_BoldClick()
		{
			// insert \b( at beginning of selection and ) at the end of the selection
			_view.InsertBeforeAndAfterSelectedText("\\b(", ")");
		}

		public void EhView_ItalicClick()
		{
			// insert \b( at beginning of selection and ) at the end of the selection
			_view.InsertBeforeAndAfterSelectedText("\\i(", ")");
		}

		public void EhView_UnderlineClick()
		{
			// insert \b( at beginning of selection and ) at the end of the selection
			_view.InsertBeforeAndAfterSelectedText("\\u(", ")");
		}

		public void EhView_SupIndexClick()
		{
			// insert \b( at beginning of selection and ) at the end of the selection
			_view.InsertBeforeAndAfterSelectedText("\\+(", ")");
		}

		public void EhView_SubIndexClick()
		{
			// insert \b( at beginning of selection and ) at the end of the selection
			_view.InsertBeforeAndAfterSelectedText("\\-(", ")");
		}

		public void EhView_GreekClick()
		{
			// insert \b( at beginning of selection and ) at the end of the selection
			_view.InsertBeforeAndAfterSelectedText("\\g(", ")");
		}

		public void EhView_NormalClick()
		{
			_view.RevertToNormal();
		}

		public void EhView_StrikeoutClick()
		{
			// insert \b( at beginning of selection and ) at the end of the selection
			_view.InsertBeforeAndAfterSelectedText("\\s(", ")");
		}

		public void EhView_PreviewPanelPaint(System.Drawing.Graphics g)
		{
			g.PageUnit = System.Drawing.GraphicsUnit.Point;

			g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			g.FillRectangle(Brushes.Transparent, g.VisibleClipBounds);
			g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

			// set position and rotation to zero
			//    m_TextObject.Position=new PointF(0,0);
			//    m_TextObject.Rotation = 0;
			_doc.Paint(g, m_Layer, true);

			// restore the original position and rotation values
			//      m_TextObject.Position = new PointF(m_PositionX,m_PositionY);
			//      m_TextObject.Rotation = _rotation;
		}

		public void EhView_EditTextChanged()
		{
			this._doc.Text = _view.EditText;
			_view.InvalidatePreviewPanel();
		}

		public void EhView_BackgroundStyleChanged()
		{
			_doc.Background = _view.SelectedBackground;
			_view.InvalidatePreviewPanel();
		}

		public void EhView_LineSpacingChanged()
		{
			_doc.LineSpacing = _view.SelectedLineSpacing;
			_view.InvalidatePreviewPanel();
		}

		public void EhView_FontFamilyChanged()
		{
			FontFamily ff = _view.SelectedFontFamily;
			// make sure that regular style is available
			if (ff.IsStyleAvailable(FontStyle.Regular))
				this._doc.Font = GdiFontManager.GetFont(ff, this._doc.Font.Size, FontStyle.Regular);
			else if (ff.IsStyleAvailable(FontStyle.Bold))
				this._doc.Font = GdiFontManager.GetFont(ff, this._doc.Font.Size, FontStyle.Bold);
			else if (ff.IsStyleAvailable(FontStyle.Italic))
				this._doc.Font = GdiFontManager.GetFont(ff, this._doc.Font.Size, FontStyle.Italic);

			_view.InvalidatePreviewPanel();
		}

		public void EhView_FontSizeChanged()
		{
			var newSize = _view.SelectedFontSize;
			FontX oldFont = this._doc.Font;
			this._doc.Font = oldFont.GetFontWithNewSize(newSize);
			_view.InvalidatePreviewPanel();
		}

		public void EhView_TextFillBrushChanged()
		{
			this._doc.TextFillBrush = _view.SelectedFontBrush;
			_view.InvalidatePreviewPanel();
		}

		#endregion ITextGraphicViewEventSink Members

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is TextGraphic))
				return false;
			_originalDoc = (TextGraphic)args[0];
			_doc = _useDocumentCopy == UseDocument.Directly ? _originalDoc : (TextGraphic)_originalDoc.Clone();
			Initialize(true); // initialize always because we have to update the temporary variables
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = value; }
		}

		#endregion IMVCANController Members

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (_view != null)
					_view.Controller = null;

				_view = value as ITextGraphicView;

				Initialize(false);

				if (_view != null)
					_view.Controller = this;
			}
		}

		public object ModelObject
		{
			get { return _originalDoc; }
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply()
		{
			_doc.Position = _view.SelectedPosition;
			_doc.Rotation = _view.SelectedRotation;
			_doc.Shear = _view.SelectedShear;
			_doc.Scale = new PointD2D(_view.SelectedScaleX, _view.SelectedScaleY);

			_doc.Location.PivotX = _view.XAnchor;
			_doc.Location.PivotY = _view.YAnchor;

			_doc.Location.ReferenceX = _view.ParentReferenceX;
			_doc.Location.ReferenceY = _view.ParentReferenceY;

			if (!object.ReferenceEquals(_originalDoc, _doc))
				_originalDoc.CopyFrom(_doc);

			return true;
		}

		#endregion IApplyController Members
	}
}