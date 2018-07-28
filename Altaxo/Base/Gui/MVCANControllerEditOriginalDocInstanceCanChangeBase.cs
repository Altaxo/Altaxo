#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui
{
  /// <summary>
  /// Base of all controllers that edit the original document directly (live). This class is especially well suited to edit a document class with
  /// many derived classes, and the user can choose among the types. If the user choose a new type of document, this type must be instantiated and edited henceforth.
  /// The old document should be released then, and the new document must be integrated in the document hierarchy.
  /// </summary>
  /// <typeparam name="TModel">The type of the document to edit.</typeparam>
  /// <typeparam name="TView">The type of the view.</typeparam>
  public abstract class MVCANControllerEditOriginalDocInstanceCanChangeBase<TModel, TView>
    : MVCANControllerEditOriginalDocBase<TModel, TView>
    where TModel : ICloneable
    where TView : class
  {
    protected Action<TModel> _setInstanceInParentNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="MVCANControllerEditOriginalDocInstanceCanChangeBase{TModel, TView}"/> class.
    /// </summary>
    /// <param name="SetInstanceInParentNode">Action that sets the instance that this controller is editing in the parent node. This action is typically called when
    /// the user chooses a new type of model in the Gui.</param>
    /// <exception cref="System.ArgumentNullException">SetInstanceInParentNode</exception>
    public MVCANControllerEditOriginalDocInstanceCanChangeBase(Action<TModel> SetInstanceInParentNode)
    {
      if (null == SetInstanceInParentNode)
        throw new ArgumentNullException("SetInstanceInParentNode");

      _setInstanceInParentNode = SetInstanceInParentNode;
    }

    /// <summary>
    /// Should be called by a derived controller class when the instance of the model has changed.
    /// </summary>
    /// <param name="oldInstance">The old instance.</param>
    /// <param name="newInstance">The new instance.</param>
    protected void OnDocumentInstanceChanged(TModel oldInstance, TModel newInstance)
    {
      Altaxo.Main.ISuspendToken newSuspendToken = null;

      if (null != _suspendToken)
      {
        if (newInstance is Altaxo.Main.ISuspendableByToken)
          newSuspendToken = ((Altaxo.Main.ISuspendableByToken)newInstance).SuspendGetToken();
      }

      // Set the instance
      if (null != _setInstanceInParentNode)
        _setInstanceInParentNode(newInstance);

      // Release old suspend token and store instead the new suspend token
      if (null != _suspendToken)
        _suspendToken.Dispose();

      _suspendToken = newSuspendToken;
    }
  }
}
