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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Main;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  #region Interfaces

  public interface ITextGraphicView
  {
    void BeginUpdate();

    void EndUpdate();

    ITextGraphicViewEventSink Controller { set; }

    IBackgroundStyle SelectedBackground { get; set; }

    object LocationView { set; }

    string EditText { get; set; }

    FontX SelectedFont { get; set; }

    double SelectedLineSpacing { get; set; }

    BrushX SelectedFontBrush { get; set; }

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
  internal class TextGraphicController : MVCANControllerEditOriginalDocBase<Altaxo.Graph.Gdi.Shapes.TextGraphic, ITextGraphicView>, ITextGraphicViewEventSink
  {
    private XYPlotLayer _parentLayerOfOriginalDoc;

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
        _parentLayerOfOriginalDoc = AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(_doc);

        _locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
        Current.Gui.FindAndAttachControlTo(_locationController);
      }

      if (_view != null)
      {
        _view.BeginUpdate();

        _view.SelectedBackground = _doc.Background;

        _view.EditText = _doc.Text;

        // fill the font name combobox with all fonts
        _view.SelectedFont = _doc.Font;

        _view.SelectedLineSpacing = _doc.LineSpacing;

        // fill the font size combobox with reasonable values
        //this.m_cbFontSize.Items.AddRange(new string[] { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" });
        //this.m_cbFontSize.Text = m_TextObject.Font.Size.ToString();

        // fill the color dialog box
        _view.SelectedFontBrush = _doc.TextFillBrush;

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

      var paintContext = new GdiPaintContext();

      // set position and rotation to zero
      //    m_TextObject.Position=new PointF(0,0);
      //    m_TextObject.Rotation = 0;
      _doc.Paint(g, paintContext, true);

      // restore the original position and rotation values
      //      m_TextObject.Position = new PointF(m_PositionX,m_PositionY);
      //      m_TextObject.Rotation = _rotation;
    }

    public void EhView_EditTextChanged()
    {
      _doc.Text = _view.EditText;
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
      var ff = _view.SelectedFont.FontFamilyName;

      // make sure that regular style is available
      if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Regular))
        _doc.Font = GdiFontManager.GetFontX(ff, _doc.Font.Size, FontXStyle.Regular);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Bold))
        _doc.Font = GdiFontManager.GetFontX(ff, _doc.Font.Size, FontXStyle.Bold);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Italic))
        _doc.Font = GdiFontManager.GetFontX(ff, _doc.Font.Size, FontXStyle.Italic);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Bold | FontXStyle.Italic))
        _doc.Font = GdiFontManager.GetFontX(ff, _doc.Font.Size, FontXStyle.Bold | FontXStyle.Italic);

      _view.InvalidatePreviewPanel();
    }

    public void EhView_FontSizeChanged()
    {
      _doc.Font = _doc.Font.WithSize(_view.SelectedFont.Size);
      _view.InvalidatePreviewPanel();
    }

    public void EhView_TextFillBrushChanged()
    {
      _doc.TextFillBrush = _view.SelectedFontBrush;
      _view.InvalidatePreviewPanel();
    }

    #endregion ITextGraphicViewEventSink Members
  }
}
