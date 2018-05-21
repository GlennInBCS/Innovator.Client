using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.QueryModel
{
  public class InOperator : IOperator, IBooleanOperator
  {
    public IExpression Left { get; set; }
    public ListExpression Right { get; set; }

    public int Precedence => (int)PrecedenceLevel.Comparison;

    public IExpression ToConditional()
    {
      var list = Right;
      if (list.Values.Count < 1)
        throw new NotSupportedException();
      var result = (IExpression)new EqualsOperator()
      {
        Left = Left,
        Right = list.Values[0]
      };
      foreach (var value in list.Values.Skip(1))
      {
        result = new OrOperator()
        {
          Left = result,
          Right = new EqualsOperator()
          {
            Left = Left,
            Right = value
          }
        };
      }
      return result;
    }

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
