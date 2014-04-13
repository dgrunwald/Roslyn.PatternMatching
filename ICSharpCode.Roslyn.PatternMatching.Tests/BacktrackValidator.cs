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

namespace ICSharpCode.Roslyn.PatternMatching.Tests
{
	/// <summary>
	/// May be wrapped around any pattern node without changing the semantics of the whole pattern.
	/// Used to enforce additional backtracking steps in nested patterns.
	/// </summary>
	class BacktrackValidator<T> : IPattern<T>, IListPattern<T> where T : SyntaxNode
	{
		readonly IPatternElement<T> baseElement;
		
		public BacktrackValidator(IPatternElement<T> baseElement)
		{
			if (baseElement == null)
				throw new ArgumentNullException("baseElement");
			this.baseElement = baseElement;
		}
		
		public bool PerformMatch(ref ListMatch listMatch, ref Match match)
		{
			int? data = listMatch.PopFromSavePoint();
			if (data == null) {
				// First call
				listMatch.AddSavePoint(ref match, 42);
				return false; 
			} else if (data == 42 || data == 4242) {
				// Do the actual work
				if (data == 42 && listMatch.PopFromSavePoint() != null)
					throw new InvalidOperationException("Extra elements on save point stack");
				int startMarker = listMatch.GetSavePointStartMarker();
				bool success = baseElement.PerformMatch(ref listMatch, ref match);
				listMatch.PushToSavePoints(startMarker, 4242);
				return success;
			} else {
				throw new InvalidOperationException();
			}
		}
	}
}
