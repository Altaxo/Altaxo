﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2195 $</version>
// </file>

using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class NRefactoryInformationProvider : IEnvironmentInformationProvider
	{
		IProjectContent _projectContent;
		
		public NRefactoryInformationProvider(IProjectContent projectContent)
		{
			if (projectContent == null)
				throw new ArgumentNullException("projectContent");
			_projectContent = projectContent;
		}
		
		public bool HasField(string fullTypeName, int typeParameterCount, string fieldName)
		{
			IClass c = _projectContent.GetClass(fullTypeName, typeParameterCount);
			if (c == null)
				return false;
			foreach (IField field in c.DefaultReturnType.GetFields()) {
				if (field.Name == fieldName)
					return true;
			}
			return false;
		}
	}
}
