// Main.cs
// Copyright (C) 2003 Mike Krueger
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are 
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list 
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials 
//   provided with the distribution.
//
// - Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to 
//   endorse or promote products derived from this software without specific prior written 
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;

namespace ICSharpCode.SharpAssembly {
	
	class MainClass
	{
		static AssemblyReader assembly = new AssemblyReader();
		
		public static void GetDataType(byte[] arr, ref int offset)
		{
			DataType dt = (DataType)arr[offset];
			++offset;
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
				case DataType.Single:
				case DataType.Double:
					Console.WriteLine(dt);
					break;
				
				case DataType.SZArray:
					GetDataType(arr, ref offset);
					Console.WriteLine("[]");
					break;
				case DataType.Array:
					GetDataType(arr, ref offset);
					int rank      = arr[offset++];
					int num_sizes = arr[offset++];
					int[] sizes   = new int[num_sizes];
					for (int i = 0; i < num_sizes; ++i) {
						sizes[i] = arr[offset++];
					}
					int num_lowerBounds = arr[offset++];
					int[] lowerBounds   = new int[num_lowerBounds];
					for (int i = 0; i < num_lowerBounds; ++i) {
						lowerBounds[i] = arr[offset++];
					}
					Console.Write("[");
					for (int i = 1; i < rank; ++i) {
						Console.Write(",");
					}
					Console.WriteLine("]");
					break;
				
				case DataType.ValueType:
				case DataType.Class:
					Console.WriteLine("Class lookup");
					int idx = assembly.BlobHeap[offset++];
					TypeDef[] typeDefTable = (TypeDef[])assembly.MetadataTable.Tables[TypeDef.TABLE_ID];
					TypeRef[] typeRefTable = (TypeRef[])assembly.MetadataTable.Tables[TypeRef.TABLE_ID];
					bool isTypeRef = (idx & 1) == 1;
					int  index     = (idx >> 2) - 1;
					Console.WriteLine(idx);
					Console.WriteLine(index);
					Console.WriteLine("def length: " + typeDefTable.Length);
					Console.WriteLine("ref length: " + typeRefTable.Length);
					
					if (isTypeRef) {
						Console.WriteLine(assembly.GetStringFromHeap(typeRefTable[index].Name));
					} else {
						Console.WriteLine(assembly.GetStringFromHeap(typeDefTable[index].Name));
					}
					break;
				
				case DataType.String:
					Console.WriteLine("string");
					break;
				case DataType.Object:
					Console.WriteLine("object");
					break;
				
				case DataType.Ptr:
					GetDataType(arr, ref offset);
					Console.WriteLine("*");
					break;
				case DataType.ByRef:
					GetDataType(arr, ref offset);
					Console.WriteLine("&");
					break;
				
				case DataType.TypeReference:
				case DataType.IntPtr:
				case DataType.UIntPtr:
					throw new System.NotImplementedException();
				default:
					throw new System.NotSupportedException();
			}
		}
		
		public static void Main(string[] args)
		{
			string fileName = @"C:\myForm.exe";
//			string fileName = @"C:\\testclass.exe";
//			string fileName = typeof(MainClass).Assembly.Location;
			DateTime old = DateTime.Now;
			
			assembly.Load(fileName);
			Field[] fieldTable = (Field[])assembly.MetadataTable.Tables[Field.TABLE_ID];
			for (int i = 0; i < fieldTable.Length; ++i) {
				Field f = fieldTable[i];
				Console.WriteLine();
				Console.WriteLine(assembly.GetStringFromHeap(f.Name) + " == " );
				CallingConvention cv = (CallingConvention)assembly.BlobHeap[f.Signature + 1];
				Console.WriteLine(cv);
				int offset = (int)f.Signature + 2;
				GetDataType(assembly.BlobHeap, ref offset);
//				for (int j = 2; j < 20; ++j) {
//					Console.WriteLine("{0:X}", assembly.BlobHeap[f.Signature + j]);
//				}
			}
			/*
			 Hashtable opcodeTable = new Hashtable();
			Type t  = typeof(OpCodes);
			foreach (FieldInfo f in t.GetFields()) {
				OpCode opCode = (OpCode)f.GetValue(null);
				ushort opCodeValue = (ushort)opCode.Value;
				opcodeTable[opCodeValue] = opCode;
			}

			MethodDef[] table = (MethodDef[])assembly.MetadataTable.Tables[MethodDef.TABLE_ID];
			for (int i = 0; i < table.Length; ++i) {
				Console.WriteLine("MethodName : " + assembly.GetStringFromHeap(table[i].Name));
				if (table[i].RVA != 0) {
					MethodBody methodBody = assembly.LoadMethodBody(table[i].RVA);
					BinaryReader binaryReader = new BinaryReader(new MemoryStream(methodBody.MethodData));
					Console.WriteLine("{");
					Console.WriteLine("// Code size\t{0} (0x{0:X})", methodBody.CodeSize);
					Console.WriteLine(".maxstack\t{0}", methodBody.MaxStack);
					try {
						do {
							Console.Write("L_{0}:  ", binaryReader.BaseStream.Position.ToString("X4"));
							
							ushort instruction;
							if (binaryReader.BaseStream.Position + 2 >= binaryReader.BaseStream.Length) {
								instruction = binaryReader.ReadByte();
							} else {
								instruction = binaryReader.ReadUInt16();
							}
							OpCode opCode = OpCodes.Nop;
							if (opcodeTable[instruction] != null) {
								opCode = (OpCode)opcodeTable[instruction];
							} else {
								instruction &= 0xFF;
								if (opcodeTable[instruction] != null) {
									opCode = (OpCode)opcodeTable[instruction];
								} else {
									Console.WriteLine("unknown opcode : " + (instruction));
								}
								binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);
							}
							Console.Write(opCode);
							
							switch (opCode.OperandType) {
								case OperandType.InlineBrTarget:
								case OperandType.InlineField:
								case OperandType.InlineI:
								case OperandType.InlineMethod:
								case OperandType.InlineSig:
								case OperandType.InlineString:
								case OperandType.InlineSwitch:
								case OperandType.InlineType:
								case OperandType.ShortInlineR:
									Console.Write(" " + binaryReader.ReadUInt32());
									break;
								case OperandType.InlineNone:
									break;
								case OperandType.InlineI8:
								case OperandType.InlineR:
									Console.Write(" " + binaryReader.ReadUInt64());
									break;
								case OperandType.InlinePhi:
									throw new System.NotImplementedException();
								case OperandType.InlineTok:
									// The operand is a FieldRef, MethodRef, or TypeRef token.
									Console.WriteLine("REF TOKEN " );
									Console.Write(" " + binaryReader.ReadUInt16());
									break;
								case OperandType.ShortInlineBrTarget:
									Console.Write(" " + binaryReader.ReadByte());
									break;
								case OperandType.InlineVar:
								case OperandType.ShortInlineI:
								case OperandType.ShortInlineVar:
									Console.Write(" " + binaryReader.ReadUInt16());
									break;
							}
							Console.WriteLine();
						} while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length);
					} catch (Exception e) {
						Console.WriteLine("unexpected exception:\n" + e);
					}
					Console.WriteLine("}");
					Console.WriteLine();
					binaryReader.Close();
				}
			}*/
		}
	}
}
