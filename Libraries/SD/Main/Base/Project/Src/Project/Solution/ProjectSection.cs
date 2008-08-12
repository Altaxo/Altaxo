﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ProjectSection.
	/// </summary>
	public class ProjectSection
	{
		string name;
		string sectionType;
		
		List<SolutionItem> items = new List<SolutionItem>();
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string SectionType {
			get {
				return sectionType;
			}
		}
		
		public List<SolutionItem> Items {
			get {
				return items;
			}
		}
		
		public ProjectSection(string name, string sectionType)
		{
			this.name = name;
			this.sectionType = sectionType;
		}
		
		public void AppendSection(StringBuilder sb, string indentString)
		{
			foreach (SolutionItem item in items) {
				item.AppendItem(sb, indentString);
			}
		}
		
		static Regex sectionPattern  = new Regex("\\s*(?<Key>.*\\S)\\s*=\\s*(?<Value>.*\\S)\\s*", RegexOptions.Compiled);
		public static ProjectSection ReadGlobalSection(TextReader sr, string name, string sectionType)
		{
			return ReadSection(sr, name, sectionType, "EndGlobalSection");
		}
		
		public static ProjectSection ReadProjectSection(TextReader sr, string name, string sectionType)
		{
			return ReadSection(sr, name, sectionType, "EndProjectSection");
		}
		
		static ProjectSection ReadSection(TextReader sr, string name, string sectionType, string endTag)
		{
			ProjectSection newFolder = new ProjectSection(name, sectionType);
			while (true) {
				string line = sr.ReadLine();
				if (line == null || line.Trim() == endTag) {
					break;
				}
				Match match = sectionPattern.Match(line);
				if (match.Success) {
					newFolder.Items.Add(new SolutionItem(match.Result("${Key}"), match.Result("${Value}")));
				}
			}
			return newFolder;
		}
		
	}
}
