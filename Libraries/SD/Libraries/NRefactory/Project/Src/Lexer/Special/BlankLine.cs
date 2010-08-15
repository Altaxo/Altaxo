﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="unknown"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;

namespace ICSharpCode.NRefactory
{
	public class BlankLine : AbstractSpecial
	{
		public BlankLine(Location point) : base(point)
		{
		}
		
		public override object AcceptVisitor(ISpecialVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
