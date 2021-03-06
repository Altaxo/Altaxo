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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Settings
{
  public abstract class OptionPanelBase<T> : IOptionPanel where T : class, IMVCANController
  {
    protected T? _controller;

    /// <summary>
    /// Creates the controller and calls _controller.InitializeDocument.
    /// </summary>
    /// <param name="optionPanelOwner">The option panel owner.</param>
    public abstract void Initialize(object? optionPanelOwner);

    /// <summary>
    /// Is called after the Apply function of the controller returned true (success). Here the document has to be retrieved from the controller,
    /// and then the retrieved document should be processed (stored etc.).
    /// </summary>
    protected abstract void ProcessControllerResult();

    public object? Owner { get; set; }

    public object? ViewObject
    {
      get
      {
        if (_controller is null)
          throw new InvalidOperationException("Option panel not initialized, controller is null!");

        if (_controller.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_controller);

        return _controller.ViewObject;
      }
    }

    public bool Apply()
    {
      if (_controller is null)
        throw new InvalidOperationException("Option panel not initialized, controller is null!");

      var result = _controller.Apply(false);
      if (result)
      {
        ProcessControllerResult();
      }
      return result;
    }
  }
}
