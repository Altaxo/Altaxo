// created on 06.08.2003 at 12:34

using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpRefactory.Parser;

namespace CSharpBinding.Parser.SharpDevelopTree
{
	public class Indexer : AbstractIndexer
	{
		public void AddModifier(ModifierEnum m)
		{
			modifiers = modifiers | m;
		}
		
		public Indexer(ReturnType type, ParameterCollection parameters, Modifier m, IRegion region, IRegion bodyRegion)
		{
			returnType      = type;
			this.parameters = parameters;
			this.region     = region;
			this.bodyRegion = bodyRegion;
			modifiers = (ModifierEnum)m;
		}
	}
}
