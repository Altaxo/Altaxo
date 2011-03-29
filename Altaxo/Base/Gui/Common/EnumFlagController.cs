using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  public interface IEnumFlagView
  {
		/// <summary>
		/// Initializes the names. The view can set i.e. checks for each item which is selected. The view is responsible for updating
		/// the <see cref="SelectableListNode.IsSelected"/> property when a check is set or unset.
		/// </summary>
		/// <param name="names"></param>
    void Initialize(SelectableListNodeList names);
  }

  [UserControllerForObject(typeof(System.Enum))]
  [ExpectedTypeOfView(typeof(IEnumFlagView))]
  class EnumFlagController : IMVCANController
  {
    System.Enum _doc;
    long _tempDoc;
    IEnumFlagView _view;

    Array _values;
		SelectableListNodeList _list;
    string[] _names;
    bool[] _checks;

		int _checkedChangeLock=0;

    void Initialize(bool initData)
    {
      if (initData)
      {
				_list = new SelectableListNodeList();
				var values = System.Enum.GetValues(_doc.GetType());
				foreach (var val in values)
				{
					var node = new SelectableListNode(System.Enum.GetName(_doc.GetType(), val), val, IsChecked(val, _tempDoc));
					node.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(EhNode_PropertyChanged);
					_list.Add(node);
				}
      }

      if (_view != null)
      {
        _view.Initialize(_list);
      }
    }

		void EhNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (0 != _checkedChangeLock || "IsSelected" != e.PropertyName)
				return;

			var node = (SelectableListNode)sender;
			
			bool b = node.IsSelected;
			long x = Convert.ToInt64(node.Tag); // get the selected flag

			if (b && (x == 0)) // if the "None" flag is selected, then no flag should be selected, so _tempDoc must be 0
			{
				_tempDoc = 0;
			}
			else // a "normal" flag is selected
			{
				if (b)
					_tempDoc |= x;
				else
					_tempDoc &= ~x;
			}


			++_checkedChangeLock; // avoid recursive calls when changing the checks in the view
			CalculateChecksFromDoc();
			--_checkedChangeLock;
		}

		static bool IsChecked(object flag, long document)
		{
			long x = Convert.ToInt64(flag);
			if (x == 0)
				return 0==document;
			else
				return (x == (x & document));
		}

    void CalculateChecksFromDoc()
    {
			foreach (var n in _list)
			{
				n.IsSelected = IsChecked(n.Tag, _tempDoc);
			}
    }

    void CalculateEnumFromChecks()
    {
			// calculate enum from checks
			long sum = 0;
			for (int i = 0; i < _list.Count; i++)
			{
				long x = Convert.ToInt64(_list[i].Tag);
				if (_list[i].IsSelected)
					sum |= x;
			}
			_tempDoc = sum;
    }


   

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is System.Enum))
        return false;

      _doc = (System.Enum)args[0];
      _tempDoc = ((IConvertible)_doc).ToInt64(System.Globalization.CultureInfo.InvariantCulture);

      Initialize(true);

      return true;
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
        _view = value as IEnumFlagView;

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
      _doc = (System.Enum)System.Enum.ToObject(_doc.GetType(), _tempDoc);
      return true;
    }

    #endregion
  }
}
