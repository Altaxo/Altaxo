#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Drawing.D3D;
using Altaxo.Drawing.DashPatternManagement;
using Altaxo.Drawing.DashPatterns;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D.Plot.Groups;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Altaxo.Gui.Drawing.DashPatternManagement
{
	/// <summary>
	/// Only to go from a class with type parameters to a class without type parameter, because Xaml can not handle class with type parameters.
	/// </summary>
	public abstract class DashPatternComboBoxBase : Altaxo.Gui.Drawing.StyleListComboBoxBase<DashPatternListManager, DashPatternList, IDashPattern>
	{
		public DashPatternComboBoxBase(DashPatternListManager manager) : base(manager)
		{
		}
	}
}