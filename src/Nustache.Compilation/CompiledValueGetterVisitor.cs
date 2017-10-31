using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nustache.Core;

namespace Nustache.Compilation
{
    internal class CompiledValueGetterVisitor : ValueGetterVisitor
    {
        private Type _targetType;
        private Expression _dataParameter;

        public CompiledValueGetterVisitor(Type targetType, Expression dataParameter)
        {
            this._targetType = targetType;
            this._dataParameter = dataParameter;
        }

        public Expression CompiledGetter { get; private set; }

        public void Visit(MethodInfoValueGetter getter)
        {
            CompiledGetter = Expression.Call(_dataParameter, getter.MethodInfo);
        }

        public void Visit(PropertyInfoValueGetter getter)
        {
            CompiledGetter = Expression.Property(_dataParameter, getter.PropertyInfo);
        }

        public void Visit(FieldInfoValueGetter getter)
        {
            CompiledGetter = Expression.Field(_dataParameter, getter.FieldInfo);
        }

        public void Visit(GenericDictionaryValueGetter getter)
        {
            var containsKeyMethod = getter.DictionaryType.GetMethod("ContainsKey");

            CompiledGetter = 
                Expression.Condition(
                    Expression.Call(_dataParameter, containsKeyMethod, Expression.Constant(getter.Key)),
                    Expression.Call(_dataParameter, getter.GetMethod, Expression.Constant(getter.Key)),
                    Expression.Default(getter.GetMethod.ReturnType));

        }

        public void NoMatch(ValueGetter getter)
        {
            throw new NotSupportedException("Compiling types is not supported for " + getter.GetType().Name);;
        }
    }
}
