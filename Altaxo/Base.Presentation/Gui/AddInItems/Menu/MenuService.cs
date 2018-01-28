﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using Altaxo.AddInItems;
using Altaxo.Main.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// using WinForms = System.Windows.Forms;

namespace Altaxo.Gui.AddInItems
{
	/// <summary>
	/// Creates WPF menu controls from the AddIn Tree.
	/// </summary>
	public static class MenuService
	{
		internal sealed class MenuCreateContext
		{
			public UIElement InputBindingOwner;
			public string ActivationMethod;
			public bool ImmediatelyExpandMenuBuildersForShortcuts;
		}

		private static Dictionary<string, System.Windows.Input.ICommand> knownCommands = LoadDefaultKnownCommands();

		private static Dictionary<string, System.Windows.Input.ICommand> LoadDefaultKnownCommands()
		{
			var knownCommands = new Dictionary<string, System.Windows.Input.ICommand>();
			foreach (Type t in new Type[] { typeof(ApplicationCommands), typeof(NavigationCommands) })
			{
				foreach (PropertyInfo p in t.GetProperties())
				{
					knownCommands.Add(p.Name, (System.Windows.Input.ICommand)p.GetValue(null, null));
				}
			}
			return knownCommands;
		}

		/// <summary>
		/// Gets a known WPF command.
		/// </summary>
		/// <param name="commandName">The name of the command, e.g. "Copy".</param>
		/// <returns>The WPF ICommand with the given name, or null if the command was not found.</returns>
		public static System.Windows.Input.ICommand GetKnownCommand(string commandName)
		{
			if (commandName == null)
				throw new ArgumentNullException("commandName");
			System.Windows.Input.ICommand command;
			lock (knownCommands)
			{
				if (knownCommands.TryGetValue(commandName, out command))
					return command;
			}
			return null;
		}

		/// <summary>
		/// Registers a WPF command for use with the &lt;MenuItem command="name"&gt; syntax.
		/// </summary>
		public static void RegisterKnownCommand(string name, System.Windows.Input.ICommand command)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (command == null)
				throw new ArgumentNullException("command");
			lock (knownCommands)
			{
				knownCommands.Add(name, command);
			}
		}

		/// <summary>
		/// Updates the status of the menu items.
		/// </summary>
		/// <param name="menuItems">The menu items.</param>
		/// <remarks>The workbench calls this function when <see cref="CommandManager.RequerySuggested"/> fires.</remarks>
		public static void UpdateStatus(IEnumerable menuItems)
		{
			if (menuItems == null)
				return;
			foreach (object o in menuItems)
			{
				IStatusUpdate cmi = o as IStatusUpdate;
				if (cmi != null)
					cmi.UpdateStatus();
			}
		}

		public static void UpdateText(IEnumerable menuItems)
		{
			if (menuItems == null)
				return;
			foreach (object o in menuItems)
			{
				IStatusUpdate cmi = o as IStatusUpdate;
				if (cmi != null)
					cmi.UpdateText();
			}
		}

		public static ContextMenu CreateContextMenu(object owner, string addInTreePath)
		{
			return CreateContextMenu(
					new MenuCreateContext { ActivationMethod = "ContextMenu" },
					owner,
					addInTreePath);
		}

		public static ContextMenu CreateContextMenu(UIElement inputBindingOwner, object owner, string addInTreePath)
		{
			return CreateContextMenu(
					new MenuCreateContext
					{
						InputBindingOwner = inputBindingOwner,
						ActivationMethod = "ContextMenu"
					},
					owner,
					addInTreePath);
		}

