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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	/// <summary>
	/// This class is able to generate a GUI definition out of a XML file.
	/// </summary>
	[Obsolete("XML Forms are obsolete")]
	public class XmlLoader
	{
		private Dictionary<string, Control> controlDictionary = new Dictionary<string, Control>();
		private object customizationObject;

		// required for the accept/cancel button/tooltips workaround
		private Form mainForm = null;

		private Hashtable tooltips = new Hashtable();
		private string acceptButtonName = String.Empty;
		private string cancelButtonName = String.Empty;

		private IStringValueFilter stringValueFilter = null;
		private IObjectCreator objectCreator = new DefaultObjectCreator();
		private IPropertyValueCreator propertyValueCreator = null;

		private static readonly Regex propertySet = new Regex(@"(?<Property>[\w]+)\s*=\s*(?<Value>[\w\d]+)", RegexOptions.Compiled);

		/// <summary>
		/// Gets the ControlDictionary for this XmlLoader.
		/// </summary>
		public Dictionary<string, Control> ControlDictionary
		{
			get
			{
				return controlDictionary;
			}
		}

		/// <summary>
		/// Gets/Sets the IStringValueFilter, could be set to <code>null</code>
		/// (for not filtering the values).
		/// </summary>
		public IStringValueFilter StringValueFilter
		{
			get
			{
				return stringValueFilter;
			}
			set
			{
				stringValueFilter = value;
			}
		}

		/// <summary>
		/// Gets/Sets the IObjectCreator, could NOT be set to <code>null</code>.
		/// </summary>
		public IObjectCreator ObjectCreator
		{
			get
			{
				return objectCreator;
			}
			set
			{
				if (value == null)
				{
					throw new System.ArgumentNullException();
				}
				objectCreator = value;
			}
		}

		/// <summary>
		/// Gets/Sets the IPropertyValueCreator, could be set to <code>null</code>.
		/// </summary>
		public IPropertyValueCreator PropertyValueCreator
		{
			get
			{
				return propertyValueCreator;
			}
			set
			{
				propertyValueCreator = value;
			}
		}

		/// <summary>
		/// Creates a new instance of XmlLoader.
		/// </summary>
		public XmlLoader()
		{
		}

		public T Get<T>(string name) where T : System.Windows.Forms.Control
		{
			string key = name + typeof(T).Name;
			if (!ControlDictionary.ContainsKey(key))
			{
				throw new System.ArgumentException("Control " + key + " not found!", "name");
			}

			return ControlDictionary[key] as T;
		}

		#region Load/Create functions

		/// <summary>
		/// Loads the XML definition from fileName and sets creates the control.
		/// </summary>
		/// <param name="fileName">
		/// The filename of the XML definition file to load.
		/// </param>
		public object CreateObjectFromFileDefinition(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);

			XmlElement el = doc.DocumentElement;

			if (doc.DocumentElement.Attributes["version"] != null)
			{
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}

			customizationObject = objectCreator.CreateObject(XmlConvert.DecodeName(el.Name), el);

			SetUpObject(customizationObject, el);
			return customizationObject;
		}

		/// <summary>
		/// Loads the XML definition from a xml definition and sets creates the control.
		/// </summary>
		public object CreateObjectFromXmlDefinition(string xmlContent)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContent);

			XmlElement el = doc.DocumentElement;
			if (doc.DocumentElement.Attributes["version"] != null)
			{
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}

			customizationObject = objectCreator.CreateObject(XmlConvert.DecodeName(el.Name), el);

			SetUpObject(customizationObject, el);
			return customizationObject;
		}

		/// <summary>
		/// Loads the XML definition from fileName and sets creates the control.
		/// </summary>
		/// <param name="customizationObject">
		/// The object to customize. (should be a control or form)
		/// This is object, because this class may be extended later.
		/// </param>
		/// <param name="fileName">
		/// The filename of the XML definition file to load.
		/// </param>
		public void LoadObjectFromFileDefinition(object customizationObject, string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			LoadObjectFromXmlDocument(customizationObject, doc);
		}

		public void LoadObjectFromStream(object customizationObject, Stream stream)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			LoadObjectFromXmlDocument(customizationObject, doc);
		}

		public void LoadObjectFromXmlDocument(object customizationObject, XmlDocument doc)
		{
			this.customizationObject = customizationObject;

			XmlElement el = doc.DocumentElement;
			if (doc.DocumentElement.Attributes["version"] != null)
			{
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}

			SetUpObject(customizationObject, el);

			// little HACK to set the Accept & Cancel Button
			if (customizationObject is Form)
			{
				Form mainForm = (Form)customizationObject;
				if (acceptButtonName != null && acceptButtonName.Length > 0)
				{
					mainForm.AcceptButton = (Button)controlDictionary[acceptButtonName];
				}
				if (cancelButtonName != null && cancelButtonName.Length > 0)
				{
					mainForm.CancelButton = (Button)controlDictionary[cancelButtonName];
				}
			}
			// little HACK to set the Tooltips...
			if (tooltips.Count > 0)
			{
				ToolTip toolTip = new ToolTip();
				foreach (DictionaryEntry entry in tooltips)
				{
					toolTip.SetToolTip((Control)entry.Key, entry.Value.ToString());
				}
			}
		}

		/// <summary>
		/// Loads the XML definition from a xml definition and sets creates the control.
		/// </summary>
		public void LoadObjectFromXmlDefinition(string xmlContent)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContent);

			XmlElement el = doc.DocumentElement;
			if (doc.DocumentElement.Attributes["version"] != null)
			{
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}
			SetUpObject(customizationObject, doc.DocumentElement);
		}

		#endregion Load/Create functions

		/// <summary>
		/// Sets the properties of an object <code>currentObject</code> to the
		/// contents of the xml element <code>element</code>
		/// </summary>
		private void SetUpObject(object currentObject, XmlElement element)
		{
			foreach (XmlNode subNode in element.ChildNodes)
			{
				if (subNode is XmlElement)
				{
					XmlElement subElement = (XmlElement)subNode;
					SetAttributes(currentObject, subElement);
				}
			}

			if (currentObject is Control)
			{
				((Control)currentObject).ResumeLayout(false);
			}
		}

		private string acceptButton = "";
		private string cancelButton = "";

		/// <summary>
		/// Sets a property called propertyName in object <code>o</code> to <code>val</code>. This method performs
		/// all neccessary casts.
		/// </summary>
		private void SetValue(object o, string propertyName, string val)
		{
			try
			{
				PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);
				if (propertyName == "AcceptButton")
				{
					this.acceptButton = val.Split(' ')[0];
					return;
				}

				if (propertyName == "CancelButton")
				{
					this.cancelButton = val.Split(' ')[0];
					return;
				}

				if (propertyName == "ToolTip")
				{
					tooltips[o] = val;
					return;
				}

				if (val.StartsWith("{") && val.EndsWith("}"))
				{
					val = val.Substring(1, val.Length - 2);
					object propertyObject = null;
					if (propertyInfo.CanWrite)
					{
						Type type = objectCreator.GetType(propertyInfo.PropertyType.FullName);
						propertyObject = type.Assembly.CreateInstance(propertyInfo.PropertyType.FullName);
					}
					else
					{
						propertyObject = propertyInfo.GetValue(o, null);
					}

					Match match = propertySet.Match(val);
					while (true)
					{
						if (!match.Success)
						{
							break;
						}
						SetValue(propertyObject, match.Result("${Property}"), match.Result("${Value}"));
						match = match.NextMatch();
					}

					if (propertyInfo.CanWrite)
					{
						propertyInfo.SetValue(o, propertyObject, null);
					}
				}
				else if (propertyInfo.PropertyType.IsEnum)
				{
					propertyInfo.SetValue(o, Enum.Parse(propertyInfo.PropertyType, val), null);
				}
				else if (propertyInfo.PropertyType == typeof(Color))
				{
					string color = val.Substring(val.IndexOf('[') + 1).Replace("]", "");
					string[] argb = color.Split(',', '=');
					if (argb.Length > 1)
					{
						propertyInfo.SetValue(o, Color.FromArgb(Int32.Parse(argb[1]), Int32.Parse(argb[3]), Int32.Parse(argb[5]), Int32.Parse(argb[7])), null);
					}
					else
					{
						propertyInfo.SetValue(o, Color.FromName(color), null);
					}
				}
				else
				{
					if (val.Length > 0)
					{
						TypeConverter conv = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
						propertyInfo.SetValue(o, conv.ConvertFromInvariantString(val), null);
					}
				}
			}
			catch (Exception e)
			{
				throw new ApplicationException("error while setting property " + propertyName + " of object " + o.ToString() + " to value '" + val + "'", e);
			}
		}

		/// <summary>
		/// Sets all properties in the object <code>o</code> to the xml element <code>el</code> defined properties.
		/// </summary>
		private void SetAttributes(object o, XmlElement el)
		{
			if (el.Name == "AcceptButton")
			{
				mainForm = (Form)o;
				acceptButtonName = el.Attributes["value"].InnerText.Split(' ')[0];
				return;
			}

			if (el.Name == "CancelButton")
			{
				mainForm = (Form)o;
				cancelButtonName = el.Attributes["value"].InnerText.Split(' ')[0];
				return;
			}

			if (el.Name == "ToolTip")
			{
				string val = el.Attributes["value"].InnerText;
				tooltips[o] = stringValueFilter != null ? stringValueFilter.GetFilteredValue(val) : val;
				return;
			}

			if (el.Attributes["value"] != null)
			{
				string val = el.Attributes["value"].InnerText;
				try
				{
					SetValue(o, el.Name, stringValueFilter != null ? stringValueFilter.GetFilteredValue(val) : val);
				}
				catch (Exception) { }
			}
			else if (el.Attributes["event"] != null)
			{
				try
				{
					EventInfo eventInfo = o.GetType().GetEvent(el.Name);
					eventInfo.AddEventHandler(o, Delegate.CreateDelegate(eventInfo.EventHandlerType, customizationObject, el.Attributes["event"].InnerText));
				}
				catch (Exception) { }
			}
			else
			{
				PropertyInfo propertyInfo = o.GetType().GetProperty(el.Name);
				object pv = propertyInfo.GetValue(o, null);
				if (pv is IList)
				{
					foreach (XmlNode subNode in el.ChildNodes)
					{
						if (subNode is XmlElement)
						{
							XmlElement subElement = (XmlElement)subNode;
							object collectionObject = objectCreator.CreateObject(XmlConvert.DecodeName(subElement.Name), subElement);
							if (collectionObject == null)
							{
								continue;
							}
							if (collectionObject is IComponent)
							{
								string name = null;
								if (subElement["Name"] != null &&
										subElement["Name"].Attributes["value"] != null)
								{
									name = subElement["Name"].Attributes["value"].InnerText;
								}

								if (name == null || name.Length == 0)
								{
									name = "CreatedObject" + num++;
								}
								//								collectionObject =  host.CreateComponent(collectionObject.GetType(), name);
							}

							SetUpObject(collectionObject, subElement);

							if (collectionObject is Control)
							{
								string name = ((Control)collectionObject).Name;
								if (name != null && name.Length > 0)
								{
									ControlDictionary[name] = (Control)collectionObject;
								}
							}

							if (collectionObject != null)
							{
								((IList)pv).Add(collectionObject);
							}
						}
					}
				}
				else
				{
					object propertyObject = objectCreator.CreateObject(o.GetType().GetProperty(el.Name).PropertyType.Name, el);
					if (propertyObject is IComponent)
					{
						PropertyInfo pInfo = propertyObject.GetType().GetProperty("Name");
						string name = null;
						if (el["Name"] != null &&
								el["Name"].Attributes["value"] != null)
						{
							name = el["Name"].Attributes["value"].InnerText;
						}

						if (name == null || name.Length == 0)
						{
							name = "CreatedObject" + num++;
						}
						propertyObject = objectCreator.CreateObject(name, el);
					}
					SetUpObject(propertyObject, el);
					propertyInfo.SetValue(o, propertyObject, null);
				}
			}
		}

		private int num = 0;
	}
}