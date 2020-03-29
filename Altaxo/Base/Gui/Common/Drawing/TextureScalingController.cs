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
using System.Linq;
using System.Text;
using Altaxo.Geometry;
using Altaxo.Graph;

namespace Altaxo.Gui.Common.Drawing
{
  public interface ITextureScalingView
  {
    TextureScalingMode ScalingMode { get; set; }

    event Action ScalingModeChanged;

    AspectRatioPreservingMode AspectPreserving { get; set; }

    event Action AspectPreservingChanged;

    double XScale { get; set; }

    double YScale { get; set; }

    double XSize { get; set; }

    double YSize { get; set; }

    event Action XChanged;

    event Action YChanged;

    bool ShowSizeNotScale { set; }
  }

  public class TextureScalingController : MVCANDControllerEditImmutableDocBase<TextureScaling, ITextureScalingView>
  {
    /// <summary>
    /// Size of the original texture.
    /// </summary>
    private VectorD2D? _sourceTextureSize;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (null != _view)
      {
        _view.ScalingMode = _doc.ScalingMode;
        _view.AspectPreserving = _doc.SourceAspectRatioPreserving;
        if (_doc.ScalingMode == TextureScalingMode.Absolute)
        {
          _view.XSize = _doc.X;
          _view.YSize = _doc.Y;
          _view.XScale = 1;
          _view.YScale = 1;
          _view.ShowSizeNotScale = true;
        }
        else
        {
          _view.XScale = _doc.X;
          _view.YScale = _doc.Y;
          _view.XSize = 10;
          _view.YSize = 10;
          _view.ShowSizeNotScale = false;
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      _view.ScalingModeChanged += EhScalingModeChanged;
      _view.AspectPreservingChanged += EhAspectPreservingChanged;
      _view.XChanged += EhXChanged;
      _view.YChanged += EhYChanged;

      base.AttachView();
    }

    protected override void DetachView()
    {
      _view.ScalingModeChanged -= EhScalingModeChanged;
      _view.AspectPreservingChanged -= EhAspectPreservingChanged;
      _view.XChanged -= EhXChanged;
      _view.YChanged -= EhYChanged;

      base.DetachView();
    }

    /// <summary>Sets the size of the source texture. This is used by the view to automatically change the value of one size/scale when the value for the other size/scale is changed.</summary>
    /// <value>The size of the source texture.</value>
    public VectorD2D SourceTextureSize
    {
      set
      {
        _sourceTextureSize = value;
      }
    }

    private void EhScalingModeChanged()
    {
      _doc = _doc.WithScalingMode(_view.ScalingMode);
      using (var supp = _suppressDirtyEvent.SuspendGetToken())
      {
        EhXChanged();
        EhYChanged();
        _view.ShowSizeNotScale = (_doc.ScalingMode == TextureScalingMode.Absolute);
      }
      OnMadeDirty();
    }

    private void EhAspectPreservingChanged()
    {
      _doc = _doc.WithSourceAspectRatioPreserving(_view.AspectPreserving);

      if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
      {
        if (_doc.ScalingMode == TextureScalingMode.Absolute)
        {
          if (_doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority && null != _sourceTextureSize)
          {
            _doc = _doc.WithY(_doc.X * _sourceTextureSize.Value.Y / _sourceTextureSize.Value.X);
            _view.YSize = _doc.Y;
          }
          else
          {
            _doc = _doc.WithX(_doc.Y * _sourceTextureSize.Value.X / _sourceTextureSize.Value.Y);
            _view.XSize = _doc.X;
          }
        }
        else
        {
          if (_doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority)
          {
            _doc = _doc.WithY(_doc.X);
            _view.YScale = _doc.Y;
          }
          else
          {
            _doc = _doc.WithX(_doc.Y);
            _view.XScale = _doc.X;
          }
        }
      }

      OnMadeDirty();
    }

    private void EhXChanged()
    {
      if (_doc.ScalingMode == TextureScalingMode.Absolute)
      {
        _doc = _doc.WithX(_view.XSize);
        if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None && null != _sourceTextureSize)
        {
          _doc = _doc.WithY(_doc.X * _sourceTextureSize.Value.Y / _sourceTextureSize.Value.X);
          _view.YSize = _doc.Y;
        }
      }
      else
      {
        _doc = _doc.WithX(_view.XScale);
        if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
        {
          _doc = _doc.WithY(_doc.X);
          _view.YScale = _doc.Y;
        }
      }

      OnMadeDirty();
    }

    private void EhYChanged()
    {
      if (_doc.ScalingMode == TextureScalingMode.Absolute)
      {
        _doc = _doc.WithY(_view.YSize);
        if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None && null != _sourceTextureSize)
        {
          _doc = _doc.WithX(_doc.Y * _sourceTextureSize.Value.X / _sourceTextureSize.Value.Y);
          _view.XSize = _doc.X;
        }
      }
      else
      {
        _doc = _doc.WithY(_view.YScale);
        if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
        {
          _doc = _doc.WithX(_doc.Y);
          _view.XScale = _doc.X;
        }
      }

      OnMadeDirty();
    }
  }
}
