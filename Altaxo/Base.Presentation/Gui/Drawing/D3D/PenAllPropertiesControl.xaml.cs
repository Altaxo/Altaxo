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

using Altaxo.Drawing.D3D;
using Altaxo.Graph.Graph3D;
using Altaxo.Gui.Drawing.D3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Drawing.D3D
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
			_glue.CbLineThickness1 = _cbThickness1;
			_glue.CbLineThickness2 = _cbThickness2;
			_glue.CbCrossSection = _guiCrossSection;

			_glue.CbLineStartCap = _cbLineStartCap;
			_glue.CbLineStartCapAbsSize = _cbLineStartCapSize;
			_glue.CbLineStartCapRelSize = _edLineStartCapRelSize;

			_glue.CbLineEndCap = _cbLineEndCap;
			_glue.CbLineEndCapAbsSize = _cbLineEndCapSize;
			_glue.CbLineEndCapRelSize = _edLineEndCapRelSize;

			_glue.CbDashPattern = _cbDashStyle;

			_glue.CbDashStartCap = _cbDashStartCap;
			_glue.CbDashStartCapAbsSize = _cbDashStartCapSize;
			_glue.CbDashStartCapRelSize = _edDashStartCapRelSize;

			_glue.CbDashEndCap = _cbDashEndCap;
			_glue.CbDashEndCapAbsSize = _cbDashEndCapSize;
			_glue.CbDashEndCapRelSize = _edDashEndCapRelSize;

			_glue.CbLineJoin = _cbLineJoin;
			_glue.CbMiterLimit = _cbMiterLimit;

			_glue.PreviewPanel = _previewPanel;
		}

		public PenX3D Pen
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