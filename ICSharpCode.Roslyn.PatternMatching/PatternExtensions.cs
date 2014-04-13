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
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
namespace ICSharpCode.Roslyn.PatternMatching
{
	/// <summary>
	/// Extension methods for <see cref="IPattern{T}"/>.
	/// </summary>
	public static class PatternExtensions
	{
		// We have to use extension methods here; the method can't be part of a covariant interface.
		/// <summary>
		/// Attempts to match the specified node against the pattern.
		/// </summary>
		/// <param name="pattern">The syntactic pattern.</param>
		/// <param name="node">The syntax node to test against the pattern.</param>
		/// <param name="semanticModel">Semantic model to use in semantic patterns. May be <c>null</c> if the pattern does not involve semantic tests.</param>
		/// <returns>
		/// Returns a match object describing the result of the matching operation.
		/// Check the <see cref="Match.Success"/> property to see whether the match was successful.
		/// For successful matches, the match object allows retrieving the nodes that were matched with the captured groups.
		/// </returns>
		public static Match Match<T>(this IPattern<T> pattern, T node, SemanticModel semanticModel = null) where T : SyntaxNode
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			Match match = new Match(semanticModel);
			match.Success = ListMatch.DoMatch(pattern, SyntaxFactory.SingletonList(node), ref match);
			return match;
		}
		
		/// <summary>
		/// Attempts to match the specified node list against the pattern.
		/// </summary>
		/// <param name="pattern">The syntactic pattern.</param>
		/// <param name="syntaxList">The syntax nodes to test against the pattern.</param>
		/// <param name="semanticModel">Semantic model to use in semantic patterns. May be <c>null</c> if the pattern does not involve semantic tests.</param>
		/// <returns>
		/// Returns a match object describing the result of the matching operation.
		/// Check the <see cref="Match.Success"/> property to see whether the match was successful.
		/// For successful matches, the match object allows retrieving the nodes that were matched with the captured groups.
		/// </returns>
		public static Match Match<T>(this IListPattern<T> pattern, SyntaxList<T> syntaxList, SemanticModel semanticModel = null) where T : SyntaxNode
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			Match match = new Match(semanticModel);
			match.Success = ListMatch.DoMatch(pattern, syntaxList, ref match);
			return match;
		}
	}
}

