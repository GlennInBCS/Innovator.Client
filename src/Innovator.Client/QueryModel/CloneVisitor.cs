using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client.QueryModel
{
  public class CloneVisitor : IQueryVisitor
  {
    private IExpression _clone;
    private PropertyReference _prop;
    private QueryItem _query;
    private readonly Dictionary<QueryItem, QueryItem> _clones = new Dictionary<QueryItem, QueryItem>();
    private Func<PropertyReference, PropertyReference> _propMapper;
    private Func<PropertyReference, ILiteral, ILiteral> _valueMapper;

    public CloneVisitor()
    {
      _propMapper = op => new PropertyReference(op.Name, GetTable(op.Table));
      _valueMapper = (prop, literal) => CloneAndReturn(literal);
    }

    public void Visit(QueryItem query)
    {
      var newQuery = new QueryItem(query.Context)
      {
        Alias = query.Alias,
        Fetch = query.Fetch,
        Offset = query.Offset,
        Type = query.Type,
        TypeProvider = query.TypeProvider
      };
      _clones[query] = newQuery;

      foreach (var join in query.Joins)
      {
        newQuery.Joins.Add(new Join()
        {
          Type = join.Type,
          Left = Clone(join.Left),
          Right = Clone(join.Right),
          Condition = CloneAndReturn(join.Condition),
        });
      }

      foreach (var orderBy in query.OrderBy)
      {
        newQuery.OrderBy.Add(new OrderByExpression()
        {
          Ascending = orderBy.Ascending,
          Expression = CloneAndReturn(orderBy.Expression)
        });
      }

      foreach (var select in query.Select)
      {
        newQuery.Select.Add(new SelectExpression()
        {
          Alias = select.Alias,
          Expression = CloneAndReturn(select.Expression),
          OnlyReturnNonNull = select.OnlyReturnNonNull
        });
      }

      newQuery.Where = CloneAndReturn(query.Where);
      _query = newQuery;
    }

    void IExpressionVisitor.Visit(AndOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(AndOperator op)
    {
      return new AndOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneAndReturn(op.Right)
      };
    }

    void IExpressionVisitor.Visit(BetweenOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(BetweenOperator op)
    {
      return new BetweenOperator()
      {
        Left = CloneAndReturn(op.Left),
        Min = CloneValue(op.Left, op.Min),
        Max = CloneValue(op.Left, op.Max)
      };
    }

    void IExpressionVisitor.Visit(BooleanLiteral op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(BooleanLiteral op)
    {
      return new BooleanLiteral(op.Value);
    }

    void IExpressionVisitor.Visit(CountAggregate op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(CountAggregate op)
    {
      var result = new CountAggregate();
      foreach (var table in op.TablePath)
        result.TablePath.Add(GetTable(table));
      return result;
    }

    void IExpressionVisitor.Visit(DateTimeLiteral op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(DateTimeLiteral op)
    {
      return new DateTimeLiteral(op.Value);
    }

    void IExpressionVisitor.Visit(EqualsOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(EqualsOperator op)
    {
      return new EqualsOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(FloatLiteral op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(FloatLiteral op)
    {
      return new FloatLiteral(op.Value);
    }

    void IExpressionVisitor.Visit(FunctionExpression op)
    {
      _clone = op.Clone(CloneAndReturn);
    }

    void IExpressionVisitor.Visit(GreaterThanOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(GreaterThanOperator op)
    {
      return new GreaterThanOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(GreaterThanOrEqualsOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(GreaterThanOrEqualsOperator op)
    {
      return new GreaterThanOrEqualsOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(InOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(InOperator op)
    {
      _prop = op.Left as PropertyReference;
      return new InOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneAndReturn(op.Right)
      };
    }

    void IExpressionVisitor.Visit(IntegerLiteral op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(IntegerLiteral op)
    {
      return new IntegerLiteral(op.Value);
    }

    void IExpressionVisitor.Visit(IsOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(IsOperator op)
    {
      return new IsOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = op.Right
      };
    }

    void IExpressionVisitor.Visit(LessThanOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(LessThanOperator op)
    {
      return new LessThanOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(LessThanOrEqualsOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(LessThanOrEqualsOperator op)
    {
      return new LessThanOrEqualsOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(LikeOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(LikeOperator op)
    {
      return new LikeOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(ListExpression op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(ListExpression op)
    {
      var clone = new ListExpression();
      foreach (var value in op.Values)
        clone.Values.Add((IOperand)CloneValue(_prop, value));
      return clone;
    }

    void IExpressionVisitor.Visit(NotEqualsOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(NotEqualsOperator op)
    {
      return new NotEqualsOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(NotOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(NotOperator op)
    {
      return new NotOperator()
      {
        Arg = CloneAndReturn(op.Arg)
      };
    }

    void IExpressionVisitor.Visit(ObjectLiteral op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(ObjectLiteral op)
    {
      return new ObjectLiteral(op.Value, op.TypeProvider, op.Context);
    }

    void IExpressionVisitor.Visit(OrOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(OrOperator op)
    {
      return new OrOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneAndReturn(op.Right)
      };
    }

    void IExpressionVisitor.Visit(PropertyReference op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(PropertyReference op)
    {
      return _propMapper(op);
    }

    void IExpressionVisitor.Visit(StringLiteral op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(StringLiteral op)
    {
      return new StringLiteral(op.Value);
    }

    void IExpressionVisitor.Visit(MultiplicationOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(MultiplicationOperator op)
    {
      return new MultiplicationOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(DivisionOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(DivisionOperator op)
    {
      return new DivisionOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(ModulusOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(ModulusOperator op)
    {
      return new ModulusOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(AdditionOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(AdditionOperator op)
    {
      return new AdditionOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(SubtractionOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(SubtractionOperator op)
    {
      return new SubtractionOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(NegationOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(NegationOperator op)
    {
      return new NegationOperator()
      {
        Arg = CloneAndReturn(op.Arg)
      };
    }

    void IExpressionVisitor.Visit(ConcatenationOperator op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(ConcatenationOperator op)
    {
      return new ConcatenationOperator()
      {
        Left = CloneAndReturn(op.Left),
        Right = CloneValue(op.Left, op.Right)
      };
    }

    void IExpressionVisitor.Visit(ParameterReference op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(ParameterReference op)
    {
      return new ParameterReference(op.Name, op.IsRaw);
    }

    void IExpressionVisitor.Visit(AllProperties op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(AllProperties op)
    {
      return new AllProperties(op.Table) { XProperties = op.XProperties };
    }

    void IExpressionVisitor.Visit(PatternList op)
    {
      _clone = Clone(op);
    }

    public virtual IExpression Clone(PatternList op)
    {
      return op.Clone();
    }

    private QueryItem GetTable(QueryItem orig)
    {
      if (_clones.TryGetValue(orig, out var result))
        return result;
      return orig;
    }

    private IExpression CloneValue(IExpression left, IExpression right)
    {
      if (left is PropertyReference prop && right is ILiteral literal)
        return ClonePropertyValue(prop, literal);
      else
        return CloneAndReturn(right);
    }

    protected virtual IExpression ClonePropertyValue(PropertyReference prop, ILiteral literal)
    {
      return _valueMapper(prop, literal);
    }

    private T CloneAndReturn<T>(T expr) where T : IExpression
    {
      expr.Visit(this);
      return (T)_clone;
    }

    private QueryItem Clone(QueryItem query)
    {
      if (_clones.TryGetValue(query, out var result))
        return result;
      Visit(query);
      return _query;
    }

  }
}
