using System;
using System.Windows.Forms;

// 
//	Mikael Wiberg 2003
//		mikwib@hotmail.com (usual HoTMaiL spam filters)
//		mick@ar.com.au (heavy spam filters on, harldy anything gets through, START the subject with C# and it will probably go through)
//		md5mw@mdstud.chalmers.se (heavy spam filters on, harldy anything gets through, START the subject with C# and it will probably go through)
//	
//	Feel free to use this code as you wish, as long as you do not take credit for it yourself.
//	If it is used in commercial projects or applications please mention my name.
//	Feel free to donate any amount of money if this code makes you happy ;)
//	Use this code at your own risk. If your machine blows up while using it - don't blame me.
// 
namespace MWCommon
{
	/// <summary>
	/// This class encapsulates a TreeNode and keeps track and deals with its ImageIndex and SelectedImageIndex properties.
	/// This class was specifically created for the purpose of simulating a TreeNode being selected and deselected. The rendering of this
	///		happens somewhere inside the MS TreeView's OnPaint EventHandler - which incidentally cannot be overridden. Therefore in order
	///		to simulate the Image part of a TreeNode being selected the Indexes have to be saved away in a class and then restored when
	///		the TreeNode is deselected.
	///	Note that as soon as an MWTreeNodeWrapper object is instantiated the TreeNode it is based on is selected. This is because the
	///		MWTreeView keeps a Hashtable of selected TreeNodes, so any MWTreeNodeWrapper object that is instantiated is assumed to be
	///		selected.
	/// </summary>
	public class MWTreeNodeWrapper
	{
		#region Variables

		/// <summary>
		/// Core TreeNode of this MWTreeNodeWrapper object.
		/// </summary>
		private TreeNode tnNode = null;

		/// <summary>
		/// Original ImageIndex of the TreeNode in the Node property.
		/// </summary>
		private int iImageIndex = -1;

		/// <summary>
		/// Original SelectedImageIndex of the TreeNode in the Node property.
		/// </summary>
		private int iSelectedImageIndex = -1;

		#endregion Variables



		#region Constructor

		/// <summary>
		/// Constructor that takes a TreeNode and wraps code around it.
		/// </summary>
		/// <param name="tn">TreeNode to wrap code around.</param>
		public MWTreeNodeWrapper(TreeNode tn)
		{
			tnNode = tn;
			iImageIndex = tnNode.ImageIndex;
			iSelectedImageIndex = tnNode.SelectedImageIndex;

			if(iImageIndex != -1)
			{
				tnNode.ImageIndex = iSelectedImageIndex;
			}
			else
			{
				tnNode.ImageIndex = tnNode.TreeView.SelectedImageIndex;
			}
		}

		#endregion Constructor



		#region Properties

		/// <summary>
		/// Core TreeNode of this MWTreeNodeWrapper object.
		/// </summary>
		public TreeNode Node
		{
			get
			{
				return tnNode;
			}
			set
			{
				tnNode = value;
			}
		}

		/// <summary>
		/// Original ImageIndex of the TreeNode in the Node property.
		/// </summary>
		public int ImageIndex
		{
			get
			{
				return iImageIndex;
			}
			set
			{
				iImageIndex = value;
			}
		}

		/// <summary>
		/// Original SelectedImageIndex of the TreeNode in the Node property.
		/// </summary>
		public int SelectedImageIndex
		{
			get
			{
				return iSelectedImageIndex;
			}
			set
			{
				iSelectedImageIndex = value;
			}
		}

		#endregion Properties



		#region Methods

		/// <summary>
		/// Select a TreeNode.
		/// </summary>
		public void Select()
		{
			tnNode.ImageIndex = iSelectedImageIndex;
		}

		/// <summary>
		/// Deselect a TreeNode.
		/// </summary>
		public void Deselect()
		{
			tnNode.ImageIndex = iImageIndex;
			tnNode.SelectedImageIndex = iSelectedImageIndex;
		}

		#endregion Methods



		#region Static Methods

		/// <summary>
		/// Select the TreeNode in the MWTreeNodeWrapper object supplied.
		/// </summary>
		/// <param name="mwtnw">MWTreeNodeWrapper containing TreeNode that should be selected.</param>
		public static void Select(MWTreeNodeWrapper mwtnw)
		{
			mwtnw.Node.ImageIndex = mwtnw.SelectedImageIndex;
		}

		/// <summary>
		/// Reselect the TreeNode in the MWTreeNodeWrapper object supplied.
		/// </summary>
		/// <param name="mwtnw">MWTreeNodeWrapper containing TreeNode that should be selected.</param>
		public static void Reselect(MWTreeNodeWrapper mwtnw)
		{
			mwtnw.Node.ImageIndex = mwtnw.ImageIndex;
		}

		/// <summary>
		/// Deselect the TreeNode in the MWTreeNodeWrapper object supplied.
		/// </summary>
		/// <param name="mwtnw">MWTreeNodeWrapper containing TreeNode that should be deselected.</param>
		public static void Deselect(MWTreeNodeWrapper mwtnw)
		{
			mwtnw.Node.ImageIndex = mwtnw.ImageIndex;
			//mwtnw.Node.SelectedImageIndex = mwtnw.SelectedImageIndex;
		}

		#endregion Static Methods



		#region Methods

		/// <summary>
		/// //XXXX
		/// Deselect the TreeNode in the MWTreeNodeWrapper object supplied.
		/// </summary>
		public void Reset()
		{
			this.Node.ImageIndex = this.ImageIndex;
			this.Node.SelectedImageIndex = this.SelectedImageIndex;
		}

		#endregion Methods

	}

}
