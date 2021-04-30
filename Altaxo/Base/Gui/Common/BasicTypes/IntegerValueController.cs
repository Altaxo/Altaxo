#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Common.BasicTypes
{
  public interface IIntegerValueView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for all integral types (byte, ubyte .. long, ulong, BigInteger).
  /// </summary>
  [UserControllerForObject(typeof(sbyte), 100)]
  [UserControllerForObject(typeof(byte), 100)]
  [UserControllerForObject(typeof(short), 100)]
  [UserControllerForObject(typeof(ushort), 100)]
  [UserControllerForObject(typeof(int), 100)]
  [UserControllerForObject(typeof(uint), 100)]
  [UserControllerForObject(typeof(long), 100)]
  [UserControllerForObject(typeof(ulong), 100)]
  [UserControllerForObject(typeof(BigInteger), 100)]
  [ExpectedTypeOfView(typeof(IIntegerValueView))]
  public class IntegerValueController : MVCANControllerEditImmutableDocBase<BigInteger, IIntegerValueView>
  {
    private object _originalObject;

    private BigInteger? _typeMinimum;
    private BigInteger? _typeMaximum;

    private BigInteger? _userMinimum;
    private BigInteger? _userMaximum;

    public IntegerValueController() { _originalObject = null!; }

    public IntegerValueController(object integer)
    {
      _originalObject = integer;

      if (!InitializeDocument(integer))
      {
        throw new ArgumentException($"Object <<{integer}>> (of type {integer.GetType()}) is not an integer handled by this controller.");
      }
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (args[0] is null)
        return false;

      switch (args[0])
      {
        case BigInteger bi:
          _doc = bi;
          _typeMinimum = null;
          _typeMaximum = null;
          break;
        case ulong ul:
          _doc = ul;
          _typeMinimum = ulong.MinValue;
          _typeMaximum = ulong.MaxValue;
          break;
        case long l:
          _doc = l;
          _typeMinimum = long.MinValue;
          _typeMaximum = long.MaxValue;
          break;
        case uint ui:
          _doc = ui;
          _typeMinimum = uint.MinValue;
          _typeMaximum = uint.MaxValue;
          break;
        case int i:
          _doc = i;
          _typeMinimum = int.MinValue;
          _typeMaximum = int.MaxValue;
          break;
        case ushort us:
          _doc = us;
          _typeMinimum = ushort.MinValue;
          _typeMaximum = ushort.MaxValue;
          break;
        case short s:
          _doc = s;
          _typeMinimum = short.MinValue;
          _typeMaximum = short.MaxValue;
          break;
        case byte b:
          _doc = b;
          _typeMinimum = byte.MinValue;
          _typeMaximum = byte.MaxValue;
          break;
        case sbyte sb:
          _doc = sb;
          _typeMinimum = sbyte.MinValue;
          _typeMaximum = sbyte.MaxValue;
          break;
        default:
          return false;
      }

      _originalDoc = _doc;
      _originalObject = args[0];
      return true;
    }

    public BigInteger? UserMinimum { get => _userMinimum; set => _userMinimum = value; }
    public BigInteger? UserMaximum { get => _userMaximum; set => _userMaximum = value; }

    #region Bindings

    public BigInteger? Minimum => _typeMinimum.HasValue && _userMinimum.HasValue ? BigInteger.Max(_typeMinimum.Value, _userMinimum.Value) : _typeMinimum ?? _userMinimum;

    public BigInteger? Maximum => _typeMaximum.HasValue && _userMaximum.HasValue ? BigInteger.Min(_typeMaximum.Value, _userMaximum.Value) : _typeMaximum ?? _userMaximum;

    public BigInteger Value
    {
      get => _doc;
      set
      {
        if (Minimum is { } min && value < min)
          value = min;
        else if (Maximum is { } max && value > max)
          value = max;

        if (!(_doc == value))
        {
          _doc = value;
          OnPropertyChanged(nameof(Value));
        }
      }
    }

    private string _minimumReplacementText = "Min";
    public string MinimumReplacementText { get => _minimumReplacementText; set { _minimumReplacementText = value; } }

    private string _maximumReplacementText = "Max";
    public string MaximumReplacementText { get => _maximumReplacementText; set { _maximumReplacementText = value; } }

    private bool _isGotoMinimumMaximumVisible = true;

    public bool IsGotoMinimumAndMaximumVisible
    {
      get
      {
        return _isGotoMinimumMaximumVisible && (Minimum is not null || Maximum is not null);

      }
      set
      {
        _isGotoMinimumMaximumVisible = value;
      }
    }

    #endregion

    public override object ModelObject
    {
      get
      {
        switch (_originalObject)
        {
          case BigInteger bi:
            return _doc;
          case ulong ul:
            return (ulong)_doc;
          case long l:
            return (long)_doc;
          case uint ui:
            return (uint)_doc;
          case int i:
            return (int)_doc;
          case ushort us:
            return (ushort)_doc;
          case short s:
            return (short)_doc;
          case byte b:
            return (byte)_doc;
          case sbyte sb:
            return (sbyte)_doc;
          default:
            throw new NotImplementedException();
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }
  }
}
