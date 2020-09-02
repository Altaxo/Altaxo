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
using System.Threading.Tasks;

namespace Altaxo.Text
{
  public static class TextDocumentOtherActions
  {
    #region Rename text document

    public static void ShowRenameDialog(this TextDocument doc)
    {
      var tvctrl = new Altaxo.Gui.Common.TextValueInputController(doc.Name, "Enter a name for the text document:")
      {
        Validator = new TextDocumentRenameValidator(doc)
      };

      if (Current.Gui.ShowDialog(tvctrl, "Rename text document", false))
        doc.Name = tvctrl.InputText.Trim();
    }

    private class TextDocumentRenameValidator : Altaxo.Gui.Common.TextValueInputController.NonEmptyStringValidator
    {
      private TextDocument _doc;

      public TextDocumentRenameValidator(TextDocument doc)
        : base("The text document's name must not be empty! Please enter a valid name.")
      {
        _doc = doc;
      }

      public override string? Validate(string name)
      {
        var err = base.Validate(name);
        if (err is not null)
          return err;

        if (_doc.Name == name)
          return null;
        else if (TextDocumentCollection.GetParentTextDocumentCollectionOf(_doc) is null)
          return null; // if there is no parent data set we can enter anything
        else if (TextDocumentCollection.GetParentTextDocumentCollectionOf(_doc)?.ContainsAnyName(name) ?? false)
          return "This text document name already exists, please choose another name!";
        else
          return null;
      }
    }

    #endregion Rename text document

    #region Show properties dialog

    public static void ShowPropertyDialog(this TextDocument doc)
    {
      var propHierarchy = new Altaxo.Main.Properties.PropertyHierarchy(PropertyExtensions.GetPropertyBags(doc));
      Current.Gui.ShowDialog(new object[] { propHierarchy }, "Text document properties", true);
    }

    #endregion Show properties dialog
  }
}
