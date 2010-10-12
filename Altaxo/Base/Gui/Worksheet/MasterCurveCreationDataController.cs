using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Gui.Worksheet
{
	#region Interfaces

	public interface IMasterCurveCreationDataView
	{
		void InitializeListData(List<SelectableListNodeList> list);
	}


	#endregion

	/// <summary>
	/// Responsible for the ordering of multiple curves that subsequently can be used to form a master curve.
	/// </summary>
	[UserControllerForObject(typeof(List<List<DoubleColumn>>))]
	[ExpectedTypeOfView(typeof(IMasterCurveCreationDataView))]
	public class MasterCurveCreationDataController : IMVCANController
	{
		IMasterCurveCreationDataView _view;
		List<List<DoubleColumn>> _doc;
		List<List<DoubleColumn>> _docOriginal;

		List<SelectableListNodeList> _viewList;

		void Initialize(bool initData)
		{
			if(initData)
			{
				_viewList = new List<SelectableListNodeList>();

				foreach(var srcGroup in _doc)
				{
					var destGroup = new SelectableListNodeList();
					_viewList.Add(destGroup);
					foreach(var srcEle in srcGroup)
					{
						var destEle = new SelectableListNode(Main.DocumentPath.GetAbsolutePath(srcEle).ToString(),srcEle,false);
						destGroup.Add(destEle);
					}
				}
			}
			if(null!=_view)
			{
				_view.InitializeListData(_viewList);
			}
		}


		void CopyDoc(List<List<DoubleColumn>> src, List<List<DoubleColumn>> dest)
		{
			dest.Clear();
			foreach(var e1 in src)
			{
				var destElement = new List<DoubleColumn>();
				dest.Add(destElement);
				foreach(var e2 in e1)
				{
					destElement.Add(e2);
				}
			}
		}


		public bool InitializeDocument(params object[] args)
		{
			if(args==null || args.Length==0 || !(args[0] is List<List<DoubleColumn>>))
				return false;

			_docOriginal = args[0] as List<List<DoubleColumn>>;
			_doc = new List<List<DoubleColumn>>();
			CopyDoc(_docOriginal,_doc);

			Initialize(true);

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IMasterCurveCreationDataView;

				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _docOriginal; }
		}

		public bool Apply()
		{
			return true;
		}
	}
}
