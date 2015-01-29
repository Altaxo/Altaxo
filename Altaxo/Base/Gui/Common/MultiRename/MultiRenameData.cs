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

namespace Altaxo.Gui.Common.MultiRename
{
	/// <summary>
	/// Data neccessary to rename a list of objects (but can also used for instance for exporting a list of objects). The idea behind is to have a bunch of shortcuts, like [N], that can be used as variables for the rename operation.
	/// Since using this class is somewhat complex, see the remarks for the details.
	/// </summary>
	/// <remarks>
	/// Momentarily, four different types of shortcuts are implemented:
	/// An Integer shortcut provide a number, a string shortcuts provides a string, a DateTime shortcut a DateTime value and an Array shortcut provides a string array.
	/// In order to use this class for rename operations, after creating an instance of this class you have to:
	/// <list type="bullet">
	/// <item><description>Add some objects that you want to process (rename etc.) with <see cref="AddObjectsToRename"/></description></item>
	/// <item><description>Add some shortcuts that can be used as variables for flexible renaming with <see cref="RegisterIntegerShortcut"/>, <see cref="RegisterStringShortcut"/>, <see cref="RegisterDateTimeShortcut"/> or <see cref="RegisterStringArrayShortcut"/>.</description></item>
	/// <item><description>Add some columns for the list that should be shown in the rename dialog with <see cref="RegisterListColumn"/>. One of the calls to <see cref="RegisterListColumn"/> should designate the column with the new name of the objects, in this case the 2nd argument (the handler) should be null.</description></item>
	/// <item><description>Add a handler that will do the processing of the items (renaming, exporting etc). with <see cref="RegisterRenameActionHandler"/>.</description></item>
	/// <item><description>Set the value of <see cref="DefaultPatternString"/> to a value that should be initially shown as pattern when the rename dialog opens, for instance [N].</description></item>
	///	</list>
	/// </remarks>
	public class MultiRenameData
	{
		#region Inner classes

		/// <summary>
		/// Shortcut types
		/// </summary>
		private enum ShortcutType { Integer, String, StringArray, DateTime };

		private class ShortcutInformation
		{
			public ShortcutInformation(ShortcutType type, string description)
			{
				ShortcutType = type;
				ShortcutDescription = description;
			}

			public ShortcutType ShortcutType { get; private set; }

			public string ShortcutDescription { get; private set; }
		}

		private class RenameInfo
		{
			public object ObjectToRename;
			public string OldName;
			public string NewName;
		}

		#endregion Inner classes

		/// <summary>
		/// Pattern string that is initially shown when the multi rename dialog opens.
		/// </summary>
		private string _defaultPatternString;

		/// <summary>
		/// Stores for every shortcut some information, for instance the type of shortcut, and a description, which can be shown in a Gui view.
		/// </summary>
		private Dictionary<string, ShortcutInformation> _shortcutToType = new Dictionary<string, ShortcutInformation>();

		/// <summary>
		/// Stores for every number shortcut a function which is able to retrieve the shortcut's item, for instance the name of the object. For this, it is neccessary to call the function with two arguments,
		/// the object itself and the current index of the object into the list view. Since the list view can be sorted by various columns, the index of the object in the list may change.
		/// </summary>
		private Dictionary<string, Func<object, int, int>> _integerShortcutToGetter = new Dictionary<string, Func<object, int, int>>();

		/// <summary>
		/// Stores for every DateTime shortcut a function which is able to retrieve a DateTime that can be deduced from the shortcut's item, for instance the creation date of the item. For this, it is neccessary to call the function with two arguments,
		/// the object itself and the current index of the object into the list view. Since the list view can be sorted by various columns, the index of the object in the list may change.
		/// </summary>
		private Dictionary<string, Func<object, int, DateTime>> _dateTimeShortcutToGetter = new Dictionary<string, Func<object, int, DateTime>>();

		/// <summary>
		/// Stores for every String shortcut a function which is able to retrieve a text string from the shortcut's item, for instance the full name of the object. For this, it is neccessary to call the function with two arguments,
		/// the object itself and the current index of the object into the list view. Since the list view can be sorted by various columns, the index of the object in the list may change.
		/// </summary>
		private Dictionary<string, Func<object, int, string>> _stringShortcutToGetter = new Dictionary<string, Func<object, int, string>>();

