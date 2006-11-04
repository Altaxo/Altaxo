﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding
{
	public class CSharpLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "C#";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(string fileName, string projectName)
		{
			return new CSharpProject(fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			CSharpProject p = new CSharpProject(info);
			if (projectOptions != null) {
				p.ImportOptions(projectOptions.Attributes);
			}
			return p;
		}
	}
}
