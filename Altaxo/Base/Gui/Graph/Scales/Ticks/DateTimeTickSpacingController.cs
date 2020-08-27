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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  #region Interfaces

  public interface IDateTimeTickSpacingView
  {
    string MajorTicks { set; }

    int? MinorTicks { get; set; }

    double MinGrace { get; set; }

    double MaxGrace { get; set; }

    int TargetNumberMajorTicks { get; set; }

    int TargetNumberMinorTicks { get; set; }

    SelectableListNodeList SnapTicksToOrg { set; }

    SelectableListNodeList SnapTicksToEnd { set; }

    string SuppressMajorTickValues { get; set; }

    string SuppressMinorTickValues { get; set; }

    string SuppressMajorTicksByNumber { get; set; }

    string SuppressMinorTicksByNumber { get; set; }

    string AddMajorTickValues { get; set; }

    string AddMinorTickValues { get; set; }

    event Action<string, CancelEventArgs> MajorTicksValidating;
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(DateTimeTickSpacing), 200)]
  [ExpectedTypeOfView(typeof(IDateTimeTickSpacingView))]
  public class DateTimeTickSpacingController : MVCANControllerEditOriginalDocBase<DateTimeTickSpacing, IDateTimeTickSpacingView>
  {
    private SelectableListNodeList _snapTicksToOrg = new SelectableListNodeList();
    private SelectableListNodeList _snapTicksToEnd = new SelectableListNodeList();


    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _snapTicksToOrg = null;
      _snapTicksToEnd = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view != null)
      {
        _view.MajorTicks = ToString(_doc.MajorTickSpan);
        _view.MinorTicks = _doc.MinorTicks;
        _view.MinGrace = _doc.OrgGrace;
        _view.MaxGrace = _doc.EndGrace;

        _snapTicksToOrg.Clear();
        _snapTicksToEnd.Clear();

        foreach (BoundaryTickSnapping s in Enum.GetValues(typeof(BoundaryTickSnapping)))
        {
          _snapTicksToOrg.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s), s, s == _doc.SnapOrgToTick));
          _snapTicksToEnd.Add(new SelectableListNode(Current.Gui.GetUserFriendlyName(s), s, s == _doc.SnapEndToTick));
        }

        _view.SnapTicksToOrg = _snapTicksToOrg;
        _view.SnapTicksToEnd = _snapTicksToEnd;

        _view.TargetNumberMajorTicks = _doc.TargetNumberOfMajorTicks;
        _view.TargetNumberMinorTicks = _doc.TargetNumberOfMinorTicks;

        _view.SuppressMajorTickValues = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByValues);
        _view.SuppressMinorTickValues = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByValues);
        _view.SuppressMajorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMajorTicks.ByNumbers);
        _view.SuppressMinorTicksByNumber = GUIConversion.ToString(_doc.SuppressedMinorTicks.ByNumbers);

        _view.AddMajorTickValues = GUIConversion.ToString(_doc.AdditionalMajorTicks.Values);
        _view.AddMinorTickValues = GUIConversion.ToString(_doc.AdditionalMinorTicks.Values);
      }
    }


    public override bool Apply(bool disposeController)
    {

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMajorTickValues, out var varVals))
      {
        _doc.SuppressedMajorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMajorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.SuppressMinorTickValues, out varVals))
      {
        _doc.SuppressedMinorTicks.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedMinorTicks.ByValues.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(_view.SuppressMajorTicksByNumber, out var intVals))
      {
        _doc.SuppressedMajorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMajorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleInt32(_view.SuppressMinorTicksByNumber, out intVals))
      {
        _doc.SuppressedMinorTicks.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedMinorTicks.ByNumbers.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMajorTickValues, out varVals))
      {
        _doc.AdditionalMajorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMajorTicks.Add(v);
      }
      else
      {
        return false;
      }

      if (GUIConversion.TryParseMultipleAltaxoVariant(_view.AddMinorTickValues, out varVals))
      {
        _doc.AdditionalMinorTicks.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.AdditionalMinorTicks.Add(v);
      }
      else
      {
        return false;
      }

      // MajorTicks were validated and set before
      _doc.MinorTicks = _view.MinorTicks;

      _doc.TargetNumberOfMajorTicks = _view.TargetNumberMajorTicks;
      _doc.TargetNumberOfMinorTicks = _view.TargetNumberMinorTicks;

      _doc.OrgGrace = _view.MinGrace;
      _doc.EndGrace = _view.MaxGrace;

      _doc.SnapOrgToTick = (BoundaryTickSnapping)_snapTicksToOrg.FirstSelectedNode.Tag;
      _doc.SnapEndToTick = (BoundaryTickSnapping)_snapTicksToEnd.FirstSelectedNode.Tag;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.MajorTicksValidating += EhMajorSpanValidating;
    }

    protected override void DetachView()
    {
      _view.MajorTicksValidating -= EhMajorSpanValidating;

      base.DetachView();
    }

    private void EhMajorSpanValidating(string txt, CancelEventArgs e)
    {
      if (IsTimeSpanExOrNull(txt, out var val))
        _doc.MajorTickSpan = val;
      else
        e.Cancel = true;
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

      if (null != result)
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
