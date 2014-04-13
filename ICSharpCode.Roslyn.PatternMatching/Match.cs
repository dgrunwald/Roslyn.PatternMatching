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
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ICSharpCode.Roslyn.PatternMatching
{
	/// <summary>
	/// Data holder for the overall pattern matching operation.
	/// </summary>
	/// <remarks>
	/// This type is a struct in order to prevent unnecessary memory allocations during pattern matching.
	/// The default value <c>default(Match)</c> represents an unsuccessful match.
	/// </remarks>
	public struct Match
	{
		static readonly List<KeyValuePair<CaptureGroup, SyntaxNode>> emptyResults = new List<KeyValuePair<CaptureGroup, SyntaxNode>>();
		
		readonly SemanticModel semanticModel;
		List<KeyValuePair<CaptureGroup, SyntaxNode>> results;
		
		/// <summary>
		/// Creates a new Match instance.
		/// </summary>
		public Match(SemanticModel semanticModel = null)
		{
			this.semanticModel = semanticModel;
			this.results = null;
		}
		
		/// <summary>
		/// Gets whether the match was successful.
		/// </summary>
		public bool Success {
			get {
				return results != null;
			}
			internal set {
				if (value) {
					if (results == null)
						results = emptyResults;
				} else {
					results = null;
				}
			}
		}
		
		/// <summary>
		/// Gets whether the match was successful.
		/// </summary>
		public static bool operator true(Match m)
		{
			return m.Success;
		}
		
		/// <summary>
		/// Gets whether the match failed.
		/// </summary>
		public static bool operator false(Match m)
		{
			return !m.Success;
		}
		
		/// <summary>
		/// Retrieves the semantic model used for pattern matching.
		/// </summary>
		public SemanticModel SemanticModel {
			get {
				return semanticModel;
			}
		}
		
		internal void Add(CaptureGroup g, SyntaxNode n)
		{
			if (results == null)
				results = new List<KeyValuePair<CaptureGroup, SyntaxNode>>();
			results.Add(new KeyValuePair<CaptureGroup, SyntaxNode>(g, n));
		}
		
		internal int CheckPoint()
		{
			return results != null ? results.Count : 0;
		}
		
		internal void RestoreCheckPoint(int checkPoint)
		{
			if (results != null)
				results.RemoveRange(checkPoint, results.Count - checkPoint);
		}
		
		public IEnumerable<SyntaxNode> Get(CaptureGroup captureGroup)
		{
			if (results != null) {
				foreach (var pair in results) {
					if (pair.Key == captureGroup)
						yield return pair.Value;
				}
			}
		}
		
		public IEnumerable<T> Get<T>(CaptureGroup<T> captureGroup) where T : SyntaxNode
		{
			if (results != null) {
				foreach (var pair in results) {
					if (pair.Key == captureGroup)
						yield return (T)pair.Value;
				}
			}
		}
	}
}
