// Copyright (c) 2014 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.CodeAnalysis;

namespace ICSharpCode.Roslyn.PatternMatching
{
	/// <summary>
	/// A capture group is used to store syntax nodes that are encountered during pattern matching for later retrieval.
	/// The <c>CaptureGroup</c> instances serve as identifiers for the actual capture groups stored within the <see cref="Match"/> object.
	/// </summary>
	public class CaptureGroup {}
	
	/// <summary>
	/// A capture group is used to store syntax nodes that are encountered during pattern matching for later retrieval.
	/// The <c>CaptureGroup</c> instances serve as identifiers for the actual capture groups stored within the <see cref="Match"/> object.
	/// </summary>
	public class CaptureGroup<T> : CaptureGroup where T : SyntaxNode
	{
		public IPattern<T> Capture(IPattern<T> pattern)
		{
			return new CaptureGroupPattern<T>(pattern, this);
		}
		
		public IListPattern<T> Capture(IListPattern<T> pattern)
		{
			return new CaptureGroupPattern<T>(pattern, this);
		}
		
		public IPatternElement<T> Capture(IPatternElement<T> pattern)
		{
			return new CaptureGroupPattern<T>(pattern, this);
		}
	}
	
	sealed class CaptureGroupPattern<T> : IPatternElement<T>, IListPattern<T>, IPattern<T> where T : SyntaxNode
	{
		readonly CaptureGroup<T> captureGroup;
		readonly IPatternElement<T> basePattern;
		
		public CaptureGroupPattern(IPatternElement<T> elementPattern, CaptureGroup<T> captureGroup)
		{
			this.basePattern = elementPattern;
			this.captureGroup = captureGroup;
		}
		
		public bool PerformMatch(ref ListMatch listMatch, ref Match match)
		{
			// The basePattern may create savepoints, so we need to save our 'startIndex' variable
			// as part of those checkpoints.
			int startIndex = listMatch.PopFromSavePoint() ?? listMatch.SyntaxIndex;
			int startMarker = listMatch.GetSavePointStartMarker();
			var success = basePattern.PerformMatch(ref listMatch, ref match);
			listMatch.PushToSavePoints(startMarker, startIndex);
			if (success) {
				for (int i = startIndex; i < listMatch.SyntaxIndex; i++) {
					match.Add(captureGroup, listMatch.SyntaxList[i]);
				}
			}
			return success;
		}
	}
}
