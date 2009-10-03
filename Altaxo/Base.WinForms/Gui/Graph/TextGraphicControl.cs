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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
  public partial class TextGraphicControl : UserControl, ITextGraphicView
  {
    public TextGraphicControl()
    {
      InitializeComponent();
    }

    #region ITextGraphicView Members

    ITextGraphicViewEventSink _controller;
    public ITextGraphicViewEventSink Controller { set { _controller = value; } }

    public void BeginUpdate()
    {
      this.SuspendLayout();
    }
    public void EndUpdate()
    {
      this.ResumeLayout();
    }

    public Altaxo.Graph.Gdi.Background.IBackgroundStyle Background
    {
      get
      {
        return _backgroundGlue.BackgroundStyle;
      }
      set
      {
        _backgroundGlue.BackgroundStyle = value;
      }
    }

    public string EditText
    {
      get
      {
        return m_edText.Text;
      }
      set
      {
        m_edText.Text = value;
      }
    }


    public PointF Position
    {
      get
      {
        return _positionSizeGlue.Position;
      }
      set
      {
        _positionSizeGlue.Position = value;
      }
    }

    public float Rotation
    {
      get
      {
        return m_cbRotation.Rotation;
      }
      set
      {
        m_cbRotation.Rotation = value;
      }
    }

    public FontFamily FontFamily
    {
      get
      {
        return m_cbFonts.FontFamilyDocument;
      }
      set
      {
        m_cbFonts.FontFamilyDocument = value;
      }
    }

    public float FontSize
    {
      get
      {
        return m_cbFontSize.FontSize;
      }
      set
      {
        m_cbFontSize.FontSize = value;
      }
    }

    public Altaxo.Graph.Gdi.BrushX FontColor
    {
      get
      {
        return m_cbFontColor.Brush;
      }
      set
      {
        m_cbFontColor.Brush = value;
      }
    }

    public void InsertBeforeAndAfterSelectedText(string insbefore, string insafter)
    {
      if (0 != this.m_edText.SelectionLength)
      {
        // insert \b( at beginning of selection and ) at the end of the selection
        int len = m_edText.Text.Length;
        int start = m_edText.SelectionStart;
        int end = m_edText.SelectionStart + m_edText.SelectionLength;
        m_edText.Text = m_edText.Text.Substring(0, start) + insbefore + m_edText.Text.Substring(start, end - start) + insafter + m_edText.Text.Substring(end, len - end);

        // now select the text plus the text before and after
        m_edText.Focus(); // necassary to show the selected area
        m_edText.Select(start, end - start + insbefore.Length + insafter.Length);
      }
    }

    public void RevertToNormal()
    {
      // remove a backslash x ( at the beginning and the closing brace at the end of the selection
      if (this.m_edText.SelectionLength >= 4)
      {
        int len = m_edText.Text.Length;
        int start = m_edText.SelectionStart;
        int end = m_edText.SelectionStart + m_edText.SelectionLength;

        if (m_edText.Text[start] == '\\' && m_edText.Text[start + 2] == '(' && m_edText.Text[end - 1] == ')')
        {
          m_edText.Text = m_edText.Text.Substring(0, start)
            + m_edText.Text.Substring(start + 3, end - start - 4)
            + m_edText.Text.Substring(end, len - end);

          // now select again the rest of the text
          m_edText.Focus(); // neccessary to show the selected area
          m_edText.Select(start, end - start - 4);
        }
      }
    }

    public void InvalidatePreviewPanel()
    {
      m_pnPreview.Invalidate();
    }

    #endregion

    private void EhSubIndex_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_SubIndexClick();
    }

    private void EhBold_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_BoldClick();
    }

    private void EhItalic_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_ItalicClick();
    }

    private void EhUnderline_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_UnderlineClick();

    }

    private void EhStrikeout_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_StrikeoutClick();

    }

    private void EhSupIndex_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_SupIndexClick();

    }

    private void EhGreek_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_GreekClick();

    }

    private void EhNormal_Click(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_NormalClick();
    }

    private void EhPreviewPanel_Paint(object sender, PaintEventArgs e)
    {
      if (_controller != null)
        _controller.EhView_PreviewPanelPaint(e.Graphics);
    }

    private void EhEditText_TextChanged(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_EditTextChanged();
    }

    private void EhBackgroundStyle_Changed(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_BackgroundStyleChanged();
    }

    private void EhFontFamilyChanged(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_FontFamilyChanged();
    }

    private void EhTextBrush_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_TextFillBrushChanged();
    }

    private void EhFontSize_Changed(object sender, EventArgs e)
    {
      if (_controller != null)
        _controller.EhView_FontSizeChanged();
    }
  }
}
