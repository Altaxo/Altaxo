// created on 04.08.2003 at 18:08

using SharpDevelop.Internal.Parser;

namespace CSharpBinding.Parser.SharpDevelopTree
{
	public class ReturnType : AbstractReturnType
	{
		public new int PointerNestingLevel {
			get {
				return base.pointerNestingLevel;
			}
			set {
				base.pointerNestingLevel = value;
			}
		}
		
		public new int[] ArrayDimensions {
			get {
				return base.arrayDimensions;
			}
			set {
				base.arrayDimensions = value;
			}
		}
		
		public ReturnType(string fullyQualifiedName)
		{
			base.fullyQualifiedName = fullyQualifiedName;
		}
		
		public ReturnType(string fullyQualifiedName, int[] arrayDimensions, int pointerNestingLevel)
		{
			this.fullyQualifiedName  = fullyQualifiedName;
			this.arrayDimensions     = arrayDimensions;
			this.pointerNestingLevel = pointerNestingLevel;
		}
		
		public ReturnType(ICSharpCode.SharpRefactory.Parser.AST.TypeReference type)
		{
			base.fullyQualifiedName  = type.SystemType;
			base.arrayDimensions     = type.RankSpecifier == null ? new int[] { } : type.RankSpecifier;
			base.pointerNestingLevel = type.PointerNestingLevel;
		}
		public ReturnType Clone()
		{
			return new ReturnType(fullyQualifiedName, arrayDimensions, pointerNestingLevel);
		}
	}
}
