using System.Linq;
using Microsoft.CodeAnalysis;

namespace Halite
{
    internal static class TypeSymbolExtensions
    {
        private const string HalNamespace = "Halite";
        private const string HalLinksTypeName = "HalLinks";
        private const string HalLinkObjectTypeName = "HalLinkObject";
        private const string HalLinkTypeName = "HalLink";
        private const string HalTemplatedLinkTypeName = "HalTemplatedLink";

        public static bool IsHalLinks(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.IsA(HalNamespace, HalLinksTypeName);
        }

        public static bool InheritsFromHalLinkObject(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.InheritsFrom(HalNamespace, HalLinkObjectTypeName);
        }

        public static bool InheritsFromHalLink(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.InheritsFrom(HalNamespace, HalLinkTypeName);
        }

        public static bool InheritsFromHalTemplatedLink(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.InheritsFrom(HalNamespace, HalTemplatedLinkTypeName);
        }

        public static bool ImplementsIEnumerableOfHalLinkObject(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.ImplementsIEnumerableOf(HalNamespace, HalLinkObjectTypeName);
        }

        public static bool IsA(this ITypeSymbol typeSymbol, string typeNamespace, string typeName)
        {
            return typeSymbol != null && (typeSymbol.IsExactly(typeNamespace, typeName) || typeSymbol.BaseType.IsA(typeNamespace, typeName));
        }

        public static bool InheritsFrom(this ITypeSymbol typeSymbol, string typeNamespace, string typeName)
        {
            return typeSymbol != null && typeSymbol.BaseType.IsA(typeNamespace, typeName);
        }

        public static bool IsExactly(this ITypeSymbol typeSymbol, string typeNamespace, string typeName)
        {
            var ns = typeSymbol.ContainingNamespace.ToString();
            var result = string.Equals(typeName, typeSymbol.Name) &&
                         string.Equals(typeNamespace, ns);
            return result;
        }

        public static bool ImplementsIEnumerableOf(this ITypeSymbol typeSymbol, string typeNamespace, string typeName)
        {
            return typeSymbol.IsIEnumerableOf(typeNamespace, typeName) ||
                   typeSymbol.AllInterfaces.Any(ts => ts.IsIEnumerableOf(typeNamespace, typeName));
        }

        public static bool IsIEnumerableOf(this ITypeSymbol typeSymbol, string typeNamespace, string typeName)
        {
            var namedTypeSymbol = typeSymbol as INamedTypeSymbol;

            if (namedTypeSymbol != null && namedTypeSymbol.IsGenericType && string.Equals("IEnumerable", namedTypeSymbol.Name))
            {
                var typeArgument = namedTypeSymbol.TypeArguments.Single();
                if (typeArgument.IsA(typeNamespace, typeName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
