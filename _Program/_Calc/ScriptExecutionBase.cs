using System;

namespace Altaxo.Calc
{
	/// <summary>
	/// ScriptExecuteBase provides the environment to execute scripts in altaxo
	/// it provides shortcuts for math functions
	/// and the mathematical functions with column types
	/// </summary>
	public class ScriptExecutionBase 
	{
		#region Double_Mathematics
		// ------------------- Double Mathematics --------------------------------------
		public const double E = System.Math.E;
		public const double PI = System.Math.PI;
		public static double Abs(double s) { return System.Math.Abs(s); }
		public static double Acos(double s) { return System.Math.Acos(s); }
		public static double Asin(double s) { return System.Math.Asin(s); }
		public static double Atan(double s) { return System.Math.Atan(s); }
		public static double Atan2(double y, double x) { return System.Math.Atan2(y,x); }
		public static double Ceiling(double s) { return System.Math.Ceiling(s); }
		public static double Cos(double s) { return System.Math.Cos(s); }
		public static double Cosh(double s) { return System.Math.Cosh(s); }
		public static double Exp(double s) { return System.Math.Exp(s); }
		public static double Floor(double s) { return System.Math.Floor(s); }
		public static double IEEERemainder(double x, double y) { return System.Math.IEEERemainder(x,y); }
		public static double Log(double s) { return System.Math.Log(s); }
		public static double Log(double s, double bas) { return System.Math.Log(s,bas); }
		public static double Max(double x, double y) { return System.Math.Max(x,y); }
		public static double Min(double x, double y) { return System.Math.Min(x,y); }
		public static double Pow(double x, double y) { return System.Math.Pow(x,y); }
		public static double Round(double x) { return System.Math.Round(x); }
		public static double Round(double x, int i) { return System.Math.Round(x,i); }
		public static double Sign(double s) { return System.Math.Sign(s); }
		public static double Sin(double s) { return System.Math.Sin(s); }
		public static double Sinh(double s) { return System.Math.Sinh(s); }
		public static double Sqrt(double s) { return System.Math.Sqrt(s); }
		public static double Tan(double s) { return System.Math.Tan(s); }
		public static double Tanh(double s) { return System.Math.Tanh(s); }
		#endregion
		

		#region AltaxoDoubleColumn_Mathematics
		// ---------------------- DoubleColumn mathematics -----------------------------
		
