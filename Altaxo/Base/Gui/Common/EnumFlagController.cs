using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common
{
  public interface IEnumFlagView
  {
    void SetNames(string[] names);
    void SetChecks(bool[] checks);
    event Action<int, bool> CheckChanged;
  }

  [UserControllerForObject(typeof(System.Enum))]
  [ExpectedTypeOfView(typeof(IEnumFlagView))]
  class EnumFlagController : IMVCANController
  {
    System.Enum _doc;
    long _tempDoc;
    IEnumFlagView _view;

    Array _values;
    string[] _names;
    bool[] _checks;


    void Initialize(bool initData)
    {
      if (initData)
      {
        _values = System.Enum.GetValues(_doc.GetType());
        _names = System.Enum.GetNames(_doc.GetType());
        _checks = new bool[_names.Length];
        CalculateChecksFromDoc();
      }

      if (_view != null)
      {
        _view.SetNames(_names);
        _view.SetChecks(_checks);
      }
    }

    void CalculateChecksFromDoc()
    {
      for (int i = 0; i < _checks.Length; i++)
      {
        long x =Convert.ToInt64(_values.GetValue(i));
       

        if (x == 0)
          _checks[i] = _tempDoc == 0;
        else
          _checks[i] = (x == (x & _tempDoc));
      }
    }

    void CalculateEnumFromChecks()
    {
      long sum = 0;
      for (int i = 0; i < _checks.Length; i++)
      {
        long x = Convert.ToInt64(_values.GetValue(i));
        if (_checks[i])
          sum |= x;
      }
      _tempDoc = sum;
    }


    void EhCheckChanged(int i, bool b)
    {
      long x = Convert.ToInt64(_values.GetValue(i));
      if (b && (x == 0))
      {
        _tempDoc = 0;
      }
      else
      {
				if (b)
					_tempDoc |= x;
				else
					_tempDoc &= ~x;
      }
      CalculateChecksFromDoc();
      _view.SetChecks(_checks);
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
        if (null != _view)
          _view.CheckChanged -= EhCheckChanged;

        _view = value as IEnumFlagView;

        if (null != _view)
        {
          Initialize(false);
          _view.CheckChanged += EhCheckChanged;
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
