﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Text;

namespace ICSharpCode.TextEditor.Document
{
	public class GapTextBufferStrategy : ITextBufferStrategy
	{
		#if DEBUG
		int creatorThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
		
		void CheckThread()
		{
			if (System.Threading.Thread.CurrentThread.ManagedThreadId != creatorThread)
				throw new InvalidOperationException("GapTextBufferStategy is not thread-safe!");
		}
		#endif
		
		char[] buffer = new char[0];
		
		int gapBeginOffset = 0;
		int gapEndOffset   = 0;
		
		int minGapLength = 32;
		int maxGapLength = 256;
		
		public int Length {
			get {
				return buffer.Length - GapLength;
			}
		}
		
		int GapLength {
			get {
				return gapEndOffset - gapBeginOffset;
			}
		}
		
		public void SetContent(string text) 
		{
			if (text == null) {
				text = String.Empty;
			}
			buffer = text.ToCharArray();
			gapBeginOffset = gapEndOffset = 0;
		}
		
		public char GetCharAt(int offset) 
		{
			return offset < gapBeginOffset ? buffer[offset] : buffer[offset + GapLength];
		}
		
		public string GetText(int offset, int length) 
		{
			#if DEBUG
			CheckThread();
			#endif
			int end = offset + length;
			
			if (end < gapBeginOffset) {
				return new string(buffer, offset, length);
			}
			
			if (offset > gapBeginOffset) {
				return new string(buffer, offset + GapLength, length);
			}
			
			int block1Size = gapBeginOffset - offset;
			int block2Size = end - gapBeginOffset;
			
			StringBuilder buf = new StringBuilder(block1Size + block2Size);
			buf.Append(buffer, offset,       block1Size);
			buf.Append(buffer, gapEndOffset, block2Size);
			return buf.ToString();
		}
		
		public void Insert(int offset, string text)
		{
			Replace(offset, 0, text);
		}
		
		public void Remove(int offset, int length)
		{
			Replace(offset, length, String.Empty);
		}
		
		public void Replace(int offset, int length, string text) 
		{
			if (text == null) {
				text = String.Empty;
			}
			
			// Math.Max is used so that if we need to resize the array
			// the new array has enough space for all old chars
			PlaceGap(offset + length, Math.Max(text.Length - length, 0));
			text.CopyTo(0, buffer, offset, text.Length);
			gapBeginOffset += text.Length - length;
		}
		
		void PlaceGap(int offset, int length) 
		{
			int deltaLength = GapLength - length;
			// if the gap has the right length, move the chars between offset and gap
			if (minGapLength <= deltaLength && deltaLength <= maxGapLength) {
				int delta = gapBeginOffset - offset;
				// check if the gap is already in place
				if (offset == gapBeginOffset) {
					return;
				} else if (offset < gapBeginOffset) {
					int gapLength = gapEndOffset - gapBeginOffset;
					Array.Copy(buffer, offset, buffer, offset + gapLength, delta);
				} else { //offset > gapBeginOffset
					Array.Copy(buffer, gapEndOffset, buffer, gapBeginOffset, -delta);
				}
				gapBeginOffset -= delta;
				gapEndOffset   -= delta;
				return;
			}
			
			// the gap has not the right length so
			// create new Buffer with new size and copy
			int oldLength       = GapLength;
			int newLength       = maxGapLength + length;
			int newGapEndOffset = offset + newLength;
			char[] newBuffer    = new char[buffer.Length + newLength - oldLength];
			
			if (oldLength == 0) {
				Array.Copy(buffer, 0, newBuffer, 0, offset);
				Array.Copy(buffer, offset, newBuffer, newGapEndOffset, newBuffer.Length - newGapEndOffset);
			} else if (offset < gapBeginOffset) {
				int delta = gapBeginOffset - offset;
				Array.Copy(buffer, 0, newBuffer, 0, offset);
				Array.Copy(buffer, offset, newBuffer, newGapEndOffset, delta);
				Array.Copy(buffer, gapEndOffset, newBuffer, newGapEndOffset + delta, buffer.Length - gapEndOffset);
			} else {
				int delta = offset - gapBeginOffset;
				Array.Copy(buffer, 0, newBuffer, 0, gapBeginOffset);
				Array.Copy(buffer, gapEndOffset, newBuffer, gapBeginOffset, delta);
				Array.Copy(buffer, gapEndOffset + delta, newBuffer, newGapEndOffset, newBuffer.Length - newGapEndOffset);
			}
			
			buffer         = newBuffer;
			gapBeginOffset = offset;
			gapEndOffset   = newGapEndOffset;
		}
	}
}
