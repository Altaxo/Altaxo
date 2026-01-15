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

  public interface IDimensionReductionAndRegressionOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="DimensionReductionAndRegressionOptions"/>
  /// </summary>
  [ExpectedTypeOfView(typeof(IDimensionReductionAndRegressionOptionsView))]
  [UserControllerForObject(typeof(DimensionReductionAndRegressionOptions))]
  public class DimensionReductionAndRegressionOptionsController : MVCANControllerEditImmutableDocBase<DimensionReductionAndRegressionOptions, IDimensionReductionAndRegressionOptionsView>
  {
    private Dictionary<Type, ISingleSpectrumPreprocessor> _knownSingleSpectrumPreprocessors = new();
    private Dictionary<Type, IEnsembleMeanScalePreprocessor> _knownEnsembleMeanScalePreprocessors = new();

    public DimensionReductionAndRegressionOptionsController()
    {
    }

    /// <summary>
    /// Constructor. Supply a document to control here.
    /// </summary>
    /// <param name="doc">The instance of option to set-up.</param>
    public DimensionReductionAndRegressionOptionsController(DimensionReductionAndRegressionOptions doc)
    {
      _doc = _originalDoc = doc;
      Initialize(true);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_singleSpectrumPreprocessorController, () => SingleSpectrumPreprocessorController = null);
      yield return new ControllerAndSetNullMethod(_ensembleMeanScalePreprocessorController, () => EnsembleMeanScalePreprocessorController = null);
    }

    #region Bindings

    private int _numberOfFactors;
    public int NumberOfFactors
    {
      get => _numberOfFactors;
      set
      {
        if (!(_numberOfFactors == value))
        {
          _numberOfFactors = value;
          OnPropertyChanged(nameof(NumberOfFactors));
        }
      }
    }

    private ItemsController<Type> _singleSpectrumPreprocessor;

    public ItemsController<Type> SingleSpectrumPreprocessor
    {
      get => _singleSpectrumPreprocessor;
      set
      {
        if (!(_singleSpectrumPreprocessor == value))
        {
          _singleSpectrumPreprocessor = value;
          OnPropertyChanged(nameof(SingleSpectrumPreprocessor));
        }
      }
    }

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


    private ItemsController<Type> _EnsembleMeanScalePreprocessor;

    public ItemsController<Type> EnsembleMeanScalePreprocessor
    {
      get => _EnsembleMeanScalePreprocessor;
      set
      {
        if (!(_EnsembleMeanScalePreprocessor == value))
        {
          _ensembleMeanScalePreprocessorController?.Dispose();
          _EnsembleMeanScalePreprocessor = value;
          OnPropertyChanged(nameof(EnsembleMeanScalePreprocessor));
        }
      }
    }

    private IMVCANController _ensembleMeanScalePreprocessorController;

    public IMVCANController EnsembleMeanScalePreprocessorController
    {
      get => _ensembleMeanScalePreprocessorController;
      set
      {
        if (!(_ensembleMeanScalePreprocessorController == value))
        {
          _ensembleMeanScalePreprocessorController = value;
          OnPropertyChanged(nameof(EnsembleMeanScalePreprocessorController));
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

    private ItemsController<Type> _CROSSPressCalculationTypes;

    public ItemsController<Type> CROSSPressCalculationTypes
    {
      get => _CROSSPressCalculationTypes;
      set
      {
        if (!(_CROSSPressCalculationTypes == value))
        {
          _CROSSPressCalculationTypes = value;
          OnPropertyChanged(nameof(CROSSPressCalculationTypes));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _knownSingleSpectrumPreprocessors[_doc.Preprocessing.GetType()] = _doc.Preprocessing;
        _knownEnsembleMeanScalePreprocessors[_doc.MeanScaleProcessing.GetType()] = _doc.MeanScaleProcessing;

        NumberOfFactors = _doc.MaximumNumberOfFactors;
        InitializeSingleSpectrumPreprocessor();
        InitializeEnsembleMeanScalePreprocessor();
        InitializeCrossPressCalculationTypes();
        InitializeAnalysisMethods();
      }
    }

    private void InitializeSingleSpectrumPreprocessor()
    {
      var list = new SelectableListNodeList();

      var fixedTypes = new HashSet<Type>
      {
        typeof(NoopSpectrumPreprocessor),
        typeof(Altaxo.Science.Spectroscopy.SpectralPreprocessingOptions),
      };

      list.Add(new SelectableListNode("None", typeof(NoopSpectrumPreprocessor), false));
      list.Add(new SelectableListNode("Standard", typeof(Altaxo.Science.Spectroscopy.SpectralPreprocessingOptions), false));

      SingleSpectrumPreprocessor = new ItemsController<Type>(list, EhSingleSpectrumPreprocessorChanged);
      SingleSpectrumPreprocessor.SelectedValue = _doc.Preprocessing.GetType();

    }


    private void EhSingleSpectrumPreprocessorChanged(Type newProcessorType)
    {
      if (newProcessorType is null)
        return;

      if (SingleSpectrumPreprocessorController is not null)
      {
        if (true == SingleSpectrumPreprocessorController.Apply(false))
        {
          var oldInstance = (ISingleSpectrumPreprocessor)SingleSpectrumPreprocessorController.ModelObject;
          _knownSingleSpectrumPreprocessors[oldInstance.GetType()] = oldInstance;
        }
      }

      ISingleSpectrumPreprocessor instance;
      if (!_knownSingleSpectrumPreprocessors.TryGetValue(newProcessorType, out instance))
      {
        instance = (ISingleSpectrumPreprocessor)Activator.CreateInstance(newProcessorType);
        _knownSingleSpectrumPreprocessors[newProcessorType] = instance;
      }

      SingleSpectrumPreprocessorController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { instance }, typeof(IMVCANController));
    }

    private void InitializeEnsembleMeanScalePreprocessor()
    {
      var list = new SelectableListNodeList();

      var fixedTypes = new HashSet<Type>
      {
        typeof(Altaxo.Science.Spectroscopy.EnsembleMeanScale.EnsembleMeanAndScaleCorrection),
        typeof(Altaxo.Science.Spectroscopy.EnsembleProcessing.MultiplicativeScatterCorrection),
      };

      list.Add(new SelectableListNode("Standard", typeof(Altaxo.Science.Spectroscopy.EnsembleMeanScale.EnsembleMeanAndScaleCorrection), false));
      list.Add(new SelectableListNode("MultiplicativeScatter", typeof(Altaxo.Science.Spectroscopy.EnsembleProcessing.MultiplicativeScatterCorrection), false));

      foreach (var t in Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IEnsembleMeanScalePreprocessor)))
      {
        if (!fixedTypes.Contains(t))
        {
          list.Add(new SelectableListNode(t.Name, t, false));
        }
      }

      EnsembleMeanScalePreprocessor = new ItemsController<Type>(list, EhEnsembleMeanScalePreprocessorChanged);
      EnsembleMeanScalePreprocessor.SelectedValue = _doc.MeanScaleProcessing.GetType();

    }


    private void EhEnsembleMeanScalePreprocessorChanged(Type newProcessorType)
    {
      if (newProcessorType is null)
        return;

      if (EnsembleMeanScalePreprocessorController is not null)
      {
        if (true == EnsembleMeanScalePreprocessorController.Apply(false))
        {
          var oldInstance = (IEnsembleMeanScalePreprocessor)EnsembleMeanScalePreprocessorController.ModelObject;
          _knownEnsembleMeanScalePreprocessors[oldInstance.GetType()] = oldInstance;
        }
      }

      IEnsembleMeanScalePreprocessor instance;
      if (!_knownEnsembleMeanScalePreprocessors.TryGetValue(newProcessorType, out instance))
      {
        instance = (IEnsembleMeanScalePreprocessor)Activator.CreateInstance(newProcessorType);
        _knownEnsembleMeanScalePreprocessors[newProcessorType] = instance;
      }

      EnsembleMeanScalePreprocessorController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { instance }, typeof(IMVCANController));
    }

    private void InitializeCrossPressCalculationTypes()
    {
      var list = new SelectableListNodeList();



      var fixedTypes = new HashSet<Type>
      {
        typeof(CrossValidationGroupingStrategyNone),
        typeof(CrossValidationGroupingStrategyExcludeSingleMeasurements),
        typeof(CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements),
        typeof(CrossValidationGroupingStrategyExcludeHalfObservations),
      };


      list.Add(new SelectableListNode("None", typeof(CrossValidationGroupingStrategyNone), false));
      list.Add(new SelectableListNode("Exclude every measurement", typeof(CrossValidationGroupingStrategyExcludeSingleMeasurements), false));
      list.Add(new SelectableListNode("Exclude groups of similar measurements", typeof(CrossValidationGroupingStrategyExcludeGroupsOfSimilarMeasurements), false));
      list.Add(new SelectableListNode("Exclude half ensemble of measurements", typeof(CrossValidationGroupingStrategyExcludeHalfObservations), false));

      foreach (var t in Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(ICrossValidationGroupingStrategy)))
      {
        if (!fixedTypes.Contains(t))
        {
          list.Add(new SelectableListNode(t.Name, t, false));
        }
      }
      CROSSPressCalculationTypes = new ItemsController<Type>(list);
      CROSSPressCalculationTypes.SelectedValue = _doc.CrossValidationGroupingStrategy.GetType();
    }

    private void InitializeAnalysisMethods()
    {
      var analysisMethods = new SelectableListNodeList();


      foreach (var definedtype in Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.Regression.Multivariate.WorksheetAnalysis)))
      {
        Attribute[] descriptionattributes = Attribute.GetCustomAttributes(definedtype, typeof(System.ComponentModel.DescriptionAttribute));

        string name =
          (descriptionattributes.Length > 0) ?
          ((System.ComponentModel.DescriptionAttribute)descriptionattributes[0]).Description : definedtype.ToString();

        analysisMethods.Add(new SelectableListNode(name, definedtype, false));
      }

      AnalysisMethods = new ItemsController<Type>(analysisMethods);
      AnalysisMethods.SelectedValue = _doc.WorksheetAnalysis.GetType();
    }

    private static bool ReferencesOwnAssembly(System.Reflection.AssemblyName[] references)
    {
      string myassembly = System.Reflection.Assembly.GetCallingAssembly().GetName().FullName;

      foreach (System.Reflection.AssemblyName assname in references)
        if (assname.FullName == myassembly)
          return true;
      return false;
    }

    private static bool IsOwnAssembly(System.Reflection.Assembly ass)
    {
      return ass.FullName == System.Reflection.Assembly.GetCallingAssembly().FullName;
    }

    public override bool Apply(bool disposeController)
    {
      if (SingleSpectrumPreprocessorController is { } ctrl1)
      {
        if (true == ctrl1.Apply(disposeController))
        {
          _doc = _doc with { Preprocessing = (ISingleSpectrumPreprocessor)ctrl1.ModelObject };
        }
        else
        {
          return ApplyEnd(false, disposeController);
        }
      }

      if (EnsembleMeanScalePreprocessorController is { } ctrl2)
      {
        if (true == ctrl2.Apply(disposeController))
        {
          _doc = _doc with { MeanScaleProcessing = (IEnsembleMeanScalePreprocessor)ctrl2.ModelObject };
        }
        else
        {
          return ApplyEnd(false, disposeController);
        }
      }


      _doc = _doc with
      {
        MaximumNumberOfFactors = NumberOfFactors,
        CrossValidationGroupingStrategy = (ICrossValidationGroupingStrategy)Activator.CreateInstance(CROSSPressCalculationTypes.SelectedValue),
        WorksheetAnalysis = (WorksheetAnalysis)Activator.CreateInstance(AnalysisMethods.SelectedValue)
      };


      return ApplyEnd(true, disposeController);
    }
  }
}

