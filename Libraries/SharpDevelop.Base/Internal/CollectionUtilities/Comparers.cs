//using ;
using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;

namespace System {
	[Serializable]
	///<summary>
	///Compares two objects by property.
	///Objects need not be of the same type,
	///they need only possess the same set of properties the used by the comparer
	///and those properties need to implement IComparable
	///</summary>
	public class PropertyComparer : IComparer {
		protected string[] Properties = null;
		
		///<param name="properties">Array of property names to compare</param>
		public PropertyComparer(string [] properties) {
			if(properties != null) {
				Properties = properties;
			}
			else {
				Properties = new string[0];
			}
		}
		
		///<summary>
		///Compare x against y by comparing the list of properties passed in constructor
		///</summary>
		int IComparer.Compare(object x, object y) {
			int cmp;
			Type typex = x.GetType();
			Type typey = y.GetType();
			
			for(int i=0; i < Properties.Length; i++) {
				string p = Properties[i];
				IComparable pvalx = (IComparable)(typex.GetProperty(p).GetValue(x, null));
				object pvaly = typey.GetProperty(p).GetValue(y, null);
				cmp = pvalx.CompareTo(pvaly);
				if (cmp != 0) {
					return cmp;
				}
			}
			return 0;
		}
	}
	
	///<summary>
	///Implements a comparer that inverts the order of comparisons.
	///Intended for inverting sort order on sorters that use IComparer
	///</summary>
	public class ReverseComparer : IComparer {
		
		public static readonly ReverseComparer Default = new ReverseComparer(Comparer.Default);
		
		private IComparer myComparer = null;
		
		protected ReverseComparer():this(Comparer.Default) {}
		
		public ReverseComparer(IComparer comparer) { myComparer = comparer; }
		
		int IComparer.Compare(object a, object b) {
			return myComparer.Compare(b, a);
		}
	}
}

namespace System.Collections.Specialized
{
	///<summary>
	///Compares TreeNodes by ImageIndex, SelectedImageIndex then Text properties
	///Intended to be used in sorters that group TreeNodes by their icon and sort them by text
	///</summary>
	[Serializable]
	public class ProjectNodeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			if (x.GetType() == y.GetType()) {
				if (x is NamedFolderNode) {
					return ((NamedFolderNode)x).SortPriority - ((NamedFolderNode)y).SortPriority;
				}
				return ((System.Windows.Forms.TreeNode)x).Text.CompareTo(((System.Windows.Forms.TreeNode)y).Text);
			}
			if (x is FileNode) {
				return 1;
			} else if (y is FileNode) {
				return -1;
			}
			if (x is DirectoryNode) {
				return 1;
			} else if (y is DirectoryNode) {
				return -1;
			}
			return TreeNodeComparer.Default.Compare(x, y);
		}
	}
	
	///<summary>
	///Compares TreeNodes by ImageIndex, SelectedImageIndex then Text properties
	///Intended to be used in sorters that group TreeNodes by their icon and sort them by text
	///</summary>
	[Serializable]
	public class TreeNodeComparer : IComparer {
		
		public static IComparer Default    = new TreeNodeComparer();
		public static IComparer ProjectNode = new ProjectNodeComparer();
		
		protected TreeNodeComparer() {}
		
		public int Compare(System.Windows.Forms.TreeNode x, System.Windows.Forms.TreeNode y) {
			int cmp = x.ImageIndex - y.ImageIndex;
			if(cmp == 0) {
//				cmp = x.SelectedImageIndex - y.SelectedImageIndex;
//				if(cmp == 0) {
					cmp = x.Text.CompareTo(y.Text);
//				}
			}
			
			return cmp;
		}
		
		int IComparer.Compare(object x, object y) {
			return Compare((System.Windows.Forms.TreeNode)x, (System.Windows.Forms.TreeNode)y);
		}
	}
}
