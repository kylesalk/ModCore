using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Logic
{
    public class LogicNull : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            return null;
        }

		public override SyntaxNode Generate(Context context)
		{
			return ReturnStatement(
						LiteralExpression(
							SyntaxKind.NullLiteralExpression
						)
					);
		}
	}

}