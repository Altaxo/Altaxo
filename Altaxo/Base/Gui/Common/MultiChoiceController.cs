using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;
namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Document class for the <see cref="MultiChoiceController"/>.
	/// </summary>
	public class MultiChoiceList
	{
		/// <summary>The description text shown above the list of items.</summary>
		public string Description { get; set; }

		/// <summary>List of items. Has to be filled before showing the control.</summary>
		public SelectableListNodeList List { get; protected set; }

		/// <summary>List of column names. The number of names must match the number of subitems in the list items.</summary>
		public List<string> ColumnNameList { get { return _columnNames; } }
		/// <summary>Set the list of column names (can be used in the initializer of this object).</summary>
    public string[] ColumnNames 
		{
			set
			{
				_columnNames.Clear();
				_columnNames.AddRange(value); 
			}
		}

		List<string> _columnNames;

		/// <summary>
		/// Constructs an empty list with no items and no column names.
		/// </summary>
		public MultiChoiceList() 
		{
			List = new SelectableListNodeList();
			_columnNames = new List<string>();
		}
	}

	/// <summary>Gui view interface for use with the <see cref="MultiChoiceController"/>.</summary>
	public interface IMultiChoiceView
	{
		/// <summary>Initialize the description text.</summary>
		/// <param name="value">Description text.</param>
		void InitializeDescription(string value);

		/// <summary>
		/// Initialize the column names.
		/// </summary>
		/// <param name="colNames">Column names.</param>
		void InitializeColumnNames(string[] colNames);
		
		/// <summary>
		/// Initializes the list. 
		/// </summary>
		/// <param name="list">List of items shown.</param>
		void InitializeList(SelectableListNodeList list);
	}

	/// <summary>
	/// Controller for the <see cref="MultiChoiceList"/> document type.
	/// </summary>
	[ExpectedTypeOfView(typeof(IMultiChoiceView))]
	[UserControllerForObject(typeof(MultiChoiceList))]
	public class MultiChoiceController : IMVCANController
	{
		MultiChoiceList _doc;
		IMultiChoiceView _view;

		protected void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.InitializeDescription(_doc.Description);

				if (_doc.ColumnNameList.Count == 0)
					_view.InitializeColumnNames(new string[] { "Name" });
				else
					_view.InitializeColumnNames(_doc.ColumnNameList.ToArray());

				_view.InitializeList(_doc.List);
			}
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length > 0 && args[0] is MultiChoiceList)
			{
        _doc = (MultiChoiceList)args[0];
				Initialize(true);
				return true;
			}
			else
				return false;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IMultiChoiceView;
				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			return true;
		}

		#endregion
	}
}
