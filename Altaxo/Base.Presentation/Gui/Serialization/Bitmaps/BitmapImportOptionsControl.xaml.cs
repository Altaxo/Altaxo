#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System.Windows.Controls;
using Altaxo.Gui.Serialization.BrukerOpus;
using Altaxo.Gui.Serialization.Jcamp;
using Altaxo.Gui.Serialization.NicoletSPA;
using Altaxo.Gui.Serialization.Renishaw;

namespace Altaxo.Gui.Serialization.Bitmaps
{
  /// <summary>
  /// Interaction logic for GalacticSPCImportOptionsControl.xaml
  /// </summary>
  public partial class BitmapImportOptionsControl : UserControl,
    IBitmapImportOptionsView
  {
    public BitmapImportOptionsControl()
    {
      InitializeComponent();
    }
  }
}