		/// <summary>
		/// Stores for every Array shortcut a function which is able to retrieve a string array from the shortcut's item, for instance the full name of the object split into individual folder parts. For this, it is neccessary to call the function with two arguments,
		/// the object itself and the current index of the object into the list view. Since the list view can be sorted by various columns, the index of the object in the list may change.
		/// </summary>
		private Dictionary<string, Func<object, int, string[]>> _stringArrayShortcutToGetter = new Dictionary<string, Func<object, int, string[]>>();

		/// <summary>
		/// Stores the objects to rename along with the old name and the new name.
		/// </summary>
		private List<RenameInfo> _objectsToRename = new List<RenameInfo>();

		/// <summary>
		/// This handler is called when the items should be processed (renamed, exported or so). If the function succeeds, the return value should be zero or an empty list.
		/// If the function is partially unsuccessfull, for instance because some items could not be renamed, the function should return those unsuccessfully processed items in the list.
		/// </summary>
		private Func<MultiRenameData, List<object>> _renameActionHandler;

		/// <summary>
		/// Stores columns of information for the objects to rename. Key is the column name, value is a function which retrieves a string for each object.
		/// Note that it is assumed that the first list item corresponds to the old name of the object. This is shown in the first column.
		/// The second column is reserved for the new name. Then the other columns follow.
		/// </summary>
		private List<KeyValuePair<string, Func<object, string>>> _columnsOfObjectInformation = new List<KeyValuePair<string, Func<object, string>>>();

		/// <summary>Gets or sets the default pattern string, i.e. the pattern string that is initially shown when the multi rename dialog opens.</summary>
		/// <value>Pattern string that is initially shown when the multi rename dialog opens.</value>
		public string DefaultPatternString
		{
			get { return _defaultPatternString; }
			set { _defaultPatternString = value; }
		}

		/// <summary>
		/// Stores columns of information for the objects to rename. Key is the column name, value is a function which retrieves a string for each object.
		/// Note that it is assumed that the first list item corresponds to the old name of the object. This is shown in the first column.
		/// The second column is reserved for the new name. Then the other columns follow.
		/// </summary>
		public List<KeyValuePair<string, Func<object, string>>> ColumnsOfObjectInformation
		{
			get { return _columnsOfObjectInformation; }
		}

		/// <summary>Return the number of objects to be processed (renamed etc).</summary>
		public int ObjectsToRenameCount
		{
			get
			{
				return _objectsToRename.Count;
			}
		}

		/// <summary>
		/// Adds the provided objects to the list of objects to be renamed.
		/// </summary>
		public void AddObjectsToRename(IEnumerable<object> list)
		{
			foreach (object o in list)
			{
				_objectsToRename.Add(new RenameInfo() { ObjectToRename = o, OldName = null, NewName = null });
			}
		}

		/// <summary>Gets the i-th object to rename.</summary>
		/// <param name="i">Index of the object in the internal list.</param>
		/// <returns>The object to rename at index i.</returns>
		public object GetObjectToRename(int i)
		{
			return _objectsToRename[i].ObjectToRename;
		}

		/// <summary>Sets the proposed new name for object. Note that the object is not renamed here, instead this is done when calling the rename handler.</summary>
		/// <param name="i">Index of the object in the internal list.</param>
		/// <param name="newName">The proposed new name of the object.</param>
		public void SetNewNameForObject(int i, string newName)
		{
			_objectsToRename[i].NewName = newName;
		}

		/// <summary>
		/// Gets the proposed new name for the object at position <paramref name="i"/>.
		/// </summary>
		/// <param name="i">Index of the object in the internal list.</param>
		/// <returns>The proposed new name of the object at index i.</returns>
		public string GetNewNameForObject(int i)
		{
			return _objectsToRename[i].NewName;
		}

		/// <summary>Gets the integer value of a integer shortcut.</summary>
		/// <param name="shortcut">The shortcut (has to be registered as integer shortcut before).</param>
		/// <param name="originalListIndex">Index of the object in the internal list.</param>
		/// <param name="currentSortIndex">Current index of the object, for instance in a Gui list that can be sorted by a column.</param>
		/// <returns>The value of the integer shortcut, i.e. an integer value that is associated with the object itself or with its position in the list.</returns>
		public int GetIntegerValueOfShortcut(string shortcut, int originalListIndex, int currentSortIndex)
		{
			return _integerShortcutToGetter[shortcut](GetObjectToRename(originalListIndex), currentSortIndex);
		}

