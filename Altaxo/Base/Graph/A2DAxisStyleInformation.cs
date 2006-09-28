using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  public enum A2DAxisSide { Left, Right };

  public class A2DAxisStyleInformation : ICloneable
  {
    CS2DLineID _identifier;

    string _nameOfAxisStyle;
    string _nameOfLeftSide;
    string _nameOfRightSide;
    A2DAxisSide _preferedLabelSide;
    bool _isShownByDefault;

   
    bool _hasTitleByDefault;

   

    public A2DAxisStyleInformation(CS2DLineID identifier)
    {
      _identifier = identifier;
    }
    public A2DAxisStyleInformation(A2DAxisStyleInformation from)
    {
      CopyFrom(from);
    }
    public void CopyFrom(A2DAxisStyleInformation from)
    {
      this._identifier = from._identifier;
      CopyWithoutIdentifierFrom(from);
    }
    public void CopyWithoutIdentifierFrom(A2DAxisStyleInformation from)
    {
      this._nameOfAxisStyle = from._nameOfAxisStyle;
      this._nameOfLeftSide = from._nameOfLeftSide;
      this._nameOfRightSide = from._nameOfRightSide;
      this._preferedLabelSide = from._preferedLabelSide;
      this._isShownByDefault = from._isShownByDefault;
      this._hasTitleByDefault = from._hasTitleByDefault;
    }

    public void SetDefaultValues()
    {
      switch (_identifier.ParallelAxisNumber)
      {
        case 0:
          _nameOfAxisStyle = "X-Axis";
          break;
        case 1:
          _nameOfAxisStyle = "Y-Axis";
          break;
        case 2:
          _nameOfAxisStyle = "Z-Axis";
          break;
        default:
          _nameOfAxisStyle = "Axis" + _identifier.ParallelAxisNumber.ToString();
          break;
      }

      _nameOfAxisStyle += string.Format(" (at L={0})",_identifier.LogicalValueOther.ToString());
      _nameOfLeftSide = "LeftSide";
      _nameOfRightSide = "RightSide";
      _preferedLabelSide = A2DAxisSide.Right;


    }

    public A2DAxisStyleInformation Clone()
    {
      return new A2DAxisStyleInformation(this);
    }
    object ICloneable.Clone()
    {
      return new A2DAxisStyleInformation(this);
    }

    public CS2DLineID Identifier
    {
      get { return _identifier; }
    }

    /// <summary>
    /// Name of the axis style. For cartesian coordinates for instance left, right, bottom or top.
    /// </summary>
    public string NameOfAxisStyle
    {
      get { return _nameOfAxisStyle; }
      set { _nameOfAxisStyle = value; }
    }

    /// <summary>
    /// Name of the left side of an axis style. For the bottom axis, this is for instance "inner".
    /// </summary>
    public string NameOfLeftSide
    {
      get { return _nameOfLeftSide; }
      set { _nameOfLeftSide = value; }
    }

    /// <summary>
    /// Name of the left side of an axis style. For the bottom axis, this is for instance "outer".
    /// </summary>
    public string NameOfRightSide
    {
      get { return _nameOfRightSide; }
      set { _nameOfRightSide = value; }
    }

    /// <summary>
    /// Side of an axis style where the label is probably shown. For the bottom axis, this is for instance the right side, i.e. the outer side.
    /// </summary>
    public A2DAxisSide PreferedLabelSide
    {
      get { return _preferedLabelSide; }
      set { _preferedLabelSide = value; }
    }

    public bool IsShownByDefault
    {
      get { return _isShownByDefault; }
      set { _isShownByDefault = value; }
    }

    public bool HasTitleByDefault
    {
      get { return _hasTitleByDefault; }
      set { _hasTitleByDefault = value; }
    }
  }
}
