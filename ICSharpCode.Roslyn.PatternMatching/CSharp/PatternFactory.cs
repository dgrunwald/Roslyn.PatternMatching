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
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ICSharpCode.Roslyn.PatternMatching.CSharp
{
	/// <summary>
	/// Static factory methods for creating C# pattern trees.
	/// </summary>
	public static partial class PatternFactory
	{
		/// <summary>
		/// Creates a pattern that captures the nodes it matches against into this capture group.
		/// </summary>
		public static IPattern<T> Capture<T>(this IPattern<T> pattern, CaptureGroup<T> captureGroup) where T : SyntaxNode
		{
			return new CaptureGroupPattern<T>(pattern, captureGroup);
		}
		
		/// <summary>
		/// Creates a pattern that captures the nodes it matches against into this capture group.
		/// </summary>
		public static IListPattern<T> Capture<T>(this IListPattern<T> pattern, CaptureGroup<T> captureGroup) where T : SyntaxNode
		{
			return new CaptureGroupPattern<T>(pattern, captureGroup);
		}
		
		public static IListPattern<T> Repeat<T>(this IPatternElement<T> pattern, int minCount = 0, int maxCount = int.MaxValue) where T : SyntaxNode
		{
			return new RepeatPattern<T>(pattern, minCount, maxCount);
		}
		
		public static IListPattern<T> List<T>() where T : SyntaxNode
		{
			return new SequencePattern<T>(ImmutableArray<IPatternElement<T>>.Empty);
		}
		
		public static IListPattern<T> SingletonList<T>(IPatternElement<T> node) where T : SyntaxNode
		{
			if (node == null)
				return List<T>();
			return new SingletonListPattern<T>(node);
		}
		
		/// <summary>
		/// Creates a list of syntax nodes.
		/// </summary>
		/// <typeparam name="TNode">The specific type of the element nodes.</typeparam>
		/// <param name="nodes">A sequence of element nodes.</param>
		public static IListPattern<TNode> List<TNode>(IEnumerable<IPatternElement<TNode>> nodes) where TNode : SyntaxNode
		{
			return new SequencePattern<TNode>(nodes.AsImmutableOrEmpty());
		}
	}
}
