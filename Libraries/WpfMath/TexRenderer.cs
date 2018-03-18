﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMath.Rendering;

namespace WpfMath
{
	public class TexRenderer
	{
		internal TexRenderer(Box box, double scale)
		{
			this.Box = box;
			this.Scale = scale;
		}

		public Box Box
		{
			get;
			set;
		}

		public double Scale
		{
			get;
			private set;
		}

		public Size RenderSize
		{
			get
			{
				return new Size(this.Box.TotalWidth * this.Scale, this.Box.TotalHeight * this.Scale);
			}
		}

		public double Baseline
		{
			get
			{
				return this.Box.Height / this.Box.TotalHeight * this.Scale;
			}
		}

		public double RelativeDepth
		{
			get
			{
				return this.Box.Depth / this.Box.TotalHeight;
			}
		}

		public void RenderFormulaTo(IElementRenderer renderer, double x, double y) =>
						renderer.RenderElement(Box, x / Scale, y / Scale + Box.Height);

		public Geometry RenderToGeometry(double x, double y)
		{
			var geometry = new GeometryGroup();
			var renderer = new GeometryElementRenderer(geometry, Scale);
			RenderFormulaTo(renderer, x, y);
			return geometry;
		}

		public BitmapSource RenderToBitmap(double x, double y)
		{
			var visual = new DrawingVisual();
			using (var drawingContext = visual.RenderOpen())
				this.Render(drawingContext, 0, 0);

			var width = (int)Math.Ceiling(this.RenderSize.Width);
			var height = (int)Math.Ceiling(this.RenderSize.Height);
			var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
			bitmap.Render(visual);

			return bitmap;
		}

		public void Render(DrawingContext drawingContext, double x, double y) =>
				RenderFormulaTo(new WpfElementRenderer(drawingContext, Scale), x, y);
	}
}
