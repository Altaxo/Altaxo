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
	public class ToStringCodeGenerator : CodeGenerator
	{
		public override string CategoryName {
			get {
				return "Generate default ToString() method";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose Properties to include in the description";
			}
		}
		public override int ImageIndex {
			get {
				ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
				return classBrowserIconService.MethodIndex;
			}
		}
		
		public ToStringCodeGenerator(IClass currentClass) : base(currentClass)
		{
			foreach (IField field in currentClass.Fields) {
				Content.Add(new FieldWrapper(field));
			}
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Public Overrides Function ToString() As String");
			} else {
				editActionHandler.InsertString("public override string ToString()");
			}
			++numOps;
			
			Return();
			if (fileExtension != ".vb") {
				editActionHandler.InsertString("{");++numOps;
			}
			Return();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("Return String.Format(\"[");
			} else {
				editActionHandler.InsertString("return String.Format(\"[");
			}
			++numOps;
			editActionHandler.InsertString(base.currentClass.Name);++numOps;
			if (items.Count > 0) {
				editActionHandler.InsertString(":");++numOps;
			}
			for (int i = 0; i < items.Count; ++i) {
				FieldWrapper fieldWrapper = (FieldWrapper)items[i];
				editActionHandler.InsertString(" ");++numOps;
				editActionHandler.InsertString(fieldWrapper.Field.Name);++numOps;
				editActionHandler.InsertString(" = {" + i + "}");++numOps;
				if (i + 1 < items.Count) {
					editActionHandler.InsertString(",");++numOps;
				}
			}
			editActionHandler.InsertString("]\"");++numOps;
			if (items.Count > 0) {
				editActionHandler.InsertString(",");++numOps;
				Return();
			}
			for (int i = 0; i < items.Count; ++i) {
				FieldWrapper fieldWrapper = (FieldWrapper)items[i];
				editActionHandler.InsertString(fieldWrapper.Field.Name);
				if (i + 1 < items.Count) {
					editActionHandler.InsertString(",");++numOps;
					Return();
				}
			}
			if (fileExtension == ".vb") {
				editActionHandler.InsertString(")");
			} else {
				editActionHandler.InsertString(");");
			}
			++numOps;
			Return();
			if (fileExtension == ".vb") {
				editActionHandler.InsertString("End Function");
			} else {
				editActionHandler.InsertString("}");
			}
			++numOps;
			Return();
		}

		class FieldWrapper
		{
			IField field;
			
			public IField Field {
				get {
					return field;
				}
			}
			
			public FieldWrapper(IField field)
			{
				this.field = field;
			}
			
			public override string ToString()
			{
				AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
				IAmbience ambience = ambienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.None;
				return ambience.Convert(field);
			}
		}
	}
}
