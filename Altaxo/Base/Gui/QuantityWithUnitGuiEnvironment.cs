using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Altaxo.Science;

namespace Altaxo.Gui
{
	/// <summary>
	/// Provides possible units that will be recognized when entering a quantity with a unit in Gui elements.
	/// </summary>
	public class QuantityWithUnitGuiEnvironment
	{
		static Dictionary<string, QuantityWithUnitGuiEnvironment> _registry = new Dictionary<string, QuantityWithUnitGuiEnvironment>();

		static ReadOnlyCollection<IUnit> _emptyUnitList = new ReadOnlyCollection<IUnit>(new List<IUnit>());

		/// <summary>
		/// Units that will not change (thus, if the list is readonly, we can keep only a reference to the collection)
		/// </summary>
		IEnumerable<IUnit> _fixedUnits;


		ObservableCollection<IUnit> _additionalUnits;

		List<IUnit> _unitsSortedByLength;

		IUnit _defaultUnit;

		IUnit _lastUsedUnit;
		IUnit _lastUsedSIPrefix;


		public QuantityWithUnitGuiEnvironment()
			: this(null)
		{
		}

		public QuantityWithUnitGuiEnvironment(IList<IUnit> fixedUnits)
			: this(fixedUnits, new IUnit[] {})
		{
		}


		public QuantityWithUnitGuiEnvironment(IList<IUnit> fixedUnits, IUnit additionalUnit)
		: this(fixedUnits, new IUnit[]{additionalUnit})
		{
		}

		public QuantityWithUnitGuiEnvironment(IList<IUnit> fixedUnits, IEnumerable<IUnit> additionalUnits)
		{
			_fixedUnits = fixedUnits ?? _emptyUnitList;
		_additionalUnits = new ObservableCollection<IUnit>(additionalUnits);
		CreateUnitListSortedByShortcutLength();
		_additionalUnits.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(EhAdditionalUnits_CollectionChanged);
		}


		public QuantityWithUnitGuiEnvironment(QuantityWithUnitGuiEnvironment from, IEnumerable<IUnit> additionalUnits)
		{
			_fixedUnits = from._fixedUnits;
			_additionalUnits = new ObservableCollection<IUnit>(additionalUnits);
			CreateUnitListSortedByShortcutLength();
			_additionalUnits.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(EhAdditionalUnits_CollectionChanged);
		}

		void EhAdditionalUnits_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			CreateUnitListSortedByShortcutLength();
		}

		private void CreateUnitListSortedByShortcutLength()
		{
			var list = new List<IUnit>();
			list.AddRange(_fixedUnits);
			list.AddRange(_additionalUnits);
			list.Sort(UnitComparisonByShortcutLength);
			_unitsSortedByLength = list;
		}

		private int UnitComparisonByShortcutLength(IUnit x, IUnit y)
		{
			string sx = x.ShortCut;
			string sy = y.ShortCut;

			if (sx.Length == sy.Length)
				return string.Compare(sx, sy);
			else
				return sx.Length < sy.Length ? -1 : 1;
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

		public IEnumerable<IUnit> UnitsSortedByShortcutLength
		{
			get
			{
				return _unitsSortedByLength;
			}
		}

		public IUnit DefaultUnit
		{
			get
			{
				return _defaultUnit;
			}
			set
			{
				_defaultUnit = value;
			}
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
