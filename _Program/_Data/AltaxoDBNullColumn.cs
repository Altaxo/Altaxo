using System;

namespace Altaxo.Data
{
	/// <summary>
	/// AltaxoDBNullColumn serves as a placeholder in case the column
	/// type is not yet known, but some attibutes of the column must
	/// already been set
	/// </summary>
	public class DBNullColumn : DataColumn
	{
		public override System.Type GetColumnStyleType()
		{ 
			return null;
		}
		public override void SetValueAt(int i, AltaxoVariant val)
		{
		}
		public override AltaxoVariant GetVariantAt(int i)
		{
			return null;
		}
		public override bool IsElementEmpty(int i)
		{
			return true;
		}

		public override void CopyDataFrom(Altaxo.Data.DataColumn v)
		{
		}
		public override void RemoveRows(int nFirstRow, int nCount) // removes nCount rows starting from nFirstRow 
		{
		}
		public override void InsertRows(int nBeforeRow, int nCount) // inserts additional empty rows
		{
		}
	}
}
