// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision: 1963 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Interface for classes that store Navigational information for 
	/// the <see cref="NavigationService"/>.
	/// </summary>
	public interface INavigationPoint : IComparable
	{
		/// <summary>
		/// The path to the file containing the <see cref="INavigationPoint"/>
		/// </summary>
		string FileName {
			get;
		}
		
		/// <summary>
		/// Gets the text that will appear in the drop-down menu to select
		/// this <see cref="INavigationPoint"/>.
		/// </summary>
		string Description {
			get;
		}
		
		/// <summary>
		/// Gets more detailed text that cam be used to describe
		/// this <see cref="INavigationPoint"/>.
		/// </summary>
		string FullDescription {
			get;
		}		
		
		string ToolTip {
			get;
		}

//		/// <summary>
//		/// 
//		/// </summary>
//		string TabName {
//			get;
//		}
		
		/// <summary>
		/// Gets the specific data, if any, needed to 
		/// navigate to this <see cref="INavigationPoint"/>.
		/// </summary>
		object NavigationData {
			get;
		}
		
		int Index {
			get;
		}
		
		/// <summary>
		/// Navigates to this <see cref="INavigationPoint"/>.
		/// </summary>
		void JumpTo();
		
		/// <summary>
		/// Updates the <see cref="FileName"/>.
		/// </summary>
		/// <param name="newName"></param>
		void FileNameChanged(string newName);
		
		/// <summary>
		/// Responsible for updating the internal data of the 
		/// <see cref="INavigationPoint"/> to synch it with 
		/// changes in the IViewContent containing the point.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ContentChanging(object sender, EventArgs e);
	}
}
