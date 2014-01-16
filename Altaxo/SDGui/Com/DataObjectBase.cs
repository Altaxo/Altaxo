using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Kernel32;
	using UnmanagedApi.Ole32;

	/// <summary>
	/// Base for the implementation of classed that use IDataObject. Note that this class does not implement IDataObject, but all functions in IDataObject are implemented here.
	/// </summary>
	public abstract class DataObjectBase : ReferenceCountedObjectBase
	{
		public DataObjectBase(ComManager comManager)
			: base(comManager)
		{
		}

		#region Abstracts

		/// <summary>
		/// Gets a data advise holder. You can return <c>null</c> if a data advise holder is unneccessary, e.g. for static data objects.
		/// Otherwise, you should maintain an instance of <see cref="ManagedDataAdviseHolder"/> in your derived class and return it here.
		/// </summary>
		/// <value>
		/// The data advise holder.
		/// </value>
		protected abstract ManagedDataAdviseHolder DataAdviseHolder { get; }

		/// <summary>
		/// Gets the list of all renderings that are currently supported.
		/// </summary>
		/// <value>
		/// The renderings.
		/// </value>
		protected abstract IList<Rendering> Renderings { get; }

		/// <summary>
		/// Internal implementation of the GetDataHere procedure. It is not neccessary to catch exceptions into this implementations, since the exceptions are catched and reported
		/// in the outer <see cref="GetDataHere"/> function.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="medium">The medium.</param>
		/// <returns><c>True</c> if the data could be provided, otherwise <c>False</c>.</returns>
		protected abstract bool InternalGetDataHere(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium);

		/// <summary>
		/// Converts this data object to a .NET data object (supports only those formats that can be supported by the .NET data object).
		/// This base implementation just does nothing. You have to override this function in order to implement some functionality.
		/// </summary>
		/// <returns>The .NET data object.</returns>
		public virtual void ConvertToNetDataObjectAndPutToClipboard()
		{
		}

		#endregion Abstracts

		#region Advise function

		public virtual void SendAdvise_DataChanged()
		{
			if (null != DataAdviseHolder)
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.SendAdvise.DataChanged -> Calling _dataAdviseHolder.SendOnDataChange()", this.GetType().Name);
#endif
				DataAdviseHolder.SendOnDataChange((IDataObject)this, 0, 0);
			}
		}

		#endregion Advise function

		#region Implementation of IDataObject

		public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
		{
			if (null == DataAdviseHolder)
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IDataObject.DAdvise -> not implemented!", this.GetType().Name);
#endif
				connection = 0;
				return ComReturnValue.E_NOTIMPL;
			}
			else
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IDataObject.DAdvise {1}, {2}", this.GetType().Name, DataObjectHelper.FormatEtcToString(pFormatetc), advf);
#endif

				try
				{
					if (pFormatetc.cfFormat != 0) // if a special format is required
					{
						int res = QueryGetData(ref pFormatetc); // ask the render helper for availability of that format
						if (res != ComReturnValue.S_OK) // if the required format is not available
						{
							connection = 0; //  return an invalid connection cookie
							return res; // and the error
						}
					}
					FORMATETC etc = pFormatetc;
					int conn = 0;
					DataAdviseHolder.Advise((IDataObject)this, ref etc, advf, adviseSink, out conn);
					connection = conn;
					return ComReturnValue.NOERROR;
				}
				catch (Exception e)
				{
#if COMLOGGING
					Debug.ReportError("{0}.IDataObject.DAdvise exception: {1}", this.GetType().Name, e);
#endif
					throw;
				}
			}
		}

		public void DUnadvise(int connection)
		{
			if (null == DataAdviseHolder)
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IDataObject.DUnadvise connection={1} -> not implemented!", this.GetType().Name, connection);
#endif
				return;
			}
			else
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IDataObject.DUnadvise connection={1}", this.GetType().Name, connection);
#endif
				try
				{
					DataAdviseHolder.Unadvise(connection);
				}
				catch (Exception e)
				{
#if COMLOGGING
					Debug.ReportError("{0}.IDataObject.DUnadvise exception {1}", this.GetType().Name, e);
#endif
					throw;
				}
			}
		}

		public int EnumDAdvise(out IEnumSTATDATA enumAdvise)
		{
			if (null == DataAdviseHolder)
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IDataObject.EnumAdvise -> not implemented!", this.GetType().Name);
#endif
				enumAdvise = null;
				return ComReturnValue.E_NOTIMPL;
			}
			else
			{
#if COMLOGGING
				Debug.ReportInfo("{0}.IDataObject.EnumAdvise", this.GetType().Name);
#endif
				enumAdvise = DataAdviseHolder.EnumAdvise();
				return ComReturnValue.S_OK;
			}
		}

		public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IDataObject.EnumFormatEtc", this.GetType().Name);
