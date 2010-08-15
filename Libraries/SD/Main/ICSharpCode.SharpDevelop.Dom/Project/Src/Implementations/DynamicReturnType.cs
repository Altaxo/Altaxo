﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5625 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DynamicReturnType : AbstractReturnType
	{
		readonly IProjectContent pc;
		
		public DynamicReturnType(IProjectContent pc)
		{
			if (pc == null)
				throw new ArgumentNullException("pc");
			this.pc = pc;
		}
		
		public override IClass GetUnderlyingClass()
		{
			return null;
		}
		
		public override List<IMethod> GetMethods()
		{
			return new List<IMethod>();
		}
		public override List<IProperty> GetProperties()
		{
			return new List<IProperty>();
		}
		public override List<IField> GetFields()
		{
			return new List<IField>();
		}
		public override List<IEvent> GetEvents()
		{
			return new List<IEvent>();
		}
		
		public override string Name {
			get { return "dynamic"; }
		}
		
		public override string FullyQualifiedName {
			get { return "dynamic"; }
			set { throw new NotSupportedException(); }
		}
	}
}
