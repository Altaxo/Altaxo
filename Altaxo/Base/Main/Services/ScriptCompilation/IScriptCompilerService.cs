using System.Reflection;

namespace Altaxo.Main.Services.ScriptCompilation
{
	/// <summary>
	/// Provides compilation of scripts to assemblies.
	/// </summary>
	public interface IScriptCompilerService
	{
		IScriptCompilerSuccessfulResult GetCompilerResult(Assembly ass);

		/// <summary>
		/// Does the compilation of the script into an assembly. The assembly is stored together with
		/// the read-only source code and returned as result. As list of compiled source codes is maintained by this class.
		/// If you provide a text that was already compiled before, the already compiled assembly is returned instead
		/// of a freshly compiled assembly.
		/// </summary>
		/// <returns>True if successfully compiles, otherwise false.</returns>
		IScriptCompilerResult Compile(string[] scriptText);
	}
}