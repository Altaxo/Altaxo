﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
  public interface IParametersView
  {
    void SetParametersSource(List<System.Data.OleDb.OleDbParameter> parms);

    void ReadParameter();
  }

  [ExpectedTypeOfView(typeof(IParametersView))]
  public class ParametersController : IMVCAController
  {
    private IParametersView _view;
    private List<System.Data.OleDb.OleDbParameter> _doc;

    public ParametersController(List<System.Data.OleDb.OleDbParameter> parms)
    {
      // TODO: Complete member initialization
      _doc = parms;
      Initialize(true);
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
      }
      if (_view is not null)
      {
        _view.SetParametersSource(_doc);
      }
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as IParametersView;
        if (_view is not null)
        {
          Initialize(false);
        }
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    public void Dispose()
    {
      ViewObject = null;
    }

    public bool Apply(bool disposeController)
    {
      _view.ReadParameter();
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }
  }
}
