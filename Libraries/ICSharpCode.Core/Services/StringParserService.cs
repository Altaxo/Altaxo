// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// this class parses internal ${xyz} tags of sd.
	/// All environment variables are avaible under the name env.[NAME]
	/// where [NAME] represents the string under which it is avaiable in
	/// the environment.
	/// </summary>
	public class StringParserService : AbstractService
	{
		PropertyDictionary properties         = new PropertyDictionary();
		Hashtable          stringTagProviders = new Hashtable();
		
		public PropertyDictionary Properties {
			get {
				return properties;
			}
		}
		
		public StringParserService()
		{
			IDictionary variables = Environment.GetEnvironmentVariables();
			foreach (string name in variables.Keys) {
				properties.Add("env:" + name, (string)variables[name]);
			}
		}
		
		public string Parse(string input)
		{
			return Parse(input, null);
		}
		
		/// <summary>
		/// Parses an array and replaces the elements
		/// </summary>
		public void Parse(ref string[] inputs)
		{
			for (int i = inputs.GetLowerBound(0); i <= inputs.GetUpperBound(0); ++i) {
				inputs[i] = Parse(inputs[i], null);
			}
		}
		
		public void RegisterStringTagProvider(IStringTagProvider tagProvider)
		{
			foreach (string str in tagProvider.Tags) {
				stringTagProviders[str.ToUpper()] = tagProvider;
			}
		}
			
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
		public string Parse(string input, string[,] customTags)
		{
			string output = input;
			if (input != null) {
				const string pattern = @"\$\{([^\}]*)\}";
				foreach (Match m in Regex.Matches(input, pattern)) {
					if (m.Length > 0) {
						string token         = m.ToString();
						string propertyName  = m.Groups[1].Captures[0].Value;
						string propertyValue = null;
						switch (propertyName.ToUpper()) {
							case "DATE": // current date
								propertyValue = DateTime.Today.ToShortDateString();
								break;
							case "TIME": // current time
								propertyValue = DateTime.Now.ToShortTimeString();
								break;
							default:
								propertyValue = null;
								if (customTags != null) {
									for (int j = 0; j < customTags.GetLength(0); ++j) {
										if (propertyName.ToUpper() == customTags[j, 0].ToUpper()) {
											propertyValue = customTags[j, 1];
											break;
										}
									}
								}
								
								if (propertyValue == null) {
									propertyValue = properties[propertyName.ToUpper()];
								}
								
								if (propertyValue == null) {
									IStringTagProvider stringTagProvider = stringTagProviders[propertyName.ToUpper()] as IStringTagProvider;
									if (stringTagProvider != null) {
										propertyValue = stringTagProvider.Convert(propertyName.ToUpper());
									}
								}
								
								if (propertyValue == null) {
									int k = propertyName.IndexOf(':');
									if (k > 0) {
										switch (propertyName.Substring(0, k).ToUpper()) {
											case "RES":
												IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
												if (resourceService != null) {
														propertyValue = Parse(resourceService.GetString(propertyName.Substring(k + 1)), customTags);
												}
												break;
											case "PROPERTY":
												PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
												propertyValue = propertyService.GetProperty(propertyName.Substring(k + 1)).ToString();
												break;
										}
									}
								}
								break;
						}
						if (propertyValue != null) {
							output = output.Replace(token, propertyValue);
						}
					}
				}
			}
			return output;
		}
	}
	
	public class PropertyDictionary : DictionaryBase
	{
		/// <summary>
		/// Maintains a list of the property names that are readonly.
		/// </summary>
		StringCollection readOnlyProperties = new StringCollection();
		
		/// <summary>
		/// Adds a property that cannot be changed.
		/// </summary>
		/// <remarks>
		/// Properties added with this method can never be changed.  Note that
		/// they are removed if the <c>Clear</c> method is called.
		/// </remarks>
		/// <param name="name">Name of property</param>
		/// <param name="value">Value of property</param>
		public void AddReadOnly(string name, string value) 
		{
			if (!readOnlyProperties.Contains(name)) {
				readOnlyProperties.Add(name);
				Dictionary.Add(name, value);
			}
		}
		
		/// <summary>
		/// Adds a property to the collection.
		/// </summary>
		/// <param name="name">Name of property</param>
		/// <param name="value">Value of property</param>
		public void Add(string name, string value) 
		{
			if (!readOnlyProperties.Contains(name)) {
				Dictionary.Add(name, value);
			}
		}
		
		public string this[string name] {
			get { 
				return (string)Dictionary[(object)name.ToUpper()];
			}
			set {
				Dictionary[name.ToUpper()] = value;
			}
		}
		
		protected override void OnClear() 
		{
			readOnlyProperties.Clear();
		}
	}
}
