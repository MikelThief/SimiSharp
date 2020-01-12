// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolReferenceResolver.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SymbolReferenceResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimiSharp.CodeAnalysis.Common;

namespace SimiSharp.CodeAnalysis.ReferenceResolvers
{
	internal static class SymbolReferenceResolver
	{
		public static IEnumerable<IGrouping<ISymbol, ReferenceLocation>> Resolve(this Compilation compilation, SyntaxNode root)
		{
			var model = compilation.GetSemanticModel(syntaxTree: root.SyntaxTree);

			var fields = root.DescendantNodes()
				.Select(
					selector: descendant =>
					{
						var symbol = model.GetSymbolInfo(node: descendant);
						return new KeyValuePair<ISymbol, SyntaxNode>(key: symbol.Symbol, value: descendant);
					})
				.Where(predicate: x => x.Key != null)
				.SelectMany(selector: x => GetSymbolDetails(x: x, model: model))
				.GroupBy(keySelector: x => x.Symbol, elementSelector: x => x.Location);

			var array = fields.AsArray();

			return array;
		}

		private static IEnumerable<SymbolDetails> GetSymbolDetails(KeyValuePair<ISymbol, SyntaxNode> x, SemanticModel model)
		{
			var containingType = ResolveContainingType(node: x.Value, model: model);
			yield return new SymbolDetails(symbol: x.Key, location: new ReferenceLocation(location: x.Value.GetLocation(), referencingType: containingType, model: model));

			var namedType = x.Key as INamedTypeSymbol;
			if (namedType != null && namedType.ConstructedFrom != null && namedType.ConstructedFrom != x.Key)
			{
				yield return new SymbolDetails(symbol: namedType.ConstructedFrom, location: new ReferenceLocation(location: x.Value.GetLocation(), referencingType: containingType, model: model));
			}

			var namedMethod = x.Key as IMethodSymbol;
			if (namedMethod != null && namedMethod.ConstructedFrom != null && namedMethod.ConstructedFrom != x.Key)
			{
				yield return new SymbolDetails(symbol: namedMethod, location: new ReferenceLocation(location: x.Value.GetLocation(), referencingType: ResolveContainingType(node: x.Value, model: model), model: model));
			}
		}

		private static ITypeSymbol ResolveContainingType(SyntaxNode node, SemanticModel model)
		{
			if (node == null)
			{
				return null;
			}

			var parent = node.Parent;
			if (parent is BaseTypeDeclarationSyntax)
			{
				var symbolInfo = model.GetDeclaredSymbol(declaration: parent);
				return symbolInfo as ITypeSymbol;
			}

			return ResolveContainingType(node: parent, model: model);
		}

		private class SymbolDetails
		{
			public SymbolDetails(ISymbol symbol, ReferenceLocation location)
			{
				Symbol = symbol;
				Location = location;
			}

			public ISymbol Symbol { get; }

			public ReferenceLocation Location { get; }
		}
	}
}