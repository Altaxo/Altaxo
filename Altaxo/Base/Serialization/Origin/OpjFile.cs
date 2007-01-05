#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Altaxo.Serialization.Origin
{
  public class OpjFile
{
    const int MAX_SPREADS=255;
    const int MAX_COLUMNS=255;
    const int MAX_LEVEL=20;
    
	private string filename;				//!< project file name
	private int version;				//!< project version
	private int nr_spreads;				//!< number of spreadsheets
	private string[] spreadname = new string[MAX_SPREADS];		//!< spreadsheet names
	private int[] nr_cols = new int[MAX_SPREADS];		//!< number of cols per spreadsheet
	private int[][] nr_rows = new int[MAX_SPREADS][];	//!< number of rows per column of spreadsheet
	private int[] maxrows = new int[MAX_SPREADS];		//!< max number of rows of spreadsheet
	private double[][][] data = new double[MAX_SPREADS][][];	//!< data per column per spreadsheet
	private string[][] colname = new string[MAX_SPREADS][];	//!< column names
	private string[][] coltype = new string[MAX_SPREADS][];	//!< column types
  private System.Text.Encoding enc = System.Text.Encoding.ASCII;
    

public OpjFile( string filename) 
{
  this.filename = filename;
	version=0;
	nr_spreads=0;
	for(int i=0;i<MAX_SPREADS;i++) 
  {
		//spreadname[i] = new char[25];
		//spreadname[i][0]=0;
		nr_cols[i]=0;
		maxrows[i]=0;
    nr_rows[i] = new int[MAX_COLUMNS];
    data[i] = new double[MAX_COLUMNS][];
    colname[i] = new string[MAX_COLUMNS];
    coltype[i] = new string[MAX_COLUMNS];

		for(int j=0;j<MAX_COLUMNS;j++) 
    {
			nr_rows[i][j]=0;
      colname[i][j] = ('A' + (char)j).ToString(); // new char[25];
			//colname[i][j][0]=0x41+j;
			//colname[i][j][1]=0;
			//coltype[i][j] = new char[25];
			if(j==0)
				coltype[i][j]="X";
			else
				coltype[i][j]="Y'";
		}
	}
}
	
    	

    //!< get version of project file
	public double Version()
  {
    return version/100.0;
  }		
	
    /// <summary>
    /// get number of spreadsheets
    /// </summary>
    /// <returns></returns>
    public int numSpreads() 
    {
      return nr_spreads;
    }			
	
    /// <summary>
    /// get name of spreadsheet s
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string spreadName(int s)
     {
       return spreadname[s]; 
     }	

    /// <summary>
    /// get number of columns of spreadsheet s
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
	public int numCols(int s)
  {
    return nr_cols[s];
  }

    /// <summary>
    /// get number of rows of column c of spreadsheet s
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <returns></returns>
	public int numRows(int s, int c) 
  {
    return nr_rows[s][c]; 
  }	

    /// <summary>
    /// get maximum number of rows of spreadsheet s
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
	public int maxRows(int s) 
  {
    return maxrows[s]; 
  }		

    /// <summary>
    /// get name of column c of spreadsheet s
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <returns></returns>
	 public string colName(int s, int c)
   {
     return colname[s][c]; 
   }	

    /// <summary>
    /// get type of column c of spreadsheet s
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <returns></returns>
	 public string colType(int s, int c) 
   {
     return coltype[s][c];
   }

    /// <summary>
    /// get data of column c of spreadsheet s
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    /// <returns></returns>
	public double[] Data(int s,int c)
  {
    return data[s][c]; 
  }


    
/* File Structure :
filepre +
	+ pre + head + data	col A
	+ pre + head + data	col B
*/

/* parse file filename complete and save values */
public int Parse()
{
  using (System.IO.FileStream fstr = new FileStream(filename, FileMode.Open))
  {

    using (System.IO.BinaryReader f = new BinaryReader(fstr))
    {

      ////////////////////////////// check version from header ///////////////////////////////
      byte[] header = new byte[14];
      header[13] = 0;
      f.Read(header, 0, 13);
      byte[] vers = new byte[5];
      vers[4] = 0;

      // get version
      fstr.Seek(0x7, SeekOrigin.Begin);
      f.Read(vers, 0, 4);
      string versionstring = enc.GetString(vers);
      version = int.Parse(versionstring);

      int FILEPRE = 0x3e;	// file header
      int PRE = 0x62;		// pre column 
      int HEAD;		// column header
      int NEW_COL;		// value for new column
      int COL_SIZE;		// value for col size
      // TODO : valuesize depends also on column type!
      int valuesize = 10;
      if (version == 130)
      {	// 4.1
        version = 410;
        FILEPRE = 0x1D;
        HEAD = 0x20;
        NEW_COL = 0x72;
        COL_SIZE = 0x1b;
        valuesize = 8;
      }
      else if (version == 210)
      {	// 5.0
        version = 410;
        FILEPRE = 0x25;
        HEAD = 0x20;
        NEW_COL = 0x72;
        COL_SIZE = 0x1b;
        valuesize = 8;
      }
      else if (version == 2625)
      {	// 6.0
        version = 600;
        FILEPRE = 0x2f;
        HEAD = 0x22;
        NEW_COL = 0x72;
        COL_SIZE = 0x1b;
        valuesize = 11;
      }
      else if (version == 2630)
      {	// 6.0 SR4
        version = 604;
        FILEPRE = 0x2f;
        HEAD = 0x22;
        NEW_COL = 0x72;
        COL_SIZE = 0x1b;
      }
      else if (version == 2635)
      {	// 6.1
        version = 610;
        FILEPRE = 0x3a;
        HEAD = 0x22;
        NEW_COL = 0x72;
        COL_SIZE = 0x1b;
      }
      else if (version == 2656)
      {	// 7.0
        version = 700;
        HEAD = 0x23;
        NEW_COL = 0x73;
        COL_SIZE = 0x1c;
      }
      else if (version == 2769)
      {	// 7.5
        version = 750;
        HEAD = 0x33;
        NEW_COL = 0x83;
        COL_SIZE = 0x2c;
      }
      else
      {
        throw new ApplicationException(
        string.Format("Found unknown project version {0}. Please contact the author.", version));
      }

      /////////////////// find column ///////////////////////////////////////////////////////////7
      fstr.Seek(FILEPRE + 0x05, SeekOrigin.Begin); //(fseek(f,FILEPRE + 0x05,SEEK_SET);
      int col_found = f.ReadInt32();//fread(&col_found,4,1,f);

      //fprintf(debug,"	[column found = 0x%X (0x%X : YES) @ 0x%X]\n",col_found,NEW_COL,FILEPRE + 0x05);

      int current_col = 1, nr = 0, POS = FILEPRE, DATA = 0;
      double a;
      byte[] name = new byte[25];
      while (col_found == NEW_COL)
      {
        //////////////////////////////// COLUMN HEADER /////////////////////////////////////////////
        // TODO : data isn't the same for all spreads !
        //fprintf(debug,"	[column header @ 0x%X]\n",POS);
        //fflush(debug);
        fstr.Seek(POS + PRE, SeekOrigin.Begin); //fseek(f,POS + PRE,SEEK_SET);
        f.Read(name, 0, 25); //fread(&name,25,1,f);
        string[] namestring = enc.GetString(name).Split(new char[] { '_', '\0' });
        string sname = namestring[0]; //strtok(name,"_");	// spreadsheet name
        string cname = namestring[1];	// column name
        if (nr_spreads == 0 || sname != spreadname[nr_spreads - 1])
        {
          //fprintf(debug,"		NEW SPREADSHEET\n");
          spreadname[nr_spreads++] = sname;  // sprintf(spreadname[nr_spreads++],"%s",sname);
          current_col = 1;
          maxrows[nr_spreads] = 0;
        }
        else
        {
          current_col++;
          nr_cols[nr_spreads - 1] = current_col;
        }
        //fprintf(debug,"		SPREADSHEET = %s COLUMN NAME = %s (%d) (@0x%X)\n",sname, cname,current_col,POS+PRE);

        colname[nr_spreads - 1][current_col - 1] = cname; //sprintf(colname[nr_spreads-1][current_col-1],"%s",cname);

        ////////////////////////////// SIZE of column /////////////////////////////////////////////
        fstr.Seek(POS + PRE + COL_SIZE, SeekOrigin.Begin); //fseek(f,POS+PRE+COL_SIZE,SEEK_SET);
        nr = f.ReadInt32(); //fread(&nr,4,1,f);
        DATA = nr;
        nr /= valuesize;
        //fprintf(debug,"	[number of rows = %d @ 0x%X]\n",nr,POS+PRE+COL_SIZE);
        //fflush(debug);
        nr_rows[nr_spreads - 1][current_col - 1] = nr;
        if (maxrows[nr_spreads - 1] < nr)
          maxrows[nr_spreads - 1] = nr;

        ////////////////////////////////////// DATA ////////////////////////////////////////////////
        fstr.Seek(POS + PRE + HEAD, SeekOrigin.Begin); //fseek(f,POS+PRE+HEAD,SEEK_SET);
        //fprintf(debug,"	[data @ 0x%X]\n",POS+PRE+HEAD);
        data[nr_spreads - 1][current_col - 1] = new double[nr]; // (double *) malloc(nr*sizeof(double));
        byte[] valuebuffer = new byte[100];
        for (int i = 0; i < nr; i++)
        {
          f.Read(valuebuffer, 0, valuesize);
          a = BitConverter.ToDouble(valuebuffer, 0); //fread(&a,valuesize,1,f);
          if (Math.Abs(a) < 1.0e-100) a = 0;
          //fprintf(debug,"%g ",a);
          data[nr_spreads - 1][(current_col - 1)][i] = a;
        }
        //fprintf(debug,"\n");
        //fflush(debug);

        DATA = DATA - 1;
        if (version == 410)
          POS += 2;
        int pos = POS + PRE + DATA + HEAD + 0x05;
        fstr.Seek(pos, SeekOrigin.Begin); //fseek(f,pos,SEEK_SET);
        col_found = f.ReadInt32(); // fread(&col_found,4,1,f);
        //fprintf(debug,"	[column found = 0x%X (0x%X : YES) (@ 0x%X)]\n",col_found,NEW_COL,pos);
        //fflush(debug);

        POS += (DATA + HEAD + PRE);
      }

      POS -= 1;
      //fprintf(debug,"\n[position @ 0x%X]\n",POS);
      //fprintf(debug,"		nr_spreads = %d\n",nr_spreads);
      //fflush(debug);

      ///////////////////// SPREADSHEET INFOS ////////////////////////////////////
      int LAYER = 0;
      int COL_JUMP = 0x1ED;
      for (int i = 0; i < nr_spreads; i++)
      {
        if (i > 0)
        {
          if (version == 750)
            POS = LAYER + 0x2759;
          else if (version == 700)
            POS += 0x2530 + nr_cols[i - 1] * COL_JUMP;
          else if (version == 610)
            POS += 0x25A4 + nr_cols[i - 1] * COL_JUMP;
          else if (version == 604)
            POS += 0x25A0 + nr_cols[i - 1] * COL_JUMP;
          else if (version == 600)
            POS += 0x2560 + nr_cols[i - 1] * COL_JUMP;
          else if (version == 500)
            POS += 0x92C + nr_cols[i - 1] * COL_JUMP;
          else if (version == 410)
            POS += 0x7FB + nr_cols[i - 1] * COL_JUMP;
        }
        //fprintf(debug,"\n");

        // HEADER
        // check header
        int ORIGIN = 0x55;
        if (version == 500)
          ORIGIN = 0x58;
        fstr.Seek(POS + ORIGIN, SeekOrigin.Begin); //fseek(f,POS + ORIGIN,SEEK_SET);	// check for 'O'RIGIN
        byte c;
        c = f.ReadByte(); //fread(&c,1,1,f);
        int jump = 0;
        while (c != 'O' && jump < MAX_LEVEL)
        {	// no inf loop
          //fprintf(debug,"	TRY %d	\"O\"RIGIN not found ! : %c (@ 0x%X)",jump+1,c,POS+ORIGIN);
          //fprintf(debug,"		POS=0x%X | ORIGIN = 0x%X\n",POS,ORIGIN);
          POS += 0x1F2;
          fstr.Seek(POS + ORIGIN, SeekOrigin.Begin); //fseek(f,POS + ORIGIN,SEEK_SET);
          c = f.ReadByte(); // fread(&c,1,1,f);
          jump++;
        }

        if (jump == MAX_LEVEL)
        {
          //fprintf(debug,"	Spreadsheet %d SECTION not found ! 	(@ 0x%X)\n",i+1,POS-10*0x1F2+0x55);
          return -5;
        }

        //fprintf(debug,"	OK. Spreadsheet %d SECTION found	(@ 0x%X)\n",i+1,POS);
        //fflush(debug);

        // check spreadsheet name
        fstr.Seek(POS + 0x12, SeekOrigin.Begin); //fseek(f,POS + 0x12,SEEK_SET);
        f.Read(name, 0, 25); //fread(&name,25,1,f);
        // fprintf(debug,"		SPREADSHEET %d NAME : %s	(@ 0x%X) has %d columns\n",	i+1,name,POS + 0x12,nr_cols[i]);

        int ATYPE = 0;
        LAYER = POS;
        if (version == 750)
        {
          // LAYER section
          LAYER += 0x4AB;
          ATYPE = 0xCF;
          COL_JUMP = 0x1F2;
          // seek for "L"ayerInfoStorage to find layer section
          fstr.Seek(LAYER + 0x53, SeekOrigin.Begin);//fseek(f,LAYER+0x53, SEEK_SET);
          c = f.ReadByte(); //fread(&c,1,1,f);
          while (c != 'L' && jump < MAX_LEVEL)
          {	// no inf loop; number of "set column value"
            LAYER += 0x99;
            fstr.Seek(LAYER + 0x53, SeekOrigin.Begin); //fseek(f,LAYER+0x53, SEEK_SET);
            c = f.ReadByte(); // fread(&c,1,1,f);
            jump++;
          }

          if (jump == MAX_LEVEL)
          {
            //fprintf(debug,"		LAYER %d SECTION not found !\nGiving up.",i+1);
            return -3;
          }

          //fprintf(debug,"		[LAYER %d @ 0x%X]\n",i+1,LAYER);
        }
        else if (version == 700)
          ATYPE = 0x2E4;
        else if (version == 610)
          ATYPE = 0x358;
        else if (version == 604)
          ATYPE = 0x354;
        else if (version == 600)
          ATYPE = 0x314;
        else if (version == 500)
        {
          COL_JUMP = 0x5D;
          ATYPE = 0x300;
        }
        else if (version == 410)
        {
          COL_JUMP = 0x58;
          ATYPE = 0x229;
        }
        //fflush(debug);

        /////////////// COLUMN Types ///////////////////////////////////////////
        for (int j = 0; j < nr_cols[i]; j++)
        {
          fstr.Seek(LAYER + ATYPE + j * COL_JUMP, SeekOrigin.Begin); // fseek(f,LAYER+ATYPE+j*COL_JUMP, SEEK_SET);
          c = f.ReadByte(); // fread(&c,1,1,f);
          if (c == 0x41 + j)
          {
            fstr.Seek(LAYER + ATYPE + j * COL_JUMP - 1, SeekOrigin.Begin); //fseek(f,LAYER+ATYPE+j*COL_JUMP-1, SEEK_SET);
            c = f.ReadByte(); // fread(&c,1,1,f);
            string type = null;
            switch (c)
            {
              case 3: type = "X"; break;
              case 0: type = "Y"; break;
              case 5: type = "Z"; break;
              case 6: type = "DX"; break;
              case 2: type = "DY"; break;
              case 4: type = "LABEL"; break;
            }
            coltype[i][j] = type;
            //fprintf(debug,"		COLUMN %c type = %s (@ 0x%X)\n",0x41+j,type,LAYER+ATYPE+j*COL_JUMP);
          }
          else
          {
            //fprintf(debug,"		COLUMN %d (%c) ? (@ 0x%X)\n",j+1,c,LAYER+ATYPE+j*COL_JUMP);
          }
        }
        //fflush(debug);
      }

      ///////////////////////////////////////////////////////////////////////////////////////////////////////////

      // TODO : GRAPHS
      /*	int graph = 0x2fc1;
        int pre_graph = 0x12;
        fseek(f,graph + pre_graph,SEEK_SET);
        fread(&name,25,1,f);
        printf("GRAPH : %s\n",name);

        fseek(f,graph + pre_graph + 0x43, SEEK_SET);
        fread(&name,25,1,f);
        printf("TYPE : %s\n",name);

        fseek(f,graph + pre_graph + 0x2b3, SEEK_SET);
        fread(&name,25,1,f);
        printf("Y AXIS TITLE : %s\n",name);
        fseek(f,graph + pre_graph + 0x38d, SEEK_SET);
        fread(&name,25,1,f);
        printf("X AXIS TITLE : %s\n",name);

        fseek(f,graph + pre_graph + 0xadb, SEEK_SET);
        fread(&name,25,1,f);
        printf("LEGEND : %s\n",name);
      */
      //fclose(debug);

      f.Close();
      fstr.Close();
    }
  }
	return 0;
} 


}
}













