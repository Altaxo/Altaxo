// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3770 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.BuildWorker.Interprocess
{
	public interface IHostObject
	{
		void ReportException(string exceptionText);
	}
}
