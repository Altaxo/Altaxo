// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class NoStartupCombineDefinedException : System.Exception
	{
		public NoStartupCombineDefinedException()
		{
		}
		public NoStartupCombineDefinedException(string msg) : base(msg)
		{
		}
	}
	
}
