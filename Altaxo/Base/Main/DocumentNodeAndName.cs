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
    private Action _setMemberToNullAction;

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

    public Action SetMemberToNullAction { get { return _setMemberToNullAction; } }

    public bool IsEmpty { get { return null == _doc; } }
  }
}
