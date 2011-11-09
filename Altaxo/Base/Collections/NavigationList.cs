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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{

	/// <summary>
	/// Implements a list that can be used to navigate forward or backward using stored navigation points.
	/// </summary>
	/// <typeparam name="T">Type of parameter that is used to restore the navigation points. In order to compare a navigation point to add with the current already present navigation point, the type must implement <see cref="IEquatable&lt;T&gt;"/> </typeparam>
	/// <remarks>Although this list can also be used for other purposes, it is developed for minimum storage requirements, but not for optimal performance. Internally,
	/// a list is used, so that removing of navigation points is relatively time consuming.</remarks>
	public class NavigationList<T> where T : IEquatable<T>
	{
		List<T> _list = new List<T>();
		int _currentNavigationPoint = -1;
		int _maxNumberOfNavigationPoints = int.MaxValue;

		/// <summary>Initializes a new instance of the <see cref="NavigationList&lt;T&gt;"/> class.</summary>
		public NavigationList()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="NavigationList&lt;T&gt;"/> class.</summary>
		/// <param name="maxNumberOfNavigationPoints">The maximum number of navigation points that is kept in this list.
		/// If the list exceeds this maximum, the oldest navigation points will be removed from the list.</param>
		public NavigationList(int maxNumberOfNavigationPoints)
		{
			if (maxNumberOfNavigationPoints < 2)
				throw new ArgumentOutOfRangeException("maxNumberOfNavigationPoints have to be >=2");

			_maxNumberOfNavigationPoints = maxNumberOfNavigationPoints;
		}

		/// <summary>
		/// Adds a navigation point to the list.
		/// </summary>
		/// <param name="point">Parameter that can later be used to restore the state of application that stored this point.</param>
		public void AddNavigationPoint(T point)
		{
			if (_currentNavigationPoint >= 0 && _currentNavigationPoint < _list.Count && _list[_currentNavigationPoint].Equals(point)) // the same navigation point is not stored twice
				return;

			if (_currentNavigationPoint < (_list.Count - 1)) // if it is a real new navigation point, then remove all forward items before adding the new navigation point
			{
				_list.RemoveRange(_currentNavigationPoint + 1, _list.Count - _currentNavigationPoint - 1);
			}

			_list.Add(point);

			if (_list.Count > _maxNumberOfNavigationPoints)
				_list.RemoveAt(0);

			_currentNavigationPoint = _list.Count - 1;
		}

		/// <summary>
		/// Tries to go one point backward in the navigation list.
		/// </summary>
		/// <param name="point">On success, returns the previous navigation point.</param>
		/// <returns>True if a previous navigation point could be returned in <paramref name="point"/>, otherwise <c>False/c>.</returns>
		public bool TryNavigateBackward(out T point)
		{
			return TryNavigateBackward(out point, null, false);
		}


		/// <summary>Tries to go backward in the navigation list to the next valid navigation point.</summary>
		/// <param name="point">On success, returns a previous navigation point, which is valid.</param>
		/// <param name="IsStillValidEvaluation">Evaluation function, which tests the navigation points in the list for validity.</param>
		/// <param name="removeInvalidNavigationPoints">If set to <c>true</c>, invalid navigation points encountered during the backward movement in the list would be removed from the navigation list.</param>
		/// <returns>Returns <c>True</c> when a valid navigation point backward in the list was found. </returns>
		public bool TryNavigateBackward(out T point, Func<T, bool> IsStillValidEvaluation, bool removeInvalidNavigationPoints)
		{
			for (; ; )
			{
				var newNavigationPoint = _currentNavigationPoint - 1;
				if (newNavigationPoint < 0)
				{
					point = default(T);
					_currentNavigationPoint = _list.Count == 0 ? -1 : 0;
					return false;
				}
				if (null == IsStillValidEvaluation || IsStillValidEvaluation(_list[newNavigationPoint]))
				{
					point = _list[newNavigationPoint];
					_currentNavigationPoint = newNavigationPoint;
					return true;
				}
				if (removeInvalidNavigationPoints)
				{
					_list.RemoveAt(newNavigationPoint);
				}
				_currentNavigationPoint = newNavigationPoint;
			}
		}

		/// <summary>
		/// Tries to go one point forward in the navigation list.
		/// </summary>
		/// <param name="point">On success, returns the previous navigation point.</param>
		/// <returns>True if a previous navigation point could be returned in <paramref name="point"/>, otherwise <c>False/c>.</returns>
		public bool TryNavigateForward(out T point)
		{
			return TryNavigateForward(out point, null, false);
		}


		/// <summary>Tries to go forward in the navigation list to the next valid navigation point.</summary>
		/// <param name="point">On success, returns the next navigation point, which is valid.</param>
		/// <param name="IsStillValidEvaluation">Evaluation function, which tests the navigation points in the list for validity.</param>
		/// <param name="removeInvalidNavigationPoints">If set to <c>true</c>, invalid navigation points encountered during the forward movement in the list would be removed from the navigation list.</param>
		/// <returns>Returns <c>True</c> when a valid navigation point forward in the list was found. </returns>
		public bool TryNavigateForward(out T point, Func<T, bool> IsStillValidEvaluation, bool removeInvalidNavigationPoints)
		{
			for (; ; )
			{
				var newNavigationPoint = _currentNavigationPoint + 1;
				if (newNavigationPoint >= _list.Count)
				{
					point = default(T);
					_currentNavigationPoint = _list.Count - 1;
					return false;
				}
				if (null == IsStillValidEvaluation || IsStillValidEvaluation(_list[newNavigationPoint]))
				{
					point = _list[newNavigationPoint];
					_currentNavigationPoint = newNavigationPoint;
					return true;
				}
				if (removeInvalidNavigationPoints)
				{
					_list.RemoveAt(newNavigationPoint);
				}
				else
				{
					_currentNavigationPoint = newNavigationPoint;
				}
			}
		}

		/// <summary>
		/// Number of currently stored navigation points.
		/// </summary>
		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		/// <summary>
		/// Clears all navigation points.
		/// </summary>
		public void Clear()
		{
			_list.Clear();
			_currentNavigationPoint = 0;
		}

	}
}
