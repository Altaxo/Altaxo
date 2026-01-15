#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Gui.Analysis.Multivariate
{

  public interface IDimensionReductionOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="DimensionReductionAndRegressionOptions"/>
  /// </summary>
  [ExpectedTypeOfView(typeof(IDimensionReductionOptionsView))]
  [UserControllerForObject(typeof(DimensionReductionOptions))]
  public class DimensionReductionOptionsController : MVCANControllerEditImmutableDocBase<DimensionReductionOptions, IDimensionReductionOptionsView>
  {
    private Dictionary<Type, ISingleSpectrumPreprocessor> _knownSingleSpectrumPreprocessors = new();
    private Dictionary<Type, IEnsembleMeanScalePreprocessor> _knownEnsembleMeanScalePreprocessors = new();

    public DimensionReductionOptionsController()
    {
    }

    /// <summary>
    /// Constructor. Supply a document to control here.
    /// </summary>
    /// <param name="doc">The instance of option to set-up.</param>
    public DimensionReductionOptionsController(DimensionReductionOptions doc)
    {
      _doc = _originalDoc = doc;
      Initialize(true);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_singleSpectrumPreprocessorController, () => SingleSpectrumPreprocessorController = null!);
      yield return new ControllerAndSetNullMethod(MethodController, () => MethodController = null!);
      yield return new ControllerAndSetNullMethod(OutputOptionsController, () => OutputOptionsController = null!);
    }

    #region Bindings

    private IMVCANController _singleSpectrumPreprocessorController;

    public IMVCANController SingleSpectrumPreprocessorController
    {
      get => _singleSpectrumPreprocessorController;
      set
      {
        if (!(_singleSpectrumPreprocessorController == value))
        {
          _singleSpectrumPreprocessorController?.Dispose();
          _singleSpectrumPreprocessorController = value;
          OnPropertyChanged(nameof(SingleSpectrumPreprocessorController));
        }
      }
    }


    private ItemsController<Type> _analysisMethods;

    public ItemsController<Type> AnalysisMethods
    {
      get => _analysisMethods;
      set
      {
        if (!(_analysisMethods == value))
        {
          _analysisMethods = value;
          OnPropertyChanged(nameof(AnalysisMethods));
        }
      }
    }


    public IMVCANController MethodController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(MethodController));
        }
      }
    }


    public IMVCANController OutputOptionsController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(OutputOptionsController));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _knownSingleSpectrumPreprocessors[_doc.SinglePreprocessing.GetType()] = _doc.SinglePreprocessing;

        InitializeSingleSpectrumPreprocessor();
        InitializeAnalysisMethods();

        OutputOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.OutputOptions }, typeof(IMVCANController));
      }
    }

    private void InitializeSingleSpectrumPreprocessor()
    {
      SpectralPreprocessingOptionsList instance;

      if (_doc.SinglePreprocessing is null)
      {
        instance = SpectralPreprocessingOptionsList.Empty;
      }
      else if (_doc.SinglePreprocessing is SpectralPreprocessingOptionsList listInstance)
      {
        instance = listInstance;
      }
      else if (_doc.SinglePreprocessing is Altaxo.Science.Spectroscopy.SpectralPreprocessingOptions optionsInstance)
      {
        instance = SpectralPreprocessingOptionsList.CreateWithoutNoneElements(optionsInstance);
      }
      else
      {
        instance = new SpectralPreprocessingOptionsList(new ISingleSpectrumPreprocessor[] { _doc.SinglePreprocessing });
      }

      SingleSpectrumPreprocessorController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { instance }, typeof(IMVCANController));
    }


    IDimensionReductionMethod _currentMethod;

    private void InitializeAnalysisMethods()
    {
      _currentMethod = _doc.DimensionReductionMethod;

      var analysisMethods = new SelectableListNodeList();


      foreach (var definedtype in Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IDimensionReductionMethod)))
      {
        Attribute[] descriptionattributes = Attribute.GetCustomAttributes(definedtype, typeof(System.ComponentModel.DescriptionAttribute));

        string name =
          (descriptionattributes.Length > 0) ?
          ((System.ComponentModel.DescriptionAttribute)descriptionattributes[0]).Description : definedtype.ToString();

        analysisMethods.Add(new SelectableListNode(name, definedtype, false));
      }

      AnalysisMethods = new ItemsController<Type>(analysisMethods, EhAnalysisMethodChanged);

      AnalysisMethods.SelectedValue = _currentMethod.GetType();
    }

    private void EhAnalysisMethodChanged(Type type)
    {
      if (type is null)
        return;

      if (type != _currentMethod?.GetType())
        _currentMethod = (IDimensionReductionMethod)Activator.CreateInstance(type);

      MethodController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _currentMethod }, typeof(IMVCANController));
    }

    public override bool Apply(bool disposeController)
    {
      if (SingleSpectrumPreprocessorController is { } ctrl1)
      {
        if (true == ctrl1.Apply(disposeController))
        {
          _doc = _doc with { SinglePreprocessing = (ISingleSpectrumPreprocessor)ctrl1.ModelObject };
        }
        else
        {
          return ApplyEnd(false, disposeController);
        }
      }

      if (OutputOptionsController is { } ctrl2)
      {
        if (true == ctrl2.Apply(disposeController))
        {
          _doc = _doc with { OutputOptions = (DimensionReductionOutputOptions)ctrl2.ModelObject };
        }
        else
        {
          return ApplyEnd(false, disposeController);
        }
      }


      _doc = _doc with
      {
        DimensionReductionMethod = (IDimensionReductionMethod)Activator.CreateInstance(AnalysisMethods.SelectedValue)
      };


      return ApplyEnd(true, disposeController);
    }
  }
}

