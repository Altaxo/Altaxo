// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	/// <summary>
	/// Indicates that field should be treated as a xml attribute 
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple = false)]
	public class XmlAttributeAttribute : Attribute
	{
		string name;
		
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public XmlAttributeAttribute(string name)
		{
			this.name  = name;
		}
		
		/// <summary>
		/// The name of the attribute.
		/// </summary>
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
	}
}
