// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1092 $</version>
// </file>

using NUnit.Framework;
using ICSharpCode.SharpDevelop.Util;
using System;
using System.Collections;
using System.IO;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests the <see cref="ProcessRunner.LineReceived"/> event.
	/// </summary>
	[TestFixture]
	public class LineReceivedFromProcessTestFixture : ConsoleAppTestFixtureBase
	{
		ProcessRunner runner;
		ArrayList lines;
		
		[SetUp]
		public void Init()
		{
			lines = new ArrayList();
			runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
		}
		
		[Test]
		public void SingleLineOutput()
		{			
			string echoText = "Test";
			string expectedOutput = String.Concat(echoText, "\r\n");
			
			runner.OutputLineReceived += new LineReceivedEventHandler(OutputLineReceived);
			
			runner.Start(GetConsoleAppFileName(), String.Concat("-echo:", echoText));
			runner.WaitForExit();
			
			Assert.AreEqual(0, runner.ExitCode, "Exit code should be zero.");
			Assert.AreEqual(expectedOutput, runner.StandardOutput, "Should have some output.");
			Assert.AreEqual(String.Empty, runner.StandardError, "Should not be any error output.");			
			Assert.AreEqual(1, lines.Count, "Should only have one output line.");
			Assert.AreEqual(echoText, lines[0], "Line received is incorrect.");
		}
		
		void OutputLineReceived(object sender, LineReceivedEventArgs e)
		{
			lines.Add(e.Line);
		}
	}
}
