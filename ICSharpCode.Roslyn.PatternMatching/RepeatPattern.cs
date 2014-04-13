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
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace ICSharpCode.Roslyn.PatternMatching
{
	class RepeatPattern<T> : IListPattern<T> where T : SyntaxNode
	{
		readonly IPatternElement<T> basePattern;
		readonly int minCount;
		readonly int maxCount;
		
		public RepeatPattern(IPatternElement<T> basePattern, int minCount, int maxCount)
		{
			Debug.Assert(basePattern != null);
			this.basePattern = basePattern;
			this.minCount = minCount;
			this.maxCount = maxCount;
		}
		
		public bool PerformMatch(ref ListMatch listMatch, ref Match match)
		{
			const int ownSavePoint = -2;
			int matchCount = listMatch.PopFromSavePoint() ?? -1;
			if (matchCount == ownSavePoint) {
				// backtracked to our own savepoint
				Debug.Assert(listMatch.PopFromSavePoint() == null);
				return true;
			} else if (matchCount == -1) {
				// first time entering this method
				if (this.minCount <= 0) {
					// If we are allowed to match zero times, create the save point for zero matches.
					listMatch.AddSavePoint(ref match, ownSavePoint);
				}
				matchCount = 0;
			}
			Debug.Assert(matchCount >= 0);
			while (matchCount < maxCount) {
				int syntaxStartIndex = listMatch.PopFromSavePoint() ?? listMatch.SyntaxIndex;
				int startMarker = listMatch.GetSavePointStartMarker();
				bool success = basePattern.PerformMatch(ref listMatch, ref match);
				listMatch.PushToSavePoints(startMarker, syntaxStartIndex);
				listMatch.PushToSavePoints(startMarker, matchCount);
				if (!success)
					break;
				matchCount++;
				if (matchCount >= minCount) {
					if (syntaxStartIndex == listMatch.SyntaxIndex) {
						// The last basePattern.PerformMatch() successfully matched zero elements.
						// To avoid looping until maxCount, we return an early success here:
						return true;
					}
					// We are allowed to return true at the current point, so create a save point here
					listMatch.AddSavePoint(ref match, ownSavePoint);
				}
			}
			return false;
		}
	}
}
