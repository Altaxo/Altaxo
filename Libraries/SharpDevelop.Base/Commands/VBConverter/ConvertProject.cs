// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Templates;

using ICSharpCode.SharpRefactory.PrettyPrinter;
using ICSharpCode.SharpRefactory.Parser;

namespace ICSharpCode.SharpDevelop.Commands
{
	public abstract class AbstractProjectConverter : AbstractMenuCommand
	{
		protected abstract string Extension {
			get;
		}
		
		protected abstract string ConvertFile(string fileName);
		
		protected virtual IProject CreateProject(string outputPath, IProject originalProject) {
			return CreateProject(outputPath, originalProject, originalProject.ProjectType);
		}
		
		protected IProject CreateProject(string outputPath, IProject originalProject, string targetLanguage) {
			LanguageBindingService languageBindingService = (LanguageBindingService)ServiceManager.Services.GetService(typeof(LanguageBindingService));
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(targetLanguage);
			
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.CombinePath = outputPath;
			info.ProjectBasePath = outputPath;
			info.ProjectName = originalProject.Name + " converted";
			
			return binding.CreateProject(info, null);
		}
		
		bool CopyFile(string original, string newFile)
		{
			try {
				File.Copy(original, newFile);
			} catch(IOException) {
				return false;
			}
			return true;
		}
		
		bool SaveFile(string fileName, string content)
		{
			try {
				if (!Directory.Exists(Path.GetDirectoryName(fileName))) {
					Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				}
				
				StreamWriter sw = new StreamWriter(fileName);
				sw.Write(content);
				sw.Close();
			} catch (Exception e) {
				Console.WriteLine("Error while saving file : " + e);
				return false;
			}
			return true;
		}
		
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			int len = projectService.CurrentSelectedProject.BaseDirectory.Length;
			
			string outputPath = Path.GetFullPath(Path.Combine(projectService.CurrentSelectedProject.BaseDirectory, "convertedProject"));
			
			if (!Directory.Exists(outputPath)) {
				Directory.CreateDirectory(outputPath);
			}
			
			IProject project = CreateProject(outputPath, projectService.CurrentSelectedProject);
			
			if (projectService.CurrentSelectedProject != null) {
				foreach (ProjectFile file in projectService.CurrentSelectedProject.ProjectFiles) {
					
					if (file.BuildAction == BuildAction.EmbedAsResource) {
						string outFile;
						
						// resource files can be outside of the project path
						if(file.Name.StartsWith(outputPath)) {
							// Path.GetFilename can't be used because the filename can be
							// a relative path that shouldn't get lost
							outFile = Path.Combine(outputPath, file.Name.Substring(len + 1));
						} else {
							outFile = Path.Combine(outputPath, Path.GetFileName(file.Name));
						}
						
						if(CopyFile(file.Name, outFile)) {
							ProjectFile pf = new ProjectFile(outFile);
							pf.BuildAction = BuildAction.EmbedAsResource;
							project.ProjectFiles.Add(pf);
						}
					} else if(file.Subtype != Subtype.Directory && File.Exists(file.Name)) {
						string outPut;
						try {
							outPut = ConvertFile(file.Name);
						} catch (Exception e) {
							outPut = "Conversion Error : " + e.ToString();
						}
						
						// Path.GetFilename can't be used because the filename can be
						// a relative path that shouldn't get lost
						string outFile = Path.Combine(outputPath, file.Name.Substring(len + 1));
						outFile = Path.ChangeExtension(outFile, Extension);
						
						if (SaveFile(outFile, outPut)) {
							project.ProjectFiles.Add(new ProjectFile(outFile));
						}
					}
				}
			}
			
			try {
				project.SaveProject(Path.Combine(outputPath, project.Name + ".prjx"));
			} catch (Exception e) {
				Console.WriteLine("Error while saving project : " + e);
				return;
			}
			IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			IResourceService resourceService = (IResourceService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IResourceService));
			string message = resourceService.GetString("ICSharpCode.SharpDevelop.Commands.ConvertCommand.FinishedMessage");
			messageService.ShowMessage(message + "\n" + outputPath);
		}
	}
	 #if !ModifiedForAltaxo
	public class VBConvertProjectToCSharp : AbstractProjectConverter
	{
		protected override string Extension {
			get {
				return ".cs";
			}
		}
		
		protected override IProject CreateProject(string outputPath, IProject originalProject) {
			return CreateProject(outputPath, originalProject, "C#");
		}
		
		protected override string ConvertFile(string fileName)
		{
			ICSharpCode.SharpRefactory.Parser.VB.Parser p = new ICSharpCode.SharpRefactory.Parser.VB.Parser();
			
			p.Parse(new ICSharpCode.SharpRefactory.Parser.VB.Lexer(new ICSharpCode.SharpRefactory.Parser.VB.FileReader(fileName)));
			
			ICSharpCode.SharpRefactory.PrettyPrinter.VB.CSharpVisitor vbv = new ICSharpCode.SharpRefactory.PrettyPrinter.VB.CSharpVisitor();
			vbv.Visit(p.compilationUnit, null);
			
			return vbv.SourceText.ToString();
		}
	}
 #endif
	public class CharpConvertProjectToVB : AbstractProjectConverter
	{
		protected override string Extension {
			get {
				return ".vb";
			}
		}
		
		protected override IProject CreateProject(string outputPath, IProject originalProject) {
			return CreateProject(outputPath, originalProject, "VBNET");
		}
		
		protected override string ConvertFile(string fileName)
		{
			Parser p = new Parser();
			
			p.Parse(new Lexer(new ICSharpCode.SharpRefactory.Parser.FileReader(fileName)));
			
			VBNetVisitor vbv = new VBNetVisitor();
			vbv.Visit(p.compilationUnit, null);
			
			return vbv.SourceText.ToString();
		}
		
	}
}
