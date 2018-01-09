// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using Altaxo.Main.Services.Implementation;

using System;

using System.Collections.ObjectModel;

namespace Altaxo.Main.Services
{
	public struct InfoWarningErrorTextMessageItem
	{
		public MessageLevel Level { get; set; }
		public string Source { get; set; }
		public string Message { get; set; }
		public DateTime TimeUtc { get; set; }

		public DateTime Time
		{
			get { return TimeUtc.ToLocalTime(); }
		}

		public InfoWarningErrorTextMessageItem(MessageLevel level, string source, string message, DateTime timeUtc)
		{
			Level = level;
			Source = source;
			Message = message;
			TimeUtc = timeUtc;
		}
	}

	/// <summary>
	/// Service that usually display infos, warnings, and errors in a pad window at the bottom of the workbench. Typically,
	/// the messages will be displayed in different colors according to their warning level.
	/// </summary>
	[GlobalService("InfoWarningErrorTextMessageService")]
	public interface IInfoWarningErrorTextMessageService
	{
		void WriteLine(MessageLevel messageLevel, string source, string message);

		void WriteLine(MessageLevel messageLevel, string source, string format, params object[] args);

		void WriteLine(MessageLevel messageLevel, string source, System.IFormatProvider provider, string format, params object[] args);

		event Action<InfoWarningErrorTextMessageItem> MessageAdded;
	}
}