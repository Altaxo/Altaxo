﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 946 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser.AST;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class InterfaceOrAbstractClassCodeGenerator : CodeGeneratorBase
	{
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.InterfaceIndex;
			}
		}
		
		protected class ClassWrapper
		{
			IReturnType c;
			public IReturnType ClassType {
				get {
					return c;
				}
			}
			public ClassWrapper(IReturnType c)
			{
				this.c = c;
			}
			
			public override string ToString()
			{
				IAmbience ambience = AmbienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.None;
				return ambience.Convert(c);
			}
		}
	}
	
	public class InterfaceImplementorCodeGenerator : InterfaceOrAbstractClassCodeGenerator
	{
		public override string CategoryName {
			get {
				return "Interface implementation";
			}
		}
		
		public override string Hint {
			get {
				return "Choose interfaces to implement";
			}
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			foreach (ClassWrapper w in items) {
				codeGen.ImplementInterface(nodes, w.ClassType,
				                           !currentClass.ProjectContent.Language.SupportsImplicitInterfaceImplementation,
				                           ModifierEnum.Public, currentClass);
			}
		}
		
		protected override void InitContent()
		{
			for (int i = 0; i < currentClass.BaseTypes.Count; i++) {
				IReturnType baseType = currentClass.GetBaseType(i);
				IClass baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
				if (baseClass != null && baseClass.ClassType == ICSharpCode.SharpDevelop.Dom.ClassType.Interface) {
					Content.Add(new ClassWrapper(baseType));
				}
			}
		}
	}
}
