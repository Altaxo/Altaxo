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
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class XmlNodeNameAttribute : Attribute
	{
		string name;
		
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public XmlNodeNameAttribute(string name) 
		{
			this.name = name;
		}
		
		/// <summary>
		/// Returns the name of the xmlnameattribute.
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
