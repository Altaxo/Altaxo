﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Reflection;
using System.Xml;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	[Serializable]
	public class ReflectionParameter : DefaultParameter
	{
		public ReflectionParameter(ParameterInfo parameterInfo, IMember member) : base(parameterInfo.Name)
		{
			Type type = parameterInfo.ParameterType;
			
			this.ReturnType = ReflectionReturnType.Create(member, type, false);
			
			if (parameterInfo.IsOut) {
				modifier = ParameterModifiers.Out;
			} else if (type.IsByRef) {
				modifier = ParameterModifiers.Ref;
			}
			
			if (parameterInfo.IsOptional) {
				modifier |= ParameterModifiers.Optional;
			}
			if (type.IsArray && type != typeof(Array)) {
				foreach (CustomAttributeData data in CustomAttributeData.GetCustomAttributes(parameterInfo)) {
					if (data.Constructor.DeclaringType.FullName == typeof(ParamArrayAttribute).FullName) {
						modifier |= ParameterModifiers.Params;
						break;
					}
				}
			}
		}
	}
}