		public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Abs(s); }
		public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Acos(s); }
		public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Asin(s); }
		public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Atan(s); }
		public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn y, Altaxo.Data.DoubleColumn x) { return Altaxo.Data.DoubleColumn.Atan2(y,x); }
		public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn y, double x) { return Altaxo.Data.DoubleColumn.Atan2(y,x); }
		public static Altaxo.Data.DoubleColumn Atan2(double y, Altaxo.Data.DoubleColumn x) { return Altaxo.Data.DoubleColumn.Atan2(y,x); }
		public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Ceiling(s); }
		public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Cos(s); }
		public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Cosh(s); }
		public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Exp(s); }
		public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Floor(s); }
		public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.IEEERemainder(x,y); }
		public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn x, double y) { return Altaxo.Data.DoubleColumn.IEEERemainder(x,y); }
		public static Altaxo.Data.DoubleColumn IEEERemainder(double x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.IEEERemainder(x,y); }
		public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Log(s); }
		public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s, Altaxo.Data.DoubleColumn bas) { return Altaxo.Data.DoubleColumn.Log(s,bas); }
		public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn s, double bas) { return Altaxo.Data.DoubleColumn.Log(s,bas); }
		public static Altaxo.Data.DoubleColumn Log(double s, Altaxo.Data.DoubleColumn bas) { return Altaxo.Data.DoubleColumn.Log(s,bas); }
		public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.Max(x,y); }
		public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn x, double y) { return Altaxo.Data.DoubleColumn.Max(x,y); }
		public static Altaxo.Data.DoubleColumn Max(double x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.Max(x,y); }
		public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.Min(x,y); }
		public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn x, double y) { return Altaxo.Data.DoubleColumn.Min(x,y); }
		public static Altaxo.Data.DoubleColumn Min(double x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.Min(x,y); }
		public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.Pow(x,y); }
		public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn x, double y) { return Altaxo.Data.DoubleColumn.Pow(x,y); }
		public static Altaxo.Data.DoubleColumn Pow(double x, Altaxo.Data.DoubleColumn y) { return Altaxo.Data.DoubleColumn.Pow(x,y); }
		public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x) { return Altaxo.Data.DoubleColumn.Round(x); }
		public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x, Altaxo.Data.DoubleColumn i) { return Altaxo.Data.DoubleColumn.Round(x,i); }
		public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn x, int i) { return Altaxo.Data.DoubleColumn.Round(x,i); }
		public static Altaxo.Data.DoubleColumn Round(double x, Altaxo.Data.DoubleColumn i) { return Altaxo.Data.DoubleColumn.Round(x,i); }
		public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Sign(s); }
		public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Sin(s); }
		public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Sinh(s); }
		public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Sqrt(s); }
		public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Tan(s); }
		public static Altaxo.Data.DoubleColumn Tanh(Altaxo.Data.DoubleColumn s) { return Altaxo.Data.DoubleColumn.Tanh(s); }
		#endregion


		#region AltaxoDataColumn_Mathematics
		// ------------------------- Altaxo.Data.DataColumn mathematics ----------------------------
		public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Abs((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Abs() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Acos((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Acos() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Asin((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Asin() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Atan((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Atan() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DataColumn y, Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType() && typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Atan2((Altaxo.Data.DoubleColumn)y,(Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Atan2() to " + y.TypeAndName + " and " + x.TypeAndName ,"x");
		}
		public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DataColumn y, double x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Atan2((Altaxo.Data.DoubleColumn)y,x);
			else throw new ArgumentException("Error: Try to apply Atan2() to " + y.TypeAndName + " and " + x.GetType() ,"x");
		}

		public static Altaxo.Data.DoubleColumn Atan2(double y, Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Atan2(y,(Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Atan2() to " + y.GetType() + " and " + x.TypeAndName ,"x");
		}


		public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Ceiling((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Ceiling() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Cos((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Cos() to " + x.TypeAndName,"x");
		}


		public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Cosh((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Cosh() to " + x.TypeAndName,"x");
		}
	
		public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Exp((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Exp() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Floor((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Floor() to " + x.TypeAndName,"x");
		}


		public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType() && typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.IEEERemainder((Altaxo.Data.DoubleColumn)x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.TypeAndName + " and " + y.TypeAndName,"x");
		}
		public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DataColumn x, double y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.IEEERemainder((Altaxo.Data.DoubleColumn)x,y);
			else throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(),"x");
		}
		public static Altaxo.Data.DoubleColumn IEEERemainder(double x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.IEEERemainder(x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply IEEERemainder() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName,"x");
		}



		public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType() && typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName + " and " + y.TypeAndName,"x");
		}
		public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DataColumn x, double y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Log((Altaxo.Data.DoubleColumn)x,y);
			else throw new ArgumentException("Error: Try to apply Log() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(),"x");
		}
		public static Altaxo.Data.DoubleColumn Log(double x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Log(x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Log() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName,"x");
		}


		public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType() && typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Max((Altaxo.Data.DoubleColumn)x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Max() to " + x.TypeAndName + " and " + y.TypeAndName,"x");
		}
		public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DataColumn x, double y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Max((Altaxo.Data.DoubleColumn)x,y);
			else throw new ArgumentException("Error: Try to apply Max() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(),"x");
		}
		public static Altaxo.Data.DoubleColumn Max(double x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Max(x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Max() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName,"x");
		}


		public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType() && typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Min((Altaxo.Data.DoubleColumn)x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Min() to " + x.TypeAndName + " and " + y.TypeAndName,"x");
		}
		public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DataColumn x, double y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Min((Altaxo.Data.DoubleColumn)x,y);
			else throw new ArgumentException("Error: Try to apply Min() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(),"x");
		}
		public static Altaxo.Data.DoubleColumn Min(double x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Min(x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Min() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName,"x");
		}


		public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType() && typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Pow((Altaxo.Data.DoubleColumn)x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Pow() to " + x.TypeAndName + " and " + y.TypeAndName,"x");
		}
		public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DataColumn x, double y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Pow((Altaxo.Data.DoubleColumn)x,y);
			else throw new ArgumentException("Error: Try to apply Pow() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(),"x");
		}
		public static Altaxo.Data.DoubleColumn Pow(double x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Pow(x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Pow() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName,"x");
		}
		public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType() && typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName + " and " + y.TypeAndName,"x");
		}
		public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DataColumn x, int y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Round((Altaxo.Data.DoubleColumn)x,y);
			else throw new ArgumentException("Error: Try to apply Round() to " + x.TypeAndName + " and " + y.GetType() + " " + y.ToString(),"x");
		}
		public static Altaxo.Data.DoubleColumn Round(double x, Altaxo.Data.DataColumn y)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==y.GetType())
				return Altaxo.Data.DoubleColumn.Round(x,(Altaxo.Data.DoubleColumn)y);
			else throw new ArgumentException("Error: Try to apply Round() to " + x.GetType() + " " + x.ToString() + " and " + y.TypeAndName,"x");
		}


		public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Sign((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Sign() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Sin((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Sin() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Sinh((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Sinh() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Sqrt((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Sqrt() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Tan((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Tan() to " + x.TypeAndName,"x");
		}

		public static Altaxo.Data.DoubleColumn Tanh(Altaxo.Data.DataColumn x)
		{ 
			if(typeof(Altaxo.Data.DoubleColumn)==x.GetType())
				return Altaxo.Data.DoubleColumn.Tanh((Altaxo.Data.DoubleColumn)x);
			else throw new ArgumentException("Error: Try to apply Tanh() to " + x.TypeAndName,"x");
		}

		#endregion


	} // end of class ScriptExecutionBase

	public class ColScriptExeBase : ScriptExecutionBase
	{
		public virtual void Execute(Altaxo.Data.DataColumn myColumn)
		{
		}
	}

}
