using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Pads.ProjectBrowser
{

	/// <summary>
	/// Helper class to store navigation points in order to move back or forward in history.
	/// </summary>
	internal struct NavigationPoint : IEquatable<NavigationPoint>
	{
		/// <summary>
		/// Enumerates the possible kinds of navigation points.
		/// </summary>
		public enum KindOfNavigationPoint { AllProjectItems, AllTables, AllGraphs, ProjectFolder }

		/// <summary>Gets or sets the kind of navigation point.</summary>
		/// <value>The kind.</value>
		public KindOfNavigationPoint Kind { get; set; }

		/// <summary>Gets or sets the folder name if Kind is ProjectFolder.</summary>
		/// <value>The folder name.</value>
		public string Folder { get; set; }

		public bool Equals(NavigationPoint other)
		{
			if (this.Kind != other.Kind)
				return false;

			if (this.Kind == KindOfNavigationPoint.ProjectFolder && this.Folder != other.Folder)
				return false;

			return true;
		}
	}
}
