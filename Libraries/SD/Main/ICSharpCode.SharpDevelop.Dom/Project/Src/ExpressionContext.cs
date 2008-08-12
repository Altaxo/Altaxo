// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Class describing a context in which an expression can be.
	/// Serves as filter for code completion results, but the contexts exposed as static fields
	/// can also be used as a kind of enumeration for special behaviour in the resolver.
	/// </summary>
	public abstract class ExpressionContext
	{
		#region Instance members
		public abstract bool ShowEntry(object o);
		
		protected bool readOnly = true;
		object suggestedItem;
		
		/// <summary>
		/// Gets if the expression is in the context of an object creation.
		/// </summary>
		public virtual bool IsObjectCreation {
			get {
				return false;
			}
			set {
				if (value)
					throw new NotSupportedException();
			}
		}
		
		/// <summary>
		/// Gets/Sets the default item that should be included in a code completion popup
		/// in this context and selected as default value.
		/// </summary>
		/// <example>
		/// "List&lt;TypeName&gt; var = new *expr*();" has as suggested item the pseudo-class
		/// "List&lt;TypeName&gt;".
		/// </example>
		public object SuggestedItem {
			get {
				return suggestedItem;
			}
			set {
				if (readOnly)
					throw new NotSupportedException();
				suggestedItem = value;
			}
		}
		
		/// <summary>
		/// Gets if the context expects an attribute.
		/// </summary>
		public virtual bool IsAttributeContext {
			get {
				return false;
			}
		}
		#endregion
		
		#region Default contexts (public static fields)
		/// <summary>Default/unknown context</summary>
		public static ExpressionContext Default = new DefaultExpressionContext();
		
		/// <summary>Context expects a namespace name</summary>
		/// <example>using *expr*;</example>
		public static ExpressionContext Namespace = new ImportableExpressionContext(false);
		
		/// <summary>Context expects an importable type (namespace or class with public static members)</summary>
		/// <example>Imports *expr*;</example>
		public static ExpressionContext Importable = new ImportableExpressionContext(true);
		
		/// <summary>Context expects a type name</summary>
		/// <example>typeof(*expr*), is *expr*, using(*expr* ...)</example>
		public static ExpressionContext Type = new TypeExpressionContext(null, false, true);
		
		/// <summary>Context expects a non-abstract type that has accessible constructors</summary>
		/// <example>new *expr*();</example>
		/// <remarks>When using this context, a resolver should treat the expression as object creation,
		/// even when the keyword "new" is not part of the expression.</remarks>
		public static ExpressionContext ObjectCreation = new TypeExpressionContext(null, true, true);
		
		/// <summary>Context expects a type deriving from System.Attribute.</summary>
		/// <example>[*expr*()]</example>
		/// <remarks>When using this context, a resolver should try resolving typenames with an
		/// appended "Attribute" suffix and treat "invocations" of the attribute type as
		/// object creation.</remarks>
		public static ExpressionContext GetAttribute(IProjectContent projectContent)
		{
			return new TypeExpressionContext(projectContent.GetClass("System.Attribute"), false, true);
		}
		
		/// <summary>Context expects a type name which has special base type</summary>
		/// <param name="baseClass">The class the expression must derive from.</param>
		/// <param name="isObjectCreation">Specifies whether classes must be constructable.</param>
		/// <example>catch(*expr* ...), using(*expr* ...), throw new ***</example>
		public static ExpressionContext TypeDerivingFrom(IClass baseClass, bool isObjectCreation)
		{
			return new TypeExpressionContext(baseClass, isObjectCreation, false);
		}
		
		/// <summary>Context expects an interface</summary>
		/// <example>Implements *expr*</example>
		public static InterfaceExpressionContext Interface = new InterfaceExpressionContext();
		
		#endregion
		
		#region DefaultExpressionContext
		sealed class DefaultExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				return true;
			}
			
			public override string ToString()
			{
				return "[" + GetType().Name + "]";
			}
		}
		#endregion
		
		#region NamespaceExpressionContext
		sealed class ImportableExpressionContext : ExpressionContext
		{
			bool allowImportClasses;
			
			public ImportableExpressionContext(bool allowImportClasses)
			{
				this.allowImportClasses = allowImportClasses;
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string)
					return true;
				IClass c = o as IClass;
				if (allowImportClasses && c != null) {
					return c.HasPublicOrInternalStaticMembers;
				}
				return false;
			}
			
			public override string ToString()
			{
				if (allowImportClasses)
					return "[ImportableExpressionContext]";
				else
					return "[NamespaceExpressionContext]";
			}
		}
		#endregion
		
		#region TypeExpressionContext
		sealed class TypeExpressionContext : ExpressionContext
		{
			IClass baseClass;
			bool isObjectCreation;
			
			public TypeExpressionContext(IClass baseClass, bool isObjectCreation, bool readOnly)
			{
				this.baseClass = baseClass;
				this.isObjectCreation = isObjectCreation;
				this.readOnly = readOnly;
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string)
					return true;
				IClass c = o as IClass;
				if (c == null)
					return false;
				if (isObjectCreation) {
					if (c.IsAbstract || c.IsStatic)    return false;
					if (c.ClassType == ClassType.Enum || c.ClassType == ClassType.Interface)
						return false;
				}
				if (baseClass == null)
					return true;
				return c.IsTypeInInheritanceTree(baseClass);
			}
			
			public override bool IsObjectCreation {
				get {
					return isObjectCreation;
				}
				set {
					if (readOnly && value != isObjectCreation)
						throw new NotSupportedException();
					isObjectCreation = value;
				}
			}
			
			public override bool IsAttributeContext {
				get {
					return baseClass != null && baseClass.FullyQualifiedName == "System.Attribute";
				}
			}
			
			public override string ToString()
			{
				if (baseClass != null)
					return "[" + GetType().Name + ": " + baseClass.FullyQualifiedName
						+ " IsObjectCreation=" + IsObjectCreation + "]";
				else
					return "[" + GetType().Name + " IsObjectCreation=" + IsObjectCreation + "]";
			}
		}
		#endregion
		
		#region CombinedExpressionContext
		public static ExpressionContext operator | (ExpressionContext a, ExpressionContext b)
		{
			return new CombinedExpressionContext(0, a, b);
		}
		
		public static ExpressionContext operator & (ExpressionContext a, ExpressionContext b)
		{
			return new CombinedExpressionContext(1, a, b);
		}
		
		public static ExpressionContext operator ^ (ExpressionContext a, ExpressionContext b)
		{
			return new CombinedExpressionContext(2, a, b);
		}
		
		sealed class CombinedExpressionContext : ExpressionContext
		{
			byte opType; // 0 = or ; 1 = and ; 2 = xor
			ExpressionContext a;
			ExpressionContext b;
			
			public CombinedExpressionContext(byte opType, ExpressionContext a, ExpressionContext b)
			{
				if (a == null)
					throw new ArgumentNullException("a");
				if (b == null)
					throw new ArgumentNullException("a");
				this.opType = opType;
				this.a = a;
				this.b = b;
			}
			
			public override bool ShowEntry(object o)
			{
				if (opType == 0)
					return a.ShowEntry(o) || b.ShowEntry(o);
				if (opType == 1)
					return a.ShowEntry(o) && b.ShowEntry(o);
				return a.ShowEntry(o) ^ b.ShowEntry(o);
			}
			
			public override string ToString()
			{
				string op = " XOR ";
				if (opType == 0)
					op = " OR ";
				else if (opType == 1)
					op = " AND ";
				return "[" + GetType().Name + ": " + a + op + b + "]";
			}
		}
		#endregion
		
		#region InterfaceExpressionContext
		public class InterfaceExpressionContext : ExpressionContext
		{
			public InterfaceExpressionContext()
			{
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string)
					return true;
				IClass c = o as IClass;
				if (c == null)
					return false;
				
				return c.ClassType == ClassType.Interface;
			}
			
			public override string ToString()
			{
				return "[" + GetType().Name + "]";
			}
		}
		#endregion
	}
}
