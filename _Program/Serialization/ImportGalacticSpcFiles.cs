#region Disclaimer
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;


namespace Altaxo.Serialization.Galactic
{
	/// <summary>
	/// Summary description for ImportGalacticSpcFiles.
	/// </summary>
	public class Import
	{
		public struct SPCHDR
		{
			public byte ftflgs;
			public byte fversn;
			public byte fexper;
			public byte fexp;
			public int fnpts;
			public double ffirst;
			public double flast;
			public int fnsub;
		}

		public struct SUBHDR
		{
			/// <summary> subflgs : always 0</summary>
			public byte		subflgs; 
			/// <summary> subexp : y-values scaling exponent (set to 0x80 means floating point representation)</summary>
			public byte		subexp; 
			/// <summary>subindx :  Integer index number of trace subfile (0=first)</summary>
			public Int16		subindx; 
			/// <summary>subtime;	 Floating time for trace (Z axis corrdinate)</summary>
			public float subtime;  
			/// <summary>subnext;	 Floating time for next trace (May be same as beg)</summary>
			public float subnext;  
			/// <summary>subnois;	 Floating peak pick noise level if high byte nonzero </summary>
			public float subnois; 
			/// <summary>subnpts;	 Integer number of subfile points for TXYXYS type </summary>
			public Int32 subnpts; 
			/// <summary>subscan;	Integer number of co-added scans or 0 (for collect) </summary>
			public Int32 subscan; // 
			/// <summary>subwlevel;	 Floating W axis value (if fwplanes non-zero) </summary>
			public float subwlevel;  
			/// <summary>subresv[4];	 Reserved area (must be set to zero) </summary>
			public Int32 subresv;

		}

		/// <summary>
		/// Imports a Galactic SPC file into a x and an y array.
		/// </summary>
		/// <param name="xvalues">The x values of the spectrum.</param>
		/// <param name="yvalues">The y values of the spectrum.</param>
		/// <param name="filename">The filename where to import from.</param>
		/// <returns>Null if successful, otherwise an error description.</returns>
		public static string ToArrays(string filename, out double [] xvalues, out double [] yvalues)
		{
			System.IO.Stream stream=null;

			SPCHDR hdr = new SPCHDR();
			SUBHDR subhdr = new SUBHDR();

			try
			{
				stream = new System.IO.FileStream(filename,System.IO.FileMode.Open);
				System.IO.BinaryReader binreader = new System.IO.BinaryReader(stream);


				hdr.ftflgs = binreader.ReadByte(); // ftflgs : not-evenly spaced data
				hdr.fversn = binreader.ReadByte(); // fversn : new version
				hdr.fexper = binreader.ReadByte(); // fexper : general experimental technique
				hdr.fexp   = binreader.ReadByte(); // fexp   : fractional scaling exponent (0x80 for floating point)

				hdr.fnpts  = binreader.ReadInt32(); // fnpts  : number of points

				hdr.ffirst = binreader.ReadDouble(); // ffirst : first x-value
				hdr.flast  = binreader.ReadDouble(); // flast : last x-value
				hdr.fnsub  = binreader.ReadInt32(); // fnsub : 1 (one) subfile only
			
				binreader.ReadByte(); //  Type of X axis units (see definitions below) 
				binreader.ReadByte(); //  Type of Y axis units (see definitions below) 
				binreader.ReadByte(); // Type of Z axis units (see definitions below)
				binreader.ReadByte(); // Posting disposition (see GRAMSDDE.H)

				binreader.Read(new byte[0x1E0],0,0x1E0); // rest of SPC header


				// ---------------------------------------------------------------------
				//   following the x-values array
				// ---------------------------------------------------------------------

				if(0!=(hdr.ftflgs & 0x80))
				{
					xvalues = new double[hdr.fnpts];
					for(int i=0;i<hdr.fnpts;i++)
						xvalues[i] = binreader.ReadSingle();
				}
				else if(0==hdr.ftflgs) // evenly spaced data
				{
					xvalues = new double[hdr.fnpts];
					for(int i=0;i<hdr.fnpts;i++)
						xvalues[i] = hdr.ffirst + i*(hdr.flast-hdr.ffirst)/(hdr.fnpts-1);
				}
				else
				{
					throw new System.FormatException("The SPC file must not be a multifile; only single file format is accepted!");
				}

				// ---------------------------------------------------------------------
				//   following the y SUBHEADER
				// ---------------------------------------------------------------------

				subhdr.subflgs = binreader.ReadByte(); // subflgs : always 0
				subhdr.subexp  = binreader.ReadByte(); // subexp : y-values scaling exponent (set to 0x80 means floating point representation)
				subhdr.subindx = binreader.ReadInt16(); // subindx :  Integer index number of trace subfile (0=first)

				subhdr.subtime = binreader.ReadSingle(); // subtime;	 Floating time for trace (Z axis corrdinate) 
				subhdr.subnext = binreader.ReadSingle(); // subnext;	 Floating time for next trace (May be same as beg) 
				subhdr.subnois = binreader.ReadSingle(); // subnois;	 Floating peak pick noise level if high byte nonzero 

				subhdr.subnpts = binreader.ReadInt32(); // subnpts;	 Integer number of subfile points for TXYXYS type 
				subhdr.subscan = binreader.ReadInt32(); // subscan;	Integer number of co-added scans or 0 (for collect) 
				subhdr.subwlevel = binreader.ReadSingle();        // subwlevel;	 Floating W axis value (if fwplanes non-zero) 
				subhdr.subresv   = binreader.ReadInt32(); // subresv[4];	 Reserved area (must be set to zero) 


				// ---------------------------------------------------------------------
				//   following the y-values array
				// ---------------------------------------------------------------------
				yvalues = new double[hdr.fnpts];
				for(int i=0;i<hdr.fnpts;i++)
					yvalues[i] = binreader.ReadSingle();
			}
			catch(Exception e)
			{
				xvalues = null;
				yvalues = null;
				return e.ToString();
			}
			finally
			{
				if(null!=stream)
					stream.Close();
			}
			
			return null;
		}

