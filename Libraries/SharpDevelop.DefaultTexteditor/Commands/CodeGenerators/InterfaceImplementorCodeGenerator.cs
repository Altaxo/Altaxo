// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using ICSharpCode.TextEditor;

using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class InterfaceImplementorCodeGenerator : CodeGenerator
	{
		ICompilationUnit unit;
		
		public override string CategoryName {
			get {
				return "Interface implementation";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose interfaces to implement";
			}
		}
		
		public override int ImageIndex {
			get {
				ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
				return classBrowserIconService.InterfaceIndex;
			}
		}
		
		public InterfaceImplementorCodeGenerator(IClass currentClass) : base(currentClass)
		{
			IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			
			foreach (string className in currentClass.BaseTypes) {
				IClass baseType = parserService.GetClass(className);
				if (baseType == null) {
					this.unit = currentClass == null ? null : currentClass.CompilationUnit;
					if (unit != null) {
						foreach (IUsing u in unit.Usings) {
							baseType = u.SearchType(className);
							if (baseType != null) {
								break;
							}
						}
					}
				}
				
				if (baseType != null && baseType.ClassType == ClassType.Interface) {
					Content.Add(new ClassWrapper(baseType));
				}
			}
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			for (int i = 0; i < items.Count; ++i) {
				ClassWrapper cw = (ClassWrapper)items[i];
				Queue interfaces = new Queue();
				interfaces.Enqueue(cw.Class);
				while (interfaces.Count > 0) {
					IClass intf = (IClass)interfaces.Dequeue();
					GenerateInterface(intf);
					
					// search an enqueue all base interfaces
					foreach (string interfaceName in intf.BaseTypes) {
						IClass baseType = null;
						foreach (IUsing u in unit.Usings) {
							baseType = u.SearchType(interfaceName);
							if (baseType != null) {
								break;
							}
						}
						if (baseType != null) {
							interfaces.Enqueue(baseType);
						}
					}
				}
			}
		}
		
		void GenerateInterface(IClass intf)
		{
			Return();
			Return();
			editActionHandler.InsertString("#region " + intf.FullyQualifiedName + " interface implementation\n\t\t");++numOps;
			
			foreach (IProperty property in intf.Properties) {
				string returnType = csa.Convert(property.ReturnType);
				editActionHandler.InsertString("public " + returnType + " " + property.Name + " {");++numOps;
				Return();
				
				if (property.CanGet) {
					editActionHandler.InsertString("\tget {");++numOps;
					Return();
					editActionHandler.InsertString("\t\treturn " + GetReturnValue(returnType) +";");++numOps;
					Return();
					editActionHandler.InsertString("\t}");++numOps;
					Return();
				}
				
				if (property.CanSet) {
					editActionHandler.InsertString("\tset {");++numOps;
					Return();
					editActionHandler.InsertString("\t}");++numOps;
					Return();
				}
				
				editActionHandler.InsertChar('}');++numOps;
				Return();
				Return();
				IndentLine();
			}
			
			for (int i = 0; i < intf.Methods.Count; ++i) {
				IMethod method = intf.Methods[i];
				string parameters = String.Empty;
				string returnType = csa.Convert(method.ReturnType);
				
				for (int j = 0; j < method.Parameters.Count; ++j) {
					parameters += csa.Convert(method.Parameters[j]);
					if (j + 1 < method.Parameters.Count) {
						parameters += ", ";
					}
				}
				
				editActionHandler.InsertString("public " + returnType + " " + method.Name + "(" + parameters + ")");++numOps;
				Return();++numOps;
				editActionHandler.InsertChar('{');++numOps;
				Return();
				
				switch (returnType) {
					case "void":
						break;
					default:
						editActionHandler.InsertString("return " + GetReturnValue(returnType) + ";");++numOps;
						break;
				}
				Return();
				
				editActionHandler.InsertChar('}');++numOps;
				if (i + 1 < intf.Methods.Count) {
					Return();
					Return();
					IndentLine();
				} else {
					IndentLine();
				}
			}
			
			Return();
			editActionHandler.InsertString("#endregion");++numOps;
			Return();
		}
		
		string GetReturnValue(string returnType)
		{
			switch (returnType) {
				case "string":
					return "String.Empty";
				case "char":
					return "'\\0'";
				case "bool":
					return "false";
				case "int":
				case "long":
				case "short":
				case "byte":
				case "uint":
				case "ulong":
				case "ushort":
				case "double":
				case "float":
				case "decimal":
					return "0";
				default:
					return "null";
			}
		}
		
		class ClassWrapper
		{
			IClass c;
			public IClass Class {
				get {
					return c;
				}
			}
			public ClassWrapper(IClass c)
			{
				this.c = c;
			}
			
			public override string ToString()
			{
				AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
				return ambienceService.CurrentAmbience.Convert(c);
			}
		}
	}
}
