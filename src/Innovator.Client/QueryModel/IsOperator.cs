using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.QueryModel
{
  public class IsOperator : IOperator
  {
    public IExpression Left { get; set; }
    public IsOperand Right { get; set; }

    public int Precedence => (int)PrecedenceLevel.Comparison;

    public void Visit(IExpressionVisitor visitor)
    {
      visitor.Visit(this);
    }

    public override string ToString()
    {
      return this.ToSqlString();
    }
  }
}