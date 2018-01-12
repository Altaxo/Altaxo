using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
	/// <summary>
	/// Interface to a service that auto update the application.
	/// </summary>
	public interface IAutoUpdateInstallationService
	{
		/// <summary>Starts the installer program, when all presumtions are fullfilled.</summary>
		/// <param name="isApplicationCurrentlyStarting">If set to <c>true</c>, the application will be restarted after the installation is done.</param>
		/// <param name="commandLineArgs">Original command line arguments. Can be <c>null</c> when calling this function on shutdown.</param>
		/// <returns>True if the installer program was started. Then the application have to be shut down immediately. Returns <c>false</c> if the installer program was not started.</returns>
		bool Run(bool isApplicationCurrentlyStarting, string[] commandLineArgs);
	}
}