using System;
using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Main
{

	/// <summary>
	/// Summary description for ICompressedFileContainerStream.
	/// </summary>
	public class ZipOutputStreamWrapper : ICompressedFileContainerStream
	{
		ICSharpCode.SharpZipLib.Zip.ZipOutputStream _stream;

		public ZipOutputStreamWrapper(ICSharpCode.SharpZipLib.Zip.ZipOutputStream stream)
		{
			_stream = stream;
		}

		public void StartFile(string filename, int level)
		{
			ZipEntry ZipEntry = new ZipEntry(filename);
			_stream.PutNextEntry(ZipEntry);
			_stream.SetLevel(level);
		}

		public System.IO.Stream Stream { get { return _stream; } }
	}

	


	public class ZipEntryWrapper :  IFileContainerItem
	{
		ZipEntry _zipEntry;
		public ZipEntryWrapper(ZipEntry zipEntry)
		{
			_zipEntry = zipEntry;
		}
		public bool IsDirectory { get { return _zipEntry.IsDirectory; }}
		public string Name { get { return _zipEntry.Name; }}
		public static implicit operator ZipEntry(ZipEntryWrapper wrap)
		{
			return wrap._zipEntry;
		}
	}


	public class ZipEntryWrapperEnumerator : System.Collections.IEnumerator
	{
		System.Collections.IEnumerator enumerator;
		public ZipEntryWrapperEnumerator(ZipFile file)
		{
			enumerator = file.GetEnumerator();
		}

		public object Current 
		{ 
			get { return new ZipEntryWrapper((ZipEntry)enumerator.Current); }
		}
		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}
		public void Reset()
		{
			enumerator.Reset();
		}
		}

	public class ZipFileWrapper : ICompressedFileContainer
	{
		ZipFile _zip;

		public ZipFileWrapper(ZipFile zipFile)
		{
			_zip = zipFile;
		}

		public System.IO.Stream GetInputStream(IFileContainerItem item)
		{
			return _zip.GetInputStream(((ZipEntryWrapper)item));
		}

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new ZipEntryWrapperEnumerator(_zip); 
		}

    public static implicit operator ZipFile(ZipFileWrapper wrapper)
    {
      return wrapper._zip;
    }
	}
}
