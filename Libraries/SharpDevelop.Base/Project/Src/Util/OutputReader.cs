// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Util
{
	/// <summary>
	/// A threaded <see cref="Process.StandardOutput"/> or
	/// <see cref="Process.StandardError"/> reader.
	/// </summary>
	public class OutputReader
	{
		StreamReader reader;
		string output = String.Empty;
		Thread thread;
		
		public event LineReceivedEventHandler LineReceived;
		
		public OutputReader(StreamReader reader)
		{
			this.reader = reader;
		}
		
		/// <summary>
		/// Starts reading the output stream.
		/// </summary>
		public void Start()
		{
			thread = new Thread(new ThreadStart(ReadOutput));
			thread.Name = "OutputReader";
			thread.Start();
		}
		
		/// <summary>
		/// Gets the text output read from the reader.
		/// </summary>
		public string Output {
			get {
				return output;
			}
		}
		
		/// <summary>
		/// Waits for the reader to finish.
		/// </summary>
		public void WaitForFinish()
		{
			if (thread != null) {
				thread.Join();
			}
		}
		
		/// <summary>
		/// Raises the <see cref="LineReceived"/> event.
		/// </summary>
		/// <param name="line"></param>
		protected void OnLineReceived(string line)
		{
			if (LineReceived != null) {
				LineReceived(this, new LineReceivedEventArgs(line));
			}
		}
		
		/// <summary>
		/// Reads the output stream on a different thread.
		/// </summary>
		void ReadOutput()
		{
			output = String.Empty;
			StringBuilder outputBuilder = new StringBuilder();
			
			bool endOfStream = false;
			while(!endOfStream)
			{
				string line = reader.ReadLine();
				
				if (line != null) {
					outputBuilder.Append(line);
					outputBuilder.Append(Environment.NewLine);
					OnLineReceived(line);
				} else {
					endOfStream = true;
				}
			} 
			
			output = outputBuilder.ToString();
		}
	}
}
