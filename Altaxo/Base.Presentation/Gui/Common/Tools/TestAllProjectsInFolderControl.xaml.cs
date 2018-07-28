#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Windows.Controls;

namespace Altaxo.Gui.Common.Tools
{
  /// <summary>
  /// Interaction logic for TestAllProjectsInFolderControl.xaml
  /// </summary>
  public partial class TestAllProjectsInFolderControl : UserControl, ITestAllProjectsInFolderView
  {
    public TestAllProjectsInFolderControl()
    {
      InitializeComponent();
    }

    public string FolderPaths
    {
      get
      {
        return _guiPathsToTest.Text;
      }
      set
      {
        _guiPathsToTest.Text = value;
      }
    }

    public string ProtocolFileName
    {
      get
      {
        return _guiProtocolFileName.Text;
      }
      set
      {
        _guiProtocolFileName.Text = value;
      }
    }

    public bool TestSavingAndReopening
    {
      get
      {
        return _guiStoreLoadTest.IsChecked == true;
      }
      set
      {
        _guiStoreLoadTest.IsChecked = value;
      }
    }
  }
}
