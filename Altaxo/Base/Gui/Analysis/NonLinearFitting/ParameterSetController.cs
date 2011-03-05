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
using System.Collections.Generic;

using Altaxo.Gui.Common;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public class ParameterSetViewItem
  {
		public string Name { get; set; }
    public string Value { get; set; }
    public bool Vary { get; set; }
    public string Variance { get; set; }
  }

  public interface IParameterSetView
  {
    void Initialize(List<ParameterSetViewItem> list);
    List<ParameterSetViewItem> GetList();

  }

  /// <summary>
  /// Summary description for ParameterSetController.
  /// </summary>
  [UserControllerForObject(typeof(ParameterSet))]
  [ExpectedTypeOfView(typeof(IParameterSetView))]
  public class ParameterSetController1 : IMVCANController
  {
    ParameterSet _doc;
    IParameterSetView _view;

    #region IMVCANController Members

    bool IMVCANController.InitializeDocument(params object[] args)
    {
      if (args == null || args.Length < 1)
        throw new ArgumentException("args null or empty");
      if (args[0] is ParameterSet)
      {
        _doc = args[0] as ParameterSet;
        Initialize(true);
        return true;
      }
      else
      {
        return false;
      }
    }


    UseDocument IMVCANController.UseDocumentCopy
    {
      set
      { 
      }
    }

    #endregion

    void Initialize(bool initDoc)
    {
      if (_view != null)
      {
        List<ParameterSetViewItem> list = new List<ParameterSetViewItem>();

        for (int i = 0; i < _doc.Count; i++)
        {
          ParameterSetViewItem item = new ParameterSetViewItem();
          item.Name = _doc[i].Name;
          item.Value = Altaxo.Serialization.GUIConversion.ToString(_doc[i].Parameter);
          item.Vary = _doc[i].Vary;
          item.Variance = Altaxo.Serialization.GUIConversion.ToString(_doc[i].Variance);

          list.Add(item);
        }

        _view.Initialize(list);
      }
    }

    #region IMVCController Members

    object IMVCController.ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as IParameterSetView;
        Initialize(false);
      }
    }

    object IMVCController.ModelObject
    {
      get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    bool IApplyController.Apply()
    {

      List<ParameterSetViewItem> list = _view.GetList();

      for (int i = 0; i < _doc.Count; i++)
      {
        double paraValue;
        double varianceValue;

        // Parameter
        if (Altaxo.Serialization.GUIConversion.IsDouble(list[i].Value, out paraValue))
        {
          _doc[i].Parameter = paraValue;
        }
        else
        {
          Current.Gui.ErrorMessageBox(string.Format("Parameter {0} is not numeric", list[i].Name));
          return false;
        }

        // Vary
        _doc[i].Vary = list[i].Vary;

        // Variance
        if (Altaxo.Serialization.GUIConversion.IsDouble(list[i].Variance, out varianceValue))
        {
          _doc[i].Variance = varianceValue;
        }
        else if (!string.IsNullOrEmpty(list[i].Variance))
        {
          Current.Gui.ErrorMessageBox(string.Format("Variance of parameter {0} is not numeric", list[i].Name));
          return false;
        }
      }

      return true;
    }

    #endregion
  }

  /*
  /// <summary>
  /// Summary description for ParameterSetController.
  /// </summary>
  [UserControllerForObject(typeof(ParameterSet))]
  public class ParameterSetController : Altaxo.Gui.Common.MultiChildController
  {
    ParameterSet _doc;

    public ParameterSetController(ParameterSet doc)
    {
      _doc = doc;
      _doc.InitializationFinished += new EventHandler(EhInitializationFinished);

      base.DescriptionText = "ParameterName                                      Value                     Vary?       Variance\r\n" +
                        "-------------------------------------------------------------------------------------------------------";
      
      EhInitializationFinished(this, EventArgs.Empty);
    }

  
    private void EhInitializationFinished(object sender, EventArgs e)
    {
      ControlViewElement[] childs = new ControlViewElement[_doc.Count];
      for (int i = 0; i < childs.Length; i++)
      {
        IMVCAController ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc[i] }, typeof(IParameterSetElementController));
        childs[i] = new ControlViewElement(null, ctrl, ctrl.ViewObject);
      }

      base.Initialize(childs, false);
    }
  }
   */
}
