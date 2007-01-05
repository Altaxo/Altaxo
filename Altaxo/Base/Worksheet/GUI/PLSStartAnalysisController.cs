#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Gui;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for PLSStartAnalysisController.
  /// </summary>
  public class PLSStartAnalysisController : IApplyController
  {
    MultivariateAnalysisOptions _doc;
    PLSStartAnalysisControl _view;

    System.Collections.ArrayList _methoddictionary = new System.Collections.ArrayList();

    public PLSStartAnalysisController(MultivariateAnalysisOptions options)
    {
      _doc = options;
    }

    void SetElements(bool bInit)
    {
      if(null!=_view)
      {
        _view.InitializeNumberOfFactors(_doc.MaxNumberOfFactors);
        _view.InitializeCrossPressCalculation(_doc.CrossPRESSCalculation);
        InitializeAnalysisMethods();
      }
    }

    public PLSStartAnalysisControl View
    {
      get { return _view; }
      set 
      {
        
        if(null!=_view)
          _view.Controller = null;
        
        _view = value;

        if(null!=_view)
        {
          _view.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    public    MultivariateAnalysisOptions Doc
    {
      get { return _doc; }
    }


    public void EhView_MaxNumberOfFactorsChanged(int numFactors)
    {
      _doc.MaxNumberOfFactors = numFactors;
    }
    public void EhView_CrossValidationSelected(CrossPRESSCalculationType val)
    {
      _doc.CrossPRESSCalculation = val; 
    }

    static bool ReferencesOwnAssembly(System.Reflection.AssemblyName[] references)
    {
      string myassembly = System.Reflection.Assembly.GetCallingAssembly().GetName().FullName;

      foreach(System.Reflection.AssemblyName assname in references)
        if(assname.FullName==myassembly)
          return true;
      return false;
    }

    static bool IsOwnAssembly(System.Reflection.Assembly ass)
    {
      return ass.FullName==System.Reflection.Assembly.GetCallingAssembly().FullName;
    }

    public void InitializeAnalysisMethods()
    {
      _methoddictionary.Clear();
      System.Collections.ArrayList nameList  = new System.Collections.ArrayList();

      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(System.Reflection.Assembly assembly in assemblies)
      {
        if(IsOwnAssembly(assembly) || ReferencesOwnAssembly(assembly.GetReferencedAssemblies()))
        {
        
          Type[] definedtypes = assembly.GetTypes();
          foreach(Type definedtype in definedtypes)
          {
            if(definedtype.IsSubclassOf(typeof(Altaxo.Calc.Regression.Multivariate.WorksheetAnalysis)) && !definedtype.IsAbstract)
            {
              Attribute[] descriptionattributes = Attribute.GetCustomAttributes(definedtype,typeof(System.ComponentModel.DescriptionAttribute));
            
              string name = 
                (descriptionattributes.Length>0) ?
                ((System.ComponentModel.DescriptionAttribute)descriptionattributes[0]).Description :  definedtype.ToString();
          
              _methoddictionary.Add(definedtype);
              nameList.Add(name);
            }
          }
        } // end foreach type
      } // end foreach assembly 
      if(_view!=null)
        _view.InitializeAnalysisMethod((string[])nameList.ToArray(typeof(string)),0);
      _doc.AnalysisMethod = (System.Type)_methoddictionary[0];
    }

    public void EhView_AnalysisMethodChanged(int item)
    {
      _doc.AnalysisMethod = (System.Type)_methoddictionary[item];
    }


    #region IApplyController Members

    public bool Apply()
    {
      // nothing to do here, since the hosted doc is a struct
      return true;
    }

    #endregion
  }
}
