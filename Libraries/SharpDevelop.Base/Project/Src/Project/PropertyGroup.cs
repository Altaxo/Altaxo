﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Contains a set of MSBuild properties.
	/// </summary>
	public sealed class PropertyGroup : IEnumerable<KeyValuePair<string, string>>, ICloneable
	{
		// TODO: Isn't MSBuild case-insensitive ???
		// TODO: merge both dictionaries into one using a custom class
		Dictionary<string, bool>   isGuardedProperty   = new Dictionary<string, bool>();
		Dictionary<string, string> properties          = new Dictionary<string, string>();
		
		/// <summary>
		/// Gets the number of properties in this group.
		/// </summary>
		public int PropertyCount {
			get {
				return properties.Count;
			}
		}
		
		public string this[string property] {
			get {
				return Get(property, String.Empty);
			}
			set {
				Set(property, String.Empty, value);
			}
		}
		
		internal Dictionary<string, bool> IsGuardedProperty {
			get {
				return isGuardedProperty;
			}
		}
		
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return properties.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return properties.GetEnumerator();
		}
		
		public PropertyGroup Clone()
		{
			PropertyGroup pg = new PropertyGroup();
			pg.Merge(this);
			return pg;
		}
		
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		
		public T Get<T>(string property, T defaultValue)
		{
			if (!properties.ContainsKey(property)) {
				return defaultValue;
			}
			TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
			try {
				return (T)c.ConvertFromInvariantString(properties[property]);
			} catch (FormatException ex) {
				ICSharpCode.Core.LoggingService.Warn("Cannot get property " + property, ex);
				return defaultValue;
			}
		}
		
		public void Set<T>(string property, T defaultValue, T value)
		{
			if (value == null || value.Equals(defaultValue)) {
				properties.Remove(property);
				return;
			}
			
			if (!properties.ContainsKey(property)) {
				properties.Add(property, value.ToString());
			} else {
				properties[property] = value.ToString();
			}
		}
		
		public void SetIsGuarded(string property, bool isGuarded)
		{
			isGuardedProperty[property] = isGuarded;
		}
		
		public bool IsSet(string property)
		{
			return properties.ContainsKey(property);
		}
		
		public bool Remove(string property)
		{
			return properties.Remove(property);
		}
		
		
		public void Set(string property, object value)
		{
			properties[property] = value.ToString();
		}
		
		public void Merge(PropertyGroup group)
		{
			foreach (KeyValuePair<string, string> entry in group.properties) {
				properties[entry.Key] = entry.Value;
			}
			foreach (KeyValuePair<string, bool> entry in group.isGuardedProperty) {
				isGuardedProperty[entry.Key] = entry.Value;
			}
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[PropertyGroup:Properties={");
			foreach (KeyValuePair<string, string> entry in properties) {
				sb.Append(entry.Key);
				sb.Append("=");
				sb.Append(entry.Value);
				sb.Append(",");
			}
			sb.Append("}]");
			return sb.ToString();
		}
		
		internal static void ReadProperties(XmlReader reader, PropertyGroup properties, string endElement)
		{
			if (reader.IsEmptyElement) {
				return;
			}
			while (reader.Read()) {
			reLoop:
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == endElement) {
							return;
						}
						break;
					case XmlNodeType.Element:
						string propertyName = reader.LocalName;
						
						properties.isGuardedProperty[propertyName] = reader.HasAttributes;
						
						if (reader.IsEmptyElement) {
							properties[propertyName] = null;
						} else {
							reader.Read();
							if (reader.NodeType != XmlNodeType.Text) {
								properties[propertyName] = null;
								goto reLoop;
							}
							properties[propertyName] = ProjectItem.MSBuildUnescape(reader.Value.Trim());
						}
						break;
				}
			}
		}
		
		internal void WriteProperties(XmlWriter writer)
		{
			WriteProperties(writer, null, properties, isGuardedProperty);
		}
		
		/// <summary>
		/// Writes a set of properties into the XmlWriter.
		/// A &lt;PropertyGroup&gt; tag is created around the properties
		/// if there are more than 0 properties. This PropertyGroup has the specified condition,
		/// or no condition if condition is string.Empty.
		/// <b>If condition is null, no &lt;PropertyGroup&gt; tag is created!!</b>
		/// </summary>
		internal static void WriteProperties(XmlWriter writer,
		                                     string condition,
		                                     IEnumerable<KeyValuePair<string, string>> properties,
		                                     Dictionary<string, bool> isGuardedProperty)
		{
			bool first = true;
			foreach (KeyValuePair<string, string> entry in properties) {
				if (first) {
					first = false;
					if (condition != null) {
						writer.WriteStartElement("PropertyGroup");
						if (condition.Length > 0) {
							writer.WriteAttributeString("Condition", condition);
						}
					}
				}
				writer.WriteStartElement(entry.Key);
				
				if (isGuardedProperty.ContainsKey(entry.Key) && isGuardedProperty[entry.Key]) {
					writer.WriteAttributeString("Condition", " '$(" + entry.Key + ")' == '' ");
				}
				
				if (entry.Value != null) {
					writer.WriteValue(ProjectItem.MSBuildEscape(entry.Value));
				}
				writer.WriteEndElement();
			}
			if (!first && condition != null) {
				// a property group was created, so close it:
				writer.WriteEndElement();
			}
		}
	}
}
