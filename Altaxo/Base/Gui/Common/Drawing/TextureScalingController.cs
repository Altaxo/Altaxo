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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;
using AUD = Altaxo.Units.Dimensionless;


namespace Altaxo.Gui.Common.Drawing
{
  public interface ITextureScalingView : IDataContextAwareView
  {
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
    }

    #region Binding

    public bool ScalingModeSource
    {
      get => _doc.ScalingMode == TextureScalingMode.Source;
      set
      {
        if(value && !ScalingModeSource)
        {
          _doc = _doc.WithScalingMode(TextureScalingMode.Source);
          OnPropertyChanged(nameof(ScalingModeSource));
          OnPropertyChanged(nameof(ScalingModeDestination));
          OnPropertyChanged(nameof(ScalingModeAbsolute));
          OnPropertyChanged(nameof(ScaleX));
          OnPropertyChanged(nameof(ScaleY));
        }
      }
    }
    public bool ScalingModeDestination
    {
      get => _doc.ScalingMode == TextureScalingMode.Destination;
      set
      {
        if (value && !ScalingModeDestination)
        {
          _doc = _doc.WithScalingMode(TextureScalingMode.Destination);
          OnPropertyChanged(nameof(ScalingModeSource));
          OnPropertyChanged(nameof(ScalingModeDestination));
          OnPropertyChanged(nameof(ScalingModeAbsolute));
          OnPropertyChanged(nameof(ScaleX));
          OnPropertyChanged(nameof(ScaleY));
        }
      }
    }
    public bool ScalingModeAbsolute
    {
      get => _doc.ScalingMode == TextureScalingMode.Absolute;
      set
      {
        if (value && !ScalingModeAbsolute)
        {
          _doc = _doc.WithScalingMode(TextureScalingMode.Absolute);
          OnPropertyChanged(nameof(ScalingModeSource));
          OnPropertyChanged(nameof(ScalingModeDestination));
          OnPropertyChanged(nameof(ScalingModeAbsolute));
          OnPropertyChanged(nameof(SizeX));
          OnPropertyChanged(nameof(SizeY));
        }
      }
    }

    public bool AspectModeNone
    {
      get => _doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.None;
      set
      {
        if (value && !AspectModeNone)
        {
          ChangeAspectPreservingMode(AspectRatioPreservingMode.None);
          OnPropertyChanged(nameof(AspectModeNone));
          OnPropertyChanged(nameof(AspectModeXPriority));
          OnPropertyChanged(nameof(AspectModeYPriority));
        }
      }
    }

    public bool AspectModeXPriority
    {
      get => _doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority;
      set
      {
        if (value && !AspectModeXPriority)
        {
          ChangeAspectPreservingMode(AspectRatioPreservingMode.PreserveXPriority);
          OnPropertyChanged(nameof(AspectModeNone));
          OnPropertyChanged(nameof(AspectModeXPriority));
          OnPropertyChanged(nameof(AspectModeYPriority));
        }
      }
    }

    public bool AspectModeYPriority
    {
      get => _doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority;
      set
      {
        if (value && !AspectModeYPriority)
        {
          ChangeAspectPreservingMode(AspectRatioPreservingMode.PreserveYPriority);
          OnPropertyChanged(nameof(AspectModeNone));
          OnPropertyChanged(nameof(AspectModeXPriority));
          OnPropertyChanged(nameof(AspectModeYPriority));
        }
      }
    }

    private void ChangeAspectPreservingMode(AspectRatioPreservingMode val)
    {
      _doc = _doc.WithSourceAspectRatioPreserving(val);

      if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
      {
        if (_doc.ScalingMode == TextureScalingMode.Absolute)
        {
          if (_sourceTextureSize.HasValue)
          {
            if (_doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority)
            {
              _doc = _doc.WithY(_doc.X * _sourceTextureSize.Value.Y / _sourceTextureSize.Value.X);
              OnPropertyChanged(nameof(SizeY));
            }
            else
            {
              _doc = _doc.WithX(_doc.Y * _sourceTextureSize.Value.X / _sourceTextureSize.Value.Y);
              OnPropertyChanged(nameof(SizeX));
            }
          }
        }
        else
        {
          if (_doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority)
          {
            _doc = _doc.WithY(_doc.X);
            OnPropertyChanged(nameof(ScaleY));
          }
          else
          {
            _doc = _doc.WithX(_doc.Y);
            OnPropertyChanged(nameof(ScaleX));
          }
        }
      }

      OnMadeDirty();
    }

