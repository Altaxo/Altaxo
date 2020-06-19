#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  public struct DocumentNodeAndName
  {
    private IDocumentLeafNode _doc;
    private string _name;
    private Action? _setMemberToNullAction;

    public DocumentNodeAndName(IDocumentLeafNode doc, string name)
    {
      _doc = doc;
      _name = name;
      _setMemberToNullAction = null;
    }

    public DocumentNodeAndName(IDocumentLeafNode doc, Action setMemberToNullAction, string name)
    {
      _doc = doc;
      _name = name;
      _setMemberToNullAction = setMemberToNullAction;
    }

    public IDocumentLeafNode DocumentNode { get { return _doc; } }

    public string Name { get { return _name; } }

    public Action? SetMemberToNullAction { get { return _setMemberToNullAction; } }

    public bool IsEmpty { get { return _doc is null; } }
  }
}
