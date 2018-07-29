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

using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  public class LanguageDependentString
  {
    private string _originalString;
    private string _transformedString;

    public event EventHandler ValueChanged;

    public LanguageDependentString(string originalString)
    {
      _originalString = originalString ?? throw new ArgumentNullException(nameof(originalString));

      LanguageChangeWeakEventManager.LanguageChanged += EhLanguageChanged;
      _transformedString = StringParser.Parse(_originalString);
    }

    private void EhLanguageChanged()
    {
      _transformedString = StringParser.Parse(_originalString);
      ValueChanged.Invoke(this, EventArgs.Empty);
    }

    public string Value
    {
      get
      {
        return _transformedString;
      }
    }
  }
}
