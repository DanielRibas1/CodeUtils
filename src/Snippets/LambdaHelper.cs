using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Snippets
{
    public static class LambdaHelper
    {
        #region Concatenate && and || Func Lambdas

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression, Expression<Func<T, bool>> newExpression)
        {
            if (expression == null)
                return newExpression;
            var visitor = new ParameterUpdateVisitor(newExpression.Parameters.First(), expression.Parameters.First());
            newExpression = visitor.Visit(newExpression) as Expression<Func<T, bool>>;
            var binaryExppression = Expression.And(expression.Body, newExpression.Body);
            return Expression.Lambda<Func<T, bool>>(binaryExppression, newExpression.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression, Expression<Func<T, bool>> newExpression)
        {
            if (expression == null)
                return newExpression;
            var visitor = new ParameterUpdateVisitor(newExpression.Parameters.First(), expression.Parameters.First());
            newExpression = visitor.Visit(newExpression) as Expression<Func<T, bool>>;
            var binaryExppression = Expression.Or(expression.Body, newExpression.Body);
            return Expression.Lambda<Func<T, bool>>(binaryExppression, newExpression.Parameters);
        }

        private class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (object.ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }

        #endregion
    }
}
