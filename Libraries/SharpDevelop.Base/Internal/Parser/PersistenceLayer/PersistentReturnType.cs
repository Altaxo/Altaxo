// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser
{
	public sealed class PersistentReturnType : IReturnType
	{
		string               fullyQualifiedName;

		int                  classProxyIndex;
		ClassProxyCollection classProxyCollection;

		int   pointerNestingLevel = -1;
		int[] arrayDimensions     = new int[] {};

		public PersistentReturnType(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			this.classProxyCollection = classProxyCollection;
			classProxyIndex      = reader.ReadInt32();

			if (classProxyIndex < 0) {
				fullyQualifiedName = reader.ReadString();
			}

			pointerNestingLevel = reader.ReadInt32();

			uint count = reader.ReadUInt32();
			arrayDimensions = new int[count];
			for (uint i = 0; i < arrayDimensions.Length; ++i) {
				arrayDimensions[i] = reader.ReadInt32();
			}
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(classProxyIndex);
			if (classProxyIndex < 0) {
				writer.Write(fullyQualifiedName);
			}
			writer.Write(pointerNestingLevel);
			if (arrayDimensions == null) {
				writer.Write((uint)0);
			} else {
				writer.Write((uint)arrayDimensions.Length);
				for (uint i = 0; i < arrayDimensions.Length; ++i) {
					writer.Write(arrayDimensions[i]);
				}
			}
		}

		public PersistentReturnType(ClassProxyCollection classProxyCollection, IReturnType returnType)
		{
			if (returnType == null) {
				classProxyIndex    = - 1;
				fullyQualifiedName = String.Empty;
			} else {
				this.classProxyCollection = classProxyCollection;
				this.pointerNestingLevel  = returnType.PointerNestingLevel;
				this.arrayDimensions      = returnType.ArrayDimensions;
				classProxyIndex           = classProxyCollection.IndexOf(returnType.FullyQualifiedName);
				fullyQualifiedName        = returnType.FullyQualifiedName;
			}
		}

		public string FullyQualifiedName {
			get {
				if (classProxyIndex < 0) {
					return fullyQualifiedName;
				}
				return classProxyCollection[classProxyIndex].FullyQualifiedName;
			}
		}

		public string Name {
 			get {
				string[] name = FullyQualifiedName.Split(new char[] {'.'});
				return name[name.Length - 1];
			}
		}

		public string Namespace {
			get {
				int index = FullyQualifiedName.LastIndexOf('.');
				return index < 0 ? String.Empty : FullyQualifiedName.Substring(0, index);
			}
		}

		public int PointerNestingLevel {
			get {
				return pointerNestingLevel;
			}
		}

		public int ArrayCount {
			get {
				return ArrayDimensions.Length;
			}
		}

		public int[] ArrayDimensions {
			get {
				return arrayDimensions;
			}
		}

       int IComparable.CompareTo(object value) {
          return 0;
       }

		// stub
       	public object DeclaredIn {
       		get {
       			return null;
       		}
       	}
	}
}
