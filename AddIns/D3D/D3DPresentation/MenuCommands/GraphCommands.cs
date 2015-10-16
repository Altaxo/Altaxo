#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Scripting;
using Altaxo.Main;
using Altaxo.Scripting;
using ICSharpCode.Core;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Altaxo.Graph3D.Commands
{
	/// <summary>
	/// Provides a abstract class for issuing commands that apply to worksheet controllers.
	/// </summary>
	public abstract class AbstractGraph3DControllerCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Determines the currently active worksheet and issues the command to that worksheet by calling
		/// Run with the worksheet as a parameter.
		/// </summary>
		public override void Run()
		{
			Altaxo.Gui.SharpDevelop.SDGraph3DViewContent ctrl
				= Current.Workbench.ActiveViewContent
				as Altaxo.Gui.SharpDevelop.SDGraph3DViewContent;

			if (null != ctrl)
				Run((Altaxo.Gui.Graph3D.Viewing.Graph3DController)ctrl.MVCController);
		}

		/// <summary>
		/// Override this function for adding own worksheet commands. You will get
		/// the worksheet controller in the parameter.
		/// </summary>
		/// <param name="ctrl">The worksheet controller this command is applied to.</param>
		public abstract void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl);
	}

	public class ViewFront : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewFront();
		}
	}

	public class ViewTop : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewTop();
		}
	}

	public class ViewRightFrontTop : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.ViewRightFrontTop();
		}
	}

	public class Export3D : AbstractGraph3DControllerCommand
	{
		public override void Run(Altaxo.Gui.Graph3D.Viewing.Graph3DController ctrl)
		{
			ctrl.Export3D();
		}
	}
}