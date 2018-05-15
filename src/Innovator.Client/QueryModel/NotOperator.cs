using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.QueryModel
{
  public class NotOperator : ILogical
  {
    public IExpression Arg { get; set; }

    public virtual int Precedence => (int)PrecedenceLevel.Not;

    public void Visit(IExpressionVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}