// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="?" email="?"/>
//     <version value="$version"/>
// </file>

using System;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractNamedEntity: AbstractDecoration
	{
		private string fullyQualifiedName;
		private string className;
		private string namespaceName;
		
		public virtual string FullyQualifiedName {
			get {
				return fullyQualifiedName;
			}
			set {
				fullyQualifiedName = value;
				className = null; namespaceName = null;
			}
		}

		public virtual string Name {
			get {
				if (className == null && fullyQualifiedName != null) {
					int lastIndex;
					
					if (CanBeSubclass) {
						lastIndex = fullyQualifiedName.LastIndexOfAny
							(new char[] { '.', '+' });
					} else {
						lastIndex = fullyQualifiedName.LastIndexOf('.');
					}
					
					if (lastIndex < 0) {
						className = fullyQualifiedName;
					} else {
						className = fullyQualifiedName.Substring(lastIndex + 1);
					}
				}
				
				return className;
			}
		}

		public virtual string Namespace {
			get {
				if (namespaceName == null && fullyQualifiedName != null) {
					int lastIndex = fullyQualifiedName.LastIndexOf('.');
					
					if (lastIndex < 0) {
						namespaceName = string.Empty;
					} else {
						namespaceName = fullyQualifiedName.Substring(0, lastIndex);
					}
				}
				
				return namespaceName;
			}
		}
		
		protected virtual bool CanBeSubclass {
			get {
				return false;
			}
		}
	}
}
