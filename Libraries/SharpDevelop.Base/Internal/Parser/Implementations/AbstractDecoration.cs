// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Collections.Utility;
using System.Reflection;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractDecoration : MarshalByRefObject, IDecoration
	{
		protected ModifierEnum               modifiers     = ModifierEnum.None;
		protected AttributeSectionCollection attributes    = null;
		string documentation = null;
//		int    documentationHash = -1;
//		public static Hashtable documentationHashtable = new Hashtable();
		
		
		public virtual ModifierEnum Modifiers {
			get {
				return modifiers;
			}
		}

		public virtual AttributeSectionCollection Attributes {
			get {
				if (attributes == null) {
					attributes = new AttributeSectionCollection();
				}
				return attributes;
			}
		}

		public string Documentation {
			get {
				if (documentation == null) {
					return String.Empty;
				}
				return documentation;
//				if (documentationHash == -1) {
//					return String.Empty;
//				}
//				return (string)documentationHashtable[documentationHash];
			}
			set {
				documentation = value;
//				documentationHash = value.GetHashCode();
//				if (documentationHashtable[documentationHash] == null) {
//					documentationHashtable[documentationHash] = value;
//				}
			}
		}
		
		public bool IsAbstract {
			get {
				return (modifiers & ModifierEnum.Abstract) == ModifierEnum.Abstract;
			}
		}

		public bool IsSealed {
			get {
				return (modifiers & ModifierEnum.Sealed) == ModifierEnum.Sealed;
			}
		}

		public bool IsStatic {
			get {
				return (modifiers & ModifierEnum.Static) == ModifierEnum.Static;
			}
		}

		public bool IsVirtual {
			get {
				return (modifiers & ModifierEnum.Virtual) == ModifierEnum.Virtual;
			}
		}

		public bool IsPublic {
			get {
				return (modifiers & ModifierEnum.Public) == ModifierEnum.Public;
			}
		}

		public bool IsProtected {
			get {
				return (modifiers & ModifierEnum.Protected) == ModifierEnum.Protected;
			}
		}

		public bool IsPrivate {
			get {
				return (modifiers & ModifierEnum.Private) == ModifierEnum.Private;
			}
		}

		public bool IsInternal {
			get {
				return (modifiers & ModifierEnum.Internal) == ModifierEnum.Internal;
			}
		}

		public bool IsProtectedAndInternal {
			get {
				return (modifiers & (ModifierEnum.Internal | ModifierEnum.Protected)) == (ModifierEnum.Internal | ModifierEnum.Protected);
			}
		}

		public bool IsProtectedOrInternal {
			get {
				return (modifiers & ModifierEnum.ProtectedOrInternal) == ModifierEnum.ProtectedOrInternal;
			}
		}

		public bool IsLiteral {
			get {
				return (modifiers & ModifierEnum.Const) == ModifierEnum.Const;
			}
		}

		public bool IsReadonly {
			get {
				return (modifiers & ModifierEnum.Readonly) == ModifierEnum.Readonly;
			}
		}

		public bool IsOverride {
			get {
				return (modifiers & ModifierEnum.Override) == ModifierEnum.Override;
			}
		}

		public bool IsFinal {
			get {
				return (modifiers & ModifierEnum.Final) == ModifierEnum.Final;
			}
		}

		public bool IsSpecialName {
			get {
				return (modifiers & ModifierEnum.SpecialName) == ModifierEnum.SpecialName;
			}
		}

		public bool IsNew {
			get {
				return (modifiers & ModifierEnum.New) == ModifierEnum.New;
			}
		}
		
		public virtual int CompareTo(IDecoration value) {
			int cmp;
			
			if(0 != (cmp = (int)(Modifiers - value.Modifiers)))
				return cmp;
			
			return DiffUtility.Compare(Attributes, value.Attributes);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IDecoration)value);
		}
	}
}
