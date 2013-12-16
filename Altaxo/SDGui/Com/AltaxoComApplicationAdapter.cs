using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Com
{
	/// <summary>
	/// Manages the connection of the Com functions to our application
	/// </summary>
	public class AltaxoComApplicationAdapter
	{
		/// <summary>
		/// Sets the host names. The application should show the containerApplicationName and containerFileName in the title bar. Additionally, if <paramref name="isInEmbeddedMode"/> is <c>true</c>,
		/// the application should switch the user interface.
		/// </summary>
		/// <param name="containerApplicationName">Name of the container application.</param>
		/// <param name="containerFileName">Name of the container file.</param>
		/// <param name="isInEmbeddedMode">if set to <c>true</c> the application is in embedded mode.</param>
		public void SetHostNames(string containerApplicationName, string containerFileName, bool isInEmbeddedMode)
		{
			// see Brockschmidt, Inside Ole 2nd ed. page 992
			// calling SetHostNames is the only sign that our object is embedded (and thus not linked)
			// this means that we have to switch the user interface from within this function
			
#if COMLOGGING
			Debug.ReportInfo("IOleObject.SetHostNames szContainerApp={0}, szContainerObj={1}", szContainerApp, szContainerObj);
#endif

			string title = string.Format("{0} - {1}", containerApplicationName, containerFileName);
			Current.Gui.BeginExecute(new Action(
				() => 
				{
					((System.Windows.Window)Current.Workbench.ViewObject).Title = title;
				})
			);

		}


		/// <summary>
		/// Starts the closing of the application asynchronously.
		/// </summary>
		public void BeginClosingApplication()
		{
			Current.Gui.BeginExecute(new Action(() => ((System.Windows.Window)Current.Workbench.ViewObject).Close())); // Begin Closing the main window
		}


		/// <summary>
		/// Makes the main window invisible to the user (but doesn't close the application);
		/// </summary>
		public void HideMainWindow()
		{
			Action hiding = () =>
			{
#if COMLOGGING
							Debug.ReportInfo("Hide main window");
#endif
				((System.Windows.Window)Current.Workbench.ViewObject).ShowInTaskbar = false;
				((System.Windows.Window)Current.Workbench.ViewObject).Visibility = System.Windows.Visibility.Hidden;
			};
			Current.Gui.Execute(hiding);
		}


		/// <summary>
		/// Makes the main window of the application visible to the user.
		/// </summary>
		public void ShowMainWindow()
		{
			Current.Gui.Execute(() =>
			{
#if COMLOGGING
							Debug.ReportInfo("Make main window visible");
#endif
				((System.Windows.Window)Current.Workbench.ViewObject).ShowInTaskbar = true;
				((System.Windows.Window)Current.Workbench.ViewObject).Visibility = System.Windows.Visibility.Visible;
			});
		}

	}
}
