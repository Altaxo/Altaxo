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

#endregion Copyright

#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
  /// <summary>
  /// View contract for editing ASCII document-analysis options.
  /// </summary>
  public interface IAsciiDocumentAnalysisOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing ASCII document-analysis options.
  /// </summary>
  [ExpectedTypeOfView(typeof(IAsciiDocumentAnalysisOptionsView))]
  [UserControllerForObject(typeof(AsciiDocumentAnalysisOptions))]
  public class AsciiDocumentAnalysisOptionsController : MVCANControllerEditImmutableDocBase<AsciiDocumentAnalysisOptions, IAsciiDocumentAnalysisOptionsView>
  {
    /// <summary>
    /// Wrapper class for a <see cref="CultureInfo"/> value that implements the <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface,
    /// and provides a parameterless constructor that initializes the value to <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    public class CultureInfoWrapper : Boxed<CultureInfo>
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="CultureInfoWrapper"/> class.
      /// </summary>
      public CultureInfoWrapper() : base(CultureInfo.InvariantCulture)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="CultureInfoWrapper"/> class.
      /// </summary>
      /// <param name="value">Initial value.</param>
      public CultureInfoWrapper(CultureInfo value) : base(value)
      {
      }
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the number of lines to analyze.
    /// </summary>
    public int NumberOfLinesToAnalyze
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfLinesToAnalyze));
        }
      }
    }

    /// <summary>
    /// Gets or sets the collection of culture-specific number formats to analyze.
    /// </summary>
    /// <remarks>Use this property to specify which number formats, represented by their associated cultures,
    /// should be included in analysis operations. Changing this collection will notify listeners of the
    /// update.</remarks>
    public ObservableCollection<CultureInfoWrapper> NumberFormatsToAnalyze
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberFormatsToAnalyze));
        }
      }
    }

    /// <summary>
    /// Gets or sets the collection of culture-specific date and time formats to analyze.
    /// </summary>
    /// <remarks>Use this property to specify which date and time formats, represented by their associated
    /// cultures, should be included in analysis operations. Changing this property raises a property changed
    /// notification.</remarks>
    public ObservableCollection<CultureInfoWrapper> DateTimeFormatsToAnalyze
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(DateTimeFormatsToAnalyze));
        }
      }
    }

    /// <summary>
    /// Gets the list of available culture formats that can be selected.
    /// </summary>
    public ObservableCollection<CultureInfoWrapper> AvailableCultureFormats
    {
      get => field;
      private set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(AvailableCultureFormats));
        }
      }
    }


    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NumberOfLinesToAnalyze = _doc.NumberOfLinesToAnalyze;

        AvailableCultureFormats = GetAvailableCultures();

        NumberFormatsToAnalyze = new System.Collections.ObjectModel.ObservableCollection<CultureInfoWrapper>(_doc.NumberFormatsToTest.Select(e => new CultureInfoWrapper(e)));

        DateTimeFormatsToAnalyze = new System.Collections.ObjectModel.ObservableCollection<CultureInfoWrapper>(_doc.DateTimeFormatsToTest.Select(e => new CultureInfoWrapper(e)));
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        NumberOfLinesToAnalyze = NumberOfLinesToAnalyze,
        NumberFormatsToTest = NumberFormatsToAnalyze.Select(x => x.Value).Distinct().ToImmutableHashSet(),
        DateTimeFormatsToTest = DateTimeFormatsToAnalyze.Select(x => x.Value).Distinct().ToImmutableHashSet(),
      };

      return ApplyEnd(true, disposeController);
    }

    /// <summary>
    /// Gets the available cultures.
    /// </summary>
    private ObservableCollection<CultureInfoWrapper> GetAvailableCultures()
    {
      var list = new ObservableCollection<CultureInfoWrapper>();
      var invCult = System.Globalization.CultureInfo.InvariantCulture;
      list.Add(new CultureInfoWrapper(invCult));

      var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
      Array.Sort(cultures, (x, y) => string.Compare(x.DisplayName, y.DisplayName));
      foreach (var cult in cultures)
        list.Add(new CultureInfoWrapper(cult));

      return list;
    }
  }
}
