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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
	public interface IBrushViewAdvanced
	{
		BrushType BrushType { get; set; }
		event Action BrushTypeChanged;

		NamedColor ForeColor { get; set; }
		event Action ForeColorChanged;
		void ForeColorEnable(bool enable);

    bool RestrictBrushColorToPlotColorsOnly { set; }

		NamedColor BackColor { get; set; }
		event Action BackColorChanged;
		void BackColorEnable(bool enable);

		bool ExchangeColors { get; set; }
		event Action ExchangeColorsChanged;
		void ExchangeColorsEnable(bool enable);

		System.Drawing.Drawing2D.WrapMode WrapMode { get; set; }
		event Action WrapModeChanged;
		void WrapModeEnable(bool enable);

		double GradientFocus { get; set; }
		event Action GradientFocusChanged;
		void GradientFocusEnable(bool enable);

		double GradientColorScale { get; set; }
		event Action GradientColorScaleChanged;
		void GradientColorScaleEnable(bool enable);

		double GradientAngle { get; set; }
		event Action GradientAngleChanged;
		void GradientAngleEnable(bool enable);

		double TextureOffsetX { get; set; }
		event Action TextureOffsetXChanged;
		void TextureOffsetXEnable(bool enable);

		double TextureOffsetY { get; set; }
		event Action TextureOffsetYChanged;
		void TextureOffsetYEnable(bool enable);

		void InitTextureImage(ImageProxy proxy, BrushType imageType);
		ImageProxy TextureImage { get;}
		event Action TextureImageChanged;
		void TextureImageEnable(bool enable);

		Main.IInstancePropertyView AdditionalPropertiesView { get; }

		ITextureScalingView TextureScalingView { get; }
		void TextureScalingViewEnable(bool enable);

		void UpdatePreview(BrushX brush);
		event Action PreviewPanelSizeChanged;
	}


	[UserControllerForObject(typeof(BrushX))]
	[ExpectedTypeOfView(typeof(IBrushViewAdvanced))]
	public class BrushControllerAdvanced : MVCANDControllerBase<BrushX,IBrushViewAdvanced>
	{
		Main.InstancePropertyController _imageProxyController;

		TextureScalingController _textureScalingController;
    bool _restrictBrushColorToPlotColorsOnly;

    public bool RestrictBrushColorToPlotColorsOnly
    {
      get
      {
        return _restrictBrushColorToPlotColorsOnly;
      }
      set
      {
        var oldValue = _restrictBrushColorToPlotColorsOnly;
        _restrictBrushColorToPlotColorsOnly = value;
        if (value != oldValue && null != _view)
        {
          _view.RestrictBrushColorToPlotColorsOnly = _restrictBrushColorToPlotColorsOnly;
        }
      }
    }



		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_imageProxyController = new Main.InstancePropertyController() { UseDocumentCopy = UseDocument.Directly };
				_imageProxyController.MadeDirty += EhAdditionalPropertiesChanged;
				_imageProxyController.InitializeDocument(_doc.TextureImage);

				_textureScalingController = new TextureScalingController() { UseDocumentCopy = UseDocument.Directly };
				_textureScalingController.MadeDirty += EhTextureScalingChanged;
				_textureScalingController.InitializeDocument(_doc.TextureScale);
				if (null != _doc.TextureImage)
					_textureScalingController.SourceTextureSize = GetSizeOfImageProxy(_doc.TextureImage);
			}

			if (null != _view)
			{
        _view.RestrictBrushColorToPlotColorsOnly = _restrictBrushColorToPlotColorsOnly;

				_view.BrushType = _doc.BrushType;
				InitializeViewElementsWhenBrushTypeChanged();
				
				_imageProxyController.ViewObject = _view.AdditionalPropertiesView;
				_textureScalingController.ViewObject = _view.TextureScalingView;
				
				EnableElementsInDependenceOnBrushType();
				_view.UpdatePreview(_doc);
			}
		}


		void InitializeViewElementsWhenBrushTypeChanged()
		{
			using (var suppressor = _suppressDirtyEvent.SuspendGetToken())
			{
				_view.ForeColor = _doc.Color;
				_view.BackColor = _doc.BackColor;
				_view.ExchangeColors = _doc.ExchangeColors;
				_view.WrapMode = _doc.WrapMode;
				_view.GradientFocus = _doc.GradientFocus;
				_view.GradientColorScale = _doc.GradientColorScale;
				_view.GradientAngle = _doc.GradientAngle;
				_view.TextureOffsetX = _doc.TextureOffsetX;
				_view.TextureOffsetY = _doc.TextureOffsetY;
				_view.InitTextureImage(_doc.TextureImage, _doc.BrushType);
			}
		}


		protected override void AttachView()
		{
			_view.BrushTypeChanged += EhBrushTypeChanged;
			_view.ForeColorChanged += EhForeColorChanged;
			_view.BackColorChanged += EhBackColorChanged;
			_view.ExchangeColorsChanged += EhExchangeColorsChanged;
			_view.WrapModeChanged += EhWrapModeChanged;
			_view.GradientFocusChanged += EhGradientFocusChanged;
			_view.GradientColorScaleChanged += EhGradientScaleChanged;
			_view.GradientAngleChanged += EhGradientAngleChanged;
			_view.TextureOffsetXChanged += EhTextureOffsetXChanged;
			_view.TextureOffsetYChanged += EhTextureOffsetYChanged;
			_view.TextureImageChanged += EhTextureImageChanged;
			_view.PreviewPanelSizeChanged += EhPreviewPanelSizeChanged;

			base.AttachView();
		}

		protected override void DetachView()
		{
			_view.BrushTypeChanged -= EhBrushTypeChanged;
			_view.ForeColorChanged -= EhForeColorChanged;
			_view.BackColorChanged -= EhBackColorChanged;
			_view.ExchangeColorsChanged -= EhExchangeColorsChanged;
			_view.WrapModeChanged -= EhWrapModeChanged;
			_view.GradientFocusChanged -= EhGradientFocusChanged;
			_view.GradientColorScaleChanged -= EhGradientScaleChanged;
			_view.GradientAngleChanged -= EhGradientAngleChanged;
			_view.TextureOffsetXChanged -= EhTextureOffsetXChanged;
			_view.TextureOffsetYChanged -= EhTextureOffsetYChanged;
			_view.TextureImageChanged -= EhTextureImageChanged;
			_view.PreviewPanelSizeChanged -= EhPreviewPanelSizeChanged;

			base.DetachView();
		}

		public override bool Apply()
		{
			_originalDoc = (BrushX)_doc.Clone();
			return true;
		}

		protected override void OnMadeDirty()
		{
			base.OnMadeDirty();

			
			if (_suppressDirtyEvent.PeekEnabled && null != _view)
				_view.UpdatePreview(_doc);

		}

		#region Other helper functions

		void EnableElementsInDependenceOnBrushType()
		{
			bool foreColor = false, backColor = false, exchangeColor = false, wrapMode = false,
				gradientFocus = false, gradientColorScale = false, gradientAngle = false,
				textureScale = false, textureImage = false, textureOffsetX=false, textureOffsetY=false, additionalProperties = false;

			switch (_doc.BrushType)
			{
				case BrushType.SolidBrush:
					foreColor = true;
					break;
				case BrushType.LinearGradientBrush:
				case BrushType.SigmaBellShapeLinearGradientBrush:
				case BrushType.TriangularShapeLinearGradientBrush:
					foreColor = true;
					backColor = true;
					exchangeColor = true;
					wrapMode = true;
					gradientAngle = true;
					if (_doc.BrushType != BrushType.LinearGradientBrush)
					{
						gradientFocus = true;
						gradientColorScale = true;
					}
					break;
				case BrushType.PathGradientBrush:
				case BrushType.SigmaBellShapePathGradientBrush:
				case BrushType.TriangularShapePathGradientBrush:
					foreColor = true;
					backColor = true;
					exchangeColor = true;
					wrapMode = true;
					textureOffsetX = true;
					textureOffsetY = true;
					if (_doc.BrushType != BrushType.PathGradientBrush)
					{
						gradientColorScale = true;
					}
					break;
				case BrushType.HatchBrush:
				case BrushType.SyntheticTextureBrush:
					foreColor = true;
					backColor = true;
					exchangeColor = true;
					wrapMode = true;
					gradientAngle = true;
					textureScale = true;
					textureImage = true;
					textureOffsetX = true;
					textureOffsetY = true;
					break;
				case BrushType.TextureBrush:
					wrapMode = true;
					gradientAngle = true;
					textureScale = true;
					textureImage = true;
					textureOffsetX = true;
					textureOffsetY = true;
					break;
			}
			_view.ForeColorEnable( foreColor);
			_view.BackColorEnable( backColor);
			_view.ExchangeColorsEnable( exchangeColor);
			_view.WrapModeEnable( wrapMode);
			_view.GradientFocusEnable(gradientFocus);
			_view.GradientColorScaleEnable(gradientColorScale);
			_view.GradientAngleEnable(gradientAngle);
			_view.TextureScalingViewEnable(textureScale);
			_view.TextureOffsetXEnable(textureOffsetX);
			_view.TextureOffsetYEnable(textureOffsetY);
			_view.TextureImageEnable(textureImage);
			//_view.AdditionalPropertiesView
		}

		private PointD2D GetSizeOfImageProxy(ImageProxy proxy)
		{
			if (proxy is ISyntheticRepeatableTexture)
			{
				return ((ISyntheticRepeatableTexture)proxy).Size;
			}
			else
			{
				var img = proxy.GetImage();
				return new PointD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
			}
		}

		#endregion

		#region Event handlers

		void EhBrushTypeChanged()
		{
			_doc.BrushType = _view.BrushType;
			InitializeViewElementsWhenBrushTypeChanged();
			EnableElementsInDependenceOnBrushType();
			OnMadeDirty();
		}

		void EhForeColorChanged()
		{
			_doc.Color = _view.ForeColor;
			OnMadeDirty();
		}

		void EhBackColorChanged()
		{
			_doc.BackColor = _view.BackColor;
			OnMadeDirty();
		}

		void EhExchangeColorsChanged()
		{
			_doc.ExchangeColors = _view.ExchangeColors;
      _view.RestrictBrushColorToPlotColorsOnly = _restrictBrushColorToPlotColorsOnly; 
			OnMadeDirty();
		}

		void EhWrapModeChanged()
		{
			_doc.WrapMode = _view.WrapMode;
			OnMadeDirty();
		}

		void EhGradientFocusChanged()
		{
			_doc.GradientFocus = (float)_view.GradientFocus;
			OnMadeDirty();
		}

		void EhGradientScaleChanged()
		{
			_doc.GradientColorScale = (float)_view.GradientColorScale;
			OnMadeDirty();
		}

		void EhGradientAngleChanged()
		{
			_doc.GradientAngle = _view.GradientAngle;
			OnMadeDirty();
		}

		void EhTextureOffsetXChanged()
		{
			_doc.TextureOffsetX = _view.TextureOffsetX;
			OnMadeDirty();
		}

		void EhTextureOffsetYChanged()
		{
			_doc.TextureOffsetY = _view.TextureOffsetY;
			OnMadeDirty();
		}

		void EhTextureScaleChanged()
		{
			//_doc.TextureScale = (float)_view.TextureScale;
			OnMadeDirty();
		}

		void EhTextureImageChanged()
		{
			
			var oldTexture = _doc.TextureImage;
			var newTexture = _view.TextureImage;
			if(newTexture is Altaxo.Main.ICopyFrom)
				((Altaxo.Main.ICopyFrom)newTexture).CopyFrom(oldTexture); // Try to keep the settings from the old texture

			_doc.TextureImage = newTexture;
			_imageProxyController.InitializeDocument(_doc.TextureImage);
			if(null!=_doc.TextureImage)
				_textureScalingController.SourceTextureSize = GetSizeOfImageProxy(_doc.TextureImage);
			OnMadeDirty();
		}

		void EhAdditionalPropertiesChanged(IMVCANController ctrl)
		{
			_doc.TextureImage = (ImageProxy)_imageProxyController.ProvisionalModelObject;
			_doc.InvalidateCachedBrush(); // we have to manully invalidate the brush since the brush is not aware off that only some members of the imageproxy have changed
			OnMadeDirty();
		}

		void EhTextureScalingChanged(IMVCANController ctrl)
		{
			_doc.TextureScale = (TextureScaling)_textureScalingController.ProvisionalModelObject;
			OnMadeDirty();
		}

		void EhPreviewPanelSizeChanged()
		{
			_view.UpdatePreview(_doc);
		}

		#endregion

	}
}
