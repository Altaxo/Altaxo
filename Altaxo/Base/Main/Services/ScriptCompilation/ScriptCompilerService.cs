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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Altaxo.Main.Services.ScriptCompilation
{
	/// <summary>
	/// Summary description for ScriptCompilerService.
	/// </summary>
	public class ScriptCompilerService
	{
		private static CachedService<IScriptCompilerService, IScriptCompilerService> _instance = new CachedService<IScriptCompilerService, IScriptCompilerService>(true, null, null);

		protected static IScriptCompilerService Instance => _instance.Instance;

		public IScriptCompilerSuccessfulResult GetCompilerResult(Assembly ass)
		{
			return Instance.GetCompilerResult(ass);
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
		/// Computes the Script text hash of a single script text.
		/// </summary>
		/// <param name="scriptText">The script text.</param>
		/// <returns>A hash string which unique identifies the script text.</returns>
		public static string ComputeScriptTextHash(string[] scriptText)
		{
			return FileHash.ComputeScriptTextHash(scriptText);
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
			return Instance.Compile(scriptText);
		}

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

		#endregion internal classes

		protected class ScriptCompilerServiceImpl : IScriptCompilerService
		{
			private ConcurrentScriptCompilerResultDictionary _compilerResults = new ConcurrentScriptCompilerResultDictionary();

			public IScriptCompilerSuccessfulResult GetCompilerResult(Assembly ass)
			{
				if (_compilerResults.TryGetValue(ass, out var result))
					return result;
				else
					return null;
			}

			/// <summary>
			/// Does the compilation of the script into an assembly. The assembly is stored together with
			/// the read-only source code and returned as result. As list of compiled source codes is maintained by this class.
			/// If you provide a text that was already compiled before, the already compiled assembly is returned instead
			/// of a freshly compiled assembly.
			/// </summary>
			/// <returns>True if successfully compiles, otherwise false.</returns>
			public IScriptCompilerResult Compile(string[] scriptText)
			{
				IScriptCompilerResult result;

				var scriptTextWithHash = new CodeTextsWithHash(scriptText);

				if (_compilerResults.TryGetValue(scriptTextWithHash.Hash, out result))
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
				foreach (string loc in Settings.Scripting.ReferencedAssemblies.All.Select(ass => ass.Location))
					parameters.ReferencedAssemblies.Add(loc);

				CompilerResults results;
				if (scriptText.Length == 1)
					results = codeProvider.CompileAssemblyFromSource(parameters, scriptText[0]);
				else
					results = codeProvider.CompileAssemblyFromSource(parameters, scriptText);

				if (results.Errors.Count > 0)
				{
					var errors = new List<ICompilerDiagnostic>(results.Errors.Count);

					foreach (CompilerError err in results.Errors)
					{
						errors.Add(new CompilerDiagnostic(err.Line, err.Column, err.IsWarning ? DiagnosticSeverity.Warning : DiagnosticSeverity.Error, err.ErrorText));
					}

					result = new ScriptCompilerFailedResult(scriptTextWithHash, errors);
					_compilerResults.TryAdd(result);

					return result;
				}
				else
				{
					result = new ScriptCompilerSuccessfulResult(scriptTextWithHash, results.CompiledAssembly);
					_compilerResults.TryAdd(result);
					return result;
				}
			}
		}
	}
}