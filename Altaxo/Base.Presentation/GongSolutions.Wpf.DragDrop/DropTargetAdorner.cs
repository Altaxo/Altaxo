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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Provides a base class for visual adorners that indicate the current drop target.
  /// </summary>
  public abstract class DropTargetAdorner : Adorner
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DropTargetAdorner"/> class.
    /// </summary>
    /// <param name="adornedElement">The element to adorn.</param>
    public DropTargetAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
      m_AdornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
      m_AdornerLayer.Add(this);
      IsHitTestVisible = false;
    }

    /// <summary>
    /// Removes this adorner from its adorner layer.
    /// </summary>
    public void Detatch()
    {
      m_AdornerLayer.Remove(this);
    }

    /// <summary>
    /// Gets or sets the drop information displayed by this adorner.
    /// </summary>
    public DropInfo DropInfo { get; set; }

    /// <summary>
    /// Creates a drop target adorner of the specified type.
    /// </summary>
    /// <param name="type">The adorner type.</param>
    /// <param name="adornedElement">The element to adorn.</param>
    /// <returns>The created adorner.</returns>
    internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
    {
      if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
      {
        throw new InvalidOperationException(
          "The requested adorner class does not derive from DropTargetAdorner.");
      }

      return (DropTargetAdorner)type.GetConstructor(new[] { typeof(UIElement) })
                                    .Invoke(new[] { adornedElement });
    }

    private readonly AdornerLayer m_AdornerLayer;
  }
}
