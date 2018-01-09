using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Units
{
	/// <summary>
	/// Service that manages a single instance of <see cref="UserDefinedUnitEnvironments"/>. During creation, it reads the user defined unit environments from
	/// the property service. During disposal, it writes the environments back to the property service.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public class UserDefinedUnitEnvironmentsService : IDisposable
	{
		/// <summary>
		/// Gets the user defined unit environments.
		/// </summary>
		/// <value>
		/// The user defined unit environments.
		/// </value>
		public UserDefinedUnitEnvironments Environments { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UserDefinedUnitEnvironmentsService"/> class.
		/// Reads the environments from the property service.
		/// </summary>
		public UserDefinedUnitEnvironmentsService()
		{
			Environments = Current.PropertyService.GetValue(UserDefinedUnitEnvironments.PropertyKeyDefaultInstance,
					Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);
		}

		/// <summary>
		/// A call to Dispose signals that this service is about to shutdown.
		/// Thus we store all unit environments in the property service
		/// </summary>
		public void Dispose()
		{
			Current.PropertyService.SetValue(UserDefinedUnitEnvironments.PropertyKeyDefaultInstance, Environments);
		}
	}
}