// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.IO;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of SharpDevelop
	/// it checks, if the files exists at every creation, and if not it doesn't list them in the 
	/// recent files, and they'll not be saved during the next option save.
	/// </summary>
	public class RecentOpen : IXmlConvertable
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		int MAX_LENGTH = 10;
		
		ArrayList lastfile    = new ArrayList();
		ArrayList lastproject = new ArrayList();
		
		public event EventHandler RecentFileChanged;
		public event EventHandler RecentProjectChanged;
		
		public ArrayList RecentFile {
			get {
				Debug.Assert(lastfile != null, "RecentOpen : set string[] LastFile (value == null)");
				return lastfile;
			}
		}

		public ArrayList RecentProject {
			get {
				Debug.Assert(lastproject != null, "RecentOpen : set string[] LastProject (value == null)");
				return lastproject;
			}
		}
		
		void OnRecentFileChange()
		{
			if (RecentFileChanged != null) {
				RecentFileChanged(this, null);
			}
		}
		
		void OnRecentProjectChange()
		{
			if (RecentProjectChanged != null) {
				RecentProjectChanged(this, null);
			}
		}

		public RecentOpen()
		{
		}
		
		public RecentOpen(XmlElement element)
		{
			XmlNodeList nodes = element["FILES"].ChildNodes;
			
			for (int i = 0; i < nodes.Count; ++i) {
				if (File.Exists(nodes[i].InnerText)) {
					lastfile.Add(nodes[i].InnerText);
				}
			}
			
			nodes  = element["PROJECTS"].ChildNodes;
			
			for (int i = 0; i < nodes.Count; ++i) {
				if (File.Exists(nodes[i].InnerText)) {
					lastproject.Add(nodes[i].InnerText);
				}
			}
		}
		
		public void AddLastFile(string name) // TODO : improve 
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				if (lastfile[i].ToString() == name) {
					lastfile.RemoveAt(i);
				}
			}
			
			while (lastfile.Count >= MAX_LENGTH) {
				lastfile.RemoveAt(lastfile.Count - 1);
			}
			
			if (lastfile.Count > 0) {
				lastfile.Insert(0, name);
			} else {
				lastfile.Add(name);
			}
			
			OnRecentFileChange();
		}
		
		public void ClearRecentFiles()
		{
			lastfile.Clear();
			
			OnRecentFileChange();
		}
		
		public void ClearRecentProjects()
		{
			lastproject.Clear();
			
			OnRecentProjectChange();
		}
		
		public void AddLastProject(string name) // TODO : improve
		{
			for (int i = 0; i < lastproject.Count; ++i) {
				if (lastproject[i].ToString() == name) {
					lastproject.RemoveAt(i);
				}
			}
			
			while (lastproject.Count >= MAX_LENGTH) {
				lastproject.RemoveAt(lastproject.Count - 1);
			}
			
			if (lastproject.Count > 0) {
				lastproject.Insert(0, name);
			} else {
				lastproject.Add(name);			
			}
			OnRecentProjectChange();
		}
		
		public object FromXmlElement(XmlElement element)
		{
			return new RecentOpen(element);
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement recent = doc.CreateElement("RECENT");
			
			XmlElement lastfiles = doc.CreateElement("FILES");
			foreach (string file in lastfile) {
				XmlElement element = doc.CreateElement("FILE");
				element.InnerText  = file;
				lastfiles.AppendChild(element);
			}
			
			XmlElement lastprojects = doc.CreateElement("PROJECTS");
			foreach (string project in lastproject) {
				XmlElement element = doc.CreateElement("PROJECT");
				element.InnerText = project;
				lastprojects.AppendChild(element);
			}
			
			recent.AppendChild(lastfiles);
			recent.AppendChild(lastprojects);
			
			return recent;
		}
		
		public void FileRemoved(object sender, FileEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.FileName == file) {
					lastfile.RemoveAt(i);
					OnRecentFileChange();
					break;
				}
			}
		}
		
		public void FileRenamed(object sender, FileEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.SourceFile == file) {
					lastfile.RemoveAt(i);
					lastfile.Insert(i, e.TargetFile);
					OnRecentFileChange();
					break;
				}
			}
		}
	}
}
