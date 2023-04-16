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


using Altaxo.Collections;

namespace Altaxo.Gui
{
  /// <summary>
  /// Extends <see cref="SelectableListNode"/>, accomodating a <see cref="IMVCAController"/>, and exposing both controller and view.
  /// </summary>
  /// <seealso cref="Altaxo.Collections.SelectableListNode" />
  class SelectableListNodeWithController : SelectableListNode
  {
    public SelectableListNodeWithController(string text, object tag, bool isSelected) : base(text, tag, isSelected) { }

    protected IMVCAController? _controller;

    /// <summary>
    /// Gets or sets the controller.
    /// </summary>
    /// <value>
    /// The controller.
    /// </value>
    public IMVCAController? Controller
    {
      get => _controller;
      set
      {
        if (!(_controller == value))
        {
          _controller = value;
          OnPropertyChanged(nameof(Controller));
          OnPropertyChanged(nameof(ViewObject));
        }
      }
    }

    public object? _viewObject;

    /// <summary>
    /// Gets or sets the view object. If this property was not explicitly set, the return value is the view object of the <see cref="Controller"/>.
    /// By explicitly setting this property to a value that is not null, you can override the <see cref="ViewObject"/>. By setting
    /// this property back to null, the default state (view object of the <see cref="Controller"/> is resumed.
    /// </summary>
    public object? ViewObject
    {
      get => _viewObject ?? Controller?.ViewObject;
      set
      {
        if (!object.ReferenceEquals(_viewObject, value))
        {
          _viewObject = value;
          OnPropertyChanged(nameof(ViewObject));
        }
      }
    }


    private object _controllerTag;

    /// <summary>
    /// Additional tag that identifies the controller (for instance index etc.).
    /// </summary>
    public object ControllerTag
    {
      get => _controllerTag;
      set
      {
        if (!(_controllerTag == value))
        {
          _controllerTag = value;
          OnPropertyChanged(nameof(ControllerTag));
        }
      }
    }


  }
}
