#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;

namespace Altaxo.Gui.Graph.Graph2D.Plot.Styles
{
  /// <summary>
  /// View interface for editing plot color influence options.
  /// </summary>
  public interface IPlotColorInfluenceView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing <see cref="PlotColorInfluence"/> values.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPlotColorInfluenceView))]
  [UserControllerForObject(typeof(PlotColorInfluence))]
  public class PlotColorInfluenceController : MVCANDControllerEditImmutableDocBase<PlotColorInfluence, IPlotColorInfluenceView>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotColorInfluenceController"/> class.
    /// </summary>
    public PlotColorInfluenceController()
    {

    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotColorInfluenceController"/> class.
    /// </summary>
    /// <param name="doc">The initial plot color influence value.</param>
    public PlotColorInfluenceController(PlotColorInfluence doc)
    {
      this.InitializeDocument(doc);
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _FillNone;

    /// <summary>
    /// Gets or sets a value indicating whether the fill color is not influenced by the plot color.
    /// </summary>
    public bool FillNone
    {
      get => _FillNone;
      set
      {
        if (!(_FillNone == value))
        {
          _FillNone = value;
          OnPropertyChanged(nameof(FillNone));
          OnMadeDirty();
        }
      }
    }


    private bool _FillFull;

    /// <summary>
    /// Gets or sets a value indicating whether the fill color fully follows the plot color.
    /// </summary>
    public bool FillFull
    {
      get => _FillFull;
      set
      {
        if (!(_FillFull == value))
        {
          _FillFull = value;
          OnPropertyChanged(nameof(FillFull));
          OnMadeDirty();

        }
      }
    }
    private bool _FillAlpha;

    /// <summary>
    /// Gets or sets a value indicating whether the fill color preserves the plot color alpha channel.
    /// </summary>
    public bool FillAlpha
    {
      get => _FillAlpha;
      set
      {
        if (!(_FillAlpha == value))
        {
          _FillAlpha = value;
          OnPropertyChanged(nameof(FillAlpha));
          OnMadeDirty();

        }
      }
    }
    private bool _FrameNone;

    /// <summary>
    /// Gets or sets a value indicating whether the frame color is not influenced by the plot color.
    /// </summary>
    public bool FrameNone
    {
      get => _FrameNone;
      set
      {
        if (!(_FrameNone == value))
        {
          _FrameNone = value;
          OnPropertyChanged(nameof(FrameNone));
          OnMadeDirty();

        }
      }
    }


    private bool _FrameFull;

    /// <summary>
    /// Gets or sets a value indicating whether the frame color fully follows the plot color.
    /// </summary>
    public bool FrameFull
    {
      get => _FrameFull;
      set
      {
        if (!(_FrameFull == value))
        {
          _FrameFull = value;
          OnPropertyChanged(nameof(FrameFull));
          OnMadeDirty();

        }
      }
    }
    private bool _FrameAlpha;

    /// <summary>
    /// Gets or sets a value indicating whether the frame color preserves the plot color alpha channel.
    /// </summary>
    public bool FrameAlpha
    {
      get => _FrameAlpha;
      set
      {
        if (!(_FrameAlpha == value))
        {
          _FrameAlpha = value;
          OnPropertyChanged(nameof(FrameAlpha));
          OnMadeDirty();
        }
      }
    }

    private bool _InsetNone;

    /// <summary>
    /// Gets or sets a value indicating whether the inset color is not influenced by the plot color.
    /// </summary>
    public bool InsetNone
    {
      get => _InsetNone;
      set
      {
        if (!(_InsetNone == value))
        {
          _InsetNone = value;
          OnPropertyChanged(nameof(InsetNone));
          OnMadeDirty();
        }
      }
    }


    private bool _InsetAlpha;

    /// <summary>
    /// Gets or sets a value indicating whether the inset color preserves the plot color alpha channel.
    /// </summary>
    public bool InsetAlpha
    {
      get => _InsetAlpha;
      set
      {
        if (!(_InsetAlpha == value))
        {
          _InsetAlpha = value;
          OnPropertyChanged(nameof(InsetAlpha));
          OnMadeDirty();
        }
      }
    }
    private bool _InsetFull;

    /// <summary>
    /// Gets or sets a value indicating whether the inset color fully follows the plot color.
    /// </summary>
    public bool InsetFull
    {
      get => _InsetFull;
      set
      {
        if (!(_InsetFull == value))
        {
          _InsetFull = value;
          OnPropertyChanged(nameof(InsetFull));
          OnMadeDirty();
        }
      }
    }


    #endregion

    /// <summary>
    /// Gets or sets the currently edited plot color influence value.
    /// </summary>
    public PlotColorInfluence Doc
    {
      get
      {
        return _doc;
      }
      set
      {
        _doc = value;
        SelectedValue = _doc;
      }

    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        SelectedValue = _doc;
      }
    }

    /// <inheritdoc />
    protected override void OnMadeDirty()
    {
      _doc = SelectedValue;
      base.OnMadeDirty();
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = SelectedValue;
      return ApplyEnd(true, disposeController);
    }


    /// <summary>
    /// Gets or sets the plot color influence represented by the current controller state.
    /// </summary>
    public PlotColorInfluence SelectedValue
    {
      get
      {
        PlotColorInfluence result = PlotColorInfluence.None;

        if (FillAlpha == true)
          result |= PlotColorInfluence.FillColorPreserveAlpha;
        if (FillFull == true)
          result |= PlotColorInfluence.FillColorFull;

        if (FrameAlpha == true)
          result |= PlotColorInfluence.FrameColorPreserveAlpha;
        if (FrameFull == true)
          result |= PlotColorInfluence.FrameColorFull;

        if (InsetAlpha == true)
          result |= PlotColorInfluence.InsetColorPreserveAlpha;
        if (InsetFull == true)
          result |= PlotColorInfluence.InsetColorFull;

        return result;
      }
      set
      {
        if (value.HasFlag(PlotColorInfluence.FillColorFull))
          FillFull = true;
        else if (value.HasFlag(PlotColorInfluence.FillColorPreserveAlpha))
          FillAlpha = true;
        else
          FillNone = true;

        if (value.HasFlag(PlotColorInfluence.FrameColorFull))
          FrameFull = true;
        else if (value.HasFlag(PlotColorInfluence.FrameColorPreserveAlpha))
          FrameAlpha = true;
        else
          FrameNone = true;

        if (value.HasFlag(PlotColorInfluence.InsetColorFull))
          InsetFull = true;
        else if (value.HasFlag(PlotColorInfluence.InsetColorPreserveAlpha))
          InsetAlpha = true;
        else
          InsetNone = true;
      }
    }
  }
}
