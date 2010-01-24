using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;
namespace Altaxo.Gui.Common
{
	public class MultiChoiceList
	{
		public string Description { get; set; }
		public SelectableListNodeList List { get; protected set; }

		List<string> _columnNames;
		public List<string> ColumnNameList { get { return _columnNames; } }
    public string[] ColumnNames { set { _columnNames.Clear(); _columnNames.AddRange(value); } }

		public MultiChoiceList() 
		{
			List = new SelectableListNodeList();
			_columnNames = new List<string>();
		}
	}

	public interface IMultiChoiceView
	{
		void InitializeDescription(string value);
		void InitializeListColumns(string[] colNames);
		void InitializeList(SelectableListNodeList list);
	}

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
					_view.InitializeListColumns(new string[] { "Name" });
				else
					_view.InitializeListColumns(_doc.ColumnNameList.ToArray());

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
