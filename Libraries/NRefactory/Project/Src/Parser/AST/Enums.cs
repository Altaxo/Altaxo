// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1018 $</version>
// </file>

using System;

namespace ICSharpCode.NRefactory.Parser.AST
{
	[Flags]
	public enum Modifier // TODO: Rename to Modifiers
	{
		None      = 0x0000,
		
		// Access
		Private   = 0x0001,
		/// <summary>C# 'internal', VB 'Friend'</summary>
		Internal  = 0x0002,
		Protected = 0x0004,
		Public    = 0x0008,
		Dim	      = 0x0010,	// VB.NET SPECIFIC
		
		// Scope
		Abstract  = 0x0010,  // == 	MustOverride/MustInherit
		Virtual   = 0x0020,
		Sealed    = 0x0040,
		/// <summary>C# 'static', VB 'Shared'</summary>
		Static    = 0x0080,
		Override  = 0x0100,
		/// <summary>For fields: readonly (c# and vb), for properties: get-only (vb)</summary>
		ReadOnly  = 0x0200,
		Const	  = 0x0400,
		/// <summary>C# 'new', VB 'Shadows'</summary>
		New       = 0x0800,
		Partial   = 0x1000,
		
		// Special
		Extern     = 0x2000,
		Volatile   = 0x4000,
		Unsafe     = 0x8000,
		Overloads  = 0x10000, // VB specific
		WithEvents = 0x20000, // VB specific
		Default    = 0x40000, // VB specific
		Narrowing  = 0x80000, // VB specific
		Widening   = 0x100000, // VB specific
		Synthetic  = 0x200000,
		/// <summary>Only for VB properties.</summary>
		WriteOnly  = 0x400000, // VB specific
		
		Visibility						= Private | Public | Protected | Internal,
		Classes							= New | Visibility | Abstract | Sealed | Partial | Static,
		VBModules						= Visibility,
		VBStructures					= Visibility | New,
		VBEnums						    = Visibility | New,
		VBInterfacs					    = Visibility | New,
		VBDelegates					    = Visibility | New,
		VBMethods						= Visibility | New | Static | Virtual | Sealed | Abstract | Override | Overloads,
		VBExternalMethods				= Visibility | New | Overloads,
		VBEvents						= Visibility | New | Overloads,
		VBProperties					= VBMethods | Default | ReadOnly | WriteOnly,
		VBCustomEvents					= Visibility | New | Overloads,
		VBOperators						= Public | Static | Overloads | New | Widening | Narrowing,
		
		
		// this is not documented in the spec
		VBInterfaceEvents				= New,
		VBInterfaceMethods				= New | Overloads,
		VBInterfaceProperties			= New | Overloads | ReadOnly | WriteOnly | Default,
		VBInterfaceEnums				= New,
		
		Fields                          = New | Visibility | Static   | ReadOnly | Volatile,
		PropertysEventsMethods          = New | Visibility | Static   | Virtual  | Sealed   | Override | Abstract | Extern,
		Indexers                        = New | Visibility | Virtual  | Sealed   | Override | Abstract | Extern,
		Operators                       = Public | Static | Extern,
		Constants                       = New | Visibility,
		StructsInterfacesEnumsDelegates = New | Visibility | Partial,
		StaticConstructors              = Extern | Static | Unsafe,
		Destructors                     = Extern | Unsafe,
		Constructors                    = Visibility | Extern,
	}
	
	public enum ClassType
	{
		Class,
		Module,
		Interface,
		Struct,
		Enum
	}
	
	public enum ParentType
	{
		ClassOrStruct,
		InterfaceOrEnum,
		Namespace,
		Unknown
	}
	
	public enum FieldDirection
	{
		None,
		In,
		Out,
		Ref
	}
	
	[Flags]
	public enum ParamModifier // TODO: Rename to ParamModifiers
	{
		// Values must be the same as in SharpDevelop's ParameterModifiers
		None = 0,
		In  = 1,
		Out = 2,
		Ref = 4,
		Params = 8,
		Optional = 16
	}
	
	public enum AssignmentOperatorType
	{
		None,
		Assign,
		
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		
		Power,         // (VB only)
		DivideInteger, // (VB only)
		ConcatString,  // (VB only)
		
		ShiftLeft,
		ShiftRight,
		
		BitwiseAnd,
		BitwiseOr,
		ExclusiveOr,
	}
	
	public enum BinaryOperatorType
	{
		None,
		
		/// <summary>'&amp;' in C#, 'And' in VB.</summary>
		BitwiseAnd,
		/// <summary>'|' in C#, 'Or' in VB.</summary>
		BitwiseOr,
		/// <summary>'&amp;&amp;' in C#, 'AndAlso' in VB.</summary>
		LogicalAnd,
		/// <summary>'||' in C#, 'OrElse' in VB.</summary>
		LogicalOr,
		/// <summary>'^' in C#, 'Xor' in VB.</summary>
		ExclusiveOr,
		
		/// <summary>&gt;</summary>
		GreaterThan,
		/// <summary>&gt;=</summary>
		GreaterThanOrEqual,
		/// <summary>'==' in C#, '=' in VB.</summary>
		Equality,
		/// <summary>'!=' in C#, '&lt;&gt;' in VB.</summary>
		InEquality,
		/// <summary>&lt;</summary>
		LessThan,
		/// <summary>&lt;=</summary>
		LessThanOrEqual,
		
		/// <summary>+</summary>
		Add,
		/// <summary>-</summary>
		Subtract,
		/// <summary>*</summary>
		Multiply,
		/// <summary>/</summary>
		Divide,
		/// <summary>'%' in C#, 'Mod' in VB.</summary>
		Modulus,
		/// <summary>VB-only: \</summary>
		DivideInteger,
		/// <summary>VB-only: ^</summary>
		Power,
		/// <summary>VB-only: &amp;</summary>
		Concat,
		
		/// <summary>C#: &lt;&lt;</summary>
		ShiftLeft,
		/// <summary>C#: &gt;&gt;</summary>
		ShiftRight,
		/// <summary>VB-only: Is</summary>
		ReferenceEquality,
		/// <summary>VB-only: IsNot</summary>
		ReferenceInequality,
		
		/// <summary>VB-only: Like</summary>
		Like,
		/// <summary>C#: ??</summary>
		NullCoalescing,
	}
	
	public enum CastType
	{
		/// <summary>
		/// direct cast (C#, VB "DirectCast")
		/// </summary>
		Cast,
		/// <summary>
		/// try cast (C# "as", VB "TryCast")
		/// </summary>
		TryCast,
		/// <summary>
		/// converting cast (VB "CType")
		/// </summary>
		Conversion,
		/// <summary>
		/// primitive converting cast (VB "CString" etc.)
		/// </summary>
		PrimitiveConversion
	}
	
	public enum UnaryOperatorType
	{
		None,
		Not,
		BitNot,
		
		Minus,
		Plus,
		
		Increment,
		Decrement,
		
		PostIncrement,
		PostDecrement,
		
		Star,
		BitWiseAnd
	}
	
	public enum ContinueType
	{
		None,
		Do,
		For,
		While
	}
	
	public enum ConditionType
	{
		None,
		Until,
		While,
		DoWhile
	}
	
	public enum ConditionPosition
	{
		None,
		Start,
		End
	}
	
	public enum ExitType
	{
		None,
		Sub,
		Function,
		Property,
		Do,
		For,
		While,
		Select,
		Try
	}
	
	public enum ConstructorInitializerType
	{
		None,
		Base,
		This
	}
	
	public enum ConversionType
	{
		None,
		Implicit,
		Explicit
	}
	
	public enum OverloadableOperatorType
	{
		None,
		
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		Concat,
		
		Not,
		BitNot,
		
		BitwiseAnd,
		BitwiseOr,
		ExclusiveOr,
		
		ShiftLeft,
		ShiftRight,
		
		GreaterThan,
		GreaterThanOrEqual,
		Equality,
		InEquality,
		LessThan,
		LessThanOrEqual,
		
		Increment,
		Decrement,
		
		True,
		False,
		
		// VB specific
		IsTrue,
		IsFalse,
		Like,
		Power,
		CType,
		DivideInteger
	}
	
	///<summary>
	/// Charset types, used in external methods
	/// declarations (VB only).
	///</summary>
	public enum CharsetModifier
	{
		None,
		Auto,
		Unicode,
		ANSI
	}
	
	///<summary>
	/// Compare type, used in the <c>Option Compare</c>
	/// pragma (VB only).
	///</summary>
	public enum OptionType
	{
		None,
		Explicit,
		Strict,
		CompareBinary,
		CompareText
	}
}
