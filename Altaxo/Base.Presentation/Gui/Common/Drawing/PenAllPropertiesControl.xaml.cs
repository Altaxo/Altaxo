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

using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for PenAllPropertiesControl.xaml
	/// </summary>
	public partial class PenAllPropertiesControl : UserControl, IPenAllPropertiesView
	{
		private PenControlsGlue _glue;

		public PenAllPropertiesControl()
		{
			InitializeComponent();

			_glue = new PenControlsGlue(true);
			_glue.CbBrush = _cbBrush;
			_glue.CbLineThickness = _cbThickness;
			_glue.CbDashStyle = _cbDashStyle;
			_glue.CbDashCap = _cbDashCap;
			_glue.CbStartCap = _cbStartCap;
			_glue.CbStartCapAbsSize = _cbStartCapSize;
			_glue.CbStartCapRelSize = _edStartCapRelSize;
			_glue.CbEndCap = _cbEndCap;
			_glue.CbEndCapAbsSize = _cbEndCapSize;
			_glue.CbEndCapRelSize = _edEndCapRelSize;
			_glue.CbLineJoin = _cbLineJoin;
			_glue.CbMiterLimit = _cbMiterLimit;
			_glue.PreviewPanel = _previewPanel;
		}

		public PenX Pen
		{
			get
			{
				return _glue.Pen;
			}
			set
			{
				_glue.Pen = value;
			}
		}

		public bool ShowPlotColorsOnly
		{
			set { _glue.ShowPlotColorsOnly = value; }
		}
	}
}