// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// This Class contains two ResourceManagers, which handle string and image resources
	/// for the application. It do handle localization strings on this level.
	/// </summary>
	public class ResourceService : AbstractService, IResourceService
	{
		readonly static string uiLanguageProperty = "CoreProperties.UILanguage";
		
		readonly static string stringResources  = "StringResources";
		readonly static string imageResources   = "BitmapResources";
		
		static string resourceDirctory;
		
		static ResourceService()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			resourceDirctory = propertyService.DataDirectory + Path.DirectorySeparatorChar + "resources";
		}
		
		Hashtable userStrings = null;
		Hashtable userIcons   = null;
		
		ResourceManager strings = null;
		ResourceManager icon    = null;
		
		Hashtable localStrings = null;
		Hashtable localIcons   = null;
		
		void ChangeProperty(object sender, PropertyEventArgs e)
		{
			if (e.Key == uiLanguageProperty && e.OldValue != e.NewValue) {
			    LoadLanguageResources();
			} 
		}
		void LoadLanguageResources()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string language = propertyService.GetProperty(uiLanguageProperty, Thread.CurrentThread.CurrentUICulture.Name);
			
			localStrings = Load(stringResources, language);
			if (localStrings == null && language.IndexOf('-') > 0) {
				localStrings = Load(stringResources, language.Split(new char[] {'-'})[0]);
			}
			
			localIcons = Load(imageResources, language);
			if (localIcons == null && language.IndexOf('-') > 0) {
				localIcons = Load(imageResources, language.Split(new char[] {'-'})[0]);
			}
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			propertyService.PropertyChanged += new PropertyEventHandler(ChangeProperty);
			
			LoadLanguageResources();
		}
		
		// core service : Can't use Initialize, because all other stuff needs this service before initialize is called.
		public ResourceService()
		{
			strings = new ResourceManager(stringResources, Assembly.GetCallingAssembly());
			icon    = new ResourceManager(imageResources,  Assembly.GetCallingAssembly());
			
			if (System.Configuration.ConfigurationSettings.AppSettings["UserStrings"] != null) {
				userStrings = Load(resourceDirctory +  Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["UserStrings"]);
			}
			if (System.Configuration.ConfigurationSettings.AppSettings["UserIcons"] != null) {
				userIcons   = Load(resourceDirctory +  Path.DirectorySeparatorChar + System.Configuration.ConfigurationSettings.AppSettings["UserIcons"]);
			}
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public Font LoadFont(string fontName, int size)
		{
			return LoadFont(fontName, size, FontStyle.Regular);
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public Font LoadFont(string fontName, int size, FontStyle style)
		{
			try {
				return new Font(fontName, size, style);
			} catch (Exception) {
				return SystemInformation.MenuFont;
			}
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public Font LoadFont(string fontName, int size, GraphicsUnit unit)
		{
			return LoadFont(fontName, size, FontStyle.Regular, unit);
		}
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
		public Font LoadFont(string fontName, int size, FontStyle style, GraphicsUnit unit)
		{
			try {
				return new Font(fontName, size, style, unit);
			} catch (Exception) {
				return SystemInformation.MenuFont;
			}
		}
		
		Hashtable Load(string fileName)
		{
			if (File.Exists(fileName)) {
				Hashtable resources = new Hashtable();
				ResourceReader rr = new ResourceReader(fileName);
				foreach (DictionaryEntry entry in rr) {
					resources.Add(entry.Key, entry.Value);
				}
				rr.Close();
				return resources;
			}
			return null;
		}
		Hashtable Load(string name, string language)
		{
			return Load(resourceDirctory + Path.DirectorySeparatorChar + name + "." + language + ".resources");
			
		}
		
		/// <summary>
		/// Returns a string from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <returns>
		/// The string in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested resource.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public string GetString(string name)
		{
			if (this.userStrings != null && this.userStrings[name] != null) {
				return userStrings[name].ToString();
			}
			if (localStrings != null && localStrings[name] != null) {
				return localStrings[name].ToString();
			}
			
			string s = strings.GetString(name);
			
			if (s == null) {
				throw new ResourceNotFoundException("string >" + name + "<");
			}
			
			return s;
		}
		
		/// <summary>
		/// Returns a icon from the resource database, it handles localization
		/// transparent for the user. In the resource database can be a bitmap
		/// instead of an icon in the dabase. It is converted automatically.
		/// </summary>
		/// <returns>
		/// The icon in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested icon.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public Icon GetIcon(string name)
		{
			object iconobj = null;
			
			if (this.userIcons != null && this.userIcons[name] != null) {
				iconobj = userIcons[name];
			} else  if (localIcons != null && localIcons[name] != null) {
				iconobj = localIcons[name];
			} else {
				iconobj = icon.GetObject(name);
			}
			
			if (iconobj == null) {
				return null;
			}
			
			if (iconobj is Icon) {
				return (Icon)iconobj;
			} else {
				return Icon.FromHandle(((Bitmap)iconobj).GetHicon());
			}
		}
		
		/// <summary>
		/// Returns a bitmap from the resource database, it handles localization
		/// transparent for the user. 
		/// </summary>
		/// <returns>
		/// The bitmap in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public Bitmap GetBitmap(string name)
		{
			if (this.userIcons != null && this.userIcons[name] != null) {
				return (Bitmap)userIcons[name];
			}
			if (localIcons != null && localIcons[name] != null) {
				return (Bitmap)localIcons[name];
			}
			Bitmap b = (Bitmap)icon.GetObject(name);
			Debug.Assert(b != null, "Resource " + name);
			
			return b;
		}
	}
}
