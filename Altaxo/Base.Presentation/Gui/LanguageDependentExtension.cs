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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Altaxo.Gui
{
  /// <summary>
  /// Base class of language dependent extensions like <see cref="LocalizeExtension"/> or <see cref="StringParseExtension"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Markup.MarkupExtension" />
  /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
  /// <seealso cref="System.Windows.IWeakEventListener" />
  public abstract class LanguageDependentExtension : MarkupExtension, INotifyPropertyChanged
  {
    protected LanguageDependentExtension()
    {
      UpdateOnLanguageChange = true;
    }

    public abstract string Value { get; }

    /// <summary>
    /// Set whether the LocalizeExtension should use a binding to automatically
    /// change the text on language changes.
    /// The default value is true.
    /// </summary>
    public bool UpdateOnLanguageChange { get; set; }

    private bool isRegisteredForLanguageChange;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (UpdateOnLanguageChange)
      {
        var binding = new Binding("Value") { Source = this, Mode = BindingMode.OneWay };
        return binding.ProvideValue(serviceProvider);
      }
      else
      {
        return Value;
      }
    }

    private event System.ComponentModel.PropertyChangedEventHandler? ChangedEvent;

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged
    {
      add
      {
        if (!isRegisteredForLanguageChange)
        {
          isRegisteredForLanguageChange = true;
          LanguageChangeWeakEventManager.LanguageChanged += EhLanguageChanged;
        }
        ChangedEvent += value;
      }
      remove { ChangedEvent -= value; }
    }

    private static readonly System.ComponentModel.PropertyChangedEventArgs
        valueChangedEventArgs = new System.ComponentModel.PropertyChangedEventArgs("Value");



    private void EhLanguageChanged()
    {
      ChangedEvent?.Invoke(this, valueChangedEventArgs);
    }
  }
}
