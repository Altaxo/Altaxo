// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Windows.Forms;
using SharpDevelop.Internal.Parser;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	public class ClassBrowserIconsService : AbstractService
	{
		const int namespaceIndex = 3;
		const int combineIndex   = 46;
		const int literalIndex   = 47;
		
		const int classIndex     = 14;
		const int structIndex    = classIndex + 1 * 4;
		const int interfaceIndex = classIndex + 2 * 4;
		const int enumIndex      = classIndex + 3 * 4;
		const int methodIndex    = classIndex + 4 * 4;
		const int propertyIndex  = classIndex + 5 * 4;
		const int fieldIndex     = classIndex + 6 * 4;
		const int delegateIndex  = classIndex + 7 * 4;
		const int eventIndex     = classIndex + 8 * 4 + 2;
		
		const int internalModifierOffset  = 1;
		const int protectedModifierOffset = 2;
		const int privateModifierOffset   = 3;
		
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		
		ImageList imglist = null;
		
		public ImageList ImageList {
			get {
				if (imglist == null) {
					LoadImageList();
				}
				return imglist;
			}
		}
		
		public int CombineIndex {
			get {
				return combineIndex;
			}
		}
		
		public int NamespaceIndex {
			get {
				return namespaceIndex;
			}
		}
		
		public int LiteralIndex {
			get {
				return literalIndex;
			}
		}
		
		public int ClassIndex {
			get {
				return classIndex;
			}
		}

		public int StructIndex {
			get {
				return structIndex;
			}
		}

		public int InterfaceIndex {
			get {
				return interfaceIndex;
			}
		}

		public int EnumIndex {
			get {
				return enumIndex;
			}
		}

		public int MethodIndex {
			get {
				return methodIndex;
			}
		}
		
		public int PropertyIndex {
			get {
				return propertyIndex;
			}
		}

		public int FieldIndex {
			get {
				return fieldIndex;
			}
		}

		public int DelegateIndex {
			get {
				return delegateIndex;
			}
		}

		public int EventIndex {
			get {
				return eventIndex;
			}
		}

		public int InternalModifierOffset {
			get {
				return internalModifierOffset;
			}
		}

		public int ProtectedModifierOffset {
			get {
				return protectedModifierOffset;
			}
		}

		public int PrivateModifierOffset {
			get {
				return privateModifierOffset;
			}
		}
		
		int GetModifierOffset(ModifierEnum modifier)
		{
			if ((modifier & ModifierEnum.Public) == ModifierEnum.Public) {
				return 0;
			}
			if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected) {
				return protectedModifierOffset;
			}
			if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal) {
				return internalModifierOffset;
			}
			return privateModifierOffset;
		}
		
		public int GetIcon(IMethod method)
		{
			return methodIndex + GetModifierOffset(method.Modifiers);
		}
		
		public int GetIcon(IProperty method)
		{
			return propertyIndex + GetModifierOffset(method.Modifiers);
		}
		
		public int GetIcon(IField field)
		{
			if (field.IsLiteral) {
				return literalIndex;
			}
			return fieldIndex + GetModifierOffset(field.Modifiers);
		}
		
		public int GetIcon(IEvent evt)
		{
			return eventIndex + GetModifierOffset(evt.Modifiers);
		}
		
		public int GetIcon(IClass c)
		{
			int imageIndex = classIndex;
			switch (c.ClassType) {
				case ClassType.Delegate:
					imageIndex = delegateIndex;
					break;
				case ClassType.Enum:
					imageIndex = enumIndex;
					break;
				case ClassType.Struct:
					imageIndex = structIndex;
					break;
				case ClassType.Interface:
					imageIndex = interfaceIndex;
					break;
			}
			return imageIndex + GetModifierOffset(c.Modifiers);
		}
		
		
		public int GetIcon(MethodBase methodinfo)
		{
			if (methodinfo.IsAssembly) {
				return methodIndex + internalModifierOffset;
			}
			if (methodinfo.IsPrivate) {
				return methodIndex + privateModifierOffset; 
			}
			if (!(methodinfo.IsPrivate || methodinfo.IsPublic)) { 
				return methodIndex + protectedModifierOffset;
			}
			
			return methodIndex;
		}
		
		public int GetIcon(PropertyInfo propertyinfo)
		{
			if (propertyinfo.CanRead && propertyinfo.GetGetMethod(true) != null) {
				return propertyIndex + GetIcon(propertyinfo.GetGetMethod(true)) - methodIndex;
			}
			if (propertyinfo.CanWrite && propertyinfo.GetSetMethod(true) != null) {
				return propertyIndex + GetIcon(propertyinfo.GetSetMethod(true)) - methodIndex;
			}
			return propertyIndex;
		}
		
		public int GetIcon(FieldInfo fieldinfo)
		{
			if (fieldinfo.IsLiteral) {
				return 13;
			}
			
			if (fieldinfo.IsAssembly) {
				return fieldIndex + internalModifierOffset;
			}
			
			if (fieldinfo.IsPrivate) {
				return fieldIndex + privateModifierOffset;
			}
			
			if (!(fieldinfo.IsPrivate || fieldinfo.IsPublic)) {
				return fieldIndex + protectedModifierOffset;
			}
			
			return fieldIndex;
		}
				
		public int GetIcon(EventInfo eventinfo)
		{
			if (eventinfo.GetAddMethod(true) != null) {
				return eventIndex + GetIcon(eventinfo.GetAddMethod(true)) - methodIndex;
			}
			return eventIndex;
		}
		
		public int GetIcon(System.Type type)
		{
			int BASE = classIndex;
			
			if (type.IsValueType) {
				BASE = structIndex;
			}
			if (type.IsEnum) {
				BASE = enumIndex;
			}
			if (type.IsInterface) {
				BASE = interfaceIndex;
			}
			if (type.IsSubclassOf(typeof(System.Delegate))) {
				BASE = delegateIndex;
			}
			
			if (type.IsNestedPrivate) {
				return BASE + 3;
			} 
			
			if (type.IsNotPublic || type.IsNestedAssembly) {
				return BASE + 1;
			} 
			
			if (type.IsNestedFamily) {
				return BASE + 2;
			}
			return BASE;
		}
		
		void LoadImageList()
		{
			imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Assembly"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenAssembly"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Library"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.NameSpace"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.SubTypes"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.SuperTypes"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Reference"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ClosedReferenceFolder"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.OpenReferenceFolder"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ResourceFileIcon"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Event"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Literal"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Class")); //14
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalClass"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedClass"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateClass"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Struct")); 
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalStruct"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedStruct"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateStruct"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Interface")); 
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalInterface"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedInterface"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateInterface"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Enum"));   
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalEnum"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedEnum"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateEnum"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Method"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalMethod"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedMethod"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateMethod"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Property"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalProperty"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedProperty"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateProperty"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Field"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalField"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedField"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateField"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Delegate"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalDelegate"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedDelegate"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateDelegate"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.CombineIcon")); // 46
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Literal")); // 47
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.Event"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.InternalEvent"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.ProtectedEvent"));
			imglist.Images.Add(resourceService.GetBitmap("Icons.16x16.PrivateEvent"));
		}
	}
}
