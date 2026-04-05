#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

#nullable disable
using System.Collections.Generic;
using Altaxo.Serialization.Ascii;

namespace Altaxo.Gui.Serialization.Ascii
{
  /// <summary>
  /// View interface for editing <see cref="SingleCharSeparationStrategy"/>.
  /// </summary>
  public interface ISingleCharSeparationStrategyView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing <see cref="SingleCharSeparationStrategy"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(ISingleCharSeparationStrategyView))]
  [UserControllerForObject(typeof(SingleCharSeparationStrategy))]
  public class SingleCharSeparationStrategyController : MVCANControllerEditImmutableDocBase<SingleCharSeparationStrategy, ISingleCharSeparationStrategyView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private char _separatorChar;

    /// <summary>
    /// Gets or sets the separator character.
    /// </summary>
    public char SeparatorChar
    {
      get => _separatorChar;
      set
      {
        if (!(_separatorChar == value))
        {
          _separatorChar = value;
          OnPropertyChanged(nameof(SeparatorChar));
        }
      }
    }


    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        SeparatorChar = _doc.SeparatorChar;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        SeparatorChar = SeparatorChar,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
