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
	public abstract class AbstractPropertyCodeGenerator : AbstractFieldCodeGenerator
	{
		public AbstractPropertyCodeGenerator(IClass currentClass) : base(currentClass)
		{
		}
		
		public override int ImageIndex {
			get {
				ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
				return classBrowserIconService.PropertyIndex;
			}
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			for (int i = 0; i < items.Count; ++i) {
				FieldWrapper fw = (FieldWrapper)items[i];
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("Public " + (fw.Field.IsStatic ? "Shared " : "") + "Property " + (Char.ToUpper(fw.Field.Name[0]) + fw.Field.Name.Substring(1)) + " As " + vba.Convert(fw.Field.ReturnType));
				} else {
					editActionHandler.InsertString("public " + (fw.Field.IsStatic ? "static " : "") + csa.Convert(fw.Field.ReturnType) + " " + Char.ToUpper(fw.Field.Name[0]) + fw.Field.Name.Substring(1) + " {");
				}
				
				++numOps;
				Return();
				
				GeneratePropertyBody(editActionHandler, fw, fileExtension);
				
				if (fileExtension == ".vb") {
					editActionHandler.InsertString("End Property");
				} else {
					editActionHandler.InsertChar('}');
				}
				
				++numOps;
				Return();
				IndentLine();
			}
		}
		
		protected void GenerateGetter(TextArea editActionHandler, FieldWrapper fw, string fileExtension)
		{
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Get");
			} else {
				editActionHandler.InsertString("get {");
			}
			++numOps;
			Return();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Return " + fw.Field.Name);
			} else {
				editActionHandler.InsertString("return " + fw.Field.Name+ ";");
			}
			++numOps;	
			Return();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("End Get");
			} else {
				editActionHandler.InsertString("}");
			}
			++numOps;
			Return();
		}
		
		protected void GenerateSetter(TextArea editActionHandler, FieldWrapper fw, string fileExtension)
		{
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Set");
			} else {
				editActionHandler.InsertString("set {");
			}
			++numOps;
			Return();
			
			if (fileExtension == ".vb") {
				editActionHandler.InsertString(fw.Field.Name+ " = Value");
			} else {
				editActionHandler.InsertString(fw.Field.Name+ " = value;");
			}
			++numOps;
			Return();
			
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("End Set");
			} else {
				editActionHandler.InsertString("}");
			}
			++numOps;
			Return();
		}
		
		protected abstract void GeneratePropertyBody(TextArea editActionHandler, FieldWrapper fw, string fileExtension);
	}
}
