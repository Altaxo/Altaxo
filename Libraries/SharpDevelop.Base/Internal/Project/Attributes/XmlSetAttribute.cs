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
	/// Indicates that field should be treated as a xml attribute for the codon or condition.
	/// The field is treated as a array, separated by ',' example :
	/// fileextensions = ".cpp,.cc,.C"
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple = false)]
	public class XmlSetAttribute : Attribute
	{
		string name = null;
		Type   type = null;
		
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public XmlSetAttribute(Type type)
		{
			this.type = type;
		}
		
		public XmlSetAttribute(Type type, string name) : this(type)
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
		
		/// <summary>
		/// The typ of the set
		/// </summary>
		public Type Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
	}
}