#endif
			try
			{
				// We only support GET
				if (DATADIR.DATADIR_GET == direction)
					return new EnumFormatEtc(new List<FORMATETC>(Renderings.Select(x => x.format)));
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IDataObject.EnumFormatEtc exception: {1}", this.GetType().Name, e);
#endif
				throw;
			}

			throw new NotImplementedException("Can not use registry here because a return value is not supported");
		}

		public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IDataObject.GetCanonicalFormatEtc {1}", this.GetType().Name, DataObjectHelper.FormatEtcToString(formatIn));
#endif

			formatOut = formatIn;

			return ComReturnValue.DV_E_FORMATETC;
		}

		public void GetData(ref FORMATETC format, out STGMEDIUM medium)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IDataObject.GetData({1})", this.GetType().Name, DataObjectHelper.FormatEtcToString(format));
#endif

			try
			{
				// Locate the data
				foreach (var rendering in Renderings)
				{
					if ((rendering.format.tymed & format.tymed) > 0
							&& rendering.format.dwAspect == format.dwAspect
							&& rendering.format.cfFormat == format.cfFormat
							&& rendering.renderer != null)
					{
						// Found it. Return a copy of the data.

						medium = new STGMEDIUM();
						medium.tymed = format.tymed;
						medium.unionmember = rendering.renderer(format.tymed);
						if (medium.tymed == TYMED.TYMED_ISTORAGE || medium.tymed == TYMED.TYMED_ISTREAM)
							medium.pUnkForRelease = Marshal.GetObjectForIUnknown(medium.unionmember);
						else
							medium.pUnkForRelease = null;
						return;
					}
				}
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IDataObject.GetData threw an exception {1}", this.GetType().Name, e);
#endif
				throw;
			}

#if COMLOGGING
			Debug.ReportInfo("{0}.IDataObject.GetData, no data delivered!", this.GetType().Name);
#endif
			medium = new STGMEDIUM();
			// Marshal.ThrowExceptionForHR(ComReturnValue.DV_E_FORMATETC);
		}

		public void GetDataHere(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IDataObject.GetDataHere({1})", this.GetType().Name, DataObjectHelper.ClipboardFormatName(format.cfFormat));
#endif
			// Allows containers to duplicate this into their own storage.
			try
			{
				if (InternalGetDataHere(ref format, ref medium))
					return; // data could be provided
			}
			catch (Exception e)
			{
#if COMLOGGING
				Debug.ReportError("{0}.IDataObject.GetDataHere threw an exception: {1}", this.GetType().Name, e);
#endif
				throw;
			}
			Marshal.ThrowExceptionForHR(ComReturnValue.DATA_E_FORMATETC);
		}

		public int QueryGetData(ref FORMATETC format)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.IDataObject.QueryGetData, tymed={1}, aspect={2}", this.GetType().Name, format.tymed, format.dwAspect);
#endif

			// We only support CONTENT aspect
			if ((DVASPECT.DVASPECT_CONTENT & format.dwAspect) == 0)
			{
				return ComReturnValue.DV_E_DVASPECT;
			}

			int ret = ComReturnValue.DV_E_TYMED;

			// Try to locate the data
			// TODO: The ret, if not S_OK, is only relevant to the last item
			foreach (var rendering in Renderings)
			{
				if ((rendering.format.tymed & format.tymed) > 0)
				{
					if (rendering.format.cfFormat == format.cfFormat)
					{
						// Found it, return S_OK;
						return ComReturnValue.S_OK;
					}
					else
					{
						// Found the medium type, but wrong format
						ret = ComReturnValue.DV_E_FORMATETC;
					}
				}
				else
				{
					// Mismatch on medium type
					ret = ComReturnValue.DV_E_TYMED;
				}
			}

#if COMLOGGING
			Debug.ReportInfo("{0}.IDataObject.QueryGetData is returning 0x{1:X}", this.GetType().Name, ret);
#endif
			return ret;
		}

		public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
		{
#if COMLOGGING
			Debug.ReportError("{0}.IDataObject.SetData - NOT SUPPORTED!", this.GetType().Name);
#endif
			throw new NotSupportedException();
		}

		#endregion Implementation of IDataObject
	}
}