// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.ComponentModel;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public abstract class AbstractConfiguration : LocalizedObject, IConfiguration
	{
		[XmlAttribute("name")]
		protected string name = null;
		
		[Browsable(false)]
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		public override string ToString()
		{
			return name;
		}
	}
}
