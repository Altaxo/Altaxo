// created on 06.08.2003 at 12:35

using SharpDevelop.Internal.Parser;
using ICSharpCode.SharpRefactory.Parser;

namespace CSharpBinding.Parser.SharpDevelopTree
{
	public class Destructor : AbstractMethod
	{
		public void AddModifier(ModifierEnum m)
		{
			modifiers = modifiers | m;
		}
		
		public Destructor(string className, Modifier m, IRegion region, IRegion bodyRegion)
		{
			FullyQualifiedName = "~" + className;
			this.region     = region;
			this.bodyRegion = bodyRegion;
			modifiers = (ModifierEnum)m;
		}
	}
}
