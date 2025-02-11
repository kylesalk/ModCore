using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Math
{
    public class MathNumber : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            return double.Parse(this.Fields.Get("NUM"), CultureInfo.InvariantCulture);
        }

		public override SyntaxNode Generate(Context context)
		{
			var value = double.Parse(this.Fields.Get("NUM"), CultureInfo.InvariantCulture);
			return LiteralExpression(
				SyntaxKind.NumericLiteralExpression,
				Literal(value)
			);
		}
	}
}