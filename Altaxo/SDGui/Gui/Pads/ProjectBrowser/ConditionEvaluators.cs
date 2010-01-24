using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.Core;
namespace Altaxo.Gui.Pads.ProjectBrowser
{
	/// <summary>
  /// This helps in conditions where the number of selected data columns and the type of the item cares.
  /// Two attributes are used: selcount and itemtype. The value of selcount is one of the comparer chars: (&lt; = &gt;)
	/// and an integer number, for instance: =1. The value of itemtype, if set, is the class name of the items, that should
	/// be selected.
  /// </summary>
	public class ListItemSelectionEvaluator : IConditionEvaluator
	{
		/// <summary>
		/// Evaluates if the chosen condition is true.
		/// </summary>
		/// <param name="caller"></param>
		/// <param name="condition"></param>
		/// <returns>True if the condition is valid.</returns>
		public bool IsValid(object caller, Condition condition)
		{
			bool result;

			string expectedSelCount = condition.Properties["selcount"].ToLowerInvariant();
			result = EvaluateSelCount(caller, expectedSelCount);
			if (false == result)
				return result;

			string expectedItemType = condition.Properties["selitems"];
			result = EvaluateItemType(caller, expectedItemType);
			if (false == result)
				return result;

			return true;
		}

		private bool EvaluateSelCount(object caller, string cond)
		{
			// the first character of cond have to be <, = or >
			// the rest of cond should be an integer
			if (string.IsNullOrEmpty(cond))
				return true; // no restriction concerning the number of items
			if (cond.Length <= 1)
				return false;

			int condNumber;
			if (!int.TryParse(cond.Substring(1), out condNumber))
				return false;

			int currItems = ((ProjectBrowseController)caller).GetNumberOfSelectedListItems();

			switch (cond[0])
			{
				case '<':
					return currItems < condNumber;
				case '=':
					return currItems == condNumber;
				case '>':
					return currItems > condNumber;
				default:
					return false;
			}
		}

		private bool EvaluateItemType(object caller, string expectedItemType)
		{
			// the first character of cond have to be <, = or >
			// the rest of cond should be an integer
			if (string.IsNullOrEmpty(expectedItemType))
				return true; // no restriction concerning the number of items

			List<object> selItems = ((ProjectBrowseController)caller).GetSelectedListItems();
			if (expectedItemType != "Folder") // if folder, than do no expansion of selected items!
				((ProjectBrowseController)caller).ExpandItemListToSubfolderItems(selItems);

			foreach(object item in selItems)
				if(expectedItemType!=item.GetType().ToString())
					return false;

			return true;		
		}

	
	}
}
