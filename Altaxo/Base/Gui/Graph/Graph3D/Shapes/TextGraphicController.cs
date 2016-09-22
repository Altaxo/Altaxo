#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Gui.Graph.Graph3D.Shapes
{
	using Altaxo.Drawing;
	using Altaxo.Drawing.D3D;
	using Altaxo.Graph.Graph3D;
	using Altaxo.Graph.Graph3D.Background;
	using Altaxo.Graph.Graph3D.Shapes;

	#region Interfaces

	public interface ITextGraphicView
	{
		void BeginUpdate();

		void EndUpdate();

		ITextGraphicViewEventSink Controller { set; }

		IBackgroundStyle SelectedBackground { get; set; }

		object LocationView { set; }

		string EditText { get; set; }

		string SelectedFontFamily { get; set; }

		double SelectedFontSize { get; set; }

		double SelectedFontDepth { get; set; }

		double SelectedLineSpacing { get; set; }

		IMaterial SelectedFontBrush { get; set; }

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

		void EhView_FontDepthChanged();

		void EhView_TextFillBrushChanged();

		void EhView_LineSpacingChanged();
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(TextGraphic))]
	[ExpectedTypeOfView(typeof(ITextGraphicView))]
	internal class TextGraphicController : MVCANControllerEditOriginalDocBase<TextGraphic, ITextGraphicView>, ITextGraphicViewEventSink
	{
		private XYZPlotLayer _parentLayerOfOriginalDoc;

		private IMVCANController _locationController;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_locationController, () => _locationController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_parentLayerOfOriginalDoc = AbsoluteDocumentPath.GetRootNodeImplementing<XYZPlotLayer>(_doc);

				_locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
				Current.Gui.FindAndAttachControlTo(_locationController);
			}

			if (_view != null)
			{
				_view.BeginUpdate();

				_view.SelectedBackground = _doc.Background;

				_view.EditText = _doc.Text;

				// fill the font name combobox with all fonts
				_view.SelectedFontFamily = _doc.Font.FontFamilyName;

				_view.SelectedFontSize = _doc.Font.Size;

				_view.SelectedFontDepth = _doc.Font.Depth;

				_view.SelectedLineSpacing = _doc.LineSpacing;

				// fill the font size combobox with reasonable values
				//this.m_cbFontSize.Items.AddRange(new string[] { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" });
				//this.m_cbFontSize.Text = m_TextObject.Font.Size.ToString();

				// fill the color dialog box
				_view.SelectedFontBrush = this._doc.TextFillBrush;

				_view.LocationView = _locationController.ViewObject;

				_view.EndUpdate();
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_locationController.Apply(disposeController))
				return false;

			if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
				_doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.Controller = this;
		}

		protected override void DetachView()
		{
			_view.Controller = null;

			base.DetachView();
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
			string fontFamilyName = _view.SelectedFontFamily;

			var ff = new FontFamily(fontFamilyName);

			// make sure that regular style is available
			if (ff.IsStyleAvailable(FontStyle.Regular))
				this._doc.Font = FontManager3D.Instance.GetFont(fontFamilyName, this._doc.Font.Size, this._doc.Font.Depth, FontXStyle.Regular);
			else if (ff.IsStyleAvailable(FontStyle.Bold))
				this._doc.Font = FontManager3D.Instance.GetFont(fontFamilyName, this._doc.Font.Size, this._doc.Font.Depth, FontXStyle.Bold);
			else if (ff.IsStyleAvailable(FontStyle.Italic))
				this._doc.Font = FontManager3D.Instance.GetFont(fontFamilyName, this._doc.Font.Size, this._doc.Font.Depth, FontXStyle.Italic);

			_view.InvalidatePreviewPanel();
		}

		public void EhView_FontSizeChanged()
		{
			var newSize = _view.SelectedFontSize;
			var oldFont = this._doc.Font;
			this._doc.Font = oldFont.WithSize(newSize);
			_view.InvalidatePreviewPanel();
		}

		public void EhView_FontDepthChanged()
		{
			var newSize = _view.SelectedFontDepth;
			var oldFont = this._doc.Font;
			this._doc.Font = oldFont.WithDepth(newSize);
			_view.InvalidatePreviewPanel();
		}

		public void EhView_TextFillBrushChanged()
		{
			this._doc.TextFillBrush = _view.SelectedFontBrush;
			_view.InvalidatePreviewPanel();
		}

		#endregion ITextGraphicViewEventSink Members
	}
}