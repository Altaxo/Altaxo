// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser
{
	public sealed class PersistentClass : IClass
	{
		int                  classProxyIndex;
		ClassProxyCollection classProxyCollection;

		// an int arraylist of base types.
		ArrayList          baseTypes         = new ArrayList();

		/// <remarks>
		/// This collection is only used during creation, if for a basetype no
		/// proxy index could be determined. It contains all those names as real name.
		/// They should be read to <code>baseTypesStringCollection</code> when the persistant
		/// class is read.
		/// </remarks>
		StringCollection   notFoundBaseTypes = new StringCollection();

		StringCollection   baseTypesStringCollection = new StringCollection();

		ClassCollection    innerClasses = new ClassCollection();
		FieldCollection    fields       = new FieldCollection();
		PropertyCollection properties   = new PropertyCollection();
		MethodCollection   methods      = new MethodCollection();
		EventCollection    events       = new EventCollection();
		IndexerCollection  indexer      = new IndexerCollection();


		string       fullyQualifiedName;
		string       documentation;
		ClassType    classType;
		ModifierEnum modifiers;

		public ICompilationUnit CompilationUnit {
			get {
				return null;
			}
		}

		public PersistentClass(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			this.classProxyCollection = classProxyCollection;
			classProxyIndex = reader.ReadInt32();

			if (classProxyIndex < 0) {
				fullyQualifiedName = reader.ReadString();
				documentation      = reader.ReadString();
				modifiers          = (ModifierEnum)reader.ReadUInt32();
				classType          = (ClassType)reader.ReadInt16();
			}

			uint count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				int baseTypeIndex = reader.ReadInt32();
				if (baseTypeIndex < 0) {
					baseTypesStringCollection.Add(reader.ReadString());
				} else {
					baseTypes.Add(baseTypeIndex);
				}
			}

			count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				innerClasses.Add(new PersistentClass(reader, classProxyCollection));
			}

			count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				fields.Add(new PersistentField(reader, classProxyCollection));
			}

			count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				properties.Add(new PersistentProperty(reader, classProxyCollection));
			}

			count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				IMethod m = new PersistentMethod(reader, classProxyCollection);
				methods.Add(m);
			}

			count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				events.Add(new PersistentEvent(reader, classProxyCollection));
			}

			count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				indexer.Add(new PersistentIndexer(reader, classProxyCollection));
			}
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(classProxyIndex);

			if (classProxyIndex < 0) {
				writer.Write(fullyQualifiedName);
				writer.Write(documentation);
				writer.Write((uint)modifiers);
				writer.Write((short)classType);
			}

			writer.Write((uint)(baseTypes.Count + notFoundBaseTypes.Count));
			foreach (int baseTypeIdx in baseTypes) {
				writer.Write(baseTypeIdx);
			}

			foreach (string baseType in notFoundBaseTypes) {
				writer.Write((int)-1);
				writer.Write(baseType);
			}


			writer.Write((uint)innerClasses.Count);
			foreach (PersistentClass innerClass in innerClasses) {
				innerClass.WriteTo(writer);
			}

			writer.Write((uint)fields.Count);
			foreach (PersistentField field in fields) {
				field.WriteTo(writer);
			}

			writer.Write((uint)properties.Count);
			foreach (PersistentProperty property in properties) {
				property.WriteTo(writer);
			}

			writer.Write((uint)methods.Count);
			foreach (PersistentMethod method in methods) {
				method.WriteTo(writer);
			}

			writer.Write((uint)events.Count);
			foreach (PersistentEvent e in events) {
				e.WriteTo(writer);
			}

			writer.Write((uint)indexer.Count);
			foreach (PersistentIndexer ind in indexer) {
				ind.WriteTo(writer);
			}
		}

		public PersistentClass(ClassProxyCollection classProxyCollection, IClass c)
		{
			this.classProxyCollection = classProxyCollection;
			classProxyIndex           = classProxyCollection.IndexOf(c.FullyQualifiedName);
			if (classProxyIndex < 0) {
				fullyQualifiedName = c.FullyQualifiedName;
				documentation      = c.Documentation;
				modifiers          = c.Modifiers;
				classType          = c.ClassType;
			}

			foreach (string baseType in c.BaseTypes) {
				int idx = classProxyCollection.IndexOf(baseType);
				if (idx < 0) {
					notFoundBaseTypes.Add(baseType);
				} else {
					baseTypes.Add(idx);
				}
			}

			foreach (IClass innerClass in c.InnerClasses) {
				innerClasses.Add(new PersistentClass(classProxyCollection, innerClass));
			}

			foreach (IField field in c.Fields) {
				fields.Add(new PersistentField(classProxyCollection, field));
			}

			foreach (IProperty property in c.Properties) {
				properties.Add(new PersistentProperty(classProxyCollection, property));
			}

			foreach (IMethod method in c.Methods) {
				methods.Add(new PersistentMethod(classProxyCollection, method));
			}

			foreach (IEvent e in c.Events) {
				events.Add(new PersistentEvent(classProxyCollection, e));
			}

			foreach (IIndexer ind in c.Indexer) {
				indexer.Add(new PersistentIndexer(classProxyCollection, ind));
			}
		}

		public ClassType ClassType {
			get {
				if (classProxyIndex < 0) {
					return classType;
				}
				return classProxyCollection[classProxyIndex].ClassType;
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

		// regions are currently useless (only assembly classes are
		// made persistant)
		public IRegion Region {
			get {
				return null;
			}
		}
		public IRegion BodyRegion {
			get {
				return null;
			}
		}

		public StringCollection BaseTypes {
			get {
				// convert base types first time they're requested
				if (baseTypes.Count > 0) {
					foreach (int index in baseTypes) {
						baseTypesStringCollection.Add(classProxyCollection[index].FullyQualifiedName);
					}
					baseTypes.Clear();
				}
				return baseTypesStringCollection;
			}
		}
		
		public ClassCollection InnerClasses {
			get {
				return innerClasses;
			}
		}

		public FieldCollection Fields {
			get {
				return fields;
			}
		}

		public PropertyCollection Properties {
			get {
				return properties;
			}
		}

		public IndexerCollection Indexer {
			get {
				return indexer;
			}
		}

		public MethodCollection Methods {
			get {
				return methods;
			}
		}

		public EventCollection Events {
			get {
				return events;
			}
		}

		// IDecoration implementation
		public ModifierEnum Modifiers {
			get {
				if (classProxyIndex < 0) {
					return modifiers;
				}
				return classProxyCollection[classProxyIndex].Modifiers;
			}
		}

		public AttributeSectionCollection Attributes {
			get {
				return null;
			}
		}

		public string Documentation {
			get {
				if (classProxyIndex < 0) {
					return documentation;
				}
				return classProxyCollection[classProxyIndex].Documentation;
			}
		}

		public bool IsStatic {
			get {
				return (Modifiers & ModifierEnum.Static) == ModifierEnum.Static;
			}
		}

		public bool IsVirtual {
			get {
				return (Modifiers & ModifierEnum.Virtual) == ModifierEnum.Virtual;
			}
		}

		public bool IsPublic {
			get {
				return (Modifiers & ModifierEnum.Public) == ModifierEnum.Public;
			}
		}

		public bool IsFinal {
			get {
				return (Modifiers & ModifierEnum.Final) == ModifierEnum.Final;
			}
		}

		public bool IsSpecialName {
			get {
				return (Modifiers & ModifierEnum.SpecialName) == ModifierEnum.SpecialName;
			}
		}

		public bool IsProtected {
			get {
				return (Modifiers & ModifierEnum.Protected) == ModifierEnum.Protected;
			}
		}

		public bool IsPrivate {
			get {
				return (Modifiers & ModifierEnum.Private) == ModifierEnum.Private;
			}
		}

		public bool IsInternal {
			get {
				return (Modifiers & ModifierEnum.Internal) == ModifierEnum.Internal;
			}
		}

		public bool IsProtectedAndInternal {
			get {
				return (Modifiers & (ModifierEnum.Internal | ModifierEnum.Protected)) == (ModifierEnum.Internal | ModifierEnum.Protected);
			}
		}

		public bool IsProtectedOrInternal {
			get {
				return (Modifiers & ModifierEnum.ProtectedOrInternal) == ModifierEnum.ProtectedOrInternal;
			}
		}

		public bool IsAbstract {
			get {
				return (Modifiers & ModifierEnum.Abstract) == ModifierEnum.Abstract;
			}
		}

		public bool IsSealed {
			get {
				return (Modifiers & ModifierEnum.Sealed) == ModifierEnum.Sealed;
			}
		}

		public bool IsLiteral {
			get {
				return (Modifiers & ModifierEnum.Const) == ModifierEnum.Const;
			}
		}

		public bool IsReadonly {
			get {
				return (Modifiers & ModifierEnum.Readonly) == ModifierEnum.Readonly;
			}
		}
		public bool IsOverride {
			get {
				return (Modifiers & ModifierEnum.Override) == ModifierEnum.Override;
			}
		}

		public bool IsNew {
			get {
				return (Modifiers & ModifierEnum.New) == ModifierEnum.New;
			}
		}

		public IEnumerable ClassInheritanceTree {
			get {
				return new AbstractClass.ClassInheritanceEnumerator(this);
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
