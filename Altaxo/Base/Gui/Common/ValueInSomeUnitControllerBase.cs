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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interface to show a length value. The use can input the value in any unit, but for the interface the value must be transfered in units of point (1/72 inch).
	/// </summary>
	public interface IDimensionfulQuantityView
	{
		Units.DimensionfulQuantity SelectedQuantity { get; set; }

		event Action SelectedQuantityChanged;

		QuantityWithUnitGuiEnvironment UnitEnvironment { set; }
	}

	[ExpectedTypeOfView(typeof(IDimensionfulQuantityView))]
	public abstract class ValueInSomeUnitControllerBase : MVCANDControllerBase<double, IDimensionfulQuantityView>
	{
		protected abstract Units.IUnit UnitOfValue { get; }

		protected abstract QuantityWithUnitGuiEnvironment UnitEnvironment { get; }

		protected override void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.UnitEnvironment = this.UnitEnvironment;
				_view.SelectedQuantity = new Units.DimensionfulQuantity(_doc, UnitOfValue).AsQuantityIn(this.UnitEnvironment.DefaultUnit);
			}
		}

		protected override void AttachView()
		{
			_view.SelectedQuantityChanged += EhQuantityChanged;
			base.AttachView();
		}

		protected override void DetachView()
		{
			_view.SelectedQuantityChanged -= EhQuantityChanged;
			base.DetachView();
		}

		public override bool Apply(bool disposeController)
		{
			GetQuantityFromView();
			_originalDoc = _doc;
			return true;
		}

		private void GetQuantityFromView()
		{
			var q = _view.SelectedQuantity;
			_doc = q.AsValueIn(UnitOfValue);
		}

		private void EhQuantityChanged()
		{
			GetQuantityFromView();
			OnMadeDirty();
		}
	}
}