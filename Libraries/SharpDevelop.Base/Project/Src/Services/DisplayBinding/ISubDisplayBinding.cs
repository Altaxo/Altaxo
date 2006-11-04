﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class defines the SharpDevelop display binding interface, it is a factory
	/// structure, which creates IViewContents.
	/// </summary>
	public interface ISecondaryDisplayBinding
	{
		bool CanAttachTo(IViewContent content);
		
		/// <summary>
		/// When you return true for this property, the CreateSecondaryViewContent method
		/// is called again after the LoadSolutionProjects thread has finished.
		/// </summary>
		bool ReattachWhenParserServiceIsReady { get; }
		
		/// <summary>
		/// Creates the secondary view contents for the given view content.
		/// If ReattachWhenParserServiceIsReady is used, the implementation is responsible
		/// for checking that no duplicate secondary view contents are added.
		/// </summary>
		ISecondaryViewContent [] CreateSecondaryViewContent(IViewContent viewContent);
	}
}
