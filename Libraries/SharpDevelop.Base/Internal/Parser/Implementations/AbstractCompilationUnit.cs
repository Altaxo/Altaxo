// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections.Specialized;

namespace SharpDevelop.Internal.Parser
{
	[Serializable]
	public abstract class AbstractCompilationUnit : ICompilationUnit
	{
		protected IUsingCollection usings = new IUsingCollection();
		protected ClassCollection classes = new ClassCollection();
		protected AttributeSectionCollection attributes = new AttributeSectionCollection();
		protected bool errorsDuringCompile = false;
		protected object tag               = null;
		
		public bool ErrorsDuringCompile {
			get {
				return errorsDuringCompile;
			}
			set {
				errorsDuringCompile = value;
			}
		}
		
		public object Tag {
			get {
				return tag;
			}
			set {
				tag = value;
			}
		}
		
		public virtual IUsingCollection Usings {
			get {
				return usings;
			}
		}

		public virtual AttributeSectionCollection Attributes {
			get {
				return attributes;
			}
		}

		public virtual ClassCollection Classes {
			get {
				return classes;
			}
		}

		public abstract CommentCollection MiscComments {
			get;
		}

		public abstract CommentCollection DokuComments {
			get;
		}

		public abstract TagCollection TagComments {
			get;
		}
	}
}
