using System;
using Altaxo.Serialization;


namespace Altaxo.Graph
{
	/// <summary>
	/// Summary description for Axis.
	/// </summary>
	public abstract class Axis
	{
		public event System.EventHandler AxisChanged;

		/// <summary>
		/// PhysicalToNormal translates physical values into a normal value linear along the axis
		/// a physical value of the axis origin must return a value of zero
		/// a physical value of the axis end must return a value of one
		/// the function physicalToNormal must be provided by any derived class
		/// </summary>
		/// <param name="x">the physical value</param>
		/// <returns>
		/// the normalized value linear along the axis,
		/// 0 for axis origin, 1 for axis end</returns>
		public abstract double PhysicalToNormal(double x);
		/// <summary>
		/// NormalToPhysical is the inverse function to PhysicalToNormal
		/// It translates a normalized value (0 for the axis origin, 1 for the axis end)
		/// into the physical value
		/// </summary>
		/// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
		/// <returns>the corresponding physical value</returns>
		public abstract double NormalToPhysical(double x);

		/// <summary>
		/// GetMajorTicks returns the physical values
		/// at which major ticks should occur
		/// </summary>
		/// <returns>physical values for the major ticks</returns>
		public abstract double[] GetMajorTicks();

	
		/// <summary>
		/// GetMinorTicks returns the physical values
		/// at which minor ticks should occur
		/// </summary>
		/// <returns>physical values for the minor ticks</returns>
		public virtual double[] GetMinorTicks()
		{
			return new double[]{}; // return a empty array per default
		}


		public abstract PhysicalBoundaries DataBounds { get; } // return a PhysicalBoundarie object that is associated with that axis

		public abstract double Org { get; set;}
		public abstract double End { get; set;}
		public abstract bool   OrgFixed { get; set; }
		public abstract bool   EndFixed { get; set; }

		/// <summary>
		/// calculates the axis org and end using the databounds
		/// the org / end is adjusted only if it is not fixed
		/// and the DataBound object contains valid data
		/// </summary>
		public abstract void ProcessDataBounds(double org, bool orgfixed, double end, bool endfixed); 
		public abstract void ProcessDataBounds();


		protected virtual void OnAxisChanged()
		{
			if(null!=AxisChanged)
				AxisChanged(this,new System.EventArgs());
		}


		protected static System.Collections.Hashtable sm_AvailableAxes;
		
		static Axis()
		{
			sm_AvailableAxes = new System.Collections.Hashtable();

			System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			foreach(System.Reflection.Assembly assembly in assemblies)
			{
				// test if the assembly supports Serialization
				
				Type[] definedtypes = assembly.GetTypes();
				foreach(Type definedtype in definedtypes)
				{
					if(definedtype.IsSubclassOf(typeof(Axis)) && !definedtype.IsAbstract)
						sm_AvailableAxes.Add(definedtype.Name,definedtype);
				}
			}
		}


		
		public static System.Collections.Hashtable AvailableAxes 
		{
			get { return sm_AvailableAxes; }
		}

	} // end of class Axis

