// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.XmlForms {
	
	/// <summary>
	/// This class is able to generate a GUI definition out of a XML file.
	/// </summary>
	public class XmlLoader
	{
		ControlDictionary controlDictionary = new ControlDictionary();
		object            customizationObject;
		
		
		// required for the accept/cancel button/tooltips workaround
		Form mainForm = null;
		
		Hashtable tooltips      = new Hashtable();
		string acceptButtonName = String.Empty;
		string cancelButtonName = String.Empty;
		
		IStringValueFilter    stringValueFilter    = null;
		IObjectCreator        objectCreator        = new DefaultObjectCreator();
		IPropertyValueCreator propertyValueCreator = null;
		
		/// <summary>
		/// Gets the ControlDictionary for this XmlLoader.
		/// </summary>
		public ControlDictionary ControlDictionary {
			get {
				return controlDictionary;
			}
		}
		
		/// <summary>
		/// Gets/Sets the IStringValueFilter, could be set to <code>null</code> 
		/// (for not filtering the values).
		/// </summary>
		public IStringValueFilter StringValueFilter {
			get {
				return stringValueFilter;
			}
			set {
				stringValueFilter = value;
			}		
		}
		
		/// <summary>
		/// Gets/Sets the IObjectCreator, could NOT be set to <code>null</code>.
		/// </summary>
		public IObjectCreator ObjectCreator {
			get {
				return objectCreator;
			}
			set {
				if (value == null) {
					throw new System.ArgumentNullException();
				}
				objectCreator = value;
			}		
		}
		
		/// <summary>
		/// Gets/Sets the IPropertyValueCreator, could be set to <code>null</code>.
		/// </summary>
		public IPropertyValueCreator PropertyValueCreator {
			get {
				return propertyValueCreator;
			}
			set {
				propertyValueCreator = value;
			}		
		}
		
		
		/// <summary>
		/// Creates a new instance of XmlLoader.
		/// </summary>
		public XmlLoader()
		{
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
			
			if (doc.DocumentElement.Attributes["version"] != null) {
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}

			customizationObject = objectCreator.CreateObject(el.Name);
			
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
			if (doc.DocumentElement.Attributes["version"] != null) {
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}

			customizationObject = objectCreator.CreateObject(el.Name);
			
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
			if (doc.DocumentElement.Attributes["version"] != null) {
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}

			SetUpObject(customizationObject, el);
			
			// little HACK to set the Accept & Cancel Button
			if (customizationObject is Form) {
				Form mainForm = (Form)customizationObject;
				if (acceptButtonName != null && acceptButtonName.Length > 0) {
					mainForm.AcceptButton = (Button)controlDictionary[acceptButtonName];
				}
				if (cancelButtonName != null && cancelButtonName.Length > 0) {
					mainForm.CancelButton = (Button)controlDictionary[cancelButtonName];
				}
			}
			// little HACK to set the Tooltips...
			if (tooltips.Count > 0) {
				ToolTip toolTip = new ToolTip();
				foreach (DictionaryEntry entry in tooltips) {
					toolTip.SetToolTip((Control)entry.Key, entry.Value.ToString());
				}
			}
		}
		
		/// <summary>
		/// Loads the XML definition from a xml definition and sets creates the control.
		/// </summary>
		public void LoadObjectFromXmlDefinition(string xmlContent)
		{
			this.customizationObject = customizationObject;
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContent);
			
			XmlElement el = doc.DocumentElement;
			if (doc.DocumentElement.Attributes["version"] != null) {
				el = (XmlElement)doc.DocumentElement.ChildNodes[0];
			}
			SetUpObject(customizationObject, doc.DocumentElement);
		}
