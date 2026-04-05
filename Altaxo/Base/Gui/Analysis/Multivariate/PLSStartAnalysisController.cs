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
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.BasicTypes;

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// Defines the view contract for configuring the start of a PLS analysis.
  /// </summary>
  public interface IPLSStartAnalysisView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for PLSStartAnalysisController.
  /// </summary>
  /// <summary>
  /// Controller for <see cref="MultivariateAnalysisOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPLSStartAnalysisView))]
   [UserControllerForObject(typeof(MultivariateAnalysisOptions))]
  public class PLSStartAnalysisController : MVCANControllerEditImmutableDocBase<MultivariateAnalysisOptions, IPLSStartAnalysisView>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PLSStartAnalysisController"/> class.
    /// </summary>
    public PLSStartAnalysisController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PLSStartAnalysisController"/> class.
    /// </summary>
    /// <param name="options">The analysis options.</param>
    public PLSStartAnalysisController(MultivariateAnalysisOptions options)
    {
      _doc = options;
      Initialize(true);
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int  _numberOfFactors;
    /// <summary>
    /// Gets or sets the maximum number of factors.
    /// </summary>
    public int  NumberOfFactors
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



    private ItemsController<Type> _analysisMethods;

    /// <summary>
    /// Gets or sets the available analysis methods.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the available cross-validation grouping strategies.
    /// </summary>
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

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        NumberOfFactors = _doc.MaxNumberOfFactors;
        InitializeCrossPressCalculationTypes();
        InitializeAnalysisMethods();
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = new MultivariateAnalysisOptions()
      {
        MaxNumberOfFactors = NumberOfFactors,
        CrossValidationGroupingStrategy = (ICrossValidationGroupingStrategy)Activator.CreateInstance(CROSSPressCalculationTypes.SelectedValue),
        AnalysisMethod = AnalysisMethods.SelectedValue
      };

      return ApplyEnd(true, disposeController);
    }

    void InitializeCrossPressCalculationTypes()
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

    void InitializeAnalysisMethods()
    {
      var analysisMethods = new SelectableListNodeList();
      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach (System.Reflection.Assembly assembly in assemblies)
      {
        if (IsOwnAssembly(assembly) || ReferencesOwnAssembly(assembly.GetReferencedAssemblies()))
        {
          Type[] definedtypes = assembly.GetTypes();
          foreach (Type definedtype in definedtypes)
          {
            if (definedtype.IsSubclassOf(typeof(Altaxo.Calc.Regression.Multivariate.WorksheetAnalysis)) && !definedtype.IsAbstract)
            {
              Attribute[] descriptionattributes = Attribute.GetCustomAttributes(definedtype, typeof(System.ComponentModel.DescriptionAttribute));

              string name =
                (descriptionattributes.Length > 0) ?
                ((System.ComponentModel.DescriptionAttribute)descriptionattributes[0]).Description : definedtype.ToString();

              analysisMethods.Add(new SelectableListNode(name, definedtype, false));
            }
          }
        } // end foreach type
      } // end foreach assembly

      AnalysisMethods = new ItemsController<Type>(analysisMethods);
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

    
  

   

    
  }
}
