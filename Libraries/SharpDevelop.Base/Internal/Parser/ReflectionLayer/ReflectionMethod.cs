// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public class ReflectionMethod : AbstractMethod 
	{
		string GetParamList(MethodBase methodBase)
		{
			StringBuilder propertyName = new StringBuilder("(");
			ParameterInfo[] p = methodBase.GetParameters();
			if (p.Length == 0) {
				return String.Empty;
			}
			for (int i = 0; i < p.Length; ++i) {
				propertyName.Append(p[i].ParameterType.FullName);
				if (i + 1 < p.Length) {
					propertyName.Append(',');
				}
			}
			propertyName.Append(')');
			return propertyName.ToString();
		}
		
		public ReflectionMethod(MethodBase methodBase, Hashtable xmlComments)
		{
			string name = methodBase.Name;
			
			if (methodBase is ConstructorInfo) {
				name = "#ctor";
			}
			FullyQualifiedName = String.Concat(methodBase.DeclaringType.FullName, ".", name);
			
			XmlNode node = null;
			
			if (xmlComments != null) {
				node = xmlComments["M:" + FullyQualifiedName + GetParamList(methodBase)] as XmlNode;
				if (node != null) {
					documentation = node.InnerXml;
				}
			}
			
			modifiers = ModifierEnum.None;
			if (methodBase.IsStatic) {
				modifiers |= ModifierEnum.Static;
			}
			if (methodBase.IsAssembly) {
				modifiers |= ModifierEnum.Internal;
			}
			if (methodBase.IsPrivate) { // I assume that private is used most and public last (at least should be)
				modifiers |= ModifierEnum.Private;
			} else if (methodBase.IsFamily) {
				modifiers |= ModifierEnum.Protected;
			} else if (methodBase.IsPublic) {
				modifiers |= ModifierEnum.Public;
			} else if (methodBase.IsFamilyOrAssembly) {
				modifiers |= ModifierEnum.ProtectedOrInternal;
			} else if (methodBase.IsFamilyAndAssembly) {
				modifiers |= ModifierEnum.Protected;
				modifiers |= ModifierEnum.Internal;
			}
			
			if (methodBase.IsVirtual) {
				modifiers |= ModifierEnum.Virtual;
			}
			if (methodBase.IsAbstract) {
				modifiers |= ModifierEnum.Abstract;
			}
			
			foreach (ParameterInfo paramInfo in methodBase.GetParameters()) {
				parameters.Add(new ReflectionParameter(paramInfo, node));
			}
			
			if (methodBase is MethodInfo) {
				returnType = new ReflectionReturnType(((MethodInfo)methodBase).ReturnType);
			} else {
				returnType = null;
			}
		}
	}
}
