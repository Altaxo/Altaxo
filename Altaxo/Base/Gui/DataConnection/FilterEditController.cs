using Altaxo.Collections;
using Altaxo.DataConnection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Altaxo.Gui.DataConnection
{
	public interface IFilterEditView
	{
		void SetValueText(string txt);

		string SingleValueText { get; set; }

		string IntervalFromText { get; set; }

		string intervalToText { get; set; }

		void SetOperatorChoices(SelectableListNodeList list);

		string OperatorText { get; set; }

		event Action SimpleUpdated;

		event Action IntervalUpdated;

		event Action ClearAll;
	}

	[ExpectedTypeOfView(typeof(IFilterEditView))]
	public class FilterEditController : IMVCAController
	{
		private static Regex _rx1 = new Regex(@"^([^<>=]*)\s*(<|>|=|<>|<=|>=)\s*([^<>=]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex _rx2 = new Regex(@"^([^<>=]*)\s*BETWEEN\s+(.+)\s+AND\s+(.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private QueryField _field;
		private IFilterEditView _view;

		private SelectableListNodeList _operatorChoices;

		private void Initialize(bool initData)
		{
			if (initData)
			{
				_operatorChoices = new SelectableListNodeList();
				foreach (var s in new string[] { ">", ">=", "<", "<=", "<>" })
					_operatorChoices.Add(new SelectableListNode(s, s, false));
			}
			if (null != _view)
			{
				_view.SetOperatorChoices(_operatorChoices);

				// initialize value
				string crit = _field.Filter;
				_view.SetValueText(crit);

				// initialize fields
				if (crit.Length > 0)
				{
					Match m = _rx1.Match(crit);
					if (m.Success)
					{
						_view.OperatorText = m.Groups[2].Value;
						_view.SingleValueText = m.Groups[3].Value;
					}
					m = _rx2.Match(crit);
					if (m.Success)
					{
						_view.IntervalFromText = m.Groups[2].Value;
						_view.intervalToText = m.Groups[3].Value;
					}
				}
			}
		}

		public QueryField QueryField
		{
			get { return _field; }
			set
			{
				// save query field
				_field = value;
			}
		}

		public object ViewObject
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				if (null != _view)
				{
					_view.SimpleUpdated -= UpdateSimple;
					_view.IntervalUpdated -= UpdateBetween;
					_view.ClearAll -= ClearAll;
				}
				_view = value as IFilterEditView;
				if (null != _view)
				{
					Initialize(false);

					_view.SimpleUpdated += UpdateSimple;
					_view.IntervalUpdated += UpdateBetween;
					_view.ClearAll += ClearAll;
				}
			}
		}

		public object ModelObject
		{
			get { return null; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply()
		{
			return true;
		}

		private void UpdateSimple()
		{
			if (!string.IsNullOrEmpty(_view.OperatorText) && _view.SingleValueText.Length > 0)
			{
				var parm = GetParameter(_view.SingleValueText);
				_view.SetValueText(string.Format("{0} {1}", _view.OperatorText, parm));
			}
		}

		private void UpdateBetween()
		{
			if (_view.IntervalFromText.Length > 0 && _view.intervalToText.Length > 0)
			{
				var parmFrom = GetParameter(_view.IntervalFromText);
				var parmTo = GetParameter(_view.intervalToText);
				_view.SetValueText(string.Format("BETWEEN {0} AND {1}", parmFrom, parmTo));
			}
		}

		private void ClearAll()
		{
			_view.OperatorText = null;
			_view.SingleValueText = string.Empty;
			_view.IntervalFromText = string.Empty;
			_view.intervalToText = string.Empty;
			_view.SetValueText(string.Empty);
		}

		private string GetParameter(string p)
		{
			// if not already in quotes
			if (p.Length < 2 || p[0] != '\'' || p[p.Length - 1] != '\'')
			{
				// and if this is not a number
				double d;
				if (!double.TryParse(p, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
				{
					// then enclose in quotes
					p = string.Format("'{0}'", p);
				}
			}

			// done
			return p;
		}
	}
}