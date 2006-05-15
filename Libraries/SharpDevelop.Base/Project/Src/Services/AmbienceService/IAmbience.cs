﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Collections;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	[Flags]
	public enum ConversionFlags {
		None                   = 0,
		ShowParameterNames     = 1,
		ShowAccessibility      = 16,
		UseFullyQualifiedNames = 2,
		ShowModifiers          = 4,
		ShowInheritanceList    = 8,
		IncludeHTMLMarkup      = 32,
		UseLinkArrayList       = 64,
		QualifiedNamesOnlyForReturnTypes = 128,
		IncludeBodies                    = 256,
		ShowReturnType                   = 512,
		
		StandardConversionFlags = ShowParameterNames | 
		                          UseFullyQualifiedNames | 
		                          ShowReturnType |
		                          ShowModifiers,
		                          
		All = ShowParameterNames | 
		      ShowAccessibility | 
		      UseFullyQualifiedNames |
		      ShowModifiers | 
		      ShowReturnType |
		      ShowInheritanceList,
		      
		AssemblyScoutDefaults = StandardConversionFlags |
		                        ShowAccessibility |	
		                        QualifiedNamesOnlyForReturnTypes |
		                        IncludeHTMLMarkup |
		                        ShowReturnType|
		                        UseLinkArrayList,
	}
	
	public interface IAmbience
	{
		ConversionFlags ConversionFlags {
			get;
			set;
		}
		
		string Convert(ModifierEnum modifier);
		
		string Convert(IClass c);
		string ConvertEnd(IClass c);
		
		string Convert(IField field);
		string Convert(IProperty property);
		string Convert(IEvent e);
		
		string Convert(IMethod m);
		string ConvertEnd(IMethod m);
		
		string Convert(IParameter param);
		string Convert(IReturnType returnType);
		
		string WrapAttribute(string attribute);
		string WrapComment(string comment);
		
		string GetIntrinsicTypeName(string dotNetTypeName);
		
		ArrayList LinkArrayList { get; set; }
	}
}
