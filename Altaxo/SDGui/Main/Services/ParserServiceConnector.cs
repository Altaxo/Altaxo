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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;


namespace Altaxo.Main.Services
{
	/// <summary>
	/// Responsible to hold connection to the parser service. Especially when referenced assemblies changed,
	/// the parser service must be updated.
	/// </summary>
	public static class ParserServiceConnector
	{
		static int _sOpenedProjectCounter = 0;

		static ParserServiceConnector()
		{
			Current.ProjectService.ProjectOpened += new ProjectEventHandler(EhProjectService_ProjectOpened);
			Current.ProjectService.ProjectClosed += new ProjectEventHandler(EhProjectService_ProjectClosed);
			Settings.Scripting.ReferencedAssemblies.AssemblyAdded += EhAssemblyAdded;
		}

		/// <summary>
		/// Initialize the connector.
		/// </summary>
		public static void Initialize()
		{
			// do nothing here, the work will be done in the static constructor
		}

		static void EhProjectService_ProjectOpened(object sender, ProjectEventArgs e)
		{
			_sOpenedProjectCounter++;
			//string solutionName = "Solution" + _sOpenedProjectCounter.ToString();
			//ICSharpCode.SharpDevelop.Project.ProjectService.LoadSolution(solutionName);
		}

		static void EhProjectService_ProjectClosed(object sender, ProjectEventArgs e)
		{
			//ICSharpCode.SharpDevelop.Project.ProjectService.CloseSolution();
		}

		static void EhAssemblyAdded(System.Reflection.Assembly asm)
		{
			List<IProjectContent> todelete = new List<IProjectContent>();
			foreach (IProjectContent c in ParserService.DefaultProjectContent.ReferencedContents)
			{
				if (!(c is ReflectionProjectContent))
					continue;
				ReflectionProjectContent rpc = (ReflectionProjectContent)c;

				if (rpc.AssemblyFullName == asm.FullName || rpc.AssemblyLocation == asm.Location)
				{
					todelete.Add(rpc);
				}
			}
			foreach (IProjectContent c in todelete)
				ParserService.DefaultProjectContent.ReferencedContents.Remove(c);

			ReferenceProjectItem item = new ReferenceProjectItem(null, asm.Location);
			((DefaultProjectContent)ParserService.DefaultProjectContent).AddReferencedContent(AssemblyParserService.GetProjectContentForReference(item));
		}

		/// <summary>
		/// Registers a script file. 
		/// </summary>
		/// <param name="scriptFileName"></param>
		public static void RegisterScriptFileName(string scriptFileName)
		{


		}
	}
}
