// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections.Utility;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractReturnType : System.MarshalByRefObject, IReturnType
	{
		protected string fullyQualifiedName;
		protected int    pointerNestingLevel;
		protected int[]  arrayDimensions;
		protected object declaredin = null;

		public virtual string FullyQualifiedName {
			get {
				return fullyQualifiedName;
			}
		}

		public virtual string Name {
			get {
				if (fullyQualifiedName == null) {
					return null;
				}
				string[] name = fullyQualifiedName.Split(new char[] {'.'});
				return name[name.Length - 1];
			}
		}

		public virtual string Namespace {
			get {
				if (fullyQualifiedName == null) {
					return null;
				}
				int index = fullyQualifiedName.LastIndexOf('.');
				return index < 0 ? String.Empty : fullyQualifiedName.Substring(0, index);
			}
		}

		public virtual int PointerNestingLevel {
			get {
				return pointerNestingLevel;
			}
		}

		public int ArrayCount {
			get {
				return ArrayDimensions.Length;
			}
		}

		public virtual int[] ArrayDimensions {
			get {
				if (arrayDimensions == null) return new int[0];
				return arrayDimensions;
			}
		}

		public virtual int CompareTo(IReturnType value) {
			int cmp;
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			cmp = (PointerNestingLevel - value.PointerNestingLevel);
			if (cmp != 0) {
				return cmp;
			}
			
			return DiffUtility.Compare(ArrayDimensions, value.ArrayDimensions);
		}
		
		int IComparable.CompareTo(object value)
		{
			return CompareTo((IReturnType)value);
		}
		
		public virtual object DeclaredIn {
			get {
				return declaredin;
			}
		}
	}
	
}
