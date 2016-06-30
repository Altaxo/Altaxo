using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	public class TransformedReadableColumn : ITransformedReadableColumn, Main.IImmutable
	{
		private IReadableColumn _originalColumn;
		private IVariantToVariantTransformation _transformation;

		#region Serialization

		// This type has not serialization.
		// Serialization should be done via the corresponding Proxy type.

		#endregion Serialization

		public TransformedReadableColumn(IReadableColumn column, IVariantToVariantTransformation transformation)
		{
			if (null == column)
				throw new ArgumentNullException(nameof(column));

			if (null == transformation)
				throw new ArgumentNullException(nameof(transformation));

			_originalColumn = column;
			_transformation = transformation;
		}

		public AltaxoVariant this[int i]
		{
			get
			{
				return _transformation.Transform(_originalColumn[i]);
			}
		}

		public int? Count
		{
			get
			{
				return _originalColumn.Count;
			}
		}

		public string FullName
		{
			get
			{
				return (_transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction) + " " + _originalColumn.FullName;
			}
		}

		public IReadableColumn UnderlyingReadableColumn
		{
			get
			{
				return _originalColumn;
			}
		}

		public IVariantToVariantTransformation Transformation
		{
			get
			{
				return _transformation;
			}
		}

		public object Clone()
		{
			return this; // this class is immutable
		}

		public bool IsElementEmpty(int i)
		{
			if (_originalColumn.IsElementEmpty(i))
				return true;
			var val = _transformation.Transform(_originalColumn[i]);
			if (val.IsType(AltaxoVariant.Content.VDouble) && double.IsNaN(val))
				return true;
			if (val.IsType(AltaxoVariant.Content.VString) && null == (string)val)
				return true;

			return false;
		}

		public ITransformedReadableColumn WithUnderlyingReadableColumn(IReadableColumn originalReadableColumn)
		{
			if (object.Equals(_originalColumn, originalReadableColumn))
			{
				return this;
			}
			else
			{
				if (null == originalReadableColumn)
					throw new ArgumentNullException(nameof(originalReadableColumn));
				var result = (TransformedReadableColumn)this.MemberwiseClone();
				result._originalColumn = originalReadableColumn;
				return result;
			}
		}

		public ITransformedReadableColumn WithTransformation(IVariantToVariantTransformation transformation)
		{
			if (object.Equals(_transformation, transformation))
			{
				return this;
			}
			else
			{
				if (null == transformation)
					throw new ArgumentNullException(nameof(transformation));
				var result = (TransformedReadableColumn)this.MemberwiseClone();
				result._transformation = transformation;
				return result;
			}
		}
	}
}