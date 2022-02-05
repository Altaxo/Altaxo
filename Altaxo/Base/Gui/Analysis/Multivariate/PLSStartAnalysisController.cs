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

  public interface IPLSStartAnalysisView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Summary description for PLSStartAnalysisController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPLSStartAnalysisView))]
   [UserControllerForObject(typeof(MultivariateAnalysisOptions))]
  public class PLSStartAnalysisController : MVCANControllerEditImmutableDocBase<MultivariateAnalysisOptions, IPLSStartAnalysisView>
  {
    public PLSStartAnalysisController()
    {
    }

    public PLSStartAnalysisController(MultivariateAnalysisOptions options)
    {
      _doc = options;
      Initialize(true);
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int  _numberOfFactors;
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

    public SelectableListNodeList CROSSPressCalculationTypes { get; } = new SelectableListNodeList();


    private ItemsController<Type> _AnalysisMethods;

    public ItemsController<Type> AnalysisMethods
    {
      get => _AnalysisMethods;
      set
      {
        if (!(_AnalysisMethods == value))
        {
          _AnalysisMethods = value;
          OnPropertyChanged(nameof(AnalysisMethods));
        }
      }
    }

    public CrossPRESSCalculationType SelectedCrossPressCalculationType
    {
      get
      {
        return CROSSPressCalculationTypes.FirstSelectedNode?.Tag is CrossPRESSCalculationType cp ? cp : CrossPRESSCalculationType.None;
      }
      set
      {
        CROSSPressCalculationTypes.ForEachDo(x => x.IsSelected = (value == (CrossPRESSCalculationType)x.Tag));
      }
    }


    #endregion


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

    public override bool Apply(bool disposeController)
    {
      _doc = new MultivariateAnalysisOptions()
      {
        MaxNumberOfFactors = NumberOfFactors,
        CrossPRESSCalculation = SelectedCrossPressCalculationType,
        AnalysisMethod = AnalysisMethods.SelectedValue
      };

      return ApplyEnd(true, disposeController);
    }

    void InitializeCrossPressCalculationTypes()
    {
      CROSSPressCalculationTypes.Clear();

      foreach(CrossPRESSCalculationType v in Enum.GetValues(typeof(CrossPRESSCalculationType)))
      {
        var text = v switch
        {
          CrossPRESSCalculationType.None => "None",
          CrossPRESSCalculationType.ExcludeEveryMeasurement => "Exclude every measurement",
          CrossPRESSCalculationType.ExcludeGroupsOfSimilarMeasurements => "Exclude groups of similar measurements",
          CrossPRESSCalculationType.ExcludeHalfEnsemblyOfMeasurements => "Exclude half ensemble of measurements",
          _ => Enum.GetName(typeof(CrossPRESSCalculationType), v)
        };

        CROSSPressCalculationTypes.Add(new SelectableListNode(text, v, v == _doc.CrossPRESSCalculation));
        SelectedCrossPressCalculationType = _doc.CrossPRESSCalculation;
      }
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
