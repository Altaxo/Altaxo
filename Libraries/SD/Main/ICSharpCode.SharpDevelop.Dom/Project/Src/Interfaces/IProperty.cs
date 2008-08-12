﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface IProperty : IMethodOrProperty
	{
		DomRegion GetterRegion {
			get;
		}

		DomRegion SetterRegion {
			get;
		}

		bool CanGet {
			get;
		}

		bool CanSet {
			get;
		}
		
		bool IsIndexer {
			get;
		}
	}
}
