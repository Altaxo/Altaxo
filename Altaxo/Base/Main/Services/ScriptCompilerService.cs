#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace Altaxo.Main.Services
{
	public interface IScriptCompilerResult
	{
		int ScriptTextCount { get; }

		string ScriptText(int i);

		string ScriptTextHash { get; }
	}

	public interface IScriptCompilerSuccessfulResult : IScriptCompilerResult
	{
		System.Reflection.Assembly ScriptAssembly { get; }
	}

	public interface IScriptCompilerFailedResult : IScriptCompilerResult
	{
		IList<string> CompileErrors { get; } // TODO NET45 replace with IReadonlyList<string>
	}

	/// <summary>
	/// Summary description for ScriptCompilerService.
	/// </summary>
	public class ScriptCompilerService
	{
		#region internal classes

		#region FileHash

		public struct FileHash : IComparable
		{
			public ulong Lo;
			public ulong Hi;

			public FileHash(byte[] hash)
				: this(hash, hash.Length)
			{
			}

			public FileHash(byte[] hash, int len)
			{
				if (hash == null)
				{
					Lo = 0;
					Hi = 0;
				}
				else if (len == 16)
				{
					Lo = System.BitConverter.ToUInt64(hash, 0);
					Hi = System.BitConverter.ToUInt64(hash, 8);
				}
				else
				{
					throw new ArgumentException("Unexpected hash length of " + hash.Length);
				}
			}

			public static FileHash FromBinHexRepresentation(string binhex)
			{
				if (binhex.Length != 32)
					throw new ArgumentException("BinHexRepresentation must have a length of 32");

				FileHash hash;
				hash.Hi = ulong.Parse(binhex.Substring(0, 16), System.Globalization.NumberStyles.AllowHexSpecifier);
				hash.Lo = ulong.Parse(binhex.Substring(16, 16), System.Globalization.NumberStyles.AllowHexSpecifier);
				return hash;
			}

			public bool Valid
			{
				get
				{
					return Lo != 0 || Hi != 0;
				}
			}

			public override bool Equals(object obj)
			{
				return (obj is FileHash) && (this == (FileHash)obj);
			}

			public override int GetHashCode()
			{
				return Lo.GetHashCode() + Hi.GetHashCode();
			}

			public string BinHexRepresentation
			{
				get
				{
					return Hi.ToString("X16") + Lo.ToString("X16");
				}
			}

			public string MediumFileName
			{
				get
				{
					return string.Format("X{0}.XXX", BinHexRepresentation);
				}
			}

			public static bool operator ==(FileHash a, FileHash b)
			{
				return a.Hi == b.Hi && a.Lo == b.Lo;
			}

			public static bool operator !=(FileHash a, FileHash b)
			{
				return a.Hi != b.Hi || a.Lo != b.Lo;
			}

			public static bool operator <(FileHash a, FileHash b)
			{
				return a.Hi < b.Hi || a.Lo < b.Lo;
			}

			public static bool operator >(FileHash a, FileHash b)
			{
				return a.Hi > b.Hi || a.Lo > b.Lo;
			}

			#region IComparable Members

			public int CompareTo(object obj)
			{
				if (obj is FileHash)
				{
					return this == (FileHash)obj ? 0 : (this > (FileHash)obj ? 1 : -1);
				}
				else if (obj == null)
				{
					return 0;
				}
				else
					throw new ArgumentException("Argument is not of expected type, but of type " + obj.GetType().ToString());
			}

			#endregion IComparable Members

			public static string ComputeScriptTextHash(string[] scripts)
			{
				Array.Sort(scripts);
				int len = 0;
				for (int i = 0; i < scripts.Length; i++)
					len += scripts[i].Length;

				byte[] hash = null;

				using (System.IO.MemoryStream stream = new System.IO.MemoryStream(len))
				{
					using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream, System.Text.Encoding.Unicode))
					{
						for (int i = 0; i < scripts.Length; i++)
						{
							sw.Write(scripts[i]);
						}
						sw.Flush();

						sw.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
						System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
						hash = md5.ComputeHash(sw.BaseStream);
						sw.Close();
					}
				}
				return new FileHash(hash).BinHexRepresentation;
			}
		}

		#endregion FileHash

		#region ScriptCompilerResult

		private class ScriptCompilerSuccessfulResult : IScriptCompilerSuccessfulResult
		{
			private Assembly _scriptAssembly;
			private string[] _scriptText;
			private string _scriptTextHash;

			public ScriptCompilerSuccessfulResult(string[] scriptText, string scriptTextHash, Assembly scriptAssembly)
			{
				_scriptText = (string[])scriptText.Clone();
				_scriptAssembly = scriptAssembly;
				_scriptTextHash = scriptTextHash;
			}

			#region IScriptCompilerResult Members

			public string ScriptTextHash
			{
				get
				{
					return _scriptTextHash;
				}
			}

			public Assembly ScriptAssembly
			{
				get
				{
					return _scriptAssembly;
				}
			}

			public int ScriptTextCount
			{
				get
				{
					return _scriptText.Length;
				}
			}

			public string ScriptText(int i)
			{
				return _scriptText[i];
			}

			#endregion IScriptCompilerResult Members
		}

		private class ScriptCompilerFailedResult : IScriptCompilerResult
		{
			private string[] _scriptText;
			private string _scriptTextHash;
			private string[] _compileErrors;

			public ScriptCompilerFailedResult(string[] scriptText, string scriptTextHash, string[] compileErrors)
			{
				_scriptText = (string[])scriptText.Clone();
				_scriptTextHash = scriptTextHash;
				_compileErrors = (string[])compileErrors.Clone();
			}

			#region IScriptCompilerResult Members

			public string ScriptTextHash
			{
				get
				{
					return _scriptTextHash;
				}
			}

			public Assembly ScriptAssembly
			{
				get
				{
					return null;
				}
			}

			public int ScriptTextCount
			{
				get
				{
					return _scriptText.Length;
				}
			}

			public string ScriptText(int i)
			{
				return _scriptText[i];
			}

			#endregion IScriptCompilerResult Members
		}

		#endregion ScriptCompilerResult

		#region ConcurrentScriptCompilerDictionary

		private class ConcurrentScriptCompilerDictionary
		{
			private Dictionary<string, IScriptCompilerResult> _compilerResultsByTextHash = new Dictionary<string, IScriptCompilerResult>();
			private Dictionary<Assembly, ScriptCompilerSuccessfulResult> _compilerResultsByAssembly = new Dictionary<Assembly, ScriptCompilerSuccessfulResult>();
			private System.Threading.ReaderWriterLockSlim _lock = new System.Threading.ReaderWriterLockSlim();

			internal bool TryAdd(IScriptCompilerResult result)
			{
				if (null == result)
					throw new ArgumentNullException(nameof(result));

				_lock.EnterUpgradeableReadLock();
				try
				{
					if (_compilerResultsByTextHash.ContainsKey(result.ScriptTextHash))
					{
						return false;
					}
					else
					{
						_lock.EnterWriteLock();
						try
						{
							_compilerResultsByTextHash.Add(result.ScriptTextHash, result);
							if (result is ScriptCompilerSuccessfulResult successfulResult)
							{
								_compilerResultsByAssembly.Add(successfulResult.ScriptAssembly, successfulResult);
							}
							return true;
						}
						finally
						{
							_lock.ExitWriteLock();
						}
					}
				}
				finally
				{
					_lock.ExitUpgradeableReadLock();
				}
			}

			internal bool TryGetValue(string scriptTextHash, out IScriptCompilerResult result)
			{
				if (string.IsNullOrEmpty(scriptTextHash))
					throw new ArgumentNullException("scriptTextHash");

				_lock.EnterReadLock();
				try
				{
					return _compilerResultsByTextHash.TryGetValue(scriptTextHash, out result);
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}

			internal bool TryGetValue(Assembly assembly, out ScriptCompilerSuccessfulResult result)
			{
				if (null == assembly)
					throw new ArgumentNullException(nameof(assembly));

				_lock.EnterReadLock();
				try
				{
					return _compilerResultsByAssembly.TryGetValue(assembly, out result);
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}
		}

		#endregion ConcurrentScriptCompilerDictionary

		#endregion internal classes

		private static ConcurrentScriptCompilerDictionary _compilerResults = new ConcurrentScriptCompilerDictionary();

		public IScriptCompilerSuccessfulResult GetCompilerResult(Assembly ass)
		{
			if (_compilerResults.TryGetValue(ass, out var result))
				return result;
			else
				return null;
		}

		/// <summary>
		/// Computes the Script text hash of a single script text.
		/// </summary>
		/// <param name="scriptText">The script text.</param>
		/// <returns>A hash string which unique identifies the script text.</returns>
		public static string ComputeScriptTextHash(string scriptText)
		{
			return FileHash.ComputeScriptTextHash(new string[] { scriptText });
		}

		/// <summary>
		/// Does the compilation of the script into an assembly. The assembly is stored together with
		/// the read-only source code and returned as result. As list of compiled source codes is maintained by this class.
		/// If you provide a text that was already compiled before, the already compiled assembly is returned instead
		/// of a freshly compiled assembly.
		/// </summary>
		/// <returns>True if successfully compiles, otherwise false.</returns>
		public static IScriptCompilerResult Compile(string[] scriptText)
		{
			IScriptCompilerResult result;

			string scriptTextHash = FileHash.ComputeScriptTextHash(scriptText);

			if (_compilerResults.TryGetValue(scriptTextHash, out result))
			{
				return result;
			}

			var providerOptions = new Dictionary<string, string>
			{
				{ "CompilerVersion", "v4.0" }
			};
			Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider(providerOptions);

			// For Visual Basic Compiler try this :
			//Microsoft.VisualBasic.VBCodeProvider

			System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();

			parameters.GenerateInMemory = true;
			parameters.IncludeDebugInformation = true;
			// parameters.OutputAssembly = this.ScriptName;

			// Add available assemblies including the application itself
			foreach (string loc in Settings.Scripting.ReferencedAssemblies.AllLocations)
				parameters.ReferencedAssemblies.Add(loc);

			CompilerResults results;
			if (scriptText.Length == 1)
				results = codeProvider.CompileAssemblyFromSource(parameters, scriptText[0]);
			else
				results = codeProvider.CompileAssemblyFromSource(parameters, scriptText);

			if (results.Errors.Count > 0)
			{
				var errors = new string[results.Errors.Count];
				int i = 0;
				foreach (CompilerError err in results.Errors)
				{
					errors[i++] = err.ToString();
				}

				result = new ScriptCompilerFailedResult(scriptText, scriptTextHash, errors);
				_compilerResults.TryAdd(result);

				return result;
			}
			else
			{
				result = new ScriptCompilerSuccessfulResult(scriptText, scriptTextHash, results.CompiledAssembly);
				_compilerResults.TryAdd(result);
				return result;
			}
		}
	}
}