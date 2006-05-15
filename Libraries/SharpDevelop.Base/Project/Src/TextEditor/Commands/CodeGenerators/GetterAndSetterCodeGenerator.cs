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
	public class GetterAndSetterCodeGenerator : AbstractPropertyCodeGenerator
	{
		public override string CategoryName {
			get {
				return "Getter and Setter";
			}
		}
		
		public override string Hint {
			get {
				return "Choose fields to generate getters and setters";
			}
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			foreach (FieldWrapper w in items) {
				nodes.Add(codeGen.CreateProperty(w.Field, true, true));
			}
		}
	}
}
