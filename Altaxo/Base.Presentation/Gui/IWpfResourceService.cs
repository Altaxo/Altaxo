using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui
{
	/// <summary>
	/// Service that handles resources especially for Wpf
	/// </summary>
	public interface IWpfResourceService
	{
		/// <summary>
		/// Creates a new System.Windows.Controls.Image object containing the image with the
		/// specified resource name.
		/// </summary>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		System.Windows.Controls.Image GetImage(string name);

		/// <summary>
		/// Returns a BitmapSource from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		BitmapSource GetBitmapSource(string name);
	}

	public static class WpfResourceService
	{
		private static IWpfResourceService _instance;

		public static IWpfResourceService Instance
		{
			get
			{
				return _instance;
			}
		}

		public static void InitializeWpfResourceService(IWpfResourceService instance)
		{
			if (instance == null)
				throw new ArgumentNullException();
			_instance = instance;
		}
	}
}