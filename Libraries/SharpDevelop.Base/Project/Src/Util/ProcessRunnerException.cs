// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1092 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// An exception thrown by a <see cref="ProcessRunner"/>
	/// instance.
	/// </summary>
	public class ProcessRunnerException : ApplicationException
	{
		public ProcessRunnerException(string message)
			: base(message)
		{
		}
	}
}
