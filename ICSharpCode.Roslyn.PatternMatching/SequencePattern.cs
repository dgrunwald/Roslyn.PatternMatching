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
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace ICSharpCode.Roslyn.PatternMatching
{
	/// <summary>
	/// A sequence of patterns.
	/// </summary>
	sealed class SequencePattern<T> : IListPattern<T> where T : SyntaxNode
	{
		ImmutableArray<IPatternElement<T>> patterns;
		
		public SequencePattern(ImmutableArray<IPatternElement<T>> patterns)
		{
			Debug.Assert(!patterns.IsDefault);
			this.patterns = patterns;
		}
		
		public bool PerformMatch(ref ListMatch listMatch, ref Match match)
		{
			// The basePattern may create savepoints, so we need to save the 'i' variable
			// as part of those checkpoints.
			for (int i = listMatch.PopFromSavePoint() ?? 0; i < patterns.Length; i++) {
				int startMarker = listMatch.GetSavePointStartMarker();
				bool success = patterns[i].PerformMatch(ref listMatch, ref match);
				listMatch.PushToSavePoints(startMarker, i);
				if (!success)
					return false;
			}
			return true;
		}
	}
}
