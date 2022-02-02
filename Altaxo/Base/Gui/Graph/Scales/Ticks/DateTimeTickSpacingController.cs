#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  public interface IDateTimeTickSpacingView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(DateTimeTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(IDateTimeTickSpacingView))]
  public class DateTimeTickSpacingController : MVCANControllerEditOriginalDocBase<DateTimeTickSpacing, IDateTimeTickSpacingView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _targetNumberOfMajorTicks;

    public int TargetNumberOfMajorTicks
    {
      get => _targetNumberOfMajorTicks;
      set
      {
        if (!(_targetNumberOfMajorTicks == value))
        {
          _targetNumberOfMajorTicks = value;
          OnPropertyChanged(nameof(TargetNumberOfMajorTicks));
        }
      }
    }
    private int _targetNumberOfMinorTicks;

    public int TargetNumberOfMinorTicks
    {
      get => _targetNumberOfMinorTicks;
      set
      {
        if (!(_targetNumberOfMinorTicks == value))
        {
          _targetNumberOfMinorTicks = value;
          OnPropertyChanged(nameof(TargetNumberOfMinorTicks));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment GraceEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _minGrace;

    public DimensionfulQuantity MinGrace
    {
      get => _minGrace;
      set
      {
        if (!(_minGrace == value))
        {
          _minGrace = value;
          OnPropertyChanged(nameof(MinGrace));
        }
      }
    }

    private DimensionfulQuantity _maxGrace;

    public DimensionfulQuantity MaxGrace
    {
      get => _maxGrace;
      set
      {
        if (!(_maxGrace == value))
        {
          _maxGrace = value;
          OnPropertyChanged(nameof(MaxGrace));
        }
      }
    }

    private DateTimeTickSpacing.TimeSpanEx? _majorTickSpan;
    private string _majorTickSpanString;

    public string MajorTickSpanString
    {
      get => _majorTickSpanString;
      set
      {
        if (!(_majorTickSpanString == value))
        {
          _majorTickSpanString = value;

          // validate the string
          if (IsTimeSpanExOrNull(value, out var val))
          {
            _majorTickSpan = val;
            MajorTickSpanError = String.Empty;
          }
          else
          {
            MajorTickSpanError = "Can not convert major tick span to a Date/Time span.";
          }

          OnPropertyChanged(nameof(MajorTickSpanString));
        }
      }
    }

    private string _majorTickSpanError;

    public string MajorTickSpanError
    {
      get => _majorTickSpanError;
      set
      {
        if (!(_majorTickSpanError == value))
        {
          _majorTickSpanError = value;
          OnPropertyChanged(nameof(MajorTickSpanError));
        }
      }
    }


    private bool _minorTicksUserSpecified;

    public bool MinorTicksUserSpecified
    {
      get => _minorTicksUserSpecified;
      set
      {
        if (!(_minorTicksUserSpecified == value))
        {
          _minorTicksUserSpecified = value;
          OnPropertyChanged(nameof(MinorTicksUserSpecified));
        }
      }
    }



    private int _minorTicks = 1;

    public int MinorTicks
    {
      get => _minorTicks;
      set
      {
        if (!(_minorTicks == value))
        {
          _minorTicks = value;
          OnPropertyChanged(nameof(MinorTicks));
        }
      }
    }







    private string _suppressMajorTickValues;

    public string SuppressMajorTicksByValue
    {
      get => _suppressMajorTickValues;
      set
      {
        if (!(_suppressMajorTickValues == value))
        {
          _suppressMajorTickValues = value;
          OnPropertyChanged(nameof(SuppressMajorTicksByValue));
        }
      }
    }

    private string _suppressMajorTicksByNumber;

    public string SuppressMajorTicksByNumber
    {
      get => _suppressMajorTicksByNumber;
      set
      {
        if (!(_suppressMajorTicksByNumber == value))
        {
          _suppressMajorTicksByNumber = value;
          OnPropertyChanged(nameof(SuppressMajorTicksByNumber));
        }
      }
    }

    private string _suppressMinorTicksByValue;

    public string SuppressMinorTicksByValue
    {
      get => _suppressMinorTicksByValue;
      set
      {
        if (!(_suppressMinorTicksByValue == value))
        {
          _suppressMinorTicksByValue = value;
          OnPropertyChanged(nameof(SuppressMinorTicksByValue));
        }
      }
    }
    private string _suppressMinorTicksByNumber;

    public string SuppressMinorTicksByNumber
    {
      get => _suppressMinorTicksByNumber;
      set
      {
        if (!(_suppressMinorTicksByNumber == value))
        {
          _suppressMinorTicksByNumber = value;
          OnPropertyChanged(nameof(SuppressMinorTicksByNumber));
        }
      }
    }
    private string _addMajorTickValues;

    public string AddMajorTickValues
    {
      get => _addMajorTickValues;
      set
      {
        if (!(_addMajorTickValues == value))
        {
          _addMajorTickValues = value;
          OnPropertyChanged(nameof(AddMajorTickValues));
        }
      }
    }

    private string _addMinorTickValues;

    public string AddMinorTickValues
    {
      get => _addMinorTickValues;
      set
      {
        if (!(_addMinorTickValues == value))
        {
          _addMinorTickValues = value;
          OnPropertyChanged(nameof(AddMinorTickValues));
        }
      }
    }


    private ItemsController<BoundaryTickSnapping> _snapTicksToOrg;

    public ItemsController<BoundaryTickSnapping> SnapTicksToOrg
    {
      get => _snapTicksToOrg;
      set
      {
        if (!(_snapTicksToOrg == value))
        {
          _snapTicksToOrg = value;
          OnPropertyChanged(nameof(SnapTicksToOrg));
        }
      }
    }
    private ItemsController<BoundaryTickSnapping> _snapTicksToEnd;

    public ItemsController<BoundaryTickSnapping> SnapTicksToEnd
    {
      get => _snapTicksToEnd;
      set
      {
        if (!(_snapTicksToEnd == value))
        {
          _snapTicksToEnd = value;
          OnPropertyChanged(nameof(SnapTicksToEnd));
        }
      }
    }


    #endregion Bindings


    public override void Dispose(bool isDisposing)
    {
      _snapTicksToOrg = null;
      _snapTicksToEnd = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MinGrace = new DimensionfulQuantity(_doc.OrgGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);
        MaxGrace = new DimensionfulQuantity(_doc.EndGrace, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(GraceEnvironment.DefaultUnit);

        TargetNumberOfMajorTicks = _doc.TargetNumberOfMajorTicks;
        TargetNumberOfMinorTicks = _doc.TargetNumberOfMinorTicks;

        SnapTicksToOrg = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapOrgToTick, useUserFriendlyName: true));
        SnapTicksToEnd = new ItemsController<BoundaryTickSnapping>(new SelectableListNodeList(_doc.SnapEndToTick, useUserFriendlyName: true));


        SuppressMajorTicksByValue = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByValues);
        SuppressMinorTicksByValue = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByValues);
        SuppressMajorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByNumbers);
        SuppressMinorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByNumbers);

        AddMajorTickValues = GUIConversion.ToString(_doc.AdditionalMajorTicks.Values);
        AddMinorTickValues = GUIConversion.ToString(_doc.AdditionalMinorTicks.Values);

        _majorTickSpan = _doc.MajorTickSpan;
        MajorTickSpanString = ToString(_majorTickSpan);
        MinorTicksUserSpecified = _doc.MinorTicks is not null;
        MinorTicks = _doc.MinorTicks ?? 1;

      }
    }



    public override bool Apply(bool disposeController)
    {

      if (GUIConversion.TryParseMultipleAltaxoVariant(SuppressMajorTicksByValue, out var varVals))
      {
        _doc.SuppressedMajorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMajorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(SuppressMinorTicksByValue, out varVals))
      {
        _doc.SuppressedMinorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMinorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(SuppressMajorTicksByNumber, out var intVals))
      {
        _doc.SuppressedMajorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMajorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(SuppressMinorTicksByNumber, out intVals))
      {
        _doc.SuppressedMinorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMinorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(AddMajorTickValues, out varVals))
      {
        _doc.AdditionalMajorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMajorTicks.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(AddMinorTickValues, out varVals))
      {
        _doc.AdditionalMinorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMinorTicks.Add(v);
      }
      else
      {
        return false;
      }

      _doc.MajorTickSpan = _majorTickSpan;
      _doc.MinorTicks = MinorTicksUserSpecified ? MinorTicks : null;

      _doc.TargetNumberOfMajorTicks = TargetNumberOfMajorTicks;
      _doc.TargetNumberOfMinorTicks = TargetNumberOfMinorTicks;

      _doc.OrgGrace = MinGrace.AsValueInSIUnits;
      _doc.EndGrace = MaxGrace.AsValueInSIUnits;

      _doc.SnapOrgToTick = _snapTicksToOrg.SelectedValue;
      _doc.SnapEndToTick = _snapTicksToEnd.SelectedValue;

      return ApplyEnd(true, disposeController);
    }

    private static bool IsTimeSpanExOrNull(string txt, out DateTimeTickSpacing.TimeSpanEx? span)
    {
      txt = txt?.Trim();

      if (string.IsNullOrEmpty(txt))
      {
        span = null;
        return true;
      }

      int idxFirstCharOfUnit = 0;
      for (int i = txt.Length - 1; i >= 0; --i)
      {
        if (char.IsLetter(txt[i]))
          idxFirstCharOfUnit = i;
        else
          break;
      }

      if (0 == idxFirstCharOfUnit)
      {
        span = null;
        return false;
      }

      string unit = txt.Substring(idxFirstCharOfUnit).Trim();
      string valString = txt.Substring(0, idxFirstCharOfUnit).Trim();

      if (!GUIConversion.IsInteger(valString, out var value))
      {
        span = null;
        return false;
      }

      DateTimeTickSpacing.TimeSpanEx? result = null;

      switch (unit)
      {
        case "Y":
        case "y":
          result = DateTimeTickSpacing.TimeSpanEx.FromYears(value);
          break;
        case "M":
          result = DateTimeTickSpacing.TimeSpanEx.FromMonths(value);
          break;
        case "W":
        case "w":
          result = DateTimeTickSpacing.TimeSpanEx.FromDays(7 * value);
          break;
        case "D":
        case "d":
          result = DateTimeTickSpacing.TimeSpanEx.FromDays(value);
          break;
        case "H":
        case "h":
          result = DateTimeTickSpacing.TimeSpanEx.FromHours(value);
          break;
        case "m":
          result = DateTimeTickSpacing.TimeSpanEx.FromMinutes(value);
          break;
        case "s":
          result = DateTimeTickSpacing.TimeSpanEx.FromSeconds(value);
          break;
        case "ms":
          result = DateTimeTickSpacing.TimeSpanEx.FromMilliSeconds(value);
          break;
        case "µs":
        case "us":
          result = DateTimeTickSpacing.TimeSpanEx.FromMicroSeconds(value);
          break;
        case "ns":
          if (value % 100 == 0)
            result = DateTimeTickSpacing.TimeSpanEx.FromTicks(value / 100);
          break;
      }

      if (result is not null)
      {
        span = result;
        return true;
      }
      else
      {
        span = null;
        return false;
      }
    }

    private static string ToString(DateTimeTickSpacing.TimeSpanEx? span)
    {
      var culture = Altaxo.Serialization.GUIConversion.CultureSettings;

      if (span is null)
      {
        return null;
      }
      else
      {
        var s = span.Value;

        if (s._unit == DateTimeTickSpacing.TimeSpanExUnit.Years)
          return string.Format(culture, "{0} Y", s.Years);
        else if (s._unit == DateTimeTickSpacing.TimeSpanExUnit.Month)
          return string.Format(culture, "{0} M", s.Months);
        else if (s._unit == DateTimeTickSpacing.TimeSpanExUnit.Span)
        {
          if (0 == (s._span.Ticks % TimeSpan.FromDays(7).Ticks))
            return string.Format(culture, "{0} W", s.Span.Ticks / TimeSpan.FromDays(7).Ticks);
          else if (0 == (s._span.Ticks % TimeSpan.FromDays(1).Ticks))
            return string.Format(culture, "{0} d", s.Span.Ticks / TimeSpan.FromDays(1).Ticks);
          else if (0 == (s._span.Ticks % TimeSpan.FromHours(1).Ticks))
            return string.Format(culture, "{0} h", s.Span.Ticks / TimeSpan.FromHours(1).Ticks);
          else if (0 == (s._span.Ticks % TimeSpan.FromMinutes(1).Ticks))
            return string.Format(culture, "{0} m", s.Span.Ticks / TimeSpan.FromMinutes(1).Ticks);
          else if (0 == (s._span.Ticks % TimeSpan.FromSeconds(1).Ticks))
            return string.Format(culture, "{0} s", s.Span.Ticks / TimeSpan.FromSeconds(1).Ticks);
          else if (0 == (s._span.Ticks % TimeSpan.FromMilliseconds(1).Ticks))
            return string.Format(culture, "{0} ms", s.Span.Ticks / TimeSpan.FromMilliseconds(1).Ticks);
          else if (0 == (s._span.Ticks % TimeSpan.FromTicks(10).Ticks))
            return string.Format(culture, "{0} µs", s.Span.Ticks / TimeSpan.FromTicks(10).Ticks);
          else
            return string.Format(culture, "{0} ns", s.Span.Ticks * 100);
        }
        else
          throw new NotImplementedException();
      }

    }
  }
}
