using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.QueryModel
{
  public class DateTimeLiteral : ILiteral
  {
    public DateTime Value { get; set; }

    public DateTimeLiteral(DateTime value)
    {
      Value = value;
    }

    public void Visit(IExpressionVisitor visitor)
    {
      visitor.Visit(this);
    }

    public override string ToString()
    {
      return Value.ToString();
    }
  }
}