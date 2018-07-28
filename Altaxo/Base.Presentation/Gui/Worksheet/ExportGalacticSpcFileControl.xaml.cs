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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// Interaction logic for ExportGalacticSpcFileControl.xaml
  /// </summary>
  public partial class ExportGalacticSpcFileControl : UserControl, IExportGalacticSpcFileView
  {
    public ExportGalacticSpcFileControl()
    {
      InitializeComponent();
    }

    public bool CreateSpectrumFromRow
    {
      get
      {
        return true == this.m_rbCreateSpectrum_FromRow.IsChecked;
      }
      set
      {
        this.m_rbCreateSpectrum_FromRow.IsChecked = value;
      }
    }

    public bool CreateSpectrumFromColumn
    {
      get { return true == this.m_rbCreateSpectrum_FromColumn.IsChecked; }
    }

    public bool XValuesContinuousNumber
    {
      get
      {
        return true == this.m_rbXValues_ContinuousNumber.IsChecked;
      }
      set
      {
        this.m_rbXValues_ContinuousNumber.IsChecked = value;
      }
    }

    public bool XValuesFromColumn
    {
      get { return true == this.m_rbXValues_FromColumn.IsChecked; }
    }

    public bool ExtendFileName_ContinuousNumber
    {
      get
      {
        return true == this.m_rbExtFileName_ContinuousNumber.IsChecked;
      }
      set
      {
        this.m_rbExtFileName_ContinuousNumber.IsChecked = value;
      }
    }

    public bool ExtendFileName_ByColumn
    {
      get { return true == this.m_rbFileName_FromColumn.IsChecked; }
    }

    public string BasicFileName
    {
      get
      {
        return this.m_edBasicFileNameAndPath.Text;
      }
      set
      {
        this.m_edBasicFileNameAndPath.Text = value;
      }
    }

    public void FillXValuesColumnBox(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(m_cbXValues_Column, list);
    }

    public bool EnableXValuesColumnBox
    {
      set { this.m_cbXValues_Column.IsEnabled = value; }
    }

    public string XValuesColumnName
    {
      get
      {
        GuiHelper.SynchronizeSelectionFromGui(m_cbXValues_Column);
        return (string)this.m_cbXValues_Column.SelectedItem;
      }
    }

    public void FillExtFileNameColumnBox(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(m_cbExtFileName_Column, list);
    }

    public bool EnableExtFileNameColumnBox
    {
      set { this.m_cbExtFileName_Column.IsEnabled = value; }
    }

    public string ExtFileNameColumnName
    {
      get
      {
        GuiHelper.SynchronizeSelectionFromGui(m_cbExtFileName_Column);
        return (string)this.m_cbExtFileName_Column.SelectedItem;
      }
    }

    public event Action BasicFileNameAndPathChoose;

    public event Action Change_CreateSpectrumFrom;

    public event Action Change__XValuesFromOption;

    public event Action Change_ExtendFileNameOptions;

    private void EhCreateSpectrumFrom_Changed(object sender, RoutedEventArgs e)
    {
      if (null != Change_CreateSpectrumFrom)
        Change_CreateSpectrumFrom();
    }

    private void EhXValues_Changed(object sender, RoutedEventArgs e)
    {
      if (null != Change__XValuesFromOption)
        Change__XValuesFromOption();
    }

    private void EhExtendFileNameBy_Changed(object sender, RoutedEventArgs e)
    {
      if (null != Change_ExtendFileNameOptions)
        Change_ExtendFileNameOptions();
    }

    private void EhChooseBasicFileName(object sender, RoutedEventArgs e)
    {
      if (null != BasicFileNameAndPathChoose)
        BasicFileNameAndPathChoose();
    }
  }
}
