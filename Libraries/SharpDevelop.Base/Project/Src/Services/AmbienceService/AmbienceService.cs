﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2198 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.SharpDevelop
{
	public static class AmbienceService
	{
		const string ambienceProperty       = "SharpDevelop.UI.CurrentAmbience";
		const string codeGenerationProperty = "SharpDevelop.UI.CodeGenerationOptions";
		const string textEditorProperty     = "ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties";
		
		static AmbienceService()
		{
			PropertyService.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
			ApplyCodeGenerationPropertiesToNRefactory();
		}
		
		public static Properties CodeGenerationProperties {
			get {
				return PropertyService.Get(codeGenerationProperty, new Properties());
			}
		}
		
		static void ApplyCodeGenerationPropertiesToNRefactory()
		{
			Properties p = CodeGenerationProperties;
			LanguageProperties.CSharp.CodeGenerator.Options.EmptyLinesBetweenMembers = p.Get("BlankLinesBetweenMembers", true);
			LanguageProperties.CSharp.CodeGenerator.Options.BracesOnSameLine = p.Get("StartBlockOnSameLine", true);
			
			System.CodeDom.Compiler.CodeGeneratorOptions cdo = new CodeDOMGeneratorUtility().CreateCodeGeneratorOptions;
			LanguageProperties.CSharp.CodeGenerator.Options.IndentString = cdo.IndentString;
		}
		
		public static bool GenerateDocumentComments {
			get {
				return CodeGenerationProperties.Get("GenerateDocumentComments", true);
			}
		}
		
		public static bool GenerateAdditionalComments {
			get {
				return CodeGenerationProperties.Get("GenerateAdditionalComments", true);
			}
		}
		
		public static bool UseFullyQualifiedNames {
			get {
				return CodeGenerationProperties.Get("UseFullyQualifiedNames", true);
			}
		}
		
		public static bool UseProjectAmbienceIfPossible {
			get {
				return PropertyService.Get("SharpDevelop.UI.UseProjectAmbience", true);
			}
			set {
				PropertyService.Set("SharpDevelop.UI.UseProjectAmbience", value);
			}
		}
		
		static AmbienceReflectionDecorator defaultAmbience;
		
		public static AmbienceReflectionDecorator CurrentAmbience {
			get {
				if (UseProjectAmbienceIfPossible) {
					ICSharpCode.SharpDevelop.Project.IProject p = ICSharpCode.SharpDevelop.Project.ProjectService.CurrentProject;
					if (p != null) {
						IAmbience ambience = p.Ambience;
						if (ambience != null) {
							return new AmbienceReflectionDecorator(ambience);
						}
					}
				}
				if (defaultAmbience == null) {
					string language = DefaultAmbienceName;
					IAmbience ambience = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/" + language, null);
					if (ambience == null) {
						MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Services.AmbienceService.AmbienceNotFoundError}");
						return null;
					}
					defaultAmbience = new AmbienceReflectionDecorator(ambience);
				}
				return defaultAmbience;
			}
		}
		
		public static string DefaultAmbienceName {
			get {
				return PropertyService.Get(ambienceProperty, "C#");
			}
			set {
				PropertyService.Set(ambienceProperty, value);
			}
		}
		
		static void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.Key == ambienceProperty) {
				defaultAmbience = null;
				OnAmbienceChanged(EventArgs.Empty);
			}
			if (e.Key == codeGenerationProperty || e.Key == textEditorProperty) {
				ApplyCodeGenerationPropertiesToNRefactory();
			}
		}
		
		static void OnAmbienceChanged(EventArgs e)
		{
			if (AmbienceChanged != null) {
				AmbienceChanged(null, e);
			}
		}
		
		public static event EventHandler AmbienceChanged;
	}
}