		public static void ShowDialog(System.Windows.Forms.Form owner, Altaxo.Data.DataTable table)
		{
			System.Text.StringBuilder errorList = new System.Text.StringBuilder();
			string firstfilename = null;
			Altaxo.Data.DoubleColumn xcol=null;

			System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
			dlg.Filter = "Galactic SPC files (*.spc)|*.spc|All files (*.*)|*.*"  ;
			dlg.FilterIndex = 1 ;
			dlg.Multiselect = true; // allow selecting more than one file

			if(System.Windows.Forms.DialogResult.OK==dlg.ShowDialog(owner))
			{
				double[] xvalues, yvalues;
				// if user has clicked ok, import all selected files into Altaxo
				Array.Sort(dlg.FileNames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
				foreach(string filename in dlg.FileNames)
				{
					string error = ToArrays(filename,out xvalues, out yvalues);
					if(null!=error)
						errorList.Append(error);

					if(null==xcol) // if this is the first file successfully imported, add the xvalues to the worksheet
					{
						firstfilename = filename;
						xcol = new Altaxo.Data.DoubleColumn(xvalues.Length);
						for(int i=0;i<xvalues.Length;i++)
							xcol[i] = xvalues[i];
						table.DataColumns.Add(xcol,"SPC X values",Altaxo.Data.ColumnKind.X);
					}
					else // xcol was set before - so check the outcoming xvalues now that they match the xcols of the first imported spcfile
					{
						if(xvalues.Length!=xcol.Count || yvalues.Length!=xcol.Count)
						{
							errorList.Append(string.Format("Warning: the length of the spectrum {0} ({1}) did not match the length of the first spectrum {2} ({3})!\n",filename,xvalues.Length,firstfilename,xcol.Count));
						}
						else
						{
							// now check the match in the xvalues
							for(int i=0;i<xvalues.Length;i++)
							{
								if(xcol[i]!=xvalues[i])
								{
									errorList.Append(string.Format("Warning: the xvalues at position [{0}] did not match between the spectrum {1} and the first imported spectrum {2}!\n",i,filename,firstfilename)); 
									break;
								}
							}
						}
		
							}

					// now add the y-values
					Altaxo.Data.DoubleColumn ycol = new Altaxo.Data.DoubleColumn(yvalues.Length);
					for(int i=0;i<yvalues.Length;i++)
						ycol[i] = yvalues[i];
					table.DataColumns.Add(ycol,filename);
				} // foreache file

				if(errorList.Length>0)
				{
					System.Windows.Forms.MessageBox.Show(owner,errorList.ToString(),"Some errors occured during import!",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Exclamation);
				}
			}
		}

	}
}