		private static ContextMenu CreateContextMenu(MenuCreateContext context, object owner, string addInTreePath)
		{
			IList items = CreateUnexpandedMenuItems(
					context,
					AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
			return CreateContextMenu(items);
		}

		public static ContextMenu ShowContextMenu(UIElement parent, object owner, string addInTreePath)
		{
			ContextMenu menu = new ContextMenu();
			menu.ItemsSource = CreateMenuItems(menu, owner, addInTreePath, "ContextMenu");
			menu.PlacementTarget = parent;
			menu.IsOpen = true;
			return menu;
		}

		internal static ContextMenu CreateContextMenu(IList subItems)
		{
			var contextMenu = new ContextMenu()
			{
				ItemsSource = new object[1]
			};
			contextMenu.Opened += (sender, args) =>
			{
				contextMenu.ItemsSource = ExpandMenuBuilders(subItems, true);
				args.Handled = true;
			};
			return contextMenu;
		}

		public static IList CreateMenuItems(UIElement inputBindingOwner, object owner, string addInTreePath, string activationMethod = null, bool immediatelyExpandMenuBuildersForShortcuts = false)
		{
			var addinTreeMenuItemDescriptors = AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false);

			IList items = CreateUnexpandedMenuItems(
					new MenuCreateContext
					{
						InputBindingOwner = inputBindingOwner,
						ActivationMethod = activationMethod,
						ImmediatelyExpandMenuBuildersForShortcuts = immediatelyExpandMenuBuildersForShortcuts
					},
					addinTreeMenuItemDescriptors
					);
			return ExpandMenuBuilders(items, false);
		}

		private sealed class MenuItemBuilderPlaceholder
		{
			private readonly IMenuItemBuilder builder;
			private readonly Codon codon;
			private readonly object caller;

			public MenuItemBuilderPlaceholder(IMenuItemBuilder builder, Codon codon, object caller)
			{
				this.builder = builder;
				this.codon = codon;
				this.caller = caller;
			}

			public IEnumerable<object> BuildItems()
			{
				return builder.BuildItems(codon, caller);
			}
		}

		internal static IList CreateUnexpandedMenuItems(MenuCreateContext context, IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			if (descriptors != null)
			{
				foreach (MenuItemDescriptor descriptor in descriptors)
				{
					result.Add(CreateMenuItemFromDescriptor(context, descriptor));
				}
			}
			return result;
		}

		private static IList ExpandMenuBuilders(ICollection input, bool addDummyEntryIfMenuEmpty)
		{
			List<object> result = new List<object>(input.Count);
			foreach (object o in input)
			{
				MenuItemBuilderPlaceholder p = o as MenuItemBuilderPlaceholder;
				if (p != null)
				{
					IEnumerable<object> c = p.BuildItems();
					if (c != null)
						result.AddRange(c);
				}
				else
				{
					result.Add(o);
					IStatusUpdate statusUpdate = o as IStatusUpdate;
					if (statusUpdate != null)
					{
						statusUpdate.UpdateStatus();
						statusUpdate.UpdateText();
					}
				}
			}
			if (addDummyEntryIfMenuEmpty && result.Count == 0)
			{
				result.Add(new MenuItem { Header = "(empty menu)", IsEnabled = false });
			}
			return result;
		}

		private static object CreateMenuItemFromDescriptor(MenuCreateContext context, MenuItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Command";

			switch (type)
			{
				case "Separator":
					return new ConditionalSeparator(codon, descriptor.Parameter, false, descriptor.Conditions);

				case "CheckBox":
					return new MenuCheckBox(context.InputBindingOwner, codon, descriptor.Parameter, descriptor.Conditions);

				case "Item":
				case "Command":
					return new MenuCommand(context.InputBindingOwner, codon, descriptor.Parameter, context.ActivationMethod, descriptor.Conditions);

				case "Menu":
					var item = new CoreMenuItem(codon, descriptor.Parameter, descriptor.Conditions)
					{
						ItemsSource = new object[1],
						_setEnabled = true
					};
					var subItems = CreateUnexpandedMenuItems(context, descriptor.SubItems);
					item.SubmenuOpened += (sender, args) =>
					{
						item.ItemsSource = ExpandMenuBuilders(subItems, true);
						args.Handled = true;
					};
					if (context.ImmediatelyExpandMenuBuildersForShortcuts)
						ExpandMenuBuilders(subItems, false);
					return item;

				case "Builder":
					var builderObj = codon.AddIn.CreateObject(codon.Properties["class"]);
					var builder = builderObj as IMenuItemBuilder;
					if (builderObj == null)
						throw new NotSupportedException("Menu item builder " + codon.Properties["class"] + " is unkown. Please check if class name is misspelled.");
					if (builder == null)
						throw new NotSupportedException("Menu item builder " + codon.Properties["class"] + " does not implement IMenuItemBuilder");
					return new MenuItemBuilderPlaceholder(builder, descriptor.Codon, descriptor.Parameter);

				default:
					throw new NotSupportedException("unsupported menu item type : " + type);
			}
		}

