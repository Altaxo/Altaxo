// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public enum DeploymentStrategy {
		Script,
		Assembly,
		File
	}
	
	[XmlNodeName("DeploymentInformation")]
	public class DeployInformation
	{
		[XmlNodeName("Exclude")]
		class ExcludeFile 
		{
			[XmlAttribute("file")]
			[ConvertToRelativePathAttribute()]
			protected string fileName;
			
			public string FileName {
				get {
					return fileName;
				}
				set {
					fileName = value;
				}
			}
			
			public ExcludeFile()
			{
			}
			
			public ExcludeFile(string fileName)
			{
				this.fileName = fileName;
			}
		}
		
		[XmlSetAttribute(typeof(ExcludeFile))]
		ArrayList excludeFiles = new ArrayList();
		
		[XmlAttribute("target")]
		string    deployTarget = "";
		
		[XmlAttribute("script")]
		string    deployScript = "";
		
		[XmlAttribute("strategy")]
		DeploymentStrategy deploymentStrategy = DeploymentStrategy.File;
		
		public DeploymentStrategy DeploymentStrategy {
			get {
				return deploymentStrategy;
			}
			set {
				deploymentStrategy = value;
			}
		}
		
		ArrayList ExcludeFiles {
			get {
				return excludeFiles;
			}
		}
		
		public string DeployTarget {
			get {
				return deployTarget;
			}
			set {
				deployTarget = value;
				if (deployTarget.EndsWith(Path.DirectorySeparatorChar.ToString())) {
					deployTarget = deployTarget.Substring(0, deployTarget.Length - 1);
				}
			}
		}
		
		public string DeployScript {
			get {
				return deployScript;
			}
			set {
				deployScript = value;
			}
		}
		
		public void ClearExcludedFiles()
		{
			excludeFiles.Clear();
		}
		
		public void AddExcludedFile(string fileName)
		{
			excludeFiles.Add(new ExcludeFile(fileName));
		}
		
		public void RemoveExcludedFile(string fileName)
		{
			foreach (ExcludeFile excludedFile in ExcludeFiles) {
				if (excludedFile.FileName == fileName) {
					ExcludeFiles.Remove(excludedFile);
					RemoveExcludedFile(fileName);
					break;
				}
			}
		}
		
		public bool IsFileExcluded(string name)
		{
			foreach (ExcludeFile file in excludeFiles) {
				if (file.FileName == name) {
					return true;
				}
			}
			return false;
		}
		public DeployInformation()
		{
		}
		
		public static void Deploy(IProject project)
		{
			switch (project.DeployInformation.DeploymentStrategy) {
				case DeploymentStrategy.File:
					new FileDeploy().DeployProject(project);
					break;
				case DeploymentStrategy.Script:
					new ScriptDeploy().DeployProject(project);
					break;
				case DeploymentStrategy.Assembly:
					new AssemblyDeploy().DeployProject(project);
					break;
			}
		}
	}
}