		/// <summary>Gets the value of a string shortcut.</summary>
		/// <param name="shortcut">The shortcut (has to be registered as string shortcut before).</param>
		/// <param name="originalListIndex">Index of the object in the internal list.</param>
		/// <param name="currentSortIndex">Current index of the object, for instance in a Gui list that can be sorted by a column.</param>
		/// <returns>The value of the string shortcut, i.e. a string value that is associated with the object itself or with its position in the list.</returns>
		public string GetStringValueOfShortcut(string shortcut, int originalListIndex, int currentSortIndex)
		{
			return _stringShortcutToGetter[shortcut](GetObjectToRename(originalListIndex), currentSortIndex);
		}

		/// <summary>Gets the value of a DateTime shortcut.</summary>
		/// <param name="shortcut">The shortcut (has to be registered as DateTime shortcut before).</param>
		/// <param name="originalListIndex">Index of the object in the internal list.</param>
		/// <param name="currentSortIndex">Current index of the object, for instance in a Gui list that can be sorted by a column.</param>
		/// <returns>The value of the DateTime shortcut, i.e. a DateTime value that is associated with the object itself or with its position in the list.</returns>
		public DateTime GetDateTimeValueOfShortcut(string shortcut, int originalListIndex, int currentSortIndex)
		{
			return _dateTimeShortcutToGetter[shortcut](GetObjectToRename(originalListIndex), currentSortIndex);
		}

		/// <summary>Gets the value of an array shortcut.</summary>
		/// <param name="shortcut">The shortcut (has to be registered as array shortcut before).</param>
		/// <param name="originalListIndex">Index of the object in the internal list.</param>
		/// <param name="currentSortIndex">Current index of the object, for instance in a Gui list that can be sorted by a column.</param>
		/// <returns>The value of the array shortcut, i.e. an array of strings that is associated with the object itself or with its position in the list.</returns>
		public string[] GetStringArrayValueOfShortcut(string shortcut, int originalListIndex, int currentSortIndex)
		{
			return _stringArrayShortcutToGetter[shortcut](GetObjectToRename(originalListIndex), currentSortIndex);
		}

		/// <summary>Registers the rename action handler.
		/// This handler is called when the items should be processed (renamed, exported or so). If the function succeeds, the return value should be zero or an empty list.
		/// If the function is partially unsuccessfull, for instance because some items could not be renamed, the function should return those unsuccessfully processed items in the list.
		/// </summary>
		/// <param name="handler">The handler function.</param>
		public void RegisterRenameActionHandler(Func<MultiRenameData, List<object>> handler)
		{
			_renameActionHandler = handler;
		}

		/// <summary>Gets the description of a shortcut.</summary>
		/// <param name="shortcut">The shortcut.</param>
		/// <returns>The description of the shortcut. This has to be provided when registering the shortcut with one of the <c>Register..Shortcut</c> functions.</returns>
		public string GetShortcutDescription(string shortcut)
		{
			return _shortcutToType[shortcut].ShortcutDescription;
		}

		/// <summary>Registers an integer shortcut.</summary>
		/// <param name="shortcut">The shortcut string (without square brackets).</param>
		/// <param name="valueGetter">The function that provided the integer value of this shortcut, given the object itself (1st argument) and its current index in a Gui list (2nd argument).</param>
		/// <param name="descriptionText">Some text that describes the shortcut.</param>
		public void RegisterIntegerShortcut(string shortcut, Func<object, int, int> valueGetter, string descriptionText)
		{
			_shortcutToType.Add(shortcut, new ShortcutInformation(ShortcutType.Integer, descriptionText));
			_integerShortcutToGetter.Add(shortcut, valueGetter);
		}

		/// <summary>Registers a string shortcut.</summary>
		/// <param name="shortcut">The shortcut string (without square brackets).</param>
		/// <param name="valueGetter">The function that provided the string value of this shortcut, given the object itself (1st argument) and its current index in a Gui list (2nd argument).</param>
		/// <param name="descriptionText">Some text that describes the shortcut.</param>
		public void RegisterStringShortcut(string shortcut, Func<object, int, string> valueGetter, string descriptionText)
		{
			_shortcutToType.Add(shortcut, new ShortcutInformation(ShortcutType.String, descriptionText));
			_stringShortcutToGetter.Add(shortcut, valueGetter);
		}

