// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public class ReflectionField : AbstractField
	{
		public ReflectionField(FieldInfo fieldInfo, Hashtable xmlComments)
		{
			Debug.Assert(fieldInfo != null);
			FullyQualifiedName = String.Concat(fieldInfo.DeclaringType.FullName, ".", fieldInfo.Name);
			
			if (xmlComments != null) {
				XmlNode node = xmlComments["F:" + FullyQualifiedName] as XmlNode;
				if (node != null) {
					documentation = node.InnerXml;
				}
			}
			if (fieldInfo.IsInitOnly) {
				modifiers |= ModifierEnum.Readonly;
			}
			
			if (fieldInfo.IsStatic) {
				modifiers |= ModifierEnum.Static;
			}
			
			if (fieldInfo.IsAssembly) {
				modifiers |= ModifierEnum.Internal;
			}
			
			if (fieldInfo.IsPrivate) { // I assume that private is used most and public last (at least should be)
				modifiers |= ModifierEnum.Private;
			} else if (fieldInfo.IsFamily) {
				modifiers |= ModifierEnum.Protected;
			} else if (fieldInfo.IsPublic) {
				modifiers |= ModifierEnum.Public;
			} else if (fieldInfo.IsFamilyOrAssembly) {
				modifiers |= ModifierEnum.ProtectedOrInternal;
			} else if (fieldInfo.IsFamilyAndAssembly) {
				modifiers |= ModifierEnum.Protected;
				modifiers |= ModifierEnum.Internal;
			}
			
			if (fieldInfo.IsLiteral) {
				modifiers |= ModifierEnum.Const;
			}
			
			returnType = new ReflectionReturnType(fieldInfo.FieldType);
		}
	}
}
