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
	/// Indicates that field value is a file or directory which should be saved
	/// relative to the project file location.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple = false)]
	public class ConvertToRelativePathAttribute : Attribute
	{
		string predicatePropertyName;
		
		public ConvertToRelativePathAttribute()
		{
		}
		
		public ConvertToRelativePathAttribute(string predicatePropertyName)
		{
			this.predicatePropertyName = predicatePropertyName;
		}
		
		/// <summary>
		/// Gets the predicate property name. A predicate property is a property
		/// which returns a bool, it indicates that the path should be converted, or not
		/// (depending on the value of the property)
		/// <summary>
		public string PredicatePropertyName {
			get {
				return predicatePropertyName;
			}
			set {
				predicatePropertyName = value;
			}
		}
	}
}
