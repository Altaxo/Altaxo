﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return fileName.StartsWith("http:")
				|| fileName.StartsWith("https:")
				|| fileName.StartsWith("ftp:")
				|| fileName.StartsWith("browser://");
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return false;
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			BrowserPane browserPane = new BrowserPane();
			if (fileName.StartsWith("browser://")) {
				browserPane.Load(fileName.Substring("browser://".Length));
			} else {
				browserPane.Load(fileName);
			}
			return browserPane;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return null;
		}
	}
}
