#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2013 Dr. Dirk Lellinger
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
using System.Windows.Media;

namespace Altaxo.Gui.Graph.Gdi.Viewing
{
	/// <summary>
	/// Helper control that fires the action <see cref="RenderTriggered"/> when its (empty) content is rendered.
	/// By calling InvalidateVisual on this control, the rendering is done with RenderPriority. This helps the bundle repetitive calls to render into one render call.
	/// </summary>
	public class RenderTrigger : Control
	{
		/// <summary>
		/// Event fired when the Gui calls OnRender. The two arguments are the actual x and y size of the RenderTrigger area.
		/// </summary>
		public event Action<double, double> RenderTriggered;

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			// Trigger the rendering
			var action = RenderTriggered;
			if (null != action)
			{
				//System.Diagnostics.Debug.WriteLine("Before firing event RenderTriggered");
				action(this.ActualWidth, this.ActualHeight);
				//System.Diagnostics.Debug.WriteLine("After firing event RenderTriggered");
			}
		}
	}
}