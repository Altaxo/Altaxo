using System;

namespace Altaxo.Main
{
	/// <summary>
	/// Summary description for ICompressedFileContainerStream.
	/// </summary>
	public interface ICompressedFileContainerStream
	{
		void StartFile(string filename, int level);
		//ZipEntry ZipEntry = new ZipEntry(filename);
		//zippedStream.PutNextEntry(ZipEntry);
		//zippedStream.SetLevel(level);

		System.IO.Stream Stream { get; }

	}

	public interface IFileContainerItem
	{
		bool IsDirectory { get; }
		string Name { get; }
	}

	public interface IFileContainerItemEnumerator 
	{
		IFileContainerItem Current { get; }
		bool MoveNext();
	}

	public interface ICompressedFileContainer : System.Collections.IEnumerator	
	{
		System.IO.Stream GetInputStream(IFileContainerItem item);
		IFileContainerItemEnumerator GetEnumerator();
	}
}
