#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Worksheet.Commands.Analysis
{
	public class FourierTransformation2DDataSource : Altaxo.Data.IAltaxoTableDataSource
	{
		private RealFourierTransformation2DOptions _transformationOptions;
		private DataTableMatrixProxy _inputData;
		private IDataSourceImportOptions _importOptions;

		public event Action<IAltaxoTableDataSource> DataSourceChanged;

		protected Main.EventSuppressor _eventSuppressor;

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2014-07-08 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourierTransformation2DDataSource), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (FourierTransformation2DDataSource)obj;

				info.AddValue("InputData", s._inputData);
				info.AddValue("TransformationOptions", s._transformationOptions);
				info.AddValue("ImportOptions", s._importOptions);
			}

			protected virtual FourierTransformation2DDataSource SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new FourierTransformation2DDataSource() : (FourierTransformation2DDataSource)o);

				s._inputData = (DataTableMatrixProxy)info.GetValue("InputData");
				s._transformationOptions = (RealFourierTransformation2DOptions)info.GetValue("TransformationOptions");
				s._importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions");

				s.InputData = s._inputData;

				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		protected FourierTransformation2DDataSource()
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);
		}

		public FourierTransformation2DDataSource(DataTableMatrixProxy inputData, RealFourierTransformation2DOptions transformationOptions, IDataSourceImportOptions importOptions)
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);

			if (null == inputData)
				throw new ArgumentNullException("inputData");
			if (null == transformationOptions)
				throw new ArgumentNullException("transformationOptions");
			if (null == importOptions)
				throw new ArgumentNullException("importOptions");

			using (var token = SuppressEventsGettingToken())
			{
				this.FourierTransformation2DOptions = transformationOptions;
				this.ImportOptions = importOptions;
				this.InputData = inputData;
			}
		}

		public FourierTransformation2DDataSource(FourierTransformation2DDataSource from)
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);

			CopyFrom(from);
		}

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as FourierTransformation2DDataSource;
			if (null != from)
			{
				using (var token = SuppressEventsGettingToken())
				{
					RealFourierTransformation2DOptions transformationOptions = null;
					DataTableMatrixProxy inputData = null;
					IDataSourceImportOptions importOptions = null;

					CopyHelper.Copy(ref importOptions, from._importOptions);
					CopyHelper.Copy(ref transformationOptions, from._transformationOptions);
					CopyHelper.Copy(ref inputData, from._inputData);

					this.FourierTransformation2DOptions = transformationOptions;
					this.ImportOptions = importOptions;
					this.InputData = inputData;

					return true;
				}
			}
			return false;
		}

		public object Clone()
		{
			return new FourierTransformation2DDataSource(this);
		}

		public void FillData(DataTable destinationTable)
		{
			try
			{
				FourierCommands.ExecuteFouriertransformation2D(_inputData, _transformationOptions, destinationTable);
			}
			catch (Exception ex)
			{
				destinationTable.Notes.WriteLine("Error during execution of data source ({0}): {1}", this.GetType().Name, ex.Message);
			}
		}

		public DataTableMatrixProxy InputData
		{
			get
			{
				return _inputData;
			}
			set
			{
				if (null != _inputData)
				{
					_inputData.Changed -= EhInputDataChanged;
				}

				_inputData = value;

				if (null != _inputData)
				{
					_inputData.Changed += EhInputDataChanged;
					EhInputDataChanged(this, EventArgs.Empty);
				}
			}
		}

		public Data.IDataSourceImportOptions ImportOptions
		{
			get
			{
				return _importOptions;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("ImportOptions");

				var oldValue = _importOptions;

				_importOptions = value;
			}
		}

		public RealFourierTransformation2DOptions FourierTransformation2DOptions
		{
			get
			{
				return _transformationOptions;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("FourierTransformation2DOptions");

				var oldValue = _transformationOptions;

				_transformationOptions = value;
			}
		}

		private void EhInputDataChanged(object sender, EventArgs e)
		{
			if (_importOptions.ImportTriggerSource == ImportTriggerSource.DataSourceChanged)
			{
				if (_eventSuppressor.GetEnabledWithCounting())
				{
					var ev = DataSourceChanged;
					if (null != ev)
						ev(this);
				}
			}
		}

		private void EhResumeSuppressedEvents()
		{
			var ev = DataSourceChanged;
			if (null != ev)
				ev(this);
		}

		public void OnAfterDeserialization()
		{
		}

		public void Dispose()
		{
		}

		public Main.ISuppressToken SuppressEventsGettingToken()
		{
			return _eventSuppressor.Suspend();
		}

		public void ResumeEvents(ref Main.ISuppressToken token)
		{
			_eventSuppressor.Resume(ref token);
		}
	}
}