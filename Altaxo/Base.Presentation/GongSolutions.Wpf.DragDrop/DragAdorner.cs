#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
// BSD 3-Clause License
//
// Copyright(c) 2015-16, Jan Karger(Steven Kirk)
//
// All rights reserved.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Displays a drag adorner that follows the mouse pointer.
  /// </summary>
  internal class DragAdorner : Adorner
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DragAdorner"/> class.
    /// </summary>
    /// <param name="adornedElement">The element to adorn.</param>
    /// <param name="adornment">The visual shown by the adorner.</param>
    public DragAdorner(UIElement adornedElement, UIElement adornment)
      : base(adornedElement)
    {
      m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
      m_AdornerLayer.Add(this);
      m_Adornment = adornment;
      IsHitTestVisible = false;
    }

    /// <summary>
    /// Gets or sets the current mouse position used to place the adorner.
    /// </summary>
    public Point MousePosition
    {
      get { return m_MousePosition; }
      set
      {
        if (m_MousePosition != value)
        {
          m_MousePosition = value;
          m_AdornerLayer.Update(AdornedElement);
        }
      }
    }

    /// <summary>
    /// Removes this adorner from its adorner layer.
    /// </summary>
    public void Detatch()
    {
      m_AdornerLayer.Remove(this);
    }

    /// <summary>
    /// Arranges the adornment within the available size.
    /// </summary>
    /// <param name="finalSize">The final size assigned to the adorner.</param>
    /// <returns>The arranged size.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      m_Adornment.Arrange(new Rect(finalSize));
      return finalSize;
    }

    /// <summary>
    /// Gets the transform used to position the adornment relative to the mouse pointer.
    /// </summary>
    /// <param name="transform">The base transform.</param>
    /// <returns>The composed transform.</returns>
    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
      var result = new GeneralTransformGroup();
      result.Children.Add(base.GetDesiredTransform(transform));
      result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));

      return result;
    }

    /// <summary>
    /// Gets the single visual child used by the adorner.
    /// </summary>
    /// <param name="index">The child index.</param>
    /// <returns>The adornment visual.</returns>
    protected override Visual GetVisualChild(int index)
    {
      return m_Adornment;
    }

    /// <summary>
    /// Measures the adornment using the available size.
    /// </summary>
    /// <param name="constraint">The available size.</param>
    /// <returns>The desired size of the adornment.</returns>
    protected override Size MeasureOverride(Size constraint)
    {
      m_Adornment.Measure(constraint);
      return m_Adornment.DesiredSize;
    }

    /// <summary>
    /// Gets the number of visual children.
    /// </summary>
    protected override int VisualChildrenCount
    {
      get { return 1; }
    }

    private readonly AdornerLayer m_AdornerLayer;
    private readonly UIElement m_Adornment;
    private Point m_MousePosition;
  }
}
