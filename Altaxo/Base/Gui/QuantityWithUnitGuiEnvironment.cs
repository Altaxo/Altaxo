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

using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Provides possible units that will be recognized when entering a quantity with a unit in Gui elements. This class is designed to fast making clones of it,
	/// where the fixed units are shared among the clones, and the additional units can be set freely in each clone.
	/// </summary>
	public class QuantityWithUnitGuiEnvironment
	{
		private static Dictionary<string, QuantityWithUnitGuiEnvironment> _registry = new Dictionary<string, QuantityWithUnitGuiEnvironment>();

		private static ReadOnlyCollection<IUnit> _emptyUnitList = new ReadOnlyCollection<IUnit>(new List<IUnit>());

		/// <summary>
		/// Units that will not change (thus, if the list is readonly, we can keep only a reference to the collection).
		/// This are possible all the fixed units, like pt, mm, cm and so on.
		/// </summary>
		private IEnumerable<IUnit> _fixedUnits;

		/// <summary>
		/// Units that can be added, depending on the situation. The most common use are relative units, like percent of graph, percent of layer and so on.
		/// </summary>
		private ObservableCollection<IUnit> _additionalUnits;

		/// <summary>
		/// Internal list where the units are sorted by its string length.
		/// </summary>
		private List<IUnit> _unitsSortedByLengthDescending;

		private IPrefixedUnit _defaultUnit;

		private IUnit _lastUsedUnit;
		private IUnit _lastUsedSIPrefix;

		private int _numberOfDisplayedDigits = 5;

		/// <summary>
		/// Triggered when the number of digits that should be displayed (in Gui boxes) changed.
		/// </summary>
		public event EventHandler NumberOfDisplayedDigitsChanged;

		/// <summary>
		/// Triggered when the default unit that is displayed in Gui boxes changed.
		/// </summary>
		public event EventHandler DefaultUnitChanged;

		public QuantityWithUnitGuiEnvironment()
			: this(null)
		{
		}

		public QuantityWithUnitGuiEnvironment(IList<IUnit> fixedUnits)
			: this(fixedUnits, new IUnit[] { })
		{
		}

		public QuantityWithUnitGuiEnvironment(IList<IUnit> fixedUnits, IUnit additionalUnit)
			: this(fixedUnits, new IUnit[] { additionalUnit })
		{
		}

		public QuantityWithUnitGuiEnvironment(IList<IUnit> fixedUnits, IEnumerable<IUnit> additionalUnits)
		{
			_fixedUnits = fixedUnits ?? _emptyUnitList;
			_additionalUnits = new ObservableCollection<IUnit>(additionalUnits);
			CreateUnitListSortedByShortcutLengthDescending();
			_additionalUnits.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(EhAdditionalUnits_CollectionChanged);
		}

		public QuantityWithUnitGuiEnvironment(QuantityWithUnitGuiEnvironment from, IEnumerable<IUnit> additionalUnits)
		{
			_fixedUnits = from._fixedUnits;
			_additionalUnits = new ObservableCollection<IUnit>(additionalUnits);
			CreateUnitListSortedByShortcutLengthDescending();
			_additionalUnits.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(EhAdditionalUnits_CollectionChanged);
		}

		private void EhAdditionalUnits_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			CreateUnitListSortedByShortcutLengthDescending();
		}

		private void CreateUnitListSortedByShortcutLengthDescending()
		{
			var list = new List<IUnit>();
			list.AddRange(_fixedUnits);
			list.AddRange(_additionalUnits);
			list.Sort(UnitComparisonByShortcutLengthDescending);
			_unitsSortedByLengthDescending = list;
		}

		private int UnitComparisonByShortcutLengthDescending(IUnit x, IUnit y)
		{
			string sx = x.ShortCut;
			string sy = y.ShortCut;

			if (sx.Length == sy.Length)
				return string.Compare(sy, sx);
			else
				return sx.Length < sy.Length ? 1 : -1;
		}

		public IEnumerable<IUnit> FixedUnits
		{
			get
			{
				return _fixedUnits;
			}
		}

		public ObservableCollection<IUnit> AdditionalUnits
		{
			get
			{
				return _additionalUnits;
			}
		}

		public IEnumerable<IUnit> UnitsSortedByShortcutLengthDescending
		{
			get
			{
				return _unitsSortedByLengthDescending;
			}
		}

		public IPrefixedUnit DefaultUnit
		{
			get
			{
				return _defaultUnit;
			}
			set
			{
				var oldValue = _defaultUnit;
				_defaultUnit = value;

				if (value != oldValue)
				{
					OnDefaultUnitChanged();
				}
			}
		}

		protected virtual void OnDefaultUnitChanged()
		{
			if (null != DefaultUnitChanged)
				DefaultUnitChanged(this, EventArgs.Empty);
		}

		public int NumberOfDisplayedDigits
		{
			get { return _numberOfDisplayedDigits; }
			set
			{
				var oldValue = _numberOfDisplayedDigits;

				value = Math.Min(15, Math.Max(3, value));
				_numberOfDisplayedDigits = value;

				if (_numberOfDisplayedDigits != oldValue)
				{
					OnNumberOfDisplayedDigitsChanged();
				}
			}
		}

		protected virtual void OnNumberOfDisplayedDigitsChanged()
		{
			if (null != NumberOfDisplayedDigitsChanged)
				NumberOfDisplayedDigitsChanged(this, EventArgs.Empty);
		}

		public static void RegisterEnvironment(string name, QuantityWithUnitGuiEnvironment env)
		{
			_registry[name] = env;
		}

		public static QuantityWithUnitGuiEnvironment TryGetEnvironment(string name)
		{
			QuantityWithUnitGuiEnvironment result;
			if (_registry.TryGetValue(name, out result))
				return result;
			else
				return null;
		}
	}
}