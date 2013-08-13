using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nustache.Compilation;
using System.Linq.Expressions;

namespace Nustache.Compilation.Tests
{
    [TestFixture]
    public class Describe_Compound_Expression_Enumerator
    {
        [Test]
        public void It_should_return_null_if_the_object_is_null()
        {
            Assert.IsNull(GetLambda<object>()(null));
        }

        [Test]
        public void It_should_concatenate_the_list_of_strings_returned_by_the_enumerator()
        {
            Assert.AreEqual("123j", 
                GetLambda<object>()(new List<object> { 1, "2", 3, "j" }));
        }

        [Test]
        public void It_should_map_properly_from_different_source_types()
        {
            Assert.AreEqual("1234", GetLambda<int>()(new List<int> { 1, 2, 3, 4 }));
            Assert.AreEqual("hello, world!", 
                GetLambda<string>()(new List<string> { "hello", ", ", "world", "!", "" }));
        }

        [Test]
        public void It_should_handle_null_callback_values()
        {
            Assert.Inconclusive("Not sure if this is imporant (the callback function does the null checks currently)");
        }

        private Func<List<T>, string> GetLambda<T>(params string[] strings)
        {
            var param = Expression.Parameter(typeof(List<T>), "p");
            var result = CompoundExpression.Enumerator(
                itemCallback: (e) => Expression.Call(e, typeof(T).GetMethod("ToString", new Type[0])),
                enumerable: param);

            Console.WriteLine(result.ToString());

            return Expression.Lambda<Func<List<T>, string>>(result, param).Compile();
        }
    }
}
