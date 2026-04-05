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

#nullable enable

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Summary description for SingleChoiceObjectController.
  /// </summary>
  /// <summary>
  /// Controller for <see cref="ISingleChoiceObject"/> instances.
  /// </summary>
  [UserControllerForObject(typeof(ISingleChoiceObject), 100)]
  public class SingleChoiceObjectController : SingleChoiceController
  {
    /// <summary>
    /// The edited choice object.
    /// </summary>
    protected ISingleChoiceObject _choiceObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleChoiceObjectController"/> class.
    /// </summary>
    /// <param name="o">The choice object to edit.</param>
    public SingleChoiceObjectController(ISingleChoiceObject o)
      :
      base(o.Choices, o.Selection)
    {
      _choiceObject = o;
    }

    /// <inheritdoc/>
    public override object ModelObject
    {
      get
      {
        return _choiceObject;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (!base.Apply(disposeController))
        return false;

      _choiceObject.Selection = base._choice;

      return true;
    }
  }
}