    public QuantityWithUnitGuiEnvironment SizeEnvironment => Altaxo.Gui.SizeEnvironment.Instance;


    public DimensionfulQuantity SizeX
    {
      get => new DimensionfulQuantity(_doc.X, AUL.Point.Instance).AsQuantityIn(SizeEnvironment.DefaultUnit);
      set
      {
        var val = value.AsValueIn(AUL.Point.Instance);
        if (!(_doc.X == val))
        {
          if (_doc.ScalingMode == TextureScalingMode.Absolute)
          {
            _doc = _doc.WithX(val);
            if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None && _sourceTextureSize is not null)
            {
              _doc = _doc.WithY(_doc.X * _sourceTextureSize.Value.Y / _sourceTextureSize.Value.X);
            }
            OnPropertyChanged(nameof(SizeX));
            OnPropertyChanged(nameof(SizeY));
            OnMadeDirty();
          }
        }
      }
    }

    public DimensionfulQuantity SizeY
    {
      get => new DimensionfulQuantity(_doc.Y, AUL.Point.Instance).AsQuantityIn(SizeEnvironment.DefaultUnit);
      set
      {
        var val = value.AsValueIn(AUL.Point.Instance);
        if (!(_doc.Y == val))
        {
          if (_doc.ScalingMode == TextureScalingMode.Absolute)
          {
            _doc = _doc.WithY(val);
            if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None && _sourceTextureSize is not null)
            {
              _doc = _doc.WithX(_doc.Y * _sourceTextureSize.Value.X / _sourceTextureSize.Value.Y);
            }
            OnPropertyChanged(nameof(SizeX));
            OnPropertyChanged(nameof(SizeY));
            OnMadeDirty();
          }
        }
      }
    }

    public QuantityWithUnitGuiEnvironment ScaleXEnvironment { get; set; } = RelationEnvironment.Instance;
   

    public DimensionfulQuantity ScaleX
    {
      get => new DimensionfulQuantity(_doc.X, AUD.Unity.Instance).AsQuantityIn(ScaleXEnvironment.DefaultUnit);
      set
      {
        var val = value.AsValueIn(AUD.Unity.Instance);
        if (!(_doc.X == val))
        {
          if (!(_doc.ScalingMode == TextureScalingMode.Absolute))
          {
            _doc = _doc.WithX(val);
            if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
            {
              _doc = _doc.WithY(_doc.X);
            }
            OnPropertyChanged(nameof(ScaleX));
            OnPropertyChanged(nameof(ScaleY));
            OnMadeDirty();
          }

        }
      }
    }

    public QuantityWithUnitGuiEnvironment ScaleYEnvironment { get; set; } = RelationEnvironment.Instance;

    public DimensionfulQuantity ScaleY
    {
      get => new DimensionfulQuantity(_doc.Y, AUD.Unity.Instance).AsQuantityIn(ScaleYEnvironment.DefaultUnit);
      set
      {
        var val = value.AsValueIn(AUD.Unity.Instance);
        if (!(_doc.Y == val))
        {
          if (!(_doc.ScalingMode == TextureScalingMode.Absolute))
          {
            _doc = _doc.WithY(val);
            if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
            {
              _doc = _doc.WithX(_doc.Y);
            }
            OnPropertyChanged(nameof(ScaleX));
            OnPropertyChanged(nameof(ScaleY));
            OnMadeDirty();
          }
        }
      }
    }

    #endregion Binding

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
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

   

    
  }
}
