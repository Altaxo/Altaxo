// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Xml;

using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;
using ICSharpCode.SharpAssembly.Assembly;

namespace SharpDevelop.Internal.Parser {
	
	[Serializable]
	public class SharpAssemblyReturnType : AbstractReturnType
	{
		ArrayList      arrayRanks = null;
		
		SharpAssemblyClass underlyingClass;
		
		public SharpAssemblyClass UnderlyingClass {
			get {
				return underlyingClass;
			}
		}
		
		public void GetDataType(SharpAssembly asm, ref uint offset)
		{
			AssemblyReader assembly = asm.Reader;
			
			DataType dt = (DataType)assembly.LoadBlob(ref offset);
			switch (dt) {
				case DataType.Void:
				case DataType.Boolean:
				case DataType.Char:
				case DataType.SByte:
				case DataType.Byte:
				case DataType.Int16:
				case DataType.UInt16:
				case DataType.Int32:
				case DataType.UInt32:
				case DataType.Int64:
				case DataType.UInt64:
				case DataType.Single:
				case DataType.Double:
				case DataType.String:
				case DataType.Object:
				case DataType.IntPtr:
				case DataType.UIntPtr:
					fullyQualifiedName = "System." + dt.ToString();
					
					// TODO : underlyingClass
					declaredin = asm.GetReference("mscorlib");
					break;
				
				case DataType.SZArray:
					GetDataType(asm, ref offset);
					arrayRanks.Add(0);
					break;
				
				case DataType.Array:
					GetDataType(asm, ref offset);
					int rank      = assembly.LoadBlob(ref offset);
					int num_sizes = assembly.LoadBlob(ref offset);
					int[] sizes   = new int[num_sizes];
					for (int i = 0; i < num_sizes; ++i) {
						sizes[i] = assembly.LoadBlob(ref offset);
					}
					int num_lowerBounds = assembly.LoadBlob(ref offset);
					int[] lowerBounds   = new int[num_lowerBounds];
					for (int i = 0; i < num_lowerBounds; ++i) {
						lowerBounds[i] = assembly.LoadBlob(ref offset);
					}
					arrayRanks.Add(rank - 1);
					break;
				
				case DataType.ValueType:
				case DataType.Class:
					uint idx = (uint)assembly.LoadBlob(ref offset);
					bool isTypeRef = (idx & 1) == 1;
					uint  index    = (idx >> 2);
					
					TypeDef[] typeDefTable = asm.Tables.TypeDef;
					TypeRef[] typeRefTable = asm.Tables.TypeRef;
					
					if (isTypeRef) {
						underlyingClass = SharpAssemblyClass.FromTypeRef(asm, index);
						if (underlyingClass != null) {
							fullyQualifiedName = underlyingClass.FullyQualifiedName;
						} else {
							fullyQualifiedName = assembly.GetStringFromHeap(typeRefTable[index].Nspace) + "." + 
						                                                assembly.GetStringFromHeap(typeRefTable[index].Name);
						    Console.WriteLine("GetDataType: TypeRef not resolved!");
						}
						declaredin = asm.GetRefAssemblyFor(index);
					} else {
						underlyingClass = SharpAssemblyClass.FromTypeDef(asm, index);
						if (underlyingClass != null) {
							fullyQualifiedName = underlyingClass.FullyQualifiedName;
						} else {
							fullyQualifiedName = assembly.GetStringFromHeap(typeDefTable[index].NSpace) + "." + 
																	assembly.GetStringFromHeap(typeDefTable[index].Name);
						}
						declaredin = asm;
					}

					break;
				
				case DataType.Ptr:
					GetDataType(asm, ref offset);
					++pointerNestingLevel;
					break;
				case DataType.ByRef:
					GetDataType(asm, ref offset);
					fullyQualifiedName += "&";
					break;
				
				case DataType.TypeReference:
					fullyQualifiedName = "typedref";
					Console.WriteLine("got typedref");
					break;
				
				case DataType.Pinned:
					GetDataType(asm, ref offset);
					//fullyQualifiedName += " pinned";
					break;
				
				case DataType.CModOpt:
				case DataType.CModReq:
					GetDataType(asm, ref offset);
					break;
				
				default:
					Console.WriteLine("NOT supported: " + dt.ToString());
					fullyQualifiedName += " NOT_SUPPORTED [" + dt.ToString() + "]";
					break;
			}
		}
		
		/// <remarks>
		/// For error purposes
		/// </remarks>
		public SharpAssemblyReturnType(string name)
		{
			fullyQualifiedName = name;
			declaredin = null;
		}
		
		public SharpAssemblyReturnType(SharpAssembly assembly, TypeDef[] typeDefTable, uint index)
		{
			underlyingClass = SharpAssemblyClass.FromTypeDef(assembly, index);
			if (underlyingClass != null) {
				fullyQualifiedName = underlyingClass.FullyQualifiedName;
			} else {
				fullyQualifiedName = assembly.Reader.GetStringFromHeap(typeDefTable[index].NSpace) + "." + 
														assembly.Reader.GetStringFromHeap(typeDefTable[index].Name);
			}
			declaredin = assembly;
		}
		
		public SharpAssemblyReturnType(SharpAssembly assembly, TypeRef[] typeRefTable, uint index)
		{
			underlyingClass = SharpAssemblyClass.FromTypeRef(assembly, index);
			if (underlyingClass != null) {
				fullyQualifiedName = underlyingClass.FullyQualifiedName;
			} else {
				fullyQualifiedName = assembly.Reader.GetStringFromHeap(typeRefTable[index].Nspace) + "." + 
			                                                assembly.Reader.GetStringFromHeap(typeRefTable[index].Name);
			    Console.WriteLine("SharpAssemblyReturnType from TypeRef: TypeRef not resolved!");
			}
			declaredin = assembly.GetRefAssemblyFor(index);
		}
		
		public SharpAssemblyReturnType(SharpAssembly assembly, ref uint blobSignatureIndex)
		{
			arrayRanks = new ArrayList();
			try {
				GetDataType(assembly, ref blobSignatureIndex);
			} catch (Exception e) {
				Console.WriteLine("Got exception in ReturnType creation: " + e.ToString());
				fullyQualifiedName = "GOT_EXCEPTION";
			}
			
			if (arrayRanks.Count > 0) {
				arrayDimensions = new int[arrayRanks.Count];
				arrayRanks.CopyTo(arrayDimensions, 0);
			} else {
				arrayRanks = null;
			}
		}
		
		public override string ToString()
		{
			return FullyQualifiedName;
		}
	}
}
