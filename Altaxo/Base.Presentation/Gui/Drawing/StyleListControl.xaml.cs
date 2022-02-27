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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Collections;
using GongSolutions.Wpf.DragDrop;

namespace Altaxo.Gui.Drawing
{
  /// <summary>
  /// Interaction logic for StyleListControl.xaml
  /// </summary>
  public partial class StyleListControl : UserControl, IStyleListView
  {
    public StyleListControl()
    {
      InitializeComponent();
    }

    private DataTemplate _currentItemsTemplate;


    /// <summary>
    /// Gets or sets the data template for the current items. Controls that use this class can set the items template to a value of choice.
    /// </summary>
    /// <value>
    /// The current items template.
    /// </value>
    public virtual DataTemplate CurrentItemsTemplate
    {
      get
      {
        if (_currentItemsTemplate is null)
          _currentItemsTemplate = FindResource("CurrentItemsTemplateResource") as DataTemplate;
        return _currentItemsTemplate;
      }
      set
      {
        _currentItemsTemplate = value;
      }
    }

    private HierarchicalDataTemplate _availableItemsTemplate;

    /// <summary>
    /// Gets or sets the data template for the current items. Controls that use this class can set the items template to a value of choice.
    /// </summary>
    /// <value>
    /// The current items template.
    /// </value>
    public virtual HierarchicalDataTemplate AvailableItemsTemplate
    {
      get
      {
        if (_availableItemsTemplate is null)
          _availableItemsTemplate = FindResource("AvailableItemsTemplateResource") as HierarchicalDataTemplate;
        return _availableItemsTemplate;
      }
      set
      {
        _availableItemsTemplate = value;
      }
    }
  }
}
