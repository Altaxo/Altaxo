// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// Originated from: SharpDevelop, AvalonEdit.Addin, src/TextMarkerService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace Altaxo.Gui.CodeEditing.TextMarkerHandling
{
  public class TextMarker : TextSegment
  {
    private readonly TextMarkerService _service;

    public TextMarker(TextMarkerService service, int startOffset, int length)
    {
      if (service == null)
        throw new ArgumentNullException(nameof(service));
      _service = service;
      StartOffset = startOffset;
      Length = length;
    }

    public event EventHandler Deleted;

    public bool IsDeleted => !IsConnectedToCollection;

    public void Delete()
    {
      _service.Remove(this);
    }

    internal void OnDeleted()
    {
      Deleted?.Invoke(this, EventArgs.Empty);
    }

    private void Redraw()
    {
      _service.Redraw(this);
    }

    private Color? _backgroundColor;

    public Color? BackgroundColor
    {
      get { return _backgroundColor; }
      set
      {
        if (_backgroundColor != value)
        {
          _backgroundColor = value;
          Redraw();
        }
      }
    }

    private Color? _foregroundColor;

    public Color? ForegroundColor
    {
      get { return _foregroundColor; }
      set
      {
        if (_foregroundColor != value)
        {
          _foregroundColor = value;
          Redraw();
        }
      }
    }

    private FontWeight? _fontWeight;

    public FontWeight? FontWeight
    {
      get { return _fontWeight; }
      set
      {
        if (_fontWeight != value)
        {
          _fontWeight = value;
          Redraw();
        }
      }
    }

    private FontStyle? _fontStyle;

    public FontStyle? FontStyle
    {
      get { return _fontStyle; }
      set
      {
        if (_fontStyle != value)
        {
          _fontStyle = value;
          Redraw();
        }
      }
    }

    public object Tag { get; set; }

    private Color _markerColor;

    public Color MarkerColor
    {
      get { return _markerColor; }
      set
      {
        if (_markerColor != value)
        {
          _markerColor = value;
          Redraw();
        }
      }
    }

    public object ToolTip { get; set; }
  }
}
