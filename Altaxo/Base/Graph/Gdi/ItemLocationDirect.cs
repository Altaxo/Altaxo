#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2013 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Represents an item location using direct coordinates, anchors, and transformations.
  /// </summary>
  [Serializable]
  public class ItemLocationDirect : Main.SuspendableDocumentLeafNodeWithEventArgs, IItemLocation
  {
    #region Members

    /// <summary>
    /// The width specification.
    /// </summary>
    protected RADouble _sizeX;

    /// <summary>
    /// The height specification.
    /// </summary>
    protected RADouble _sizeY;

    /// <summary>
    /// The X position specification.
    /// </summary>
    protected RADouble _positionX;

    /// <summary>
    /// The Y position specification.
    /// </summary>
    protected RADouble _positionY;

    /// <summary>
    /// The local X anchor specification.
    /// </summary>
    protected RADouble _localAnchorX;

    /// <summary>
    /// The local Y anchor specification.
    /// </summary>
    protected RADouble _localAnchorY;

    /// <summary>
    /// The parent X anchor specification.
    /// </summary>
    protected RADouble _parentAnchorX;

    /// <summary>
    /// The parent Y anchor specification.
    /// </summary>
    protected RADouble _parentAnchorY;

    /// <summary>The rotation angle (in degrees) of the layer.</summary>
    protected double _rotation; // Rotation

    /// <summary>
    /// The shear factor.
    /// </summary>
    protected double _shear; // Shear

    /// <summary>The scaling factor of the layer, normally 1.</summary>
    protected double _scaleX;  // X-Scale

    /// <summary>
    /// The Y scale factor.
    /// </summary>
    protected double _scaleY; // Y-Scale

    // Cached and not-to-serialize members

    /// <summary>
    /// The cached parent size.
    /// </summary>
    protected PointD2D _parentSize;

    #endregion Members

    #region Serialization

    /// <summary>
    /// 2013-10-01 initial version.
    /// 2015-11-14 Version 1 Moved to Altaxo.Graph.Gdi namespace.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ItemLocationDirect", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ItemLocationDirect), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ItemLocationDirect)o;

        info.AddValue("ParentSize", s._parentSize);

        info.AddValue("SizeX", s._sizeX);
        info.AddValue("SizeY", s._sizeY);

        info.AddValue("PositionX", s._positionX);
        info.AddValue("PositionY", s._positionY);

        info.AddValue("LocalAnchorX", s._localAnchorX);
        info.AddValue("LocalAnchorY", s._localAnchorY);

        info.AddValue("ParentAnchorX", s._parentAnchorX);
        info.AddValue("ParentAnchorY", s._parentAnchorY);

        info.AddValue("Rotation", s._rotation);
        info.AddValue("ShearX", s._shear);
        info.AddValue("ScaleX", s._scaleX);
        info.AddValue("ScaleY", s._scaleY);
      }

      protected virtual ItemLocationDirect SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ItemLocationDirect?)o ?? new ItemLocationDirect();

        s._parentSize = (PointD2D)info.GetValue("ParentSize", s);

        s._sizeX = (RADouble)info.GetValue("SizeX", s);
        s._sizeY = (RADouble)info.GetValue("SizeY", s);

        s._positionX = (RADouble)info.GetValue("PositionX", s);
        s._positionY = (RADouble)info.GetValue("PositionY", s);

        s._localAnchorX = (RADouble)info.GetValue("LocalAnchorX", s);
        s._localAnchorY = (RADouble)info.GetValue("LocalAnchorY", s);

        s._parentAnchorX = (RADouble)info.GetValue("ParentAnchorX", s);
        s._parentAnchorY = (RADouble)info.GetValue("ParentAnchorY", s);

        s._rotation = info.GetDouble("Rotation");
        s._shear = info.GetDouble("ShearX");
        s._scaleX = info.GetDouble("ScaleX");
        s._scaleY = info.GetDouble("ScaleY");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ItemLocationDirect s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Serialization

    #region Construction and copying

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemLocationDirect"/> class.
    /// </summary>
    public ItemLocationDirect()
    {
      _localAnchorX = RADouble.NewRel(0);
      _localAnchorY = RADouble.NewRel(0);
      _parentAnchorX = RADouble.NewRel(0);
      _parentAnchorY = RADouble.NewRel(0);
      _scaleX = 1;
      _scaleY = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemLocationDirect"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public ItemLocationDirect(ItemLocationDirect from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemLocationDirect"/> class from another item location.
    /// </summary>
    /// <param name="from">The source item location.</param>
    public ItemLocationDirect(IItemLocation from)
    {
      CopyFrom(from);
    }

    /// <inheritdoc />
    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is ItemLocationDirect)
      {
        var from = (ItemLocationDirect)obj;
        _parentSize = from._parentSize;
        _positionX = from._positionX;
        _positionY = from._positionY;
        _sizeX = from._sizeX;
        _sizeY = from._sizeY;
        _localAnchorX = from._localAnchorX;
        _localAnchorY = from._localAnchorY;
        _parentAnchorX = from._parentAnchorX;
        _parentAnchorY = from._parentAnchorY;
        _rotation = from._rotation;
        _scaleX = from._scaleX;
        _scaleY = from._scaleY;
        _shear = from._shear;
        EhSelfChanged();
        return true;
      }
      else if (obj is IItemLocation)
      {
        var from = (IItemLocation)obj;
        _rotation = from.Rotation;
        _shear = from.ShearX;
        _scaleX = from.ScaleX;
        _scaleY = from.ScaleY;
        EhSelfChanged();
        return true;
      }

      return false;
    }

    /// <inheritdoc />
    object System.ICloneable.Clone()
    {
      return new ItemLocationDirect(this);
    }

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public virtual ItemLocationDirect Clone()
    {
      return new ItemLocationDirect(this);
    }

    #endregion Construction and copying

    #region Properties

    /// <summary>
    /// Sets the parent size used to resolve relative values.
    /// </summary>
    /// <param name="parentSize">The parent size.</param>
    /// <param name="shouldTriggerChangedEvent">If set to <c>true</c>, raises a changed event when the value changes.</param>
    public virtual void SetParentSize(PointD2D parentSize, bool shouldTriggerChangedEvent)
    {
      var oldValue = _parentSize;
      _parentSize = parentSize;

      if (shouldTriggerChangedEvent && oldValue != _parentSize)
        EhSelfChanged();
    }

    /// <summary>
    /// Gets the current parent size used to resolve relative values.
    /// </summary>
    public PointD2D ParentSize
    {
      get
      {
        return _parentSize;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the this location belongs to a graphical element which is auto sized.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [can set size]; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsAutoSized
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// The width of the item, either as absolute value in point (1/72 inch), or as
    /// value relative to the parent's width.
    /// </summary>
    public virtual RADouble SizeX
    {
      get { return _sizeX; }
      set
      {
        var chg = _sizeX != value;
        InternalSetSizeXSilent(value);

        if (chg)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// The height of the item, either as absolute value in point (1/72 inch), or as
    /// value relative to the parent's height.
    /// </summary>
    public virtual RADouble SizeY
    {
      get { return _sizeY; }
      set
      {
        var chg = _sizeY != value;
        InternalSetSizeYSilent(value);

        if (chg)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// The x position of the item, either absolute in points or relative to the parent's width.
    /// </summary>
    public RADouble PositionX
    {
      get { return _positionX; }
      set
      {
        var oldvalue = _positionX;
        _positionX = value;

        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// The y position of the item, either absolute or relative to the parent's height.
    /// </summary>
    public RADouble PositionY
    {
      get { return _positionY; }
      set
      {
        var oldvalue = _positionY;
        _positionY = value;

        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the local anchor's x position. This is an absolute value (in points = 1/72 inch) or a value relative to the own item width.
    /// The local anchor point is the point, for which the location can be set in the position/size dialog of the item.
    /// A relative value of 0 designates the left boundary of the item, a relative value of 0.5 designates the horizontal center of the item, and a relative value of 1 designates the right boundary of the item.
    /// </summary>
    /// <value>
    /// The local anchor's x position.
    /// </value>
    public RADouble LocalAnchorX
    {
      get { return _localAnchorX; }
      set
      {
        var oldvalue = _localAnchorX;
        _localAnchorX = value;

        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the local anchor's y position. This is an absolute value (in points = 1/72 inch) or a value relative to the own item height.
    /// The local anchor point is the point, for which the location can be set in the position/size dialog of the item.
    /// A relative value of 0 designates the upper boundary of the item, a relative value of 0.5 designates the vertical center of the item, and a relative value of 1 designates the lower boundary of the item.
    /// </summary>
    /// <value>
    /// The local anchor's x position.
    /// </value>
    public RADouble LocalAnchorY
    {
      get { return _localAnchorY; }
      set
      {
        var oldvalue = _localAnchorY;
        _localAnchorY = value;

        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the parent anchor's x position. This is an absolute value (in points = 1/72 inch) or a value relative to the parent's width.
    /// The parent anchor point is the point inside the parent item, from which the location of the item is measured. Stricly speaking, the position of the item (as shown in the dialog) is the vector from
    /// the parent's anchor point to the local anchor point).
    /// A relative value of 0 designates the left boundary of the parent, a relative value of 0.5 designates the horizontal center of the parent, and a relative value of 1 designates the right boundary of the parent.
    /// </summary>
    /// <value>
    /// The parent anchor's x position.
    /// </value>
    public RADouble ParentAnchorX
    {
      get { return _parentAnchorX; }
      set
      {
        var oldvalue = _parentAnchorX;
        _parentAnchorX = value;

        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the parent anchor's y position. This is an absolute value (in points = 1/72 inch) or a value relative to the parent's height.
    /// The parent anchor point is the point inside the parent item, from which the location of the item is measured. Stricly speaking, the position of the item (as shown in the dialog) is the vector from
    /// the parent's anchor point to the local anchor point).
    /// A relative value of 0 designates the upper boundary of the parent, a relative value of 0.5 designates the vertical center of the parent, and a relative value of 1 designates the lower boundary of the parent.
    /// </summary>
    /// <value>
    /// The parent anchor's y position.
    /// </value>
    public RADouble ParentAnchorY
    {
      get { return _parentAnchorY; }
      set
      {
        var oldvalue = _parentAnchorY;
        _parentAnchorY = value;

        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Sets the position and size.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public virtual void SetPositionAndSize(RADouble x, RADouble y, RADouble width, RADouble height)
    {
      bool isChanged = x != _positionX || y != _positionY || width != _sizeX || height != _sizeY;

      _positionX = x;
      _positionY = y;

      InternalSetSizeSilent(width, height);

      if (isChanged)
        EhSelfChanged();
    }

    /// <summary>
    /// Sets the x-axis scale factor without raising a change event.
    /// </summary>
    /// <param name="value">The new x-axis scale factor.</param>
    /// <returns><see langword="true"/> if the scale changed; otherwise, <see langword="false"/>.</returns>
    protected virtual bool InternalSetScaleXSilent(double value)
    {
      bool chg = _scaleX != value;
      _scaleX = value;
      return chg;
    }

    /// <summary>
    /// Sets the y-axis scale factor without raising a change event.
    /// </summary>
    /// <param name="value">The new y-axis scale factor.</param>
    /// <returns><see langword="true"/> if the scale changed; otherwise, <see langword="false"/>.</returns>
    protected virtual bool InternalSetScaleYSilent(double value)
    {
      bool chg = _scaleY != value;
      _scaleY = value;
      return chg;
    }

    /// <summary>
    /// Sets both scale factors without raising a change event.
    /// </summary>
    /// <param name="value">The new scale factors.</param>
    /// <returns><see langword="true"/> if either scale factor changed; otherwise, <see langword="false"/>.</returns>
    protected virtual bool InternalSetScaleSilent(PointD2D value)
    {
      bool chg = _scaleX != value.X || _scaleY != value.Y;
      _scaleX = value.X;
      _scaleY = value.Y;
      return chg;
    }

    /// <summary>The scaling factor of the item, normally 1.</summary>
    public PointD2D Scale
    {
      get
      {
        return new PointD2D(_scaleX, _scaleY);
      }
      set
      {
        bool isChanged = InternalSetScaleSilent(value);
        if (isChanged)
          EhSelfChanged();
      }
    }

    /// <summary>The scaling factor of the item in x-direction, normally 1.</summary>
    public double ScaleX
    {
      get { return _scaleX; }
      set
      {
        if (InternalSetScaleXSilent(value))
          EhSelfChanged();
      }
    }

    /// <summary>The scaling factor of the item in y-direction, normally 1.</summary>
    public double ScaleY
    {
      get { return _scaleY; }
      set
      {
        if (InternalSetScaleYSilent(value))
          EhSelfChanged();
      }
    }

    /// <summary>The rotation angle (in degrees) of the layer.</summary>
    public double Rotation
    {
      get { return _rotation; }
      set
      {
        double oldvalue = _rotation;
        _rotation = value;

        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    /// <summary>The shear factor of the item.</summary>
    public double ShearX
    {
      get { return _shear; }
      set
      {
        double oldvalue = _shear;
        _shear = value;
        if (value != oldvalue)
          EhSelfChanged();
      }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Gets the absolute enclosing rectangle without taking into account ScaleX, ScaleY, Rotation and Shear (SSRS).
    /// </summary>
    /// <returns>The enclosing rectangle in absolute values.</returns>
    public RectangleD2D GetAbsoluteEnclosingRectangleWithoutSSRS()
    {
      var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
      var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);

      var myPosX = _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X) - _localAnchorX.GetValueRelativeTo(mySizeX);
      var myPosY = _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y) - _localAnchorY.GetValueRelativeTo(mySizeY);

      return new RectangleD2D(myPosX, myPosY, mySizeX, mySizeY);
    }

    /// <summary>
    /// Gets the absolute enclosing rectangle, taking into account ScaleX, ScaleY, Rotation and Shear (SSRS).
    /// </summary>
    /// <returns>The enclosing rectangle in absolute values.</returns>
    public RectangleD2D GetAbsoluteEnclosingRectangle()
    {
      var m = new MatrixD2D();
      m.SetTranslationRotationShearxScale(AbsolutePivotPositionX, AbsolutePivotPositionY, -Rotation, ShearX, ScaleX, ScaleY);
      m.TranslatePrepend(AbsoluteVectorPivotToLeftUpper.X, AbsoluteVectorPivotToLeftUpper.Y);

      var s = AbsoluteSize;
      var p1 = m.TransformPoint(new PointD2D(0, 0));
      var p2 = m.TransformPoint(new PointD2D(s.X, 0));
      var p3 = m.TransformPoint(new PointD2D(0, s.Y));
      var p4 = m.TransformPoint(new PointD2D(s.X, s.Y));

      var r = new RectangleD2D(p1, PointD2D.Empty);
      r.ExpandToInclude(p2);
      r.ExpandToInclude(p3);
      r.ExpandToInclude(p4);
      return r;
    }

    /// <summary>
    /// Sets the width without raising a change event.
    /// </summary>
    /// <param name="value">The new width value.</param>
    protected virtual void InternalSetSizeXSilent(RADouble value)
    {
      _sizeX = value;
    }

    /// <summary>
    /// Sets the height without raising a change event.
    /// </summary>
    /// <param name="value">The new height value.</param>
    protected virtual void InternalSetSizeYSilent(RADouble value)
    {
      _sizeY = value;
    }

    /// <summary>
    /// Sets the width and height without raising a change event.
    /// </summary>
    /// <param name="valueX">The new width value.</param>
    /// <param name="valueY">The new height value.</param>
    protected virtual void InternalSetSizeSilent(RADouble valueX, RADouble valueY)
    {
      _sizeX = valueX;
      _sizeY = valueY;
    }

    /// <summary>
    /// Sets the absolute width without raising a change event.
    /// </summary>
    /// <param name="value">The new absolute width.</param>
    protected virtual void InternalSetAbsoluteSizeXSilent(double value)
    {
      if (_sizeX.IsAbsolute)
        InternalSetSizeXSilent(RADouble.NewAbs(value));
      else if (_parentSize.X != 0 && !double.IsNaN(_parentSize.X))
        InternalSetSizeXSilent(RADouble.NewRel(value / _parentSize.X));
      else
        throw new InvalidOperationException("_parentSize.X is undefined or zero");
    }

    /// <summary>
    /// Sets the absolute height without raising a change event.
    /// </summary>
    /// <param name="value">The new absolute height.</param>
    protected virtual void InternalSetAbsoluteSizeYSilent(double value)
    {
      if (_sizeY.IsAbsolute)
        InternalSetSizeYSilent(RADouble.NewAbs(value));
      else if (_parentSize.Y != 0 && !double.IsNaN(_parentSize.Y))
        InternalSetSizeYSilent(RADouble.NewRel(value / _parentSize.Y));
      else
        throw new InvalidOperationException("_parentSize.Y is undefined or zero");
    }

    /// <summary>
    /// Sets the absolute size without raising a change event.
    /// </summary>
    /// <param name="value">The new absolute size.</param>
    protected virtual void InternalSetAbsoluteSizeSilent(PointD2D value)
    {
      RADouble sizeX, sizeY;

      if (_sizeX.IsAbsolute)
        sizeX = RADouble.NewAbs(value.X);
      else if (_parentSize.X != 0 && !double.IsNaN(_parentSize.X))
        sizeX = RADouble.NewRel(value.X / _parentSize.X);
      else
        throw new InvalidOperationException("_parentSize.X is undefined or zero");

      if (_sizeY.IsAbsolute)
        sizeY = RADouble.NewAbs(value.Y);
      else if (_parentSize.Y != 0 && !double.IsNaN(_parentSize.Y))
        sizeY = RADouble.NewRel(value.Y / _parentSize.Y);
      else
        throw new InvalidOperationException("_parentSize.Y is undefined or zero");

      InternalSetSizeSilent(sizeX, sizeY);
    }

    /// <summary>
    /// Gets or sets the absolute width.
    /// </summary>
    public virtual double AbsoluteSizeX
    {
      get
      {
        return _sizeX.GetValueRelativeTo(_parentSize.X);
      }
      set
      {
        var oldValue = _sizeX;
        InternalSetAbsoluteSizeXSilent(value);
        if (oldValue != _sizeX)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the absolute height.
    /// </summary>
    public virtual double AbsoluteSizeY
    {
      get
      {
        return _sizeY.GetValueRelativeTo(_parentSize.Y);
      }
      set
      {
        var oldValue = _sizeY;
        InternalSetAbsoluteSizeYSilent(value);
        if (oldValue != _sizeY)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the absolute size.
    /// </summary>
    public virtual PointD2D AbsoluteSize
    {
      get
      {
        return new PointD2D(AbsoluteSizeX, AbsoluteSizeY);
      }
      set
      {
        SetAbsoluteSize(value, Main.EventFiring.Enabled);
      }
    }

    /// <summary>
    /// Sets the absolute size.
    /// </summary>
    /// <param name="value">The absolute size.</param>
    /// <param name="eventFiring">Controls whether change events are fired.</param>
    public virtual void SetAbsoluteSize(PointD2D value, Main.EventFiring eventFiring)
    {
      var oldSizeX = _sizeX;
      var oldSizeY = _sizeY;
      InternalSetAbsoluteSizeSilent(value);

      if (eventFiring == Main.EventFiring.Enabled)
      {
        if (oldSizeX != _sizeX || oldSizeY != _sizeY)
          EhSelfChanged();
      }
    }

    private void InternalSetAbsolutePositionXSilent(double value)
    {
      var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
      if (_positionX.IsAbsolute)
        _positionX = RADouble.NewAbs(value - _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _localAnchorX.GetValueRelativeTo(mySizeX));
      else if (0 != _parentSize.X && _parentSize.X.IsFinite())
        _positionX = RADouble.NewRel((value - _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _localAnchorX.GetValueRelativeTo(mySizeX)) / _parentSize.X);
      else
        throw new InvalidOperationException("_parentSize.X is undefined or zero");
    }

    private void InternalSetAbsolutePositionYSilent(double value)
    {
      var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
      if (_positionY.IsAbsolute)
        _positionY = RADouble.NewAbs(value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _localAnchorY.GetValueRelativeTo(mySizeY));
      else if (0 != _parentSize.Y && _parentSize.Y.IsFinite())
        _positionY = RADouble.NewRel((value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _localAnchorY.GetValueRelativeTo(mySizeY)) / _parentSize.Y);
      else
        throw new InvalidOperationException("_parentSize.Y is undefined or zero");
    }

    /// <summary>
    /// Gets or sets the absolute x position of the upper-left corner.
    /// </summary>
    public double AbsolutePositionX
    {
      get
      {
        var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
        return _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X) - _localAnchorX.GetValueRelativeTo(mySizeX);
      }
      set
      {
        var oldValue = _positionX;
        InternalSetAbsolutePositionXSilent(value);
        if (oldValue != _positionX)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the absolute y position of the upper-left corner.
    /// </summary>
    public double AbsolutePositionY
    {
      get
      {
        var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
        return _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y) - _localAnchorY.GetValueRelativeTo(mySizeY);
      }
      set
      {
        var oldValue = _positionY;
        InternalSetAbsolutePositionYSilent(value);
        if (oldValue != _positionY)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the absolute position of the upper-left corner.
    /// </summary>
    public PointD2D AbsolutePosition
    {
      get
      {
        return new PointD2D(AbsolutePositionX, AbsolutePositionY);
      }
      set
      {
        var oldValueX = _positionX;
        var oldValueY = _positionY;

        InternalSetAbsolutePositionXSilent(value.X);
        InternalSetAbsolutePositionYSilent(value.Y);

        if (oldValueX != _positionX || oldValueY != _positionY)
          EhSelfChanged();
      }
    }

    private void InternalSetAbsolutePivotPositionXSilent(double value)
    {
      var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
      if (_positionX.IsAbsolute)
        _positionX = RADouble.NewAbs(value - _parentAnchorX.GetValueRelativeTo(_parentSize.X));
      else if (0 != _parentSize.X && _parentSize.X.IsFinite())
        _positionX = RADouble.NewRel((value - _parentAnchorX.GetValueRelativeTo(_parentSize.X)) / _parentSize.X);
      else
        throw new InvalidOperationException("_parentSize.X is undefined or zero");
    }

    private void InternalSetAbsolutePivotPositionYSilent(double value)
    {
      var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
      if (_positionY.IsAbsolute)
        _positionY = RADouble.NewAbs(value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y));
      else if (0 != _parentSize.Y && _parentSize.Y.IsFinite())
        _positionY = RADouble.NewRel((value - _parentAnchorY.GetValueRelativeTo(_parentSize.Y)) / _parentSize.Y);
      else
        throw new InvalidOperationException("_parentSize.Y is undefined or zero");
    }

    /// <summary>
    /// Gets or sets the absolute x position of the pivot point of the item.
    /// </summary>
    /// <value>
    /// The absolute pivot x position.
    /// </value>
    /// <exception cref="System.InvalidOperationException">_parentSize.X is undefined or zero</exception>
    public double AbsolutePivotPositionX
    {
      get
      {
        var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
        return _parentAnchorX.GetValueRelativeTo(_parentSize.X) + _positionX.GetValueRelativeTo(_parentSize.X);
      }
      set
      {
        var oldValue = _positionX;
        InternalSetAbsolutePivotPositionXSilent(value);
        if (oldValue != _positionX)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the absolute y position of the pivot point of the item.
    /// </summary>
    /// <value>
    /// The absolute pivot y position.
    /// </value>
    /// <exception cref="System.InvalidOperationException">_parentSize.Y is undefined or zero</exception>
    public double AbsolutePivotPositionY
    {
      get
      {
        var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
        return _parentAnchorY.GetValueRelativeTo(_parentSize.Y) + _positionY.GetValueRelativeTo(_parentSize.Y);
      }
      set
      {
        var oldValue = _positionY;
        InternalSetAbsolutePositionYSilent(value);
        if (oldValue != _positionY)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the absolute pivot position.
    /// </summary>
    public PointD2D AbsolutePivotPosition
    {
      get
      {
        return new PointD2D(AbsolutePivotPositionX, AbsolutePivotPositionY);
      }
      set
      {
        SetAbsolutePivotPosition(value, Main.EventFiring.Enabled);
      }
    }

    /// <summary>
    /// Sets the absolute pivot position.
    /// </summary>
    /// <param name="value">The pivot position.</param>
    /// <param name="eventFiring">Controls whether change events are fired.</param>
    public void SetAbsolutePivotPosition(PointD2D value, Main.EventFiring eventFiring)
    {
      var oldValueX = _positionX;
      var oldValueY = _positionY;

      InternalSetAbsolutePivotPositionXSilent(value.X);
      InternalSetAbsolutePivotPositionYSilent(value.Y);

      if (eventFiring == Main.EventFiring.Enabled && (oldValueX != _positionX || oldValueY != _positionY))
        EhSelfChanged();
    }

    /// <summary>
    /// Gets the absolute vector between the pivot point of the item and its left upper edge.
    /// </summary>
    /// <value>
    /// The absolute vector between the pivot point of the item and its left upper edge.
    /// </value>
    public PointD2D AbsoluteVectorPivotToLeftUpper
    {
      get
      {
        var mySizeX = _sizeX.GetValueRelativeTo(_parentSize.X);
        var mySizeY = _sizeY.GetValueRelativeTo(_parentSize.Y);
        return new PointD2D(-_localAnchorX.GetValueRelativeTo(mySizeX), -_localAnchorY.GetValueRelativeTo(mySizeY));
      }
    }

    /// <summary>
    /// Sets relative size and position values from absolute values.
    /// </summary>
    /// <param name="absSize">The absolute size.</param>
    /// <param name="absPos">The absolute position.</param>
    public virtual void SetRelativeSizePositionFromAbsoluteValues(PointD2D absSize, PointD2D absPos)
    {
      var oldSizeX = SizeX;
      var oldSizeY = SizeY;
      var oldPosX = PositionX;
      var oldPosY = PositionY;

      if (_parentSize.X == 0)
        throw new InvalidOperationException("ParentSize.X is zero. This would lead to an undefined relative value!");
      if (_parentSize.Y == 0)
        throw new InvalidOperationException("ParentSize.Y is zero. This would lead to an undefined relative value!");

      InternalSetSizeSilent(
       RADouble.NewRel(absSize.X / _parentSize.X),
       RADouble.NewRel(absSize.Y / _parentSize.Y)
      );

      _positionX = RADouble.NewRel(absPos.X / _parentSize.X);
      _positionY = RADouble.NewRel(absPos.Y / _parentSize.Y);

      if (oldSizeX != _sizeX || oldSizeY != _sizeY || oldPosX != _positionX || oldPosY != _positionY)
        EhSelfChanged();
    }

    /// <summary>
    /// Converts relative size values to absolute size values.
    /// </summary>
    public void ChangeRelativeSizeValuesToAbsoluteSizeValues()
    {
      if (_sizeX.IsRelative)
        _sizeX = RADouble.NewAbs(AbsoluteSizeX);
      if (_sizeY.IsRelative)
        _sizeY = RADouble.NewAbs(AbsoluteSizeY);
    }

    /// <summary>
    /// Converts relative position values to absolute position values.
    /// </summary>
    public void ChangeRelativePositionValuesToAbsolutePositionValues()
    {
      if (_positionX.IsRelative)
        _positionX = RADouble.NewAbs(AbsolutePositionX);
      if (_positionY.IsRelative)
        _positionY = RADouble.NewAbs(AbsolutePositionY);
    }

    /// <summary>
    /// Changes the parent anchor while preserving the absolute position.
    /// </summary>
    /// <param name="newParentAnchorX">The new parent anchor x value.</param>
    /// <param name="newParentAnchorY">The new parent anchor y value.</param>
    public void ChangeParentAnchorButKeepPosition(RADouble newParentAnchorX, RADouble newParentAnchorY)
    {
      var oldRefX = _parentAnchorX.GetValueRelativeTo(_parentSize.X);
      var oldRefY = _parentAnchorY.GetValueRelativeTo(_parentSize.Y);
      var newRefX = newParentAnchorX.GetValueRelativeTo(_parentSize.X);
      var newRefY = newParentAnchorY.GetValueRelativeTo(_parentSize.Y);

      var oldPos = AbsolutePosition;
      InternalSetAbsolutePositionXSilent(oldPos.X + (oldRefX - newRefX));
      InternalSetAbsolutePositionYSilent(oldPos.Y + (oldRefY - newRefY));
      _parentAnchorX = newParentAnchorX;
      _parentAnchorY = newParentAnchorY;
    }

    /// <summary>
    /// Changes the parent anchor to the top-left corner while preserving the absolute position.
    /// </summary>
    public void ChangeParentAnchorToLeftTopButKeepPosition()
    {
      ChangeParentAnchorButKeepPosition(RADouble.NewRel(0), RADouble.NewRel(0));
    }

    #endregion Methods
  }
}
