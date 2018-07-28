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

using Altaxo.AddInItems;
using System;

namespace Altaxo.Main.Commands
{
  /// <summary>
  /// Evaluates to <c>true</c> if we are in embedded object mode, i.e. Altaxo was started with
  /// -embedding object args and has loaded an embedded document.
  /// </summary>
  public class EmbeddedObjectModeEvaluator : IConditionEvaluator
  {
    public bool IsValid(object caller, Condition condition)
    {
      string expectedValueS = condition.Properties["value"].ToLowerInvariant();
      bool expectedValue;
      switch (expectedValueS)
      {
        case "false":
          expectedValue = false;
          break;

        case "true":
          expectedValue = true;
          break;

        default:
          throw new ArgumentException(string.Format("In {0}: property 'value' should be either 'false' or 'true', but is here: '{1}'", this.GetType().Name, expectedValueS));
      }

      bool currentValue = Current.ComManager != null && Current.ComManager.IsInEmbeddedMode;

      return currentValue == expectedValue;
    }
  }
}
