//// Main.cs
//// Copyright (C) 2001 Mike Krueger
////
//// This program is free software; you can redistribute it and/or
//// modify it under the terms of the GNU General Public License
//// as published by the Free Software Foundation; either version 2
//// of the License, or (at your option) any later version.
////
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with this program; if not, write to the Free Software
//// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
////
//// Linking this library statically or dynamically with other modules is
//// making a combined work based on this library.  Thus, the terms and
//// conditions of the GNU General Public License cover the whole
//// combination.
//// 
//// As a special exception, the copyright holders of this library give you
//// permission to link this library with independent modules to produce an
//// executable, regardless of the license terms of these independent
//// modules, and to copy and distribute the resulting executable under
//// terms of your choice, provided that you also meet, for each linked
//// independent module, the terms and conditions of the license of that
//// module.  An independent module is a module which is not derived from
//// or based on this library.  If you modify this library, you may extend
//// this exception to your version of the library, but you are not
//// obligated to do so.  If you do not wish to do so, delete this
//// exception statement from your version.
//
//using System;
//using System.Text;
//using System.IO;
//using ICSharpCode.SharpZipLib.Checksums;
//using ICSharpCode.SharpZipLib.Zip.Compression;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
//using ICSharpCode.SharpZipLib.GZip;
//
//namespace ICSharpCode.SharpZipLib.Zip
//{
//	class MainClass
//	{
//		public static void Main(string[] args)
//		{
//			Crc32 crc = new Crc32();
//			ZipOutputStream s = new ZipOutputStream(File.Create("C:\\mytest.zip"));
//			s.Password = "MyPassword";
//			s.SetLevel(9); // 0 - store only to 9 - means best compression
//			
//			FileStream fs = File.OpenRead("C:\\a.cs");
//			
//			byte[] buffer = new byte[fs.Length];
//			fs.Read(buffer, 0, buffer.Length);
//			ZipEntry entry = new ZipEntry("a.cs");
//			entry.DateTime = DateTime.Now;
//			
//			// set Size and the crc, because the information
//			// about the size and crc should be stored in the header
//			// if it is not set it is automatically written in the footer.
//			// (in this case size == crc == -1 in the header)
//			// Some ZIP programs have problems with zip files that don't store
//			// the size and crc in the header.
//			entry.Size = fs.Length;
//			fs.Close();
//			
//			crc.Reset();
//			crc.Update(buffer);
//			
//			entry.Crc  = crc.Value;
//			
//			s.PutNextEntry(entry);
//			
//			s.Write(buffer, 0, buffer.Length);
//			
//			s.Finish();
//			s.Close();
//		}
//	}
//}