		/// <summary>
		/// Converts from the Windows-Forms style label format (accessor key marked with '&amp;')
		/// to a WPF label format (accessor key marked with '_').
		/// </summary>
		public static string ConvertLabel(string label)
		{
			return label.Replace("_", "__").Replace("&", "_");
		}

		/// <summary>
		/// Creates an KeyGesture for a shortcut.
		/// </summary>
		public static KeyGesture ParseShortcut(string text)
		{
			return (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString(text.Replace('|', '+'));
		}

		public static string GetDisplayStringForShortcut(KeyGesture kg)
		{
			string old = kg.GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
			string text = KeyCodeConversion.KeyToUnicode(kg.Key).ToString();
			if (text != null && !text.Any(ch => char.IsWhiteSpace(ch)))
			{
				if ((kg.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
					text = StringParser.Format("${res:Global.Shortcuts.Alt}+{0}", text);
				if ((kg.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
					text = StringParser.Format("${res:Global.Shortcuts.Shift}+{0}", text);
				if ((kg.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
					text = StringParser.Format("${res:Global.Shortcuts.Ctrl}+{0}", text);
				if ((kg.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
					text = StringParser.Format("${res:Global.Shortcuts.Win}+{0}", text);
				return text;
			}
			return old;
		}
	}

	internal static class KeyCodeConversion
	{
		[DllImport("user32.dll")]
		private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[]
																																lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
																																int cchBuff, uint wFlags, IntPtr dwhkl);

		[DllImport("user32.dll")]
		private static extern bool GetKeyboardState(byte[] pbKeyState);

		[DllImport("user32.dll")]
		private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

		[DllImport("user32.dll")]
		private static extern IntPtr GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll")]
		private static extern int MapVirtualKey(uint uCode, uint uMapType);

		/*

/// <remarks>Only works with Windows.Forms.Keys. The WPF Key enum seems to be horribly distorted!</remarks>
public static string KeyToUnicode(WinForms.Keys key)
{
	StringBuilder sb = new StringBuilder(256);
	IntPtr hkl = GetKeyboardLayout(0);

	uint scanCode = MapVirtualKeyEx((uint)key, 0, hkl);
	if (scanCode < 1) return null;

	ClearKeyboardBuffer(hkl);
	int len = ToUnicodeEx((uint)key, scanCode, new byte[256], sb, sb.Capacity, 0, hkl);
	if (len > 0)
		return sb.ToString(0, len).ToUpper();

	ClearKeyboardBuffer(hkl);
	return null;
}

private static void ClearKeyboardBuffer(IntPtr hkl)
{
	StringBuilder sb = new StringBuilder(10);
	uint key = (uint)WinForms.Keys.Space;
	int rc;
	do
	{
		rc = ToUnicodeEx(key, MapVirtualKeyEx(key, 0, hkl), new byte[256], sb, sb.Capacity, 0, hkl);
	} while (rc < 0);
}

		public static WinForms.Keys ToKeys(this Key key)
{
	WinForms.Keys result;
	if (Enum.TryParse(key.ToString(), true, out result))
		return result;
	return WinForms.Keys.None;
}

*/

		public static char KeyToUnicode(this Key keyData)
		{
			// 2 is used to translate into an unshifted character value
			int nonVirtualKey = MapVirtualKey((uint)keyData, 2);

			char mappedChar = Convert.ToChar(nonVirtualKey);

			return mappedChar;
		}
	}
}