using System;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace Altaxo.Main.Commands
{


	public class AddReferenceToOwnAssemblies : AbstractCommand
	{
	


		public override void Run()
		{

			
			// get the parser service

			ICSharpCode.SharpDevelop.Services.IParserService parserService = (ICSharpCode.SharpDevelop.Services.IParserService)ServiceManager.Services.GetService(typeof(ICSharpCode.SharpDevelop.Services.IParserService));

			HelperProject project = new HelperProject();
			System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			foreach(System.Reflection.Assembly assembly in assemblies)
			{

				// for now, reference only that assemblies that contain "Altaxo" in their name
				// since otherwise the internal database becomes crazy large
				if(assembly.Location.ToLower().IndexOf("altaxo")<0)
					continue;

				ICSharpCode.SharpDevelop.Internal.Project.ProjectReference reference
					= new ICSharpCode.SharpDevelop.Internal.Project.ProjectReference(ICSharpCode.SharpDevelop.Internal.Project.ReferenceType.Assembly, assembly.Location);

				project.ProjectReferences.Add(reference);
				parserService.AddReferenceToCompletionLookup(project,reference);

			}
			
		
		}


		/// <summary>
		/// This class'es purpose in only to add assembly references to the parser service,
		/// since direct adding of assemblies is not possible in #D 0.98
		/// </summary>
		private class HelperProject : ICSharpCode.SharpDevelop.Internal.Project.IProject
		{
			ICSharpCode.SharpDevelop.Internal.Project.Collections.ProjectReferenceCollection _projectReferences = new ICSharpCode.SharpDevelop.Internal.Project.Collections.ProjectReferenceCollection();

			#region IProject Members

			public ICSharpCode.SharpDevelop.Internal.Project.Collections.ProjectFileCollection ProjectFiles
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public ICSharpCode.SharpDevelop.Internal.Project.NewFileSearch NewFileSearch
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public string BaseDirectory
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public ICSharpCode.SharpDevelop.Internal.Project.DeployInformation DeployInformation
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public bool IsFileInProject(string fileName)
			{
				throw new NotImplementedException();
			}

			public bool IsCompileable(string fileName)
			{
				throw new NotImplementedException();
			}

			public ICSharpCode.SharpDevelop.Internal.Project.Collections.ProjectReferenceCollection ProjectReferences
			{
				get
				{
					return this._projectReferences;
				}
			}

			public string Description
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public event System.EventHandler NameChanged;

			public System.Collections.ArrayList Configurations
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public string ProjectType
			{
				get
				{
					return "C#";
				}
			}

			public ICSharpCode.SharpDevelop.Internal.Project.IConfiguration ActiveConfiguration
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public void LoadProject(string fileName)
			{
				throw new NotImplementedException();
			}

			public string Name
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public string GetParseableFileContent(string fileName)
			{
				throw new NotImplementedException();
			}

			public void CopyReferencesToOutputPath(bool force)
			{
				throw new NotImplementedException();
			}

			public bool EnableViewState
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public ICSharpCode.SharpDevelop.Internal.Project.IConfiguration CreateConfiguration()
			{
				throw new NotImplementedException();
			}

			ICSharpCode.SharpDevelop.Internal.Project.IConfiguration ICSharpCode.SharpDevelop.Internal.Project.IProject.CreateConfiguration(string name)
			{
				throw new NotImplementedException();
			}

			public void SaveProject(string fileName)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				// TODO:  Add HelperProject.Dispose implementation
			}

			#endregion
		}

	}

}
