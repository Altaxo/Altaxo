#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

extern alias MCW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.CodeEditing.Completion;
using MCW::Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.LiveDocumentFormatting
{
  /// <summary>
  /// Responsible for formatting the document live when entering certain characters, e.g. semicolon, or curly brace.
  /// </summary>
  public interface ILiveDocumentFormatter
  {
    /// <summary>
    /// Formats the document after entering a trigger character. Trigger chars are e.g. closing curly brace (then format whole paragraph)
    /// or semicolon (then format line).
    /// </summary>
    /// <param name="caretPosition">The caret position after (!) the trigger char.</param>
    /// <param name="triggerChar">The trigger char.</param>
    /// <returns></returns>
    Task FormatDocumentAfterEnteringTriggerChar(Workspace workspace, DocumentId documentId, RoslynSourceTextContainerAdapter sourceText, int caretPosition, char triggerChar);
  }
}
