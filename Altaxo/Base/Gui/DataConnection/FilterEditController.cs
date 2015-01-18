#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

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

		private string _value;

		private SelectableListNodeList _operatorChoices;

		public FilterEditController(QueryField field)
		{
			QueryField = field;
			Initialize(true);
		}

		private void Initialize(bool initData)
		{
			if (initData)
			{
				_operatorChoices = new SelectableListNodeList();
				foreach (var s in new string[] { "", "=", ">", ">=", "<", "<=", "<>" })
					_operatorChoices.Add(new SelectableListNode(s, s, false));

				_value = _field.Filter;
			}
			if (null != _view)
			{
				_view.SetOperatorChoices(_operatorChoices);

				// initialize value
				_view.SetValueText(_value);

				// initialize fields
				if (_value.Length > 0)
				{
					Match m = _rx1.Match(_value);
					if (m.Success)
					{
						SetOperatorText(m.Groups[2].Value);
						_view.SingleValueText = m.Groups[3].Value;
					}
					m = _rx2.Match(_value);
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
			private set
			{
				// save query field
				_field = value;
			}
		}

		public object ViewObject
		{
			get
			{
				return _view;
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
			get { return _value; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply(bool disposeController)
		{
			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		private string GetOperatorText()
		{
			var node = _operatorChoices.FirstSelectedNode;
			return null == node ? null : (string)node.Tag;
		}

		private void SetOperatorText(string op)
		{
			_operatorChoices.ClearSelectionsAll();
			foreach (var node in _operatorChoices)
			{
				if (op == (string)node.Tag)
				{
					node.IsSelected = true;
					break;
				}
			}
			if (null != _view)
			{
				_view.SetOperatorChoices(_operatorChoices);
			}
		}

		private void UpdateSimple()
		{
			if (!string.IsNullOrEmpty(GetOperatorText()) && _view.SingleValueText.Length > 0)
			{
				var parm = GetParameter(_view.SingleValueText);
				_value = string.Format("{0} {1}", GetOperatorText(), parm);
				if (null != _view)
					_view.SetValueText(_value);
			}
		}

		private void UpdateBetween()
		{
			if (_view.IntervalFromText.Length > 0 && _view.intervalToText.Length > 0)
			{
				var parmFrom = GetParameter(_view.IntervalFromText);
				var parmTo = GetParameter(_view.intervalToText);
				_value = string.Format("BETWEEN {0} AND {1}", parmFrom, parmTo);
				if (null != _view)
					_view.SetValueText(_value);
			}
		}

		private void ClearAll()
		{
			_operatorChoices.ClearSelectionsAll();
			_operatorChoices[0].IsSelected = true;
			_view.SetOperatorChoices(_operatorChoices);
			_view.SingleValueText = string.Empty;
			_view.IntervalFromText = string.Empty;
			_view.intervalToText = string.Empty;
			_value = string.Empty;
			_view.SetValueText(_value);
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