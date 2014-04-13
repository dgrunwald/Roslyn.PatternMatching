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
}