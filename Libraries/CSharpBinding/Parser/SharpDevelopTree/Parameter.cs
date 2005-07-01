// created on 07.08.2003 at 20:12

using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpRefactory.Parser;

namespace CSharpBinding.Parser.SharpDevelopTree
{
	public class Parameter : AbstractParameter
	{
		public Parameter(string name, ReturnType type, ParamModifiers m)
		{
			Name = name;
			returnType = type;
			if (m == ParamModifiers.Ref) {
				this.Modifier = ParameterModifier.Ref;
			} else if (m == ParamModifiers.Out) {
				this.Modifier = ParameterModifier.Out;
			} else if (m == ParamModifiers.Params) {
				this.Modifier = ParameterModifier.Params;
			}
		}
	}
}
