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
	public sealed class PersistentIndexer : AbstractIndexer
	{
		public PersistentIndexer(BinaryReader reader, ClassProxyCollection classProxyCollection)
		{
			FullyQualifiedName = reader.ReadString();
			documentation      = reader.ReadString();
			modifiers          = (ModifierEnum)reader.ReadUInt32();
			
			returnType         = new PersistentReturnType(reader, classProxyCollection);
			if (returnType.Name == null) {
				returnType = null;
			}
			
			uint count = reader.ReadUInt32();
			for (uint i = 0; i < count; ++i) {
				parameters.Add(new PersistentParameter(reader, classProxyCollection));
			}
		}
		
		public void WriteTo(BinaryWriter writer)
		{
			writer.Write(FullyQualifiedName);
			writer.Write(documentation);
			
			writer.Write((uint)modifiers);
			((PersistentReturnType)returnType).WriteTo(writer);
			
			writer.Write((uint)parameters.Count);
			foreach (PersistentParameter p in parameters) {
				p.WriteTo(writer);
			}
		}
		
		public PersistentIndexer(ClassProxyCollection classProxyCollection, IIndexer indexer)
		{
			FullyQualifiedName = indexer.Name;
			documentation = indexer.Documentation;
			if (documentation == null) {
				documentation = String.Empty;
			}
			modifiers = indexer.Modifiers;
			returnType         = new PersistentReturnType(classProxyCollection, indexer.ReturnType);
			
			foreach (IParameter param in indexer.Parameters) {
				parameters.Add(new PersistentParameter(classProxyCollection, param));
			}
			
			region = getterRegion = setterRegion = null;
		}
		
	}
}
