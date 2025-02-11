using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IronBlock.Blocks.Controls
{
	public class ControlsWhileUntil : IBlock
    {
        public override async Task<object> EvaluateAsync(Context context)
        {
            var mode = this.Fields.Get("MODE");
            var value = this.Values.FirstOrDefault(x => x.Name == "BOOL");
            
            if (!this.Statements.Any(x => x.Name == "DO") || null == value) return await base.EvaluateAsync(context);
            
            var statement = this.Statements.Get("DO");

            if (mode == "WHILE")
            {
                while((bool) await value.EvaluateAsync(context))
                {
                    await statement.EvaluateAsync(context);
                }
            }
            else
            {
                while(!(bool) await value.EvaluateAsync(context))
                {
                    await statement.EvaluateAsync(context);
                }
            }

            return await base.EvaluateAsync(context);
        }

		public override SyntaxNode Generate(Context context)
		{
			var mode = this.Fields.Get("MODE");
			var value = this.Values.FirstOrDefault(x => x.Name == "BOOL");

			if (!this.Statements.Any(x => x.Name == "DO") || null == value) return base.Generate(context);

			var statement = this.Statements.Get("DO");

			var conditionExpression = value.Generate(context) as ExpressionSyntax;
			if (conditionExpression == null) throw new ApplicationException($"Unknown expression for condition.");

			var whileContext = new Context(context.Dependency) { Parent = context };
			if (statement?.Block != null)
			{
				var statementSyntax = statement.Block.GenerateStatement(whileContext);
				if (statementSyntax != null)
				{
					whileContext.Statements.Add(statementSyntax);
				}
			}

			if (mode != "WHILE")
			{
				conditionExpression = PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, conditionExpression);
			}

			var whileStatement = 
					WhileStatement(
						conditionExpression,
						Block(whileContext.Statements)
					);			

			return Statement(whileStatement, base.Generate(context), context);
		}
	}

}