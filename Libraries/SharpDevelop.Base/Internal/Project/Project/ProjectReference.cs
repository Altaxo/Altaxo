// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using ICSharpCode.SharpDevelop.Services;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public enum ReferenceType {
		Assembly,
		Project,
		Gac,
		Typelib
	}
	
	/// <summary>
	/// This class represent a reference information in an IProject object.
	/// </summary>
	[XmlNodeName("Reference")]
	public class ProjectReference : LocalizedObject, ICloneable
	{
		[XmlAttribute("type")]
		ReferenceType referenceType;
		
		[XmlAttribute("refto")]
		[ConvertToRelativePath("IsAssembly")]
		string        reference = String.Empty;
		
		[XmlAttribute("localcopy")]
		bool          localCopy = true;
		
		bool IsAssembly {
			get {
				return referenceType == ReferenceType.Assembly;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.ReferenceType}",
		                   Description ="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.ReferenceType.Description})")]
		public ReferenceType ReferenceType {
			get {
				return referenceType;
			}
			set {
				referenceType = value;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Reference}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Reference.Description}")]
		public string Reference {
			get {
				return reference;
			}
			set {
				reference = value;
				OnReferenceChanged(EventArgs.Empty);
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy.Description}")]
		[DefaultValue(true)]
		public bool LocalCopy {
			get {
				return localCopy;
			}
			set {
				localCopy = value;
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				projectService.SaveCombine();
			}
		}
		
		/// <summary>
		/// Returns the file name to an assembly, regardless of what 
		/// type the assembly is.
		/// </summary>
		public string GetReferencedFileName(IProject project)
		{
			switch (ReferenceType) {
				case ReferenceType.Typelib:
					return new TypelibImporter().Import(this, project);
				case ReferenceType.Assembly:
					return reference;
				
				case ReferenceType.Gac: 
					return GetPathToGACAssembly(this);
				
				case ReferenceType.Project:
					IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					string projectOutputLocation   = projectService.GetOutputAssemblyName(reference);
					return projectOutputLocation;
				
				default:
					throw new NotImplementedException("unknown reference type : " + ReferenceType);
			}
		}
		
		public ProjectReference()
		{
		}
		
		public ProjectReference(ReferenceType referenceType, string reference)
		{
			this.referenceType = referenceType;
			this.reference     = reference;
		}
		
		
		/// <summary>
		/// This method returns the absolute path to an GAC assembly.
		/// </summary>
		/// <param name ="refInfo">
		/// The reference information containing a GAC reference information.
		/// </param>
		/// <returns>
		/// the absolute path to the GAC assembly which refInfo points to.
		/// </returns>
		static string GetPathToGACAssembly(ProjectReference refInfo)
		{ // HACK : Only works on windows.
			System.Diagnostics.Debug.Assert(refInfo.ReferenceType == ReferenceType.Gac);
			string[] info = refInfo.Reference.Split(',');
			
			if (info.Length < 4) {
				Assembly refAssembly = Assembly.LoadWithPartialName(info[0]);				
				// if it failed, then return just the short name
				if (refAssembly == null) {
					return info[0];
				}
				// store the full name from the assembly
				refInfo.Reference = refAssembly.FullName;
				
				// split up the peices again to find the assembly file path
				info = refInfo.Reference.Split(',');
			}
			
			string aName      = info[0];
			string aVersion   = info[1].Substring(info[1].LastIndexOf('=') + 1);
			string aPublicKey = info[3].Substring(info[3].LastIndexOf('=') + 1);
			
			return System.Environment.GetFolderPath(Environment.SpecialFolder.System) + 
			       Path.DirectorySeparatorChar + ".." +
			       Path.DirectorySeparatorChar + "assembly" +
			       Path.DirectorySeparatorChar + "GAC" +
			       Path.DirectorySeparatorChar + aName +
			       Path.DirectorySeparatorChar + aVersion + "__" + aPublicKey +
			       Path.DirectorySeparatorChar + aName + ".dll";
		}
		
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is ProjectReference)) return false;
			if (this == obj) return true;
			ProjectReference myProjectReference = (ProjectReference)obj;
			if (referenceType != myProjectReference.referenceType) return false;
			if (reference == null) {
				return myProjectReference.reference == null;
			}
			return reference.ToLower().Equals(myProjectReference.reference.ToLower());
		}
		
		public override int GetHashCode()
		{
			return referenceType.GetHashCode() ^ (reference != null ? reference.GetHashCode() : 0);
		}
		
		protected virtual void OnReferenceChanged(EventArgs e) 
		{
			if (ReferenceChanged != null) {
				ReferenceChanged(this, e);
			}
		}
		
		public event EventHandler ReferenceChanged;
	}
}
