#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Main.GUI;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  /// <summary>
  /// Summary description for ParameterSetController.
  /// </summary>
  [UserControllerForObject(typeof(ParameterSet))]
  public class ParameterSetController : Altaxo.Main.GUI.MultiChildController
  {
    ParameterSet _doc;

    public ParameterSetController(ParameterSet doc)
    {
      _doc = doc;
      _doc.InitializationFinished += new EventHandler(EhInitializationFinished);

      IMVCAController[] childs = new IMVCAController[_doc.Count];
      for(int i=0;i<childs.Length;i++)
        childs[i] = (IMVCAController)Current.Gui.GetControllerAndControl(new object[]{_doc[i]},typeof(IParameterSetElementController));

      base.DescriptionText =  "ParameterName                                      Value                     Vary?       Variance\r\n" +
                              "-------------------------------------------------------------------------------------------------------";
      base.Initialize(childs);
    }

  
    private void EhInitializationFinished(object sender, EventArgs e)
    {
      IMVCAController[] childs = new IMVCAController[_doc.Count];
      for(int i=0;i<childs.Length;i++)
        childs[i] = (IMVCAController)Current.Gui.GetControllerAndControl(new object[]{_doc[i]},typeof(IMVCAController));

      base.Initialize(childs);
    }
  }
}
