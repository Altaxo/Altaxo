﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Altaxo.Main.Services;

namespace Altaxo.Gui
{
  public static class GuiTransformationExtensions
  {
    #region DPI independence

    public static Rect TransformToDevice(this Rect rect, Visual visual)
    {
      Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
      return Rect.Transform(rect, matrix);
    }

    public static Rect TransformFromDevice(this Rect rect, Visual visual)
    {
      Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
      return Rect.Transform(rect, matrix);
    }

    public static Size TransformToDevice(this Size size, Visual visual)
    {
      Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
      return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
    }

    public static Size TransformFromDevice(this Size size, Visual visual)
    {
      Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
      return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
    }

    public static Point TransformToDevice(this Point point, Visual visual)
    {
      Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
      return matrix.Transform(point);
    }

    public static Point TransformFromDevice(this Point point, Visual visual)
    {
      Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
      return matrix.Transform(point);
    }

    #endregion DPI independence
  }
}
