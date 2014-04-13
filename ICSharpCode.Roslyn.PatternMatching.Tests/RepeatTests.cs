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
using ICSharpCode.Roslyn.PatternMatching;
using ICSharpCode.Roslyn.PatternMatching.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace ICSharpCode.Roslyn.PatternMatching.Tests
{
	public class RepeatTests
	{
		[Test]
		public void RepeatEmpty()
		{
			var pattern = PatternFactory.Repeat(PatternFactory.List<IdentifierNameSyntax>());
			Assert.IsTrue(pattern.Match(SyntaxFactory.List<IdentifierNameSyntax>()).Success);
			Assert.IsFalse(pattern.Match(SyntaxFactory.SingletonList(SyntaxFactory.IdentifierName("a"))).Success);
		}
		
		/// <summary>
		/// (AB)*C
		/// </summary>
		readonly IListPattern<IdentifierNameSyntax> abcPattern =
			PatternFactory.List(
				PatternFactory.Repeat(PatternFactory.List(
					PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "A"),
					PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "B")
				)),
				PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "C")
			);
		
		[Test]
		public void ABC_Matches_C()
		{
			var syntax = SyntaxFactory.List(new[] { 
				SyntaxFactory.IdentifierName("C") 
			});
			Assert.IsTrue(abcPattern.Match(syntax).Success);
		}
		
		[Test]
		public void ABC_Does_Not_Match_D()
		{
			var syntax = SyntaxFactory.List(new[] { 
				SyntaxFactory.IdentifierName("D") 
			});
			Assert.IsFalse(abcPattern.Match(syntax).Success);
		}
		
		[Test]
		public void ABC_Matches_ABC()
		{
			var syntax = SyntaxFactory.List(new[] { 
				SyntaxFactory.IdentifierName("A"), 
				SyntaxFactory.IdentifierName("B"), 
				SyntaxFactory.IdentifierName("C") 
			});
			Assert.IsTrue(abcPattern.Match(syntax).Success);
		}
		
		[Test]
		public void ABC_Matches_ABABC()
		{
			var syntax = SyntaxFactory.List(new[] { 
				SyntaxFactory.IdentifierName("A"), 
				SyntaxFactory.IdentifierName("B"), 
				SyntaxFactory.IdentifierName("A"), 
				SyntaxFactory.IdentifierName("B"), 
				SyntaxFactory.IdentifierName("C") 
			});
			Assert.IsTrue(abcPattern.Match(syntax).Success);
		}
		
		/// <summary>
		/// (AB)*ABA
		/// </summary>
		readonly IListPattern<IdentifierNameSyntax> ababaPattern =
			PatternFactory.List(
				PatternFactory.Repeat(PatternFactory.List(
					PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "A"),
					new BacktrackValidator<IdentifierNameSyntax>(PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "B"))
				)),
				PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "A"),
				PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "B"),
				PatternFactory.Predicate((IdentifierNameSyntax name) => name.Identifier.Text == "A")
			);
		
		[Test]
		public void ABABA_Matches_ABABA()
		{
			var syntax = SyntaxFactory.List(new[] { 
				SyntaxFactory.IdentifierName("A"), 
				SyntaxFactory.IdentifierName("B"), 
				SyntaxFactory.IdentifierName("A"), 
				SyntaxFactory.IdentifierName("B"), 
				SyntaxFactory.IdentifierName("A") 
			});
			Assert.IsTrue(ababaPattern.Match(syntax).Success);
		}
		
	}
}
