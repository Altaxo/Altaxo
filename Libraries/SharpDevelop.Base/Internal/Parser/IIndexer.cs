// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;

namespace SharpDevelop.Internal.Parser
{
	public interface IIndexer: IMember
	{
		IRegion Region {
			get;
		}
		
		IRegion BodyRegion {
			get;
		}
		
		IRegion GetterRegion {
			get;
		}

		IRegion SetterRegion {
			get;
		}

		ParameterCollection Parameters {
			get;
		}
	}
}