	[SerializationSurrogate(0,typeof(Altaxo.Data.DataColumn.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class LinearAxis : Axis, System.Runtime.Serialization.IDeserializationCallback
	{
		// primary values
		protected double m_BaseOrg=0; // proposed value of org
		protected double m_BaseEnd=1; // proposed value of end
		protected double m_AxisOrgByMajor=0;
		protected double m_AxisEndByMajor=5;
		protected double m_MajorSpan=0.2; // physical span value between two major ticks
		protected int    m_MinorTicks=2; // Minor ticks per Major tick ( if there is one minor tick between two major ticks m_minorticks is 2!
		protected bool   m_AxisOrgFixed = false;
		protected bool   m_AxisEndFixed = false;
		protected FinitePhysicalBoundaries m_DataBounds = new FinitePhysicalBoundaries();

		// cached values
		protected double m_AxisOrg=0;
		protected double m_AxisEnd=1;
		protected double m_AxisSpan=1;
		protected double m_OneByAxisSpan=1;


		#region Serialization
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				LinearAxis s = (LinearAxis)obj;
				// I decided _not_ to serialize the parent object, since if we only want
				// to serialize this column, we would otherwise serialize the entire object
				// graph
				// info.AddValue("Parent",s.m_Table); // 
				/*
				info.AddValue("Name",s.m_ColumnName);
				info.AddValue("Number",s.m_ColumnNumber);
				info.AddValue("Count",s.m_Count);
				*/
			}
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				LinearAxis s = (LinearAxis)obj;
				// s.m_Table = (Altaxo.Data.DataTable)(info.GetValue("Parent",typeof(Altaxo.Data.DataTable)));
				/*
				s.m_Table = null;
				s.m_ColumnName = info.GetString("Name");
				s.m_ColumnNumber = info.GetInt32("Number");
				s.m_Count = info.GetInt32("Count");

				// set the helper data
				s.m_MinRowChanged=int.MaxValue; // area of rows, which changed during event off period
				s.m_MaxRowChanged=int.MinValue;
				*/
				return s;
			}
		}

		public virtual void OnDeserialization(object obj)
		{
		}
		#endregion


		public LinearAxis()
		{
			m_DataBounds = new FinitePhysicalBoundaries();
			m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
		}


		public override double Org
		{
			get { return m_AxisOrg; } 
			set 
			{
				m_AxisOrg = value;
				ProcessDataBounds(m_AxisOrg,true,m_AxisEnd,true);
			}
		}
		public override double End 
		{
			get { return m_AxisEnd; } 
			set
			{ 
				m_AxisEnd = value;
				ProcessDataBounds(m_AxisOrg,true,m_AxisEnd,true);
			}
		}

		public override bool OrgFixed
		{
			get { return m_AxisOrgFixed; } 
			set { m_AxisOrgFixed = value; }
		}
		public override bool EndFixed 
		{
			get { return m_AxisEndFixed; } 
			set {	m_AxisEndFixed = value; }
		}


		public override PhysicalBoundaries DataBounds { get { return m_DataBounds; } }

		public override double PhysicalToNormal(double x)
		{
			return (x- m_AxisOrg ) * m_OneByAxisSpan; 
		}

		public override double NormalToPhysical(double x)
		{
			return m_AxisOrg + x * m_AxisSpan;
		}

		public override double[] GetMajorTicks()
		{
			int j;
			double i,beg,end;
			double[] retv;
			if(m_AxisOrgByMajor<=m_AxisEndByMajor) // normal case org<end
			{
				beg=System.Math.Ceiling(m_AxisOrgByMajor);
				end=System.Math.Floor(m_AxisEndByMajor);
				retv = new double[1+(int)(end-beg)];
				for(j=0,i=beg;i<=end;i+=1,j++)
					retv[j]=i*m_MajorSpan;
			}
			else
			{
				beg=System.Math.Floor(m_AxisOrgByMajor);
				end=System.Math.Ceiling(m_AxisEndByMajor);
				retv = new double[1+(int)(beg-end)];
				for(j=0,i=beg;i>=end;i-=1,j++)
					retv[j]=i*m_MajorSpan;
			}
			return retv;
		}

		public override double[] GetMinorTicks()
		{
			int j;
			double i,beg,end;
			double[] retv;
			if(m_MinorTicks<2)
				return new double[]{}; // below 2 there are no minor ticks per definition

			if(m_AxisOrgByMajor<=m_AxisEndByMajor) // normal case org<end
			{
				beg=System.Math.Ceiling(m_AxisOrgByMajor);
				end=System.Math.Floor(m_AxisEndByMajor);
				int majorticks = 1+(int)(end-beg);
				beg = System.Math.Ceiling(m_AxisOrgByMajor*m_MinorTicks);
				end = System.Math.Floor(m_AxisEndByMajor*m_MinorTicks);
				int minorticks = 1+(int)(end-beg) - majorticks;
				retv = new double[minorticks];
				for(j=0,i=beg;i<=end && j<minorticks;i+=1)
				{
					if(i%m_MinorTicks!=0)
					{
						retv[j]=i*m_MajorSpan/m_MinorTicks;
						j++;
					}
				}
			}
			else
			{
				beg=System.Math.Floor(m_AxisOrgByMajor);
				end=System.Math.Ceiling(m_AxisEndByMajor);
				retv = new double[1+(int)(beg-end)];
				for(j=0,i=beg;i>=end;i-=1,j++)
					retv[j]=i*m_MajorSpan;
			}
			return retv;
		}

		public override void ProcessDataBounds()
		{
			if(null==this.m_DataBounds || this.m_DataBounds.IsEmpty)
				return;
		
		ProcessDataBounds(m_DataBounds.LowerBound,this.m_AxisOrgFixed,m_DataBounds.UpperBound,this.m_AxisEndFixed); 
		}


		public  override void ProcessDataBounds(double xorg, bool xorgfixed, double xend, bool xendfixed)
		{
			double oldAxisOrgByMajor = m_AxisOrgByMajor;
			double oldAxisEndByMajor = m_AxisEndByMajor;
			double oldMajorSpan      = m_MajorSpan;
			int    oldMinorTicks		 = m_MinorTicks;

			m_BaseOrg = xorg;
			m_BaseEnd = xend;
			
			CalculateTicks(xorg, xend, out m_MajorSpan, out m_MinorTicks);
			if(xend>=xorg)
			{
				if(xorgfixed)
					m_AxisOrgByMajor = xorg/m_MajorSpan;
				else
					m_AxisOrgByMajor = System.Math.Floor(m_MinorTicks * xorg/m_MajorSpan)/m_MinorTicks;

				if(xendfixed)
					m_AxisEndByMajor = xend/m_MajorSpan;
				else
					m_AxisEndByMajor = System.Math.Ceiling(m_MinorTicks * xend /m_MajorSpan)/m_MinorTicks;
			}
			else // org is greater than end !
			{
				if(xorgfixed)
					m_AxisOrgByMajor = xorg/m_MajorSpan;
				else
					m_AxisOrgByMajor = System.Math.Ceiling(m_MinorTicks * xorg/m_MajorSpan)/m_MinorTicks;

				if(xendfixed)
					m_AxisEndByMajor = xend/m_MajorSpan;
				else
					m_AxisEndByMajor = System.Math.Floor(m_MinorTicks * xend /m_MajorSpan)/m_MinorTicks;
			}

		SetCachedValues();

			// compare with the saved values to find out whether or not something changed
			if(oldAxisOrgByMajor!=m_AxisOrgByMajor ||
				oldAxisEndByMajor!=m_AxisEndByMajor ||
				oldMajorSpan != m_MajorSpan ||
				oldMinorTicks != m_MinorTicks)
			{
				OnAxisChanged();
			}
		}

		protected void SetCachedValues()
		{
			m_AxisOrg = m_AxisOrgByMajor * m_MajorSpan;
			m_AxisEnd = m_AxisEndByMajor * m_MajorSpan;
			m_AxisSpan = m_AxisEnd - m_AxisOrg;
			m_OneByAxisSpan = 1/m_AxisSpan;
		}

		protected void OnBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			bool bIsRelevant=false;
			bIsRelevant |= !this.m_AxisOrgFixed && e.LowerBoundChanged;
			bIsRelevant |= !this.m_AxisEndFixed && e.UpperBoundChanged;

			if(bIsRelevant) // if something really relevant changed
			{
				ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
			}
		}


