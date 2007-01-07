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
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface ITextGraphicView
  {
    void BeginUpdate();
    void EndUpdate();

    ITextGraphicViewEventSink Controller { set; }
    IBackgroundStyle Background { get; set; }
    string EditText { get; set; }
    System.Drawing.PointF Position { get; set; }
    float Rotation { get; set; }
    System.Drawing.FontFamily FontFamily { get; set; }
    float FontSize { get; set; }
    BrushX FontColor { get; set; }

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
  }

  #endregion
  [UserControllerForObject(typeof(Altaxo.Graph.Gdi.Shapes.TextGraphic))]
  [ExpectedTypeOfView(typeof(ITextGraphicView))]
  class TextGraphicController : ITextGraphicViewEventSink, IMVCANController
  {
    UseDocument _useDocumentCopy;
    TextGraphic _doc, _originalDoc;
    ITextGraphicView _view;

    XYPlotLayer m_Layer;

    public TextGraphicController()
    {
    }

    void Initialize(bool initDocument)
    {
      if (initDocument)
      {
        m_Layer = Main.DocumentPath.GetRootNodeImplementing<XYPlotLayer>(_originalDoc);
      }

      if (_view != null)
      {
        _view.BeginUpdate();

        _view.Background = _doc.Background;

        _view.EditText = _doc.Text;

        _view.Position = _doc.Position;
        _view.Rotation = _doc.Rotation;

        // fill the font name combobox with all fonts
        _view.FontFamily = _doc.Font.FontFamily;

        _view.FontSize = _doc.Font.Size;

        // fill the font size combobox with reasonable values
        //this.m_cbFontSize.Items.AddRange(new string[] { "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" });
        //this.m_cbFontSize.Text = m_TextObject.Font.Size.ToString();


        // fill the color dialog box
        _view.FontColor = this._doc.TextFillBrush;

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
      _doc.Background = _view.Background;
      _view.InvalidatePreviewPanel();
    }

    public void EhView_FontFamilyChanged()
    {
      FontFamily ff = _view.FontFamily;
      // make sure that regular style is available
      if (ff.IsStyleAvailable(FontStyle.Regular))
        this._doc.Font = new Font(ff, this._doc.Font.Size, FontStyle.Regular, GraphicsUnit.World);
      else if (ff.IsStyleAvailable(FontStyle.Bold))
        this._doc.Font = new Font(ff, this._doc.Font.Size, FontStyle.Bold, GraphicsUnit.World);
      else if (ff.IsStyleAvailable(FontStyle.Italic))
        this._doc.Font = new Font(ff, this._doc.Font.Size, FontStyle.Italic, GraphicsUnit.World);

      _view.InvalidatePreviewPanel();
    }

    public void EhView_FontSizeChanged()
    {
      float newSize = _view.FontSize;
      Font oldFont = this._doc.Font;
      this._doc.Font = new Font(oldFont.FontFamily, newSize, oldFont.Style, GraphicsUnit.World);
      _view.InvalidatePreviewPanel();

    }
    public void EhView_TextFillBrushChanged()
    {
      this._doc.TextFillBrush = _view.FontColor;
      _view.InvalidatePreviewPanel();
    }



    #endregion


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

    #endregion

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

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc.Position = _view.Position;
      _doc.Rotation = _view.Rotation;

      if(!object.ReferenceEquals(_originalDoc,_doc))
        _originalDoc.CopyFrom(_doc);

      return true;
    }

    #endregion
  }
}
