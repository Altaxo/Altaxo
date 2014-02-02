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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Properties
{
	/// <summary>
	/// Stores a hierarchy of property bags (<see cref="IPropertyBag"/>).
	/// The topmost bag is usually used to edit items, the items underneath are used to show the effective property values.
	/// When you add bags, please note that bags are added to the bottom of the hierarchy. Thus you have to add the topmost bag as the first item.
	/// </summary>
	public class PropertyHierarchy
	{
		private List<PropertyBagWithInformation> _propertyBags;

		/// <summary>
		/// Initializes a new empty instance of the <see cref="PropertyHierarchy"/> class.
		/// </summary>
		public PropertyHierarchy()
		{
			_propertyBags = new List<PropertyBagWithInformation>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyHierarchy"/> class.
		/// </summary>
		/// <param name="tuplesBagnameWithBag">Enumeration of tuples of bag information and the bag forming the initial content of this instance.</param>
		public PropertyHierarchy(IEnumerable<PropertyBagWithInformation> tuplesBagnameWithBag)
		{
			_propertyBags = new List<PropertyBagWithInformation>(tuplesBagnameWithBag);
		}

		/// <summary>
		/// Adds the bag. This bag will be added at the bottom of the hierarchy.
		/// </summary>
		/// <param name="description">The information about the bag.</param>
		/// <param name="bag">The bag to add.</param>
		public void AddBag(PropertyBagInformation description, PropertyBag bag)
		{
			_propertyBags.Add(new PropertyBagWithInformation(description, bag));
		}

		/// <summary>
		/// Tries to get a specific property value. The bags are search for the property, starting from the topmost bag and iterating to the bag at the bottom.
		/// </summary>
		/// <typeparam name="T">Type of the property value to search for.</typeparam>
		/// <param name="p">The property key.</param>
		/// <param name="value">On successfull return, this contains the retrieved property value.</param>
		/// <param name="bag">On successfull return, this contains the property bag from which the property value was retrieved.</param>
		/// <param name="bagInfo">On successfull return, this contains the information about the property bag from which the property value was retrieved.</param>
		/// <returns><c>True</c> if the property value could be successfully retrieved; <c>false</c> otherwise.</returns>
		public bool TryGetValue<T>(PropertyKey<T> p, out T value, out IPropertyBag bag, out PropertyBagInformation bagInfo)
		{
			foreach (var tuple in _propertyBags)
			{
				if (tuple.Bag.TryGetValue<T>(p, out value))
				{
					bag = tuple.Bag;
					bagInfo = tuple.BagInformation;
					return true;
				}
			}

			value = default(T);
			bag = null;
			bagInfo = default(PropertyBagInformation);
			return false;
		}

		/// <summary>
		/// Tries to get a specific property value. The bags are search for the property, starting from the topmost bag and iterating to the bag at the bottom.
		/// </summary>
		/// <typeparam name="T">Type of the property value to search for.</typeparam>
		/// <param name="propName">The property key as string value.</param>
		/// <param name="value">On successfull return, this contains the retrieved property value.</param>
		/// <param name="bag">On successfull return, this contains the property bag from which the property value was retrieved.</param>
		/// <param name="bagInfo">On successfull return, this contains the information about the property bag from which the property value was retrieved.</param>
		/// <returns><c>True</c> if the property value could be successfully retrieved; <c>false</c> otherwise.</returns>
		public bool TryGetValue<T>(string propName, out T value, out IPropertyBag bag, out PropertyBagInformation bagInfo)
		{
			return TryGetValue<T>(propName, false, out value, out bag, out bagInfo);
		}

		/// <summary>
		/// Tries to get a specific property value. The bags are search for the property, starting from the topmost bag and iterating to the bag at the bottom.
		/// </summary>
		/// <typeparam name="T">Type of the property value to search for.</typeparam>
		/// <param name="propName">The property key as string value.</param>
		/// <param name="useTopmostBagOnly">If <c>true</c>, the search is done only on the topmost bag. All other bags in the hierarchy are ignored.</param>
		/// <param name="value">On successfull return, this contains the retrieved property value.</param>
		/// <param name="bag">On successfull return, this contains the property bag from which the property value was retrieved.</param>
		/// <param name="bagInfo">On successfull return, this contains the information about the property bag from which the property value was retrieved.</param>
		/// <returns><c>True</c> if the property value could be successfully retrieved; <c>false</c> otherwise.</returns>
		public bool TryGetValue<T>(string propName, bool useTopmostBagOnly, out T value, out IPropertyBag bag, out PropertyBagInformation bagInfo)
		{
			foreach (var tuple in _propertyBags)
			{
				if (tuple.Bag.TryGetValue<T>(propName, out value))
				{
					bag = tuple.Bag;
					bagInfo = tuple.BagInformation;
					return true;
				}
				if (useTopmostBagOnly)
					break;
			}

			value = default(T);
			bag = null;
			bagInfo = default(PropertyBagInformation);
			return false;
		}

		/// <summary>
		/// Gets all property names in all bags currently present (unsorted).
		/// </summary>
		/// <returns>All property names in all bags.</returns>
		public IEnumerable<string> GetAllPropertyNames()
		{
			HashSet<string> result = new HashSet<string>();
			foreach (var tuple in _propertyBags)
			{
				var bag = tuple.Bag;
				foreach (var item in bag)
					result.Add(item.Key);
			}
			return result;
		}

		/// <summary>
		/// Creates a copy of this property hierarchy. The topmost bag is cloned, the other bags are only copyied (shallow) to the new hierarchy.
		/// This function is intended for use with the Gui system, were all properties in the hierarchy should be displayed, but only the topmost bag should be edited.
		/// </summary>
		/// <returns>Copy of this property hierarchy with the topmost bag cloned.</returns>
		public PropertyHierarchy CreateCopyWithOnlyTopmostBagCloned()
		{
			var result = new PropertyHierarchy();

			if (this._propertyBags.Count > 0)
				result._propertyBags.Add(new PropertyBagWithInformation(this._propertyBags[0].BagInformation, (IPropertyBag)this._propertyBags[0].Bag.Clone()));
			for (int i = 1; i < this._propertyBags.Count; ++i)
				result._propertyBags.Add(new PropertyBagWithInformation(this._propertyBags[i].BagInformation, this._propertyBags[i].Bag));

			return result;
		}

		/// <summary>
		/// Gets the topmost bag. This is the bag that usually can be edited (i.e. values can be added, changed, or removed from this bag).
		/// </summary>
		/// <value>
		/// The topmost property bag.
		/// </value>
		public IPropertyBag TopmostBag { get { return _propertyBags[0].Bag; } }

		/// <summary>
		/// Gets the information about the topmost property bag.
		/// </summary>
		/// <value>
		/// Information about the topmost property bag.
		/// </value>
		public PropertyBagInformation TopmostBagInformation { get { return _propertyBags[0].BagInformation; } }
	}
}