// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;
using System.Collections.Utility;

namespace SharpDevelop.Internal.Parser
{

	[Serializable]
	public abstract class AbstractParameter : System.MarshalByRefObject, IParameter
	{
		string              name;
		string              documentation;
		
//		int nameHashCode      = -1;
//		int documentationHash = -1;
		
		protected IReturnType         returnType;
		protected ParameterModifier   modifier;
		AttributeCollection attributeCollection;

		public bool IsOut {
			get {
				return (modifier & ParameterModifier.Out) == ParameterModifier.Out;
			}
		}
		public bool IsRef {
			get {
				return (modifier & ParameterModifier.Ref) == ParameterModifier.Ref;
			}
		}
		public bool IsParams {
			get {
				return (modifier & ParameterModifier.Params) == ParameterModifier.Params;
			}
		}

		public virtual string Name {
			get {
				return name;
//				return (string)AbstractNamedEntity.fullyQualifiedNames[nameHashCode];
			}
			set {
				name = value;
//				nameHashCode = value.GetHashCode();
//				if (AbstractNamedEntity.fullyQualifiedNames[nameHashCode] == null) {
//					AbstractNamedEntity.fullyQualifiedNames[nameHashCode] = value;
//				}
			}
		}

		public virtual IReturnType ReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}

		public virtual AttributeCollection AttributeCollection {
			get {
				if (attributeCollection == null) {
					attributeCollection = new AttributeCollection();
				}
				return attributeCollection;
			}
		}

		public virtual ParameterModifier Modifier {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}

		public string Documentation {
			get {
				return documentation;
//				if (documentationHash == -1) {
//					return String.Empty;
//				}
//				return (string)AbstractDecoration.documentationHashtable[documentationHash];
			}
			set {
				documentation = value;
//				documentationHash = value.GetHashCode();
//				if (AbstractDecoration.documentationHashtable[documentationHash] == null) {
//					AbstractDecoration.documentationHashtable[documentationHash] = value;
//				}
			}
		}
		
		public virtual int CompareTo(IParameter value) {
			int cmp;
			
			if (Name != null) {
				cmp = Name.CompareTo(value.Name);
				if (cmp != 0) {
					return cmp;
				}
			}
			if (ReturnType != null) {
				if(0 != (cmp = ReturnType.CompareTo(value.ReturnType)))
					return cmp;
			}
			
			if(0 != (cmp = (int)(Modifier - value.Modifier)))
				return cmp;
			
			return DiffUtility.Compare(AttributeCollection, value.AttributeCollection);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IParameter)value);
		}
	}
}
