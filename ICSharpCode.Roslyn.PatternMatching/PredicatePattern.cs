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
	class PredicatePattern<T> : IPattern<T> where T : SyntaxNode
	{
		readonly Func<T, bool> predicate;
		
		public PredicatePattern(Func<T, bool> predicate)
		{
			Debug.Assert(predicate != null);
			this.predicate = predicate;
		}
		
		public bool PerformMatch(ref ListMatch listMatch, ref Match match)
		{
			if (listMatch.SyntaxIndex < listMatch.SyntaxList.Count) {
				var t = listMatch.SyntaxList[listMatch.SyntaxIndex] as T;
				if (t != null && predicate(t)) {
					listMatch.SyntaxIndex++;
					return true;
				}
			}
			return false;
		}
	}
	
	class SemanticPredicatePattern<T> : IPattern<T> where T : SyntaxNode
	{
		readonly Func<T, SemanticModel, bool> predicate;
		
		public SemanticPredicatePattern(Func<T, SemanticModel, bool> predicate)
		{
			Debug.Assert(predicate != null);
			this.predicate = predicate;
		}
		
		public bool PerformMatch(ref ListMatch listMatch, ref Match match)
		{
			if (match.SemanticModel == null)
				throw new InvalidOperationException("This pattern involves semantic predicates. You need to pass a SemanticModel instance to the Match() method.");
			if (listMatch.SyntaxIndex < listMatch.SyntaxList.Count) {
				var t = listMatch.SyntaxList[listMatch.SyntaxIndex] as T;
				if (t != null && predicate(t, match.SemanticModel)) {
					listMatch.SyntaxIndex++;
					return true;
				}
			}
			return false;
		}
	}
}
