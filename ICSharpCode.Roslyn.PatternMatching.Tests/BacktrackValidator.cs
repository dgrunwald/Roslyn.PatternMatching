/*
* Created by SharpDevelop.
* User: Daniel
* Date: 4/13/2014
* Time: 20:10
* 
* To change this template use Tools | Options | Coding | Edit Standard Headers.
*/
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
			} else if (data == 42) {
				// Do the actual work
				int startMarker = listMatch.GetSavePointStartMarker();
				bool success = baseElement.PerformMatch(ref listMatch, ref match);
				listMatch.PushToSavePoints(startMarker, 42);
				return success;
			} else {
				throw new InvalidOperationException();
			}
		}
	}
}
