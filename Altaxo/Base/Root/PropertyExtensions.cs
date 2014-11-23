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

using Altaxo.Main.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo
{
	/// <summary>
	/// Helper class to get the property bags in the hierarchy as used in the Altaxo project, and to retrieve property values using this hierarchy.
	/// </summary>
	public static class PropertyExtensions
	{
		/// <summary>
		/// Gets the property bags in the hierarchy. The first bag in the enumeration is the bag that owns the <paramref name="owner"/>. Then the bags of the folders
		/// in which the owner is located, its parent folders, and then UserSettings, ApplicationSettings, and BuiltinSettings follow.
		/// </summary>
		/// <param name="owner">The owner of a property bag with which to start the enumeration.</param>
		/// <returns>Enumeration of the property bags in the project hierarchy.</returns>
		public static IEnumerable<PropertyBagWithInformation> GetPropertyBags(this IPropertyBagOwner owner)
		{
			if (!(owner is ProjectFolderPropertyDocument)) // Project folder bags are handled further down
			{
				var bagInfo = new PropertyBagInformation(owner.GetType().Name, PropertyLevel.Document, owner.GetType());
				yield return new PropertyBagWithInformation(bagInfo, owner.PropertyBagNotNull);
			}

			var namedOwner = owner as Main.INameOwner;
			var proj = Current.Project;
			ProjectFolderPropertyDocument bag;
			if (null != namedOwner && !string.IsNullOrEmpty(namedOwner.Name))
			{
				var folder = Main.ProjectFolder.GetFolderPart(namedOwner.Name);
				while (!string.IsNullOrEmpty(folder))
				{
					if (proj.ProjectFolderProperties.TryGetValue(folder, out bag) && bag.PropertyBag != null)
					{
						var bagInfo = new PropertyBagInformation(string.Format("Folder \"{0}\"", folder), PropertyLevel.ProjectFolder);
						yield return new PropertyBagWithInformation(bagInfo, bag.PropertyBag);
					}
					folder = Main.ProjectFolder.GetFoldersParentFolder(folder);
				}
			}
			// now return the project's property bag even for unnamed items
			if (proj.ProjectFolderProperties.TryGetValue(string.Empty, out bag) && bag.PropertyBag != null)
			{
				var bagInfo = new PropertyBagInformation("Project (RootFolder)", PropertyLevel.Project);
				yield return new PropertyBagWithInformation(bagInfo, bag.PropertyBag);
			}

			// and now the user's settings
			{
				var bagInfo = new PropertyBagInformation("UserSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.UserSettings);
			}

			// then the application settings
			{
				var bagInfo = new PropertyBagInformation("ApplicationSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.ApplicationSettings);
			}

			// and finally the built-in settings
			{
				var bagInfo = new PropertyBagInformation("BuiltinSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.BuiltinSettings);
			}
		}

		/// <summary>
		/// Gets the property bags in the hierarchy. The first bag in the enumeration is the bag that is a project folder bag belonging to the provided folder name. Then the bags in its
		/// parent folders, and then UserSettings, ApplicationSettings, and BuiltinSettings follow.
		/// </summary>
		/// <param name="owner">The owner of a property bag with which to start the enumeration.</param>
		/// <returns>Enumeration of the property bags in the project hierarchy.</returns>
		public static IEnumerable<PropertyBagWithInformation> GetPropertyBagsStartingFromFolder(string folder)
		{
			var proj = Current.Project;
			ProjectFolderPropertyDocument bag;
			while (!string.IsNullOrEmpty(folder))
			{
				if (proj.ProjectFolderProperties.TryGetValue(folder, out bag) && bag.PropertyBag != null)
				{
					var bagInfo = new PropertyBagInformation(string.Format("Folder \"{0}\"", folder), PropertyLevel.ProjectFolder);
					yield return new PropertyBagWithInformation(bagInfo, bag.PropertyBag);
				}
				folder = Main.ProjectFolder.GetFoldersParentFolder(folder);
			}

			// now return the project's property bag even for unnamed items
			if (proj.ProjectFolderProperties.TryGetValue(string.Empty, out bag) && bag.PropertyBag != null)
			{
				var bagInfo = new PropertyBagInformation("Project (RootFolder)", PropertyLevel.Project);
				yield return new PropertyBagWithInformation(bagInfo, bag.PropertyBag);
			}

			// and now the user's settings
			{
				var bagInfo = new PropertyBagInformation("UserSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.UserSettings);
			}

			// then the application settings
			{
				var bagInfo = new PropertyBagInformation("ApplicationSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.ApplicationSettings);
			}

			// and finally the built-in settings
			{
				var bagInfo = new PropertyBagInformation("BuiltinSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.BuiltinSettings);
			}
		}

		/// <summary>
		/// Gets the property bags 'UserSettings, 'ApplicationSettings' and 'BuiltinSettings' along with the bag information.
		/// </summary>
		/// <returns>Property bags 'UserSettings, 'ApplicationSettings' and 'BuiltinSettings' along with the bag information.</returns>
		public static IEnumerable<PropertyBagWithInformation> GetPropertyBagsStartingFromUserSettings()
		{
			// and now the user's settings
			{
				var bagInfo = new PropertyBagInformation("UserSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.UserSettings);
			}
			// then the application settings
			{
				var bagInfo = new PropertyBagInformation("ApplicationSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.ApplicationSettings);
			}

			// and finally the built-in settings
			{
				var bagInfo = new PropertyBagInformation("BuiltinSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.BuiltinSettings);
			}
		}

		/// <summary>
		/// Gets the property bags 'ApplicationSettings' and 'BuiltinSettings' along with the bag information.
		/// </summary>
		/// <returns>Property bags 'ApplicationSettings' and 'BuiltinSettings' along with the bag information.</returns>
		public static IEnumerable<PropertyBagWithInformation> GetPropertyBagsStartingFromApplicationSettings()
		{
			// then the application settings
			{
				var bagInfo = new PropertyBagInformation("ApplicationSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.ApplicationSettings);
			}

			// and finally the built-in settings
			{
				var bagInfo = new PropertyBagInformation("BuiltinSettings", PropertyLevel.Application);
				yield return new PropertyBagWithInformation(bagInfo, Current.PropertyService.BuiltinSettings);
			}
		}

		/// <summary>
		/// Gets the property bags in the hierarchal order. The first bag in the enumeration is the bag that owns the <paramref name="owner"/>. Then the bags of the folders
		/// in which the owner is located, its parent folders, and then UserSettings, ApplicationSettings, and BuiltinSettings follow.
		/// </summary>
		/// <param name="owner">The owner of a property bag with which to start the enumeration.</param>
		/// <returns>Property bags in the project hierarchy wrapped in a  <see cref="PropertyHierarchy"/>.</returns>
		public static PropertyHierarchy GetPropertyHierarchy(this IPropertyBagOwner owner)
		{
			return new PropertyHierarchy(GetPropertyBags(owner));
		}

		/// <summary>
		/// Gets the property bags in the hierarchal order. The first bag in the enumeration is the bag is in the provided project <paramref name="folder"/>. Then the bags of
		/// its parent folders, and then UserSettings, ApplicationSettings, and BuiltinSettings follow.
		/// </summary>
		/// <param name="folder">The project folder with which to start the enumeration.</param>
		/// <returns>Property bags in the project hierarchy wrapped in a  <see cref="PropertyHierarchy"/>.</returns>
		public static PropertyHierarchy GetPropertyContextOfProjectFolder(string folder)
		{
			return new PropertyHierarchy(GetPropertyBagsStartingFromFolder(folder));
		}

		/// <summary>
		/// Gets the property context of the document node. If the provided node is not implementing <see cref="IPropertyBagOwner"/>, the parent nodes of this node are searched,
		/// until a node which implements  <see cref="IPropertyBagOwner"/> is found.
		/// </summary>
		/// <param name="node">The node for which the property context is retrieved.</param>
		/// <returns>A <see cref="PropertyHierarchy"/> instance representing the property context of the provided document node.</returns>
		public static PropertyHierarchy GetPropertyContext(this Altaxo.Main.IDocumentNode node)
		{
			var owner = node as IPropertyBagOwner;
			if (null == owner)
				owner = Altaxo.Main.DocumentPath.GetRootNodeImplementing<IPropertyBagOwner>(node);

			return new PropertyHierarchy(GetPropertyBags(owner));
		}

		/// <summary>
		/// Gets the property bags 'ApplicationSettings' 'BuiltinSettings' and 'UserSettings' stored in a <see cref="PropertyHierarchy"/>.
		/// </summary>
		/// <returns>Property bags 'ApplicationSettings' 'BuiltinSettings'  and 'UserSettings'  wrapped in a <see cref="PropertyHierarchy"/>.</returns>
		public static PropertyHierarchy GetPropertyHierarchyStartingFromUserSettings()
		{
			return new PropertyHierarchy(GetPropertyBagsStartingFromUserSettings());
		}

		/// <summary>
		/// Gets the property bags 'ApplicationSettings' and 'BuiltinSettings' stored in a <see cref="PropertyHierarchy"/>.
		/// </summary>
		/// <returns>Property bags 'ApplicationSettings' and 'BuiltinSettings' wrapped in a <see cref="PropertyHierarchy"/>.</returns>
		public static PropertyHierarchy GetPropertyHierarchyStartingFromApplicationSettings()
		{
			return new PropertyHierarchy(GetPropertyBagsStartingFromApplicationSettings());
		}

		/// <summary>
		/// Gets the project folder property documents down the project folder hierarchie.
		/// </summary>
		/// <param name="namedOwner">The named owner. Can be any project item.</param>
		/// <returns>All project folder property documents, starting from the deepest folder level, and ending with the property document in the root folder (if there is any).</returns>
		public static IEnumerable<ProjectFolderPropertyDocument> GetProjectFolderPropertyDocuments(this Main.INameOwner namedOwner)
		{
			var proj = Current.Project;

			ProjectFolderPropertyDocument bag;

			if (null != namedOwner)
			{
				var folder = Main.ProjectFolder.GetFolderPart(namedOwner.Name);
				while (!string.IsNullOrEmpty(folder))
				{
					if (proj.ProjectFolderProperties.TryGetValue(folder, out bag))
					{
						yield return bag;
					}
					folder = Main.ProjectFolder.GetFoldersParentFolder(folder);
				}

				if (proj.ProjectFolderProperties.TryGetValue(Main.ProjectFolder.RootFolderName, out bag))
				{
					yield return bag;
				}
			}
		}

		/// <summary>
		/// Gets the property value either from the application settings or from the builtin settings.
		/// </summary>
		/// <typeparam name="T">Type of the property value to be retrieved.</typeparam>
		/// <param name="p">The property key.</param>
		/// <param name="resultCreationIfNotFound">If the property is not found, a new property value can be created by this procedure. If this value is <c>null</c>, the default
		/// value for this type of property value is returned.</param>
		/// <returns>If the property is found anywhere in the hierarchy of property bags, the property value of the topmost bag that contains the property is returned.
		/// Otherwise, if <paramref name="resultCreationIfNotFound"/> is not null, the result of this procedure is returned. Else the default value of the type of property value is returned.</returns>
		public static T GetPropertyValueStartingFromApplicationSettings<T>(PropertyKey<T> p, Func<T> resultCreationIfNotFound)
		{
			T returnValue; ;
			foreach (var bagTuple in GetPropertyBagsStartingFromApplicationSettings())
			{
				if (bagTuple.Bag.TryGetValue<T>(p, out returnValue))
					return returnValue;
			}

			if (null != resultCreationIfNotFound)
				return resultCreationIfNotFound();
			else
				return default(T);
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <typeparam name="T">Type of the property value to be retrieved.</typeparam>
		/// <param name="owner">The owner of a property bag to start the search for the property. Then the other bags down the hierarchy are also searched for the property.</param>
		/// <param name="p">The property key.</param>
		/// <param name="resultCreationIfNotFound">If the property is not found, a new property value can be created by this procedure. If this value is <c>null</c>, the default
		/// value for this type of property value is returned.</param>
		/// <returns>If the property is found anywhere in the hierarchy of property bags, the property value of the topmost bag that contains the property is returned.
		/// Otherwise, if <paramref name="resultCreationIfNotFound"/> is not null, the result of this procedure is returned. Else the default value of the type of property value is returned.</returns>
		public static T GetPropertyValue<T>(this IPropertyBagOwner owner, PropertyKey<T> p, Func<T> resultCreationIfNotFound)
		{
			T returnValue; ;
			foreach (var bagTuple in GetPropertyBags(owner))
			{
				if (bagTuple.Bag.TryGetValue<T>(p, out returnValue))
					return returnValue;
			}

			if (null != resultCreationIfNotFound)
				return resultCreationIfNotFound();
			else
				return default(T);
		}
	}
}