// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class contains the state of the <code>MdiWorkspace</code>, it is used to 
	/// make the <code>MdiWorkspace</code> state persistent.
	/// </summary>
	public class WorkbenchMemento : IXmlConvertable
	{
		FormWindowState windowstate        = FormWindowState.Normal;
		FormWindowState defaultwindowstate = FormWindowState.Normal;
		Rectangle       bounds             = new Rectangle(0, 0, 640, 480);
		bool            fullscreen         = false;
		
		public FormWindowState DefaultWindowState {
			get {
				return defaultwindowstate;
			}
			set {
				defaultwindowstate = value;
			}
		}
		
		public FormWindowState WindowState {
			get {
				return windowstate;
			}
			set {
				windowstate = value;
			}
		}
		
		public Rectangle Bounds {
			get {
				return bounds;
			}
			set {
				bounds = value;
			}
		}
		
		public bool FullScreen {
			get {
				return fullscreen;
			}
			set {
				fullscreen = value;
			}
		}
		
		/// <summary>
		/// Creates a new instance of the <code>MdiWorkspaceMemento</code>.
		/// </summary>
		public WorkbenchMemento()
		{
			windowstate = FormWindowState.Maximized;
			bounds      = new Rectangle(0, 0, 640, 480);
			fullscreen  = false;
		}
		
		WorkbenchMemento(XmlElement element)
		{
			string[] boundstr = element.Attributes["bounds"].InnerText.Split(new char [] { ',' });
			
			bounds = new Rectangle(Int32.Parse(boundstr[0]), Int32.Parse(boundstr[1]), 
			                       Int32.Parse(boundstr[2]), Int32.Parse(boundstr[3]));
			
			windowstate = (FormWindowState)Enum.Parse(typeof(FormWindowState), element.Attributes["formwindowstate"].InnerText);
			
			if (element.Attributes["defaultformwindowstate"] != null) {
				defaultwindowstate = (FormWindowState)Enum.Parse(typeof(FormWindowState), element.Attributes["defaultformwindowstate"].InnerText);
			}
			
			fullscreen  = Boolean.Parse(element.Attributes["fullscreen"].InnerText);
		}

		public object FromXmlElement(XmlElement element)
		{
			return new WorkbenchMemento(element);
		}
		
		public XmlElement ToXmlElement(XmlDocument doc)
		{
			XmlElement element = doc.CreateElement("WindowState");
			XmlAttribute attr;
			
			attr = doc.CreateAttribute("bounds");
			attr.InnerText = bounds.X + "," + bounds.Y + "," + bounds.Width + "," + bounds.Height;
			element.Attributes.Append(attr);
			
			attr = doc.CreateAttribute("formwindowstate");
			attr.InnerText = windowstate.ToString();
			element.Attributes.Append(attr);
			
			attr = doc.CreateAttribute("defaultformwindowstate");
			attr.InnerText = defaultwindowstate.ToString();
			element.Attributes.Append(attr);
			
			attr = doc.CreateAttribute("fullscreen");
			attr.InnerText = fullscreen.ToString();
			element.Attributes.Append(attr);
			
			return element;
		}
	}
}
