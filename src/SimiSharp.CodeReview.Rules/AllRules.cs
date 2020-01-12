// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllRules.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AllRules type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using SimiSharp.CodeAnalysis.Common;
using SimiSharp.CodeAnalysis.Common.CodeReview;

namespace SimiSharp.CodeReview.Rules
{
	public static class AllRules
	{
		public static IEnumerable<ISyntaxEvaluation> GetSyntaxRules(ISpellChecker spellChecker)
		{
			var types = (from type in typeof(AllRules).Assembly.GetTypes()
						 where typeof(ISyntaxEvaluation).IsAssignableFrom(c: type)
						 where !type.IsInterface && !type.IsAbstract
						 select type).AsArray();
			var simple =
				types.Where(predicate: x => x.GetConstructors().Any(predicate: c => c.GetParameters().Length == 0))
					.Select(selector: Activator.CreateInstance)
					.Cast<ISyntaxEvaluation>();
			var spelling =
				types.Where(
					predicate: x =>
					x.GetConstructors()
						.Any(
							predicate: c => c.GetParameters().Length == 1 && typeof(ISpellChecker).IsAssignableFrom(c: c.GetParameters()[index0: 0].ParameterType)))
					.Select(selector: x => Activator.CreateInstance(type: x, spellChecker))
					.Cast<ISyntaxEvaluation>();

			return simple.Concat(second: spelling).OrderBy(keySelector: x => x.ID).AsArray();
		}

		public static IEnumerable<ISymbolEvaluation> GetSymbolRules()
		{
			var types = from type in typeof(AllRules).Assembly.GetTypes()
				   where typeof(ISymbolEvaluation).IsAssignableFrom(c: type)
				   where !type.IsInterface && !type.IsAbstract
				   select type;

			var simple =
				types.Where(predicate: x => x.GetConstructors().Any(predicate: c => c.GetParameters().Length == 0))
					.Select(selector: Activator.CreateInstance)
					.Cast<ISymbolEvaluation>();

			return simple.AsArray();
		}
	}
}
