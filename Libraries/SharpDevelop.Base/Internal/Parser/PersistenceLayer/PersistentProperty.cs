// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpDevelop.Services;

namespace SharpDevelop.Internal.Parser
{
	public sealed class PersistentProperty : AbstractProperty
	{
		const uint canGetFlag = (uint)(1 << 29);
		const uint canSetFlag = (uint)(1 << 30);
		
		bool canGet = false;
		bool canSet = false;
		
		public override bool CanGet {
			get {
				return canGet;
			}
		}
		
		public override bool CanSet {
			get {
				return canSet;
			}
		}
		
		public PersistentProperty(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			FullyQualifiedName = reader.ReadString();
			documentation      = reader.ReadString();
			uint m = reader.ReadUInt32();
			modifiers          = (ModifierEnum)(m & (canGetFlag - 1));
			canGet             = (m & canGetFlag) == canGetFlag;
			canSet             = (m & canSetFlag) == canSetFlag;
			
			returnType         = new PersistentReturnType(reader, classProxyCollection);
			if (returnType.Name == null) {
				returnType = null;
			}
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(FullyQualifiedName);
			writer.Write(documentation);
			writer.Write((uint)modifiers + (CanGet ? canGetFlag : 0) + (CanSet ? canSetFlag : 0));
			((PersistentReturnType)returnType).WriteTo(writer);
		}
		
		public PersistentProperty(ClassProxyCollection classProxyCollection, IProperty property)
		{
			FullyQualifiedName = property.Name;
			modifiers          = property.Modifiers;
			documentation      = property.Documentation;
			if (documentation == null) {
				documentation = String.Empty;
			}
			
			if (property.ReturnType != null) {
				returnType     = new PersistentReturnType(classProxyCollection, property.ReturnType);
			}
			region = getterRegion = setterRegion = null;
			canGet = property.CanGet;
			canSet = property.CanSet;
		}
	}
}
