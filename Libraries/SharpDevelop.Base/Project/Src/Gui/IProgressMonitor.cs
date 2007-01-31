﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2059 $</version>
// </file>

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is a basic interface to a "progress bar" type of
	/// control.
	/// </summary>
	public interface IProgressMonitor
	{
		/// <summary>
		/// Begins a new task with the specified name and total amount of work.
		/// </summary>
		/// <param name="name">Name of the task. Use null to display a default message</param>
		/// <param name="totalWork">Total amount of work in work units. Use 0 for unknown amount of work.</param>
		/// <param name="allowCancel">Specifies whether the task can be cancelled.</param>
		void BeginTask(string name, int totalWork, bool allowCancel);
		
		/// <summary>
		/// Gets/Sets the amount of work already done
		/// </summary>
		int WorkDone {
			get;
			set;
		}
		
		/// <summary>
		/// Marks the current task as Done.
		/// </summary>
		void Done();
		
		/// <summary>
		/// Gets/Sets the current task name.
		/// </summary>
		string TaskName {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/sets if the task current shows a modal dialog. Set this property to true to make progress dialogs windows
		/// temporarily invisible while your modal dialog is showing.
		/// </summary>
		bool ShowingDialog {
			get;
			set;
		}
		
		/// <summary>
		/// Gets whether the user has cancelled the operation.
		/// </summary>
		bool IsCancelled {
			get;
		}
	}
	
	internal class DummyProgressMonitor : IProgressMonitor
	{
		int workDone;
		string taskName;
		bool showingDialog;
		
		public int WorkDone {
			get { return workDone; }
			set { workDone = value; }
		}
		
		public string TaskName {
			get { return taskName; }
			set { taskName = value; }
		}
		
		public void BeginTask(string name, int totalWork, bool allowCancel)
		{
			taskName = name;
			workDone = 0;
		}
		
		public void Done()
		{
		}
		
		public bool IsCancelled {
			get { return false; }
		}
		
		public bool ShowingDialog {
			get { return showingDialog; }
			set { showingDialog = value; }
		}
	}
}
