// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;
using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractClass : AbstractNamedEntity, IClass, IComparable
	{
		protected ClassType        classType;
		protected IRegion          region;
		protected IRegion          bodyRegion;
		protected object           declaredIn;

		StringCollection    baseTypes   = null;

		ClassCollection    innerClasses = null;
		FieldCollection    fields       = null;
		PropertyCollection properties   = null;
		MethodCollection   methods      = null;
		EventCollection    events       = null;
		IndexerCollection  indexer      = null;

		public abstract ICompilationUnit CompilationUnit {
			get;
		}

		public virtual ClassType ClassType {
			get {
				return classType;
			}
		}

		public virtual IRegion Region {
			get {
				return region;
			}
		}
		
		public virtual IRegion BodyRegion {
			get {
				return bodyRegion;
			}
		}

		public object DeclaredIn {
			get {
				return declaredIn;
			}
		}

		public virtual StringCollection BaseTypes {
			get {
				if (baseTypes == null) {
					baseTypes = new StringCollection();
				}
				return baseTypes;
			}
		}
		
		public virtual ClassCollection InnerClasses {
			get {
				if (innerClasses == null) {
					innerClasses = new ClassCollection();
				}
				return innerClasses;
			}
		}

		public virtual FieldCollection Fields {
			get {
				if (fields == null) {
					fields = new FieldCollection();
				}
				return fields;
			}
		}

		public virtual PropertyCollection Properties {
			get {
				if (properties == null) {
					properties = new PropertyCollection();
				}
				return properties;
			}
		}

		public IndexerCollection Indexer {
			get {
				if (indexer == null) {
					indexer = new IndexerCollection();
				}
				return indexer;
			}
		}

		public virtual MethodCollection Methods {
			get {
				if (methods == null) {
					methods = new MethodCollection();
				}
				return methods;
			}
		}

		public virtual EventCollection Events {
			get {
				if (events == null) {
					events = new EventCollection();
				}
				return events;
			}
		}


		public virtual int CompareTo(IClass value)
		{
			int cmp;
			
			if(0 != (cmp = base.CompareTo((IDecoration)value))) {
				return cmp;
			}
			
			if (FullyQualifiedName != null) {
				cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			if (Region != null) {
				cmp = Region.CompareTo(value.Region);
				if (cmp != 0) {
					return cmp;
				}
			}
			
			cmp = DiffUtility.Compare(BaseTypes, value.BaseTypes);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(InnerClasses, value.InnerClasses);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Fields, value.Fields);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Properties, value.Properties);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Indexer, value.Indexer);
			if(cmp != 0)
				return cmp;
			
			cmp = DiffUtility.Compare(Methods, value.Methods);
			if(cmp != 0)
				return cmp;
			
			return DiffUtility.Compare(Events, value.Events);
		}
		
		int IComparable.CompareTo(object o) 
		{
			return CompareTo((IClass)o);
		}
		
		public IEnumerable ClassInheritanceTree {
			get {
				return new ClassInheritanceEnumerator(this);
			}
		}
		
		protected override bool CanBeSubclass {
			get {
				return true;
			}
		}

		public class ClassInheritanceEnumerator : IEnumerator, IEnumerable
		{
			static IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			IClass topLevelClass;
			IClass currentClass  = null;
			Queue  baseTypeQueue = new Queue();

			public ClassInheritanceEnumerator(IClass topLevelClass)
			{
				this.topLevelClass = topLevelClass;
				baseTypeQueue.Enqueue(topLevelClass.FullyQualifiedName);
				PutBaseClassesOnStack(topLevelClass);
				baseTypeQueue.Enqueue("System.Object");
			}
			public IEnumerator GetEnumerator()
			{
				return this;
			}

			void PutBaseClassesOnStack(IClass c)
			{
				foreach (string baseTypeName in c.BaseTypes) {
					baseTypeQueue.Enqueue(baseTypeName);
				}
			}

			public IClass Current {
				get {
					return currentClass;
				}
			}

			object IEnumerator.Current {
				get {
					return currentClass;
				}
			}

			public bool MoveNext()
			{
				if (baseTypeQueue.Count == 0) {
					return false;
				}
				string baseTypeName = baseTypeQueue.Dequeue().ToString();

				IClass baseType = parserService.GetClass(baseTypeName, true);
				if (baseType == null) {
					ICompilationUnit unit = currentClass == null ? null : currentClass.CompilationUnit;
					if (unit != null) {
						foreach (IUsing u in unit.Usings) {
							baseType = u.SearchType(baseTypeName);
							if (baseType != null) {
								break;
							}
						}
					}
				}

				if (baseType != null) {
					currentClass = baseType;
					PutBaseClassesOnStack(currentClass);
				}

				return baseType != null;
			}

			public void Reset()
			{
				baseTypeQueue.Clear();
				baseTypeQueue.Enqueue(topLevelClass.FullyQualifiedName);
				PutBaseClassesOnStack(topLevelClass);
				baseTypeQueue.Enqueue("System.Object");
			}
		}
	}
}
