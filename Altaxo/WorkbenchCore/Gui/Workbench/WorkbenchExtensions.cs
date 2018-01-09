using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
	public static class WorkbenchExtensions
	{
		/// <summary>
		/// Equivalent to <code>SD.Workbench.ActiveViewContent.GetService(type)</code>,
		/// but does not throw a NullReferenceException when ActiveViewContent is null.
		/// (instead, null is returned).
		/// </summary>
		public static object GetActiveViewContentService(Type type)
		{
			var workbench = Altaxo.Current.GetRequiredService<IWorkbenchEx>();
			if (workbench != null)
			{
				var activeViewContent = workbench.ActiveViewContent;
				if (activeViewContent != null)
				{
					return activeViewContent.GetService(type);
				}
			}
			return null;
		}
	}
}