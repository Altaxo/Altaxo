﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class wraps a ILanguageConversion to System.Reflection
	/// </summary>
	public class AmbienceReflectionDecorator : IAmbience
	{
		IAmbience conv;
		
		public ConversionFlags ConversionFlags {
			get {
				return conv.ConversionFlags;
			}
			set {
				conv.ConversionFlags = value;
			}
		}
		
		public string Convert(ModifierEnum modifier)
		{
			return conv.Convert(modifier);
		}
		
		public string Convert(IClass c)
		{
			return conv.Convert(c);
		}
		
		public string ConvertEnd(IClass c)
		{
			return conv.ConvertEnd(c);
		}
		
		public string Convert(IField field)
		{
			return conv.Convert(field);
		}

		public string Convert(IProperty property)
		{
			return conv.Convert(property);
		}
		
		public string Convert(IEvent e)
		{
			return conv.Convert(e);
		}
		
		public string Convert(IMethod m)
		{
			return conv.Convert(m);
		}
		
		public string ConvertEnd(IMethod m)
		{
			return conv.ConvertEnd(m);
		}
		
		public string Convert(IParameter param)
		{
			return conv.Convert(param);
		}		
		
		public string Convert(IReturnType returnType)
		{
			return conv.Convert(returnType);
		}
		
		public AmbienceReflectionDecorator(IAmbience conv)
		{
			if (conv == null) {
				throw new System.ArgumentNullException("conv");
			}
			this.conv = conv;
		}
		
		public string WrapAttribute(string attribute)
		{
			return conv.WrapAttribute(attribute);
		}
		
		public string WrapComment(string comment)
		{
			return conv.WrapComment(comment);
		}
		
		public string GetIntrinsicTypeName(string dotNetTypeName)
		{
			return conv.GetIntrinsicTypeName(dotNetTypeName);
		}
	}
}
