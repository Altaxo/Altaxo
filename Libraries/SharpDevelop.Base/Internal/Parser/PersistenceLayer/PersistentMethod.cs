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
	public sealed class PersistentMethod : AbstractMethod
	{
		public PersistentMethod(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			FullyQualifiedName = reader.ReadString();
			Documentation      = reader.ReadString();
			
			modifiers          = (ModifierEnum)reader.ReadUInt32();
			returnType         = new PersistentReturnType(reader, classProxyCollection);
			if (returnType.Name == null) {
				returnType = null;
			}
			
			uint count = reader.ReadUInt32();
			
			Parameters.Clear();
			for (uint i = 0; i < count; ++i) {
				Parameters.Add(new PersistentParameter(reader, classProxyCollection));
			}
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(FullyQualifiedName);
			writer.Write(Documentation);
			writer.Write((uint)modifiers);
			((PersistentReturnType)returnType).WriteTo(writer);
			
			writer.Write((uint)Parameters.Count);
			foreach (PersistentParameter p in Parameters) {
				p.WriteTo(writer);
			}
		}
		
		public PersistentMethod(ClassProxyCollection classProxyCollection, IMethod method)
		{
			FullyQualifiedName = method.Name;
			if (method.Documentation != null) {
				Documentation = method.Documentation;
			} else {
				Documentation = String.Empty;
			}
			
			modifiers  = method.Modifiers;
			returnType = new PersistentReturnType(classProxyCollection, method.ReturnType);
			
			Parameters.Clear();
			foreach (IParameter param in method.Parameters) {
				Parameters.Add(new PersistentParameter(classProxyCollection, param));
			}
			
			region = null;
		}
		
	}
}
