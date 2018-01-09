#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Altaxo.Calc.Regression.Nonlinear;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	public class ParameterSetViewItem
	{
		public string Name { get; set; }

		public string Value { get; set; }

		public bool Vary { get; set; }

		public string Variance { get; set; }
	}

	public interface IParameterSetView
	{
		void Initialize(List<ParameterSetViewItem> list);

		List<ParameterSetViewItem> GetList();
	}

	/// <summary>
	/// Summary description for ParameterSetController.
	/// </summary>
	[UserControllerForObject(typeof(ParameterSet))]
	[ExpectedTypeOfView(typeof(IParameterSetView))]
	public class ParameterSetController1 : MVCANControllerEditOriginalDocBase<ParameterSet, IParameterSetView>
	{
		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (_view != null)
			{
				List<ParameterSetViewItem> list = new List<ParameterSetViewItem>();

				for (int i = 0; i < _doc.Count; i++)
				{
					ParameterSetViewItem item = new ParameterSetViewItem();
					item.Name = _doc[i].Name;
					item.Value = Altaxo.Serialization.GUIConversion.ToString(_doc[i].Parameter);
					item.Vary = _doc[i].Vary;
					item.Variance = Altaxo.Serialization.GUIConversion.ToString(_doc[i].Variance);

					list.Add(item);
				}

				Current.Dispatcher.InvokeIfRequired(() => _view.Initialize(list));
			}
		}

		public override bool Apply(bool disposeController)
		{
			List<ParameterSetViewItem> list = _view.GetList();

			for (int i = 0; i < _doc.Count; i++)
			{
				double paraValue;
				double varianceValue;

				// Parameter
				if (Altaxo.Serialization.GUIConversion.IsDouble(list[i].Value, out paraValue))
				{
					_doc[i].Parameter = paraValue;
				}
				else
				{
					Current.Gui.ErrorMessageBox(string.Format("Parameter {0} is not numeric", list[i].Name));
					return false;
				}

				// Vary
				_doc[i].Vary = list[i].Vary;

				// Variance
				if (Altaxo.Serialization.GUIConversion.IsDouble(list[i].Variance, out varianceValue))
				{
					_doc[i].Variance = varianceValue;
				}
				else if (!string.IsNullOrEmpty(list[i].Variance))
				{
					Current.Gui.ErrorMessageBox(string.Format("Variance of parameter {0} is not numeric", list[i].Name));
					return false;
				}
			}

			return ApplyEnd(true, disposeController);
		}
	}
}