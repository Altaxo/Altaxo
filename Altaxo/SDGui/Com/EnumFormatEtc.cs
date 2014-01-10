using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;  // For use of the GuidAttribute, ProgIdAttribute and ClassInterfaceAttribute.
using System.Runtime.InteropServices.ComTypes;

namespace Altaxo.Com
{
	/// <summary>
	/// Delegate to the procedure that renders data.
	/// </summary>
	/// <param name="tymed">The type of medium to render the data to.</param>
	/// <returns></returns>
	public delegate IntPtr RenderDataProcedure(TYMED tymed);

	/// <summary>
	/// Bundles a rendering format with the corresponding procedure to render this data.
	/// </summary>
	public struct Rendering
	{
		/// <summary>The rendering format.</summary>
		public FORMATETC format;

		/// <summary>The rendering procedure.</summary>
		public RenderDataProcedure renderer;

		public Rendering(short format, TYMED tymed, RenderDataProcedure renderer)
		{
			this.format = new FORMATETC()
			{
				cfFormat = format,
				ptd = IntPtr.Zero,
				dwAspect = DVASPECT.DVASPECT_CONTENT,
				lindex = -1, // all
				tymed = tymed
			};
			this.renderer = renderer;
		}
	}

	/// <summary>
	/// Helps enumerate the formats available in our DataObject class.
	/// </summary>
	[ComVisible(true)]
	public class EnumFormatEtc : ManagedEnumBase<FORMATETC>, IEnumFORMATETC
	{
		public EnumFormatEtc(IEnumerable<FORMATETC> formats)
			: base(formats)
		{
		}

		public EnumFormatEtc(EnumFormatEtc from)
			: base(from)
		{
		}

		public void Clone(out IEnumFORMATETC newEnum)
		{
			newEnum = new EnumFormatEtc(this);
		}
	}
}