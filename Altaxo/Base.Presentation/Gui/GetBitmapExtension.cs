using System;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui
{
	/// <summary>
	/// Markup extension that gets a BitmapSource object for a ResourceService bitmap.
	/// </summary>
	[MarkupExtensionReturnType(typeof(BitmapSource))]
	public class GetBitmapExtension : MarkupExtension
	{
		protected string _key;

		public GetBitmapExtension(string key)
		{
			this._key = key;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var instance = WpfResourceService.Instance;
			if (null != instance)
				return WpfResourceService.Instance.GetBitmapSource(_key);
			else return _key;
		}
	}
}