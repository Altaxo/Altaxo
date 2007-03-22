﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2340 $</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	/// <summary>
	/// Dummy class that implements the IClass interface. The
	/// only property this mock class implements is the DefaultReturnType
	/// property.
	/// </summary>
	public class MockClass : IClass
	{
		IReturnType returnType;
		string qualifiedName = String.Empty;
		
		public MockClass(string qualifiedName)
		{
			this.qualifiedName = qualifiedName;
		}
		
		public string FullyQualifiedName {
			get {
				return qualifiedName;
			}
			set {
				qualifiedName = value;
			}
		}
		
		public IReturnType DefaultReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}
		
		public string DotNetName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Namespace {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ClassType ClassType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICompilationUnit CompilationUnit {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DomRegion Region {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DomRegion BodyRegion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<IReturnType> BaseTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<IClass> InnerClasses {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<IField> Fields {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<IProperty> Properties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<IMethod> Methods {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<IEvent> Events {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<ITypeParameter> TypeParameters {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<IClass> ClassInheritanceTree {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IClass BaseClass {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType BaseType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool HasPublicOrInternalStaticMembers {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool HasExtensionMethods {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsPartial {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IClass DeclaringType {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ModifierEnum Modifiers {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IList<IAttribute> Attributes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Documentation {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsAbstract {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsSealed {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsStatic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsConst {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsVirtual {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsPublic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsProtected {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsPrivate {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsInternal {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsReadonly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsProtectedAndInternal {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsProtectedOrInternal {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOverride {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsOverridable {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsNew {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsSynthetic {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object UserData {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IReturnType GetBaseType(int index)
		{
			throw new NotImplementedException();
		}
		
		public IClass GetCompoundClass()
		{
			throw new NotImplementedException();
		}
		
		public IClass GetInnermostClass(int caretLine, int caretColumn)
		{
			throw new NotImplementedException();
		}
		
		public List<IClass> GetAccessibleTypes(IClass callingClass)
		{
			throw new NotImplementedException();
		}
		
		public IMember SearchMember(string memberName, LanguageProperties language)
		{
			throw new NotImplementedException();
		}
		
		public bool IsTypeInInheritanceTree(IClass possibleBaseClass)
		{
			throw new NotImplementedException();
		}
		
		public bool IsAccessible(IClass callingClass, bool isClassInInheritanceTree)
		{
			throw new NotImplementedException();
		}
		
		public bool MustBeShown(IClass callingClass, bool showStatic, bool isClassInInheritanceTree)
		{
			throw new NotImplementedException();
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		
	}
}