		/// <summary>Registers a DateTime shortcut.</summary>
		/// <param name="shortcut">The shortcut string (without square brackets).</param>
		/// <param name="valueGetter">The function that provided the DateTime value of this shortcut, given the object itself (1st argument) and its current index in a Gui list (2nd argument).</param>
		/// <param name="descriptionText">Some text that describes the shortcut.</param>
		public void RegisterDateTimeShortcut(string shortcut, Func<object, int, DateTime> valueGetter, string descriptionText)
		{
			_shortcutToType.Add(shortcut, new ShortcutInformation(ShortcutType.DateTime, descriptionText));
			_dateTimeShortcutToGetter.Add(shortcut, valueGetter);
		}

		/// <summary>Registers an array shortcut.</summary>
		/// <param name="shortcut">The shortcut string (without square brackets).</param>
		/// <param name="valueGetter">The function that provided the string array value of this shortcut, given the object itself (1st argument) and its current index in a Gui list (2nd argument).</param>
		/// <param name="descriptionText">Some text that describes the shortcut.</param>
		public void RegisterStringArrayShortcut(string shortcut, Func<object, int, string[]> valueGetter, string descriptionText)
		{
			_shortcutToType.Add(shortcut, new ShortcutInformation(ShortcutType.StringArray, descriptionText));
			_stringArrayShortcutToGetter.Add(shortcut, valueGetter);
		}

		/// <summary>
		/// Register a column for a Gui list that shows the properties of the objects to rename, for instance its old name, its creation date, its new name etc.
		/// </summary>
		/// <param name="columnName">The text that is used as header for this column.</param>
		/// <param name="valueGetter">A function that gets the value of this column for this object, e.g. its old name etc. If this argument is null, it is assumed that the column you want to register here is the column of the new name, so the new name is shown in this column.</param>
		public void RegisterListColumn(string columnName, Func<object, string> valueGetter)
		{
			_columnsOfObjectInformation.Add(new KeyValuePair<string, Func<object, string>>(columnName, valueGetter));
		}

		/// <summary>
		/// Retrieves the value of the Gui list, given the column and the row number.
		/// </summary>
		/// <param name="columnNumber">Column number of the list.</param>
		/// <param name="rowNumber">Index of the object in the internal list (index before some sort).</param>
		/// <returns></returns>
		public string GetListValue(int columnNumber, int rowNumber)
		{
			return _columnsOfObjectInformation[columnNumber].Value(_objectsToRename[rowNumber]);
		}

		/// <summary>
		/// Retrieves the names of all integer shortcuts.
		/// </summary>
		/// <returns>An enumeration of all integer shortcut names.</returns>
		public IEnumerable<string> GetIntegerShortcuts()
		{
			return _integerShortcutToGetter.Keys;
		}

		/// <summary>
		/// Retrieves the names of all string shortcuts.
		/// </summary>
		/// <returns>An enumeration of all string shortcut names.</returns>
		public IEnumerable<string> GetStringShortcuts()
		{
			return _stringShortcutToGetter.Keys;
		}

		/// <summary>
		/// Retrieves the names of all DateTime shortcuts.
		/// </summary>
		/// <returns>An enumeration of all DateTime shortcut names.</returns>
		public IEnumerable<string> GetDateTimeShortcuts()
		{
			return _dateTimeShortcutToGetter.Keys;
		}

		/// <summary>
		/// Retrieves the names of all array shortcuts.
		/// </summary>
		/// <returns>An enumeration of all array shortcut names.</returns>
		public IEnumerable<string> GetArrayShortcuts()
		{
			return _stringArrayShortcutToGetter.Keys;
		}

		/// <summary>
		/// Calls the function that finally process the items, for instance rename them, export them etc.
		/// </summary>
		/// <returns>True when the action was successfull. In this case a Gui dialog can be closed.
		/// If the return value is false, the action failed at least for some items. Then, this instance will contain now those items for which the action failed. In this case a dialog should not
		/// be closed, and the items which remain should be renamed in a second step then.</returns>
		public bool DoRename()
		{
			if (null != _renameActionHandler)
			{
				var list = _renameActionHandler(this);

				if (list != null && list.Count != 0)
				{
					_objectsToRename.Clear();
					AddObjectsToRename(list);
					return false;
				}
				else
				{
					return true;
				}
			}
			return true;
		}
	}
}