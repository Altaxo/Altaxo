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
	public class OverrideMethodsCodeGenerator : CodeGenerator
	{
		public override string CategoryName {
			get {
				return "Override methods";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose methods to override";
			}
		}
		
		public override int ImageIndex {
			get {
				ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
				return classBrowserIconService.MethodIndex;
			}
		}
		
		public OverrideMethodsCodeGenerator(IClass currentClass) : base(currentClass)
		{
			foreach (IClass c in currentClass.ClassInheritanceTree) {
				if (c.FullyQualifiedName != currentClass.FullyQualifiedName) {
					foreach (IMethod method in c.Methods) {
						if (!method.IsPrivate && (method.IsAbstract || method.IsVirtual || method.IsOverride)) {
							Content.Add(new MethodWrapper(method));
						}
					}
				}
			}
		}
		
		protected override void StartGeneration(IList items, string fileExtension)
		{
//			bool moveToMethod = sf.SelectedItems.Count == 1;
//			int  caretPos     = 0;
			for (int i = 0; i < items.Count; ++i) {
				MethodWrapper mw = (MethodWrapper)items[i];
				
				string parameters = String.Empty;
				string paramList  = String.Empty;
				string returnType = csa.Convert(mw.Method.ReturnType);
				
				for (int j = 0; j < mw.Method.Parameters.Count; ++j) {
					paramList  += mw.Method.Parameters[j].Name;
					parameters += csa.Convert(mw.Method.Parameters[j]);
					if (j + 1 < mw.Method.Parameters.Count) {
						parameters += ", ";
						paramList  += ", ";
					}
				}
				
				editActionHandler.InsertString(csa.Convert(mw.Method.Modifiers) + "override " + returnType + " " + mw.Method.Name + "(" + parameters + ")");++numOps;
				Return();
				editActionHandler.InsertChar('{');++numOps;
				Return();
				
				if (returnType != "void") {
					string str = "return base." + mw.Method.Name + "(" + paramList + ");";
					editActionHandler.InsertString(str);++numOps;
				}
				
				Return();
//				caretPos = editActionHandler.Document.Caret.Offset;

				editActionHandler.InsertChar('}');++numOps;
				Return();
				IndentLine();
			}
//			if (moveToMethod) {
//				editActionHandler.Document.Caret.Offset = caretPos;
//			}
		}
		
		class MethodWrapper
		{
			IMethod method;
			
			public IMethod Method {
				get {
					return method;
				}
			}
			
			public MethodWrapper(IMethod method)
			{
				this.method = method;
			}
			
			public override string ToString()
			{
				AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
				IAmbience ambience = ambienceService.CurrentAmbience;
				ambience.ConversionFlags = ConversionFlags.None;
				return ambience.Convert(method);
			}
		}
	}
}
