﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Command called from <see cref="VisualizerPicker"/>.
	/// </summary>
	public interface IVisualizerCommand
	{
		/// <summary>
		/// Can this command execute?
		/// </summary>
		bool CanExecute { get; }
		
		/// <summary>
		/// Executes this visualizer command.
		/// </summary>
		void Execute();
	}
}
