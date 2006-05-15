// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// ConstructedReturnType is a reference to generic class that specifies the type parameters.
	/// When getting the Members, this return type modifies the lists in such a way that the
	/// <see cref="GenericReturnType"/>s are replaced with the return types in the type parameters
	/// collection.
	/// Example: List&lt;string&gt;
	/// </summary>
	public sealed class ConstructedReturnType : ProxyReturnType
	{
		// Return types that should be substituted for the generic types
		// If a substitution is unknown (type could not be resolved), the list
		// contains a null entry.
		IList<IReturnType> typeArguments;
		IReturnType baseType;
		
		public override IList<IReturnType> TypeArguments {
			get {
				return typeArguments;
			}
		}
		
		public ConstructedReturnType(IReturnType baseType, IList<IReturnType> typeArguments)
		{
			if (baseType == null)
				throw new ArgumentNullException("baseType");
			if (typeArguments == null)
				throw new ArgumentNullException("typeArguments");
			this.typeArguments = typeArguments;
			this.baseType = baseType;
		}
		
		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			if (rt == null) return false;
			return this.DotNetName == rt.DotNetName;
		}
		
		public override int GetHashCode()
		{
			int code = baseType.GetHashCode();
			foreach (IReturnType t in typeArguments) {
				if (t != null) {
					code ^= t.GetHashCode();
				}
			}
			return code;
		}
		
		public override IReturnType BaseType {
			get {
				return baseType;
			}
		}
		
		public override IReturnType UnboundType {
			get {
				return baseType;
			}
		}
		
		bool CheckReturnType(IReturnType t)
		{
			if (t is GenericReturnType) {
				GenericReturnType rt = (GenericReturnType)t;
				return rt.TypeParameter.Method == null;
			} else if (t.ArrayDimensions > 0) {
				return CheckReturnType(t.ArrayElementType);
			} else if (t.TypeArguments != null) {
				foreach (IReturnType para in t.TypeArguments) {
					if (para != null) {
						if (CheckReturnType(para)) return true;
					}
				}
				return false;
			} else {
				return false;
			}
		}
		
		bool CheckParameters(IList<IParameter> l)
		{
			foreach (IParameter p in l) {
				if (CheckReturnType(p.ReturnType)) return true;
			}
			return false;
		}
		
		public override string DotNetName {
			get {
				string baseName = baseType.DotNetName;
				int pos = baseName.LastIndexOf('`');
				StringBuilder b;
				if (pos < 0)
					b = new StringBuilder(baseName);
				else
					b = new StringBuilder(baseName, 0, pos, pos + 20);
				b.Append('{');
				for (int i = 0; i < typeArguments.Count; ++i) {
					if (i > 0) b.Append(',');
					if (typeArguments[i] != null) {
						b.Append(typeArguments[i].DotNetName);
					}
				}
				b.Append('}');
				return b.ToString();
			}
		}
		
		public static IReturnType TranslateType(IReturnType input, IList<IReturnType> typeParameters, bool convertForMethod)
		{
			if (typeParameters == null || typeParameters.Count == 0) {
				return input; // nothing to do when there are no type parameters specified
			}
			if (input is GenericReturnType) {
				GenericReturnType rt = (GenericReturnType)input;
				if (convertForMethod ? (rt.TypeParameter.Method != null) : (rt.TypeParameter.Method == null)) {
					if (rt.TypeParameter.Index < typeParameters.Count) {
						IReturnType newType = typeParameters[rt.TypeParameter.Index];
						if (newType != null) {
							return newType;
						}
					}
				}
			} else if (input.ArrayDimensions > 0) {
				IReturnType e = input.ArrayElementType;
				IReturnType t = TranslateType(e, typeParameters, convertForMethod);
				if (e != t && t != null)
					return new ArrayReturnType(t, input.ArrayDimensions);
			} else if (input.TypeArguments != null) {
				List<IReturnType> para = new List<IReturnType>(input.TypeArguments.Count);
				foreach (IReturnType argument in input.TypeArguments) {
					para.Add(TranslateType(argument, typeParameters, convertForMethod));
				}
				return new ConstructedReturnType(input.UnboundType, para);
			}
			return input;
		}
		
		IReturnType TranslateType(IReturnType input)
		{
			return TranslateType(input, typeArguments, false);
		}
		
		public override List<IMethod> GetMethods()
		{
			List<IMethod> l = baseType.GetMethods();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType) || CheckParameters(l[i].Parameters)) {
					l[i] = (IMethod)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
					for (int j = 0; j < l[i].Parameters.Count; ++j) {
						l[i].Parameters[j].ReturnType = TranslateType(l[i].Parameters[j].ReturnType);
					}
				}
			}
			return l;
		}
		
		public override List<IProperty> GetProperties()
		{
			List<IProperty> l = baseType.GetProperties();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType) || CheckParameters(l[i].Parameters)) {
					l[i] = (IProperty)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
					for (int j = 0; j < l[i].Parameters.Count; ++j) {
						l[i].Parameters[j].ReturnType = TranslateType(l[i].Parameters[j].ReturnType);
					}
				}
			}
			return l;
		}
		
		public override List<IField> GetFields()
		{
			List<IField> l = baseType.GetFields();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType)) {
					l[i] = (IField)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
				}
			}
			return l;
		}
		
		public override List<IEvent> GetEvents()
		{
			List<IEvent> l = baseType.GetEvents();
			for (int i = 0; i < l.Count; ++i) {
				if (CheckReturnType(l[i].ReturnType)) {
					l[i] = (IEvent)l[i].Clone();
					l[i].ReturnType = TranslateType(l[i].ReturnType);
				}
			}
			return l;
		}
		
		public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		public override string ToString()
		{
			string r = "[ConstructedReturnType: ";
			r += baseType;
			r += "<";
			for (int i = 0; i < typeArguments.Count; i++) {
				if (i > 0) r += ",";
				if (typeArguments[i] != null) {
					r += typeArguments[i];
				}
			}
			return r + ">]";
		}
	}
}