#endregion

		/// <summary>
		/// Sets the properties of an object <code>currentObject</code> to the
		/// contents of the xml element <code>element</code>
		/// </summary>
		void SetUpObject(object currentObject, XmlElement element)
		{
			foreach (XmlNode subNode in element.ChildNodes) {
				if (subNode is XmlElement){
					XmlElement subElement = (XmlElement)subNode;
					SetAttributes(currentObject, subElement);
				}
			}
			
			if (currentObject is Control) {
				((Control)currentObject).ResumeLayout(false);
			}
		}
		
		/// <summary>
		/// Sets a property called propertyName in object <code>o</code> to <code>val</code>. This method performs
		/// all neccessary casts.
		/// </summary>
		void SetValue(object o, string propertyName, string val)
		{
			try {
				PropertyInfo propertyInfo = o.GetType().GetProperty(propertyName);
				
				if (val.StartsWith("{") && val.EndsWith("}")) { // parse {Property1=value, Property2=value, ... Propertyn=value} style values
					val = val.Substring(1, val.Length - 2);
					object propertyObject = null;
					if (propertyInfo.CanWrite) {
						propertyObject = objectCreator.CreateObject(propertyInfo.PropertyType.FullName);
					} else {
						propertyObject = propertyInfo.GetValue(o, null);
					}
					
					Regex propertySet  = new Regex(@"(?<Property>[\w]+)\s*=\s*(?<Value>[\w\d]+)", RegexOptions.Compiled);
					Match match = propertySet.Match(val);
					while (true) {
						if (!match.Success) {
							break;
						}
						SetValue(propertyObject, match.Result("${Property}"), match.Result("${Value}"));
						match = match.NextMatch();
					}
					
					if (propertyInfo.CanWrite) {
						propertyInfo.SetValue(o, propertyObject, null);
					}
				} else if (propertyValueCreator != null && propertyValueCreator.CanCreateValueForType(propertyInfo.PropertyType)) {
					propertyInfo.SetValue(o, propertyValueCreator.CreateValue(propertyInfo.PropertyType, val) , null);
				} else if (propertyInfo.PropertyType.IsEnum) {
					propertyInfo.SetValue(o, Enum.Parse(propertyInfo.PropertyType, val), null);
				} else if (propertyInfo.PropertyType == typeof(Color)) {
					propertyInfo.SetValue(o, Color.FromName(val.Substring(7, val.Length - 8)), null);
				} else if (propertyInfo.PropertyType == typeof(Font)) {
					string[] font = val.Split(new char[]{','});
					propertyInfo.SetValue(o, new Font(font[0], Int32.Parse(font[1])), null);
				} else {
					propertyInfo.SetValue(o, Convert.ChangeType(val, propertyInfo.PropertyType), null);
				}
			} catch (Exception) {
				throw new ApplicationException("error while setting property " + propertyName + " of object "+ o.ToString() + " to value '" + val+ "'");
			}
		}
		
		/// <summary>
		/// Sets all properties in the object <code>o</code> to the xml element <code>el</code> defined properties.
		/// </summary>
		void SetAttributes(object o, XmlElement el)
		{
			if (el.Name == "AcceptButton") {
				mainForm = (Form)o;
				acceptButtonName = el.Attributes["value"].InnerText.Split(' ')[0];
				return;
			}
			
			if (el.Name == "CancelButton") {
				mainForm = (Form)o;
				cancelButtonName = el.Attributes["value"].InnerText.Split(' ')[0];
				return;
			}
			
			if (el.Name  == "ToolTip") {
				string val = el.Attributes["value"].InnerText;
				tooltips[o] = stringValueFilter != null ? stringValueFilter.GetFilteredValue(val) : val;
				return;
			}
				

			if (el.Attributes["value"] != null) {
				string val = el.Attributes["value"].InnerText;
				try {
					SetValue(o, el.Name, stringValueFilter != null ? stringValueFilter.GetFilteredValue(val) : val);
				} catch (Exception) {}
			} else if (el.Attributes["event"] != null) {
				try {
					EventInfo eventInfo = o.GetType().GetEvent(el.Name);
					eventInfo.AddEventHandler(o, Delegate.CreateDelegate(eventInfo.EventHandlerType, customizationObject, el.Attributes["event"].InnerText));
				} catch (Exception) {}
			} else {
				PropertyInfo propertyInfo = o.GetType().GetProperty(el.Name);
				object pv = propertyInfo.GetValue(o, null);
				if (pv is IList) {
					foreach (XmlNode subNode in el.ChildNodes) {
						if (subNode is XmlElement){
							XmlElement subElement = (XmlElement)subNode;
							object collectionObject = objectCreator.CreateObject(subElement.Name);
							if (collectionObject == null) {
								continue;
							}
							if (collectionObject is IComponent) {
								string name = null;
								if (subElement["Name"] != null &&
								    subElement["Name"].Attributes["value"] != null) {
								    name = subElement["Name"].Attributes["value"].InnerText;
								}
								    
								if (name == null || name.Length == 0) {
									name = "CreatedObject" + num++;
								}
//								collectionObject =  host.CreateComponent(collectionObject.GetType(), name);
							}
							
							SetUpObject(collectionObject, subElement);
							
							if (collectionObject is Control) {
								string name = ((Control)collectionObject).Name;
								if (name != null && name.Length > 0) {
									ControlDictionary[name] = (Control)collectionObject;
								}
							}
							
							if (collectionObject != null) {
								((IList)pv).Add(collectionObject);
							} 
							
							
						}
					}
				} else {
					object propertyObject = objectCreator.CreateObject(o.GetType().GetProperty(el.Name).PropertyType.Name);
					if (propertyObject is IComponent) {
						PropertyInfo pInfo = propertyObject.GetType().GetProperty("Name");
						string name = null;
						if (el["Name"] != null &&
						    el["Name"].Attributes["value"] != null) {
						    name = el["Name"].Attributes["value"].InnerText;
						}
											
						if (name == null || name.Length == 0) {
							name = "CreatedObject" + num++;
						}
						propertyObject = objectCreator.CreateObject(name);
					}
					SetUpObject(propertyObject, el);
					propertyInfo.SetValue(o, propertyObject, null);
				}
			}
		}
		int num = 0;
	}
}
