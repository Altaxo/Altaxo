// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Resources;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class ScriptRunner
	{
		FileTemplate item;
		FileDescriptionTemplate file;
		
		public string CompileScript(FileTemplate item, FileDescriptionTemplate file)
		{
			Regex r = new Regex("<%.*%>");
			Match m = r.Match(file.Content);m = m.NextMatch();
			if (m.Success) {
				this.item = item;
				this.file = file;
				return CompileAndGetOutput(GenerateCode());
			} else {
				return file.Content;
			}
		}
		
		string CompileAndGetOutput(string fileContent)
		{
			TempFileCollection  tf = new TempFileCollection ();
			
			string path = Path.Combine(tf.BasePath, tf.TempDir);
			Directory.CreateDirectory(path);
			string generatedScript = Path.Combine(path, "InternalGeneratedScript.cs");
			string generatedDLL    = Path.Combine(path, "A.DLL");
			tf.AddFile(generatedScript, false);
			tf.AddFile(generatedDLL, false);
			
			StreamWriter sw = new StreamWriter(generatedScript);
			sw.Write(fileContent);
			sw.Close();
			
			string output = "", error = "";
			
			Executor.ExecWaitWithCapture(GetCompilerName() + " /target:library \"/out:" + generatedDLL + "\" \"" + generatedScript +"\"", tf, ref output, ref error);
			
			if (!File.Exists(generatedDLL)) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				StreamReader sr = File.OpenText(output);
				string errorMessage = sr.ReadToEnd();
				sr.Close();
				messageService.ShowMessage(errorMessage);
				return ">>>>ERROR IN CODE GENERATION GENERATED SCRIPT WAS:\n" + fileContent + "\n>>>>END";
			}
			
			Assembly asm = Assembly.LoadFile(generatedDLL);
			object templateInstance = asm.CreateInstance("Template");
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			foreach (TemplateProperty property in item.Properties) {
				FieldInfo fieldInfo = templateInstance.GetType().GetField(property.Name);
				fieldInfo.SetValue(templateInstance, Convert.ChangeType(stringParserService.Properties["Properties." + property.Name], property.Type.StartsWith("Types:") ? typeof(string): Type.GetType(property.Type)));
			}
			MethodInfo methodInfo = templateInstance.GetType().GetMethod("GenerateOutput");
			string ret = methodInfo.Invoke(templateInstance, null).ToString();
			tf.Delete();
			return ret;
		}
		
		string GetCompilerName()
		{
			string runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			return '"' + Path.Combine(runtimeDirectory, "csc.exe") + '"';
		}
		
		string GenerateCode()
		{
			StringBuilder outPut = new StringBuilder();
			int lastIndex = 0;
			outPut.Append("public class Template {\n");
			foreach (TemplateProperty property in item.Properties) {
				outPut.Append("public ");
				// internal generated enum types are nothing other than strings
				if (property.Type.StartsWith("Types:")) {
					outPut.Append("string");
				} else {
					outPut.Append(property.Type);
				}
				outPut.Append(' ');
				outPut.Append(property.Name);
				outPut.Append(";\n");
			}
			outPut.Append("public string GenerateOutput() {\n");
			outPut.Append("System.Text.StringBuilder outPut = new System.Text.StringBuilder();\n");
			
			Regex r = new Regex("<%.*%>");
			for (Match m = r.Match(file.Content); m.Success; m = m.NextMatch()) {
				Group g = m.Groups[0];
				outPut.Append("outPut.Append(@\"");
				outPut.Append(file.Content.Substring(lastIndex, g.Index - lastIndex));
				outPut.Append("\");\n");
				outPut.Append(g.Value.Substring(2, g.Length - 4));
				lastIndex = g.Index + g.Length;
			}
			outPut.Append("outPut.Append(@\"");
			string formattedContent = Regex.Replace(file.Content.Substring(lastIndex, file.Content.Length - lastIndex), "\"", "\"\"");
			outPut.Append(formattedContent);
			outPut.Append("\");\n");
			outPut.Append("return outPut.ToString();\n");
			outPut.Append("}}\n");
			return outPut.ToString();
		}
	}
}

