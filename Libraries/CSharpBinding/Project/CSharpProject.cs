// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace CSharpBinding
{
	/// <summary>
	/// This class describes a C Sharp project and it compilation options.
	/// </summary>
	public class CSharpProject : AbstractProject
	{		
		public override string ProjectType {
			get {
				return CSharpLanguageBinding.LanguageName;
			}
		}
		
		public CSharpProject()
		{
		}
		
		public override IConfiguration CreateConfiguration()
		{
			return new CSharpCompilerParameters();
		}
		
		public CSharpProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			if (info != null) {
				Name = info.ProjectName;
				Configurations.Add(CreateConfiguration("Debug"));
				Configurations.Add(CreateConfiguration("Release"));
				
				foreach (CSharpCompilerParameters parameter in Configurations) {
					parameter.OutputDirectory = info.BinPath + Path.DirectorySeparatorChar + parameter.Name;
					parameter.OutputAssembly  = Name;
					//}
					//XmlElement el = info.ProjectTemplate.ProjectOptions; -- moved above foreach loop
					//para variable renamed parameter
					//CSharpCompilerParameters para = ActiveConfiguration;?? - removed as nolonger needed
					if (projectOptions != null) {
						if (projectOptions.Attributes["Target"] != null) {
							parameter.CompileTarget = (CompileTarget)Enum.Parse(typeof(CompileTarget), projectOptions.Attributes["Target"].InnerText);
						}
						if (projectOptions.Attributes["PauseConsoleOutput"] != null) {
							parameter.PauseConsoleOutput = Boolean.Parse(projectOptions.Attributes["PauseConsoleOutput"].InnerText);
						}
					}
				}
			}
		}
	}
}