		static double TenToThePowerOf(int ii)
		{
			if(ii==0)
				return 1;
			else if(ii==1)
				return 10;
			else
			{
				int i = System.Math.Abs(ii);
				int halfi = i/2;
				double hret = TenToThePowerOf(halfi);
				double ret =(halfi+halfi)==i ? hret*hret : 10*hret*hret; 
				return ii<0 ? 1/ret : ret;
			}
		}



		static void CalculateTicks(	
			double min,                // Minimum of data 
			double max,                // Maximum of data
			out double majorspan,      // the span between two major ticks
			out int    minorticks      // number of ticks in a major tick span 
			)
		{
			if(min>max) // should not happen, but can happen when there are no data and min and max are uninitialized 
			{
				min=max=0;
			}

			double span = max - min; // span width between max and min

			if(0==span)
			{
				double diff;
				// if span width is zero, then 1% of the velue, in case of min==max==0 we use 1
				if(0==max || 0==min) // if one is null, the other should also be null, but to be secure...
					diff = 1;
				else
					diff = System.Math.Abs(min/100); // wir can be sure, that min==max, because span==0

				min -= diff;
				max += diff;
					
				span = max - min;
			} // if 0==span


			// we have to norm span in that way, that 100<=normspan<1000
			int nSpanPotCorr = (int)(System.Math.Floor(System.Math.Log10(span)))-2; // nSpanPotCorr will be 0 if 100<=span<1000 
			double normspan = span / TenToThePowerOf(nSpanPotCorr);

			// we divide normspan by 10, 20, 25, 50, 100, 200 and calculate the
			// number of major ticks this will give
			// we can break if the number of major ticks is below 10
			int majornormspan=1;
			int minornormspan=1;
			for(int finep=0;finep<=5;finep++)
			{
				switch(finep)
				{
					case 0:
						majornormspan = 10;
						minornormspan = 5;
						break;
					case 1:
						majornormspan = 20;
						minornormspan = 10;
						break;
					case 2:
						majornormspan = 25;
						minornormspan = 5;
						break;
					case 3:
						majornormspan = 50;
						minornormspan = 25;
						break;
					case 4:
						majornormspan = 100;
						minornormspan = 50;
						break;
					case 5:
					default:
						majornormspan = 200;
						minornormspan = 100;
						break;
				} // end of switch
				double majorticks = 1+System.Math.Floor(normspan/majornormspan);
				if(majorticks<=10)
					break;
			}
		majorspan = majornormspan * TenToThePowerOf(nSpanPotCorr);
		minorticks = (int)(majornormspan / minornormspan);
		} // end of function



/*
		static void CalculateDataBorders(	
			double min,                // Minimum of data 
			double max,                // Maximum of data
			out int  nSpanPotCorr,     // corrected power of span
			out int nAxisPotCorr,      // power of axis 
			out double  aMaxTeiler,    // multiplikator für maximum
			out double  aMinTeiler,    // Multiplikator for Minimum
			out double aTeilerStep,    // Teilerschritt unter Berücksichtigung der korrigierten Span-Potenz
			out double aEffectiveStep, // Teilerschritt ohne Berücksichtigung der korrigierten Potenz
			out double aAxisStep       // Teilerschritt auf der Achse unter Berücksichtigung der Korrigierten Potenz
			)
		{
			int i, finep, nDiffPotCorr;
			double nMaxTeiler, nMinTeiler;
			double teiler, xSpanPotCorr, xAxisPotCorr, xDiffPotCorr, x;

			if(min>max) // should not happen, but can happen when there are no data and min and max are uninitialized 
			{
				min=max=0;
			}

			nSpanPotCorr = 0; xSpanPotCorr=1; // for the first step no power correction

			double span = max - min; // span width between max and min

			if(0==span)
			{
				double diff;
				// if span width is zero, then 1% of the velue, in case of min==max==0 we use 1
				if(0==max || 0==min) // if one is null, the other should also be null, but to be secure...
					diff = 1;
				else
					diff = Math.Abs(min/100); // wir can be sure, that min==max, because span==0

				min -= diff;
				max += diff;
					
				span = max - min;
			} // if 0==span



			// now we calculate the power (order) of the axis values, 
			// for this we use the greates absolute value
			// of the minimum and the maximum
			double max_axis_value = System.Math.Max(fabs(min),fabs(max));
			nAxisPotCorr = (int)(3*Math.Floor((System.Math.Log10(max_axis_value)-1)/3));  // between 10 and  9999.9999 no power, above and below engineering powers
			// Ermitteln der Potenz der Spannweite
			nSpanPotCorr = (int)(3*Math.Floor((Math.Log10(span)-1)/3));  // zwischen 10 und 9999.9999 soll keine Potenz, rest entsprechend alle 3 Dekaden verschoben

			nDiffPotCorr = nSpanPotCorr - nAxisPotCorr; // Differenz der Korrektur-Potenzen

			// Teiler oder Multiplikator for die Axis-Korrektur ermitteln
			xAxisPotCorr = TenToThePowerOf(Math.Abs(nAxisPotCorr));

			// Teiler oder Multiplikator für die Span-Korrektur ermitteln
			xSpanPotCorr = TenToThePowerOf(Math.Abs(nSpanPotCorr));

			// Teiler oder Multiplikator für die Differenz-Korrektur ermitteln
			xDiffPotCorr = TenToThePowerOf(Math.Abs(nDiffPotCorr));

			if(nSpanPotCorr<0)
			{
				max *= xSpanPotCorr;
				min *= xSpanPotCorr;
				span = max - min;
			}
			else if(nSpanPotCorr>0)
			{
				// Span, min und max durch Potenz dividieren
				min /= xSpanPotCorr;
				max /= xSpanPotCorr;
				span = max - min;
			}


			for(x = 10; x<span; x*=10);

			for(finep=0;finep<=4;finep++)
			{
				switch(finep)
				{
					case 0:
						teiler = x/10;
						break;
					case 1:
						teiler = x/20;
						break;
					case 2:
						teiler = x/25;
						break;
					case 3:
						teiler = x/50;
						break;
					case 4:
						teiler = x/100;
						break;
				} // end of switch

				if(teiler<1)
					break;  // sonst ist die Annahme nicht erfüllt, dass der Teiler eine Ganze Zahl ist


				// wir nehmen mal an, daß der Teiler eine ganze Zahl ist

				nMaxTeiler = Math.Floor(max/teiler);

				if(nMaxTeiler*teiler < max)
					nMaxTeiler++;

				nMinTeiler = Math.Floor(min/teiler);

				if(nMinTeiler*teiler > min)
					nMinTeiler--;


				// wenn es genug Teiler sind, können wir mit der Verfeinerung aufhören

				if((nMaxTeiler - nMinTeiler)>=3)
					break;

			} // end of for-Statement


			aMaxTeiler = nMaxTeiler;
			aMinTeiler = nMinTeiler;
			aTeilerStep = teiler;
			aEffectiveStep = nSpanPotCorr < 0 ? teiler / xSpanPotCorr : teiler * xSpanPotCorr;
			aAxisStep =      nDiffPotCorr < 0 ? teiler / xDiffPotCorr : teiler * xDiffPotCorr;
		} // end of funktion
*/		

	} // end of class LinearAxis


	public class Log10Axis : Axis
	{
		double m_Log10Org=0; // Log10 of physical axis org
		double m_Log10End=1; // Log10 of physical axis end
		int    m_DecadesPerMajorTick=1; // how many decades is one major tick
		protected bool   m_AxisOrgFixed = false;
		protected bool   m_AxisEndFixed = false;
		protected PositiveFinitePhysicalBoundaries m_DataBounds = null;

		public Log10Axis()
		{
			m_DataBounds = new PositiveFinitePhysicalBoundaries();
			m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
		}


		/// <summary>
		/// PhysicalToNormal translates physical values into a normal value linear along the axis
		/// a physical value of the axis origin must return a value of zero
		/// a physical value of the axis end must return a value of one
		/// the function physicalToNormal must be provided by any derived class
		/// </summary>
		/// <param name="x">the physical value</param>
		/// <returns>
		/// the normalized value linear along the axis,
		/// 0 for axis origin, 1 for axis end</returns>
		public override double PhysicalToNormal(double x)
		{
			if(x<=0)
				return Double.NaN;

			double log10x = Math.Log10(x);
			return (log10x-m_Log10Org)/(m_Log10End-m_Log10Org);
		}
		/// <summary>
		/// NormalToPhysical is the inverse function to PhysicalToNormal
		/// It translates a normalized value (0 for the axis origin, 1 for the axis end)
		/// into the physical value
		/// </summary>
		/// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
		/// <returns>the corresponding physical value</returns>
		public override double NormalToPhysical(double x)
		{
			double log10x = m_Log10Org + (m_Log10End-m_Log10Org)*x;
			return Math.Pow(10,log10x);
		}

		/// <summary>
		/// GetMajorTicks returns the physical values
		/// at which major ticks should occur
		/// </summary>
		/// <returns>physical values for the major ticks</returns>
		public override double[] GetMajorTicks()
		{
			double log10org;
			double log10end;

			// ensure that log10org<log10end
			if(m_Log10Org<m_Log10End)
			{
				log10org = m_Log10Org;
				log10end = m_Log10End;
			}
			else
			{
				log10org = m_Log10End;
				log10end = m_Log10Org;
			}

			// calculate the number of major ticks

			int nFullDecades = (int)(1+Math.Floor(log10end)-Math.Ceiling(log10org));
			int nMajorTicks = (int)Math.Floor((nFullDecades+m_DecadesPerMajorTick-1)/m_DecadesPerMajorTick);


			double[] retval = new double[nMajorTicks];
			int beg=(int)Math.Ceiling(log10org);
			int end=(int)Math.Floor(log10end);

			int i,j;
			for(i=beg,j=0 ;(i<=end) && (j<nMajorTicks) ; i+=m_DecadesPerMajorTick,j++)
			{
				retval[j] = Math.Pow(10,i);
			}
		return retval;
		}

	
		/// <summary>
		/// GetMinorTicks returns the physical values
		/// at which minor ticks should occur
		/// </summary>
		/// <returns>physical values for the minor ticks</returns>
		public override double[] GetMinorTicks()
		{
			double log10org;
			double log10end;

			// ensure that log10org<log10end
			if(m_Log10Org<m_Log10End)
			{
				log10org = m_Log10Org;
				log10end = m_Log10End;
			}
			else
			{
				log10org = m_Log10End;
				log10end = m_Log10Org;
			}

			double decadespan = Math.Abs(m_Log10Org-m_Log10End);

			// guess from the span the tickiness (i.e. the increment of the multiplicator)
			// so that not more than 50 minor ticks are visible
			double minorsperdecade = 50.0/decadespan;
			
			// do not allow more than 10 minors per decade than 
			if(decadespan>0.3 && minorsperdecade>10) minorsperdecade=10;

			// if minorsperdecade is lesser than one, we dont have minors, so we can
			// return an empty field
			if(minorsperdecade<=1)
				return new double[0];


			// ensure the minorsperdecade are one of the following values
			// 1,2,4,5,8,10,20,40,50,80,100 usw.
				double dec=1;
				for(int i=0;;i++)
				{
					double val;
					switch(i%5)
					{
						default:
						case 0: val=1*dec; break;
						case 1: val=2*dec; break;
						case 2: val=4*dec; break;
						case 3: val=5*dec; break;
						case 4: val=8*dec; dec*=10; break;
					}
					if(val>=minorsperdecade)
					{
						minorsperdecade=val;
						break;
					}
				}
			// now if minorsperdecade is at least 2, it is a good "even" value

			// now get the major ticks
			double [] majorticks = GetMajorTicks();
			// and calculate begin and end of minor ticks

			int majorcount = majorticks.Length;

			// of cause this increment is only valid in the decade between 1 and 10
			double minorincrement = 10/minorsperdecade;

			// there are two cases now, either we have at least one major tick,
			// then we have two different decades on left and right of the axis,
			// or there is no major tick, so the whole axis is in the same decade
			if(majorcount>=1) // the "normal" case
			{
				int i,j,k;
				// count the ticks on left of the axis
				// note: we normalized so that the "lesser values" are on the left
				double org = Math.Pow(10,log10org);
				double firstmajor = majorticks[0];
				for(i=1;firstmajor*(1-i*minorincrement/10)>=org;i++) {}
				int leftminorticks = i-1;

				// count the ticks on the right of the axis
				double end = Math.Pow(10,log10end);
				double lastmajor = majorticks[majorcount-1];
				for(i=1;lastmajor*(1+i*minorincrement)<=end;i++){}
				int rightminorticks = i-1;


				// calculate the total minorticks count
				double [] minors = new double[leftminorticks+rightminorticks+(majorcount-1)*((int)minorsperdecade-1)];

				// now fill the array
				for(j=0,i=leftminorticks;i>0;j++,i--)
					minors[j] = firstmajor*(1-i*minorincrement/10); 

				for(k=0;k<(majorcount-1);k++)
				{
					for(i=1;i<minorsperdecade;j++,i++)
						minors[j] = majorticks[k]*(1+i*minorincrement);
				}
				for(i=1;i<=rightminorticks;j++,i++)
					minors[j] = lastmajor*(1+i*minorincrement);
			
				return minors;
			}
			else // in case there is no major tick
			{

				// determine the upper decade (major tick)
				double firstmajor = Math.Pow(10,Math.Floor(log10org));
				double groundpow = Math.Floor(log10org);
				double norg = Math.Pow(10,log10org-groundpow);
				double nend = Math.Pow(10,log10end-groundpow);

				// norg and nend now is between 1 and 10
				// so calculate directly the indices
				double firstidx = Math.Ceiling(norg/minorincrement);
				double lastidx  = Math.Floor(nend/minorincrement);


				// do not do anything if something goes wrong
				if((lastidx<firstidx) || ((lastidx-firstidx)>100))
				{
					return new double[0];
				}

				double[] minors = new double[(int)(1+lastidx-firstidx)];
				
					// fill the array
				int j;
				double di;
				for(j=0,di=firstidx;di<=lastidx;j++,di+=1)
					minors[j] = firstmajor*(di*minorincrement);

				return minors; // return a empty array per default
			}



		}


		public override PhysicalBoundaries DataBounds
		{
			get { return this.m_DataBounds; }
		} // return a PhysicalBoundarie object that is associated with that axis

		public override double Org
		{
			get { return Math.Pow(10,m_Log10Org); } 
			set 
			{
				if(value>0)
				{
				ProcessDataBounds(value,true,Math.Pow(10,m_Log10End),true);
				}
			}
		}
		public override double End 
		{
			get { return Math.Pow(10,m_Log10End); } 
			set
			{
				if(value>0)
				{
					ProcessDataBounds(Math.Pow(10,m_Log10Org),true,value,true);
				}
			}
		}

		public override bool OrgFixed
		{
			get { return m_AxisOrgFixed; } 
			set { m_AxisOrgFixed = value; }
		}
		public override bool EndFixed 
		{
			get { return m_AxisEndFixed; } 
			set {	m_AxisEndFixed = value; }
		}

		/// <summary>
		/// calculates the axis org and end using the databounds
		/// the org / end is adjusted only if it is not fixed
		/// and the DataBound object contains valid data
		/// </summary>
		public override void ProcessDataBounds(double org, bool orgfixed, double end, bool endfixed)
		{

			// if one of the bounds is not valid, use the old bounds instead

			double log10org = org>0 ? Math.Log10(org) : m_Log10Org;
			double log10end = end>0 ? Math.Log10(end) : m_Log10End;


			// do something if org and end are the same
			if(log10org==log10end)
			{
				log10org += 1;
				log10end -= 1;
			}

			// calculate the number of decades between end and org
			double decades = Math.Abs(log10end-log10org);

			// limit the number of major ticks to about 10
			m_DecadesPerMajorTick = (int)Math.Ceiling(decades/10.0);


			m_Log10Org = log10org;
			m_Log10End = log10end;

		}
		public override void ProcessDataBounds()
		{
			if(null==this.m_DataBounds || this.m_DataBounds.IsEmpty)
				return;
		
			ProcessDataBounds(m_DataBounds.LowerBound,this.m_AxisOrgFixed,m_DataBounds.UpperBound,this.m_AxisEndFixed); 
		}

		protected void OnBoundariesChanged(object sender, BoundariesChangedEventArgs e)
		{
			bool bIsRelevant=false;
			bIsRelevant |= !this.m_AxisOrgFixed && e.LowerBoundChanged;
			bIsRelevant |= !this.m_AxisEndFixed && e.UpperBoundChanged;

			if(bIsRelevant) // if something really relevant changed
			{
				ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
			}
		}


	} // end of class Log10Axis
}
