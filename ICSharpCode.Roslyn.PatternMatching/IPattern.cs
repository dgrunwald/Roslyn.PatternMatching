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
	/// Represents a syntactic pattern that matches a list or single node. 
	/// </summary>
	public interface IPatternElement<out T> where T : SyntaxNode
	{
		/// <summary>
		/// Infrastructure method. Use the IsMatch()/Match() extension methods instead.
		/// </summary>
		/// <param name="listMatch">Stores state about the current list match.</param>
		/// <param name="match">The match object, used to store global state during the match (such as the results of capture groups).</param>
		/// <returns>Returns whether the (partial) match was successful.
		/// If the method returns true, it updates listMatch.SyntaxIndex to point to the next node that was not part of the match,
		/// and adds the capture groups (if any) to the match.
		/// If the method returns false, the listMatch and match objects remain in a partially-updated state and need to be restored
		/// before they can be reused.</returns>
		bool PerformMatch(ref ListMatch listMatch, ref Match match);
	}
	
	/// <summary>
	/// Represents a syntactic pattern that matches a single syntax node.
	/// </summary>
	/// <remarks>
	/// This should be a class with internal abstract members, but we need an interface for covariance.
	/// The interface is covariant so that only valid pattern trees can be built.
	/// </remarks>
	public interface IPattern<out T> : IPatternElement<T> where T : SyntaxNode
	{
		// The Match() method of this interface is defined as an extension method.
	}
	
	/// <summary>
	/// Represents a syntactic pattern that matches a list of syntax nodes.
	/// </summary>
	public interface IListPattern<out T> : IPatternElement<T> where T : SyntaxNode
	{
		// The Match() method of this interface is defined as an extension method.
	}
	
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