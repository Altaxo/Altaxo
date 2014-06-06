using System;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui
{
	/// <summary>
	/// Markup extension that gets an Image with the size of 16 x 16 directly for usage in buttons, tree view items etc.
	/// </summary>
	[MarkupExtensionReturnType(typeof(Image))]
	public class GetIconExtension : MarkupExtension
	{
		protected string _key;

		public GetIconExtension(string key)
		{
			this._key = key;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var instance = WpfResourceService.Instance;
			if (null != instance)
			{
				var imgSource = WpfResourceService.Instance.GetBitmapSource(_key);
				var image = new System.Windows.Controls.Image();
				image.Height = 16;
				image.Width = 16;
				image.Source = imgSource;
				return image;
			}
			else
			{
				return new Image();
			}
		}
	}
}