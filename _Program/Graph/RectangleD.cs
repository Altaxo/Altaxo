using System;

namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for RectangleD.
	/// </summary>
	[Serializable]
	public struct RectangleD
	{
		private double x, y, width, height;

		public RectangleD(double x, double y, double width, double height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public double X
		{
			get { return x; }
			set { x = value; }
		}

		public double Y
		{
			get { return y; }
			set { y = value; }
		}

		public double Width
		{
			get { return width; }
			set { width = value; }
		}

		public double Height
		{
			get { return height; }
			set { height = value; }
		}
	}
}
