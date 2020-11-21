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

#nullable disable warnings
using System;
using Altaxo.Gui.Workbench;

namespace Altaxo.Gui.Pads.DataDisplay
{
  public interface IDataDisplayView
  {
    string Text { get; set; }
  }

  /// <summary>
  /// Controls the data display window pad that shows the data obtained from the data reader.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDataDisplayView))]
  public class DataDisplayController : AbstractPadContent, Altaxo.Main.Services.IDataDisplayService
  {
    private IDataDisplayView _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataDisplayController"/> class.
    /// Since this is created as a pad codon, it is not created in a service codon, thus we register it as a service
    /// in this constructor.
    /// </summary>
    public DataDisplayController()
    {
      Current.AddService<Altaxo.Main.Services.IDataDisplayService>(this);
    }

    #region IPadContent Members

    private void AttachView()
    {
    }

    private void DetachView()
    {
    }

    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (!object.ReferenceEquals(_view, value))
        {
          if (_view is not null)
            DetachView();

          _view = value as IDataDisplayView;

          if (_view is not null)
            AttachView();
        }
      }
    }

    public override object ModelObject
    {
      get { return null; }
    }

    #endregion IPadContent Members

    private void InternalWrite(string text)
    {
      _view.Text = text;

      if (!(IsSelected && IsVisible))
      {
        var ww = Current.Workbench.ActiveContent;

        // bring this pad to front
        IsActive = true;
        IsSelected = true;
        IsVisible = true;

        // afterwards, bring originally view content to view
        if (ww is IViewContent)
        {
          ww.IsActive = true;
          ww.IsSelected = true;
        }
      }
    }

    #region IDataDisplayService Members

    /// <summary>Writes a string to the output.</summary>
    /// <param name="text">The text to write to the output.</param>
    public void WriteOneLine(string text)
    {
      InternalWrite(text);
    }

    /// <summary>
    /// Writes two lines to the window.
    /// </summary>
    /// <param name="line1">First line.</param>
    /// <param name="line2">Second line.</param>
    public void WriteTwoLines(string line1, string line2)
    {
      InternalWrite(line1 + System.Environment.NewLine + line2);
    }

    /// <summary>
    /// Writes three lines to the output.
    /// </summary>
    /// <param name="line1">First line.</param>
    /// <param name="line2">Second line.</param>
    /// <param name="line3">Three line.</param>
    public void WriteThreeLines(string line1, string line2, string line3)
    {
      InternalWrite(line1 + System.Environment.NewLine + line2 + System.Environment.NewLine + line3);
    }

    #endregion IDataDisplayService Members
  }
}
