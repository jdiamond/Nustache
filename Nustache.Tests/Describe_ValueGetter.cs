using System.Collections;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Nustache.Core;

namespace Nustache.Tests
{
    [TestFixture]
    public class ValueGetterTests
    {
        [Test]
        public void Invalid()
        {
            Assert.That(ValueGetter.CanGetValue(this, "x"), Is.False);
        }

        [Test]
        public void IntField()
        {
            ReadWriteInts target = new ReadWriteInts();
            target.IntField = 123;
            Assert.That(ValueGetter.GetValue(target, "IntField"), Is.EqualTo(123));
        }

        [Test]
        public void IntProperty()
        {
            ReadWriteInts target = new ReadWriteInts();
            target.IntField = 123;
            Assert.That(ValueGetter.GetValue(target, "IntProperty"), Is.EqualTo(123));
        }

        [Test]
        public void IntMethod()
        {
            ReadWriteInts target = new ReadWriteInts();
            target.IntField = 123;
            Assert.That(ValueGetter.GetValue(target, "IntMethod"), Is.EqualTo(123));
        }

        [Test]
        public void WriteOnlyIntField()
        {
            WriteOnlyInts target = new WriteOnlyInts();
            Assert.That(ValueGetter.CanGetValue(target, "IntField"), Is.False);
        }

        [Test]
        public void WriteOnlyIntProperty()
        {
            WriteOnlyInts target = new WriteOnlyInts();
            Assert.That(ValueGetter.CanGetValue(target, "IntProperty"), Is.False);
        }

        [Test]
        public void WriteOnlyIntMethod()
        {
            WriteOnlyInts target = new WriteOnlyInts();
            Assert.That(ValueGetter.CanGetValue(target, "IntMethod"), Is.False);
        }
        [Test]
        public void Hashtable()
        {
            Hashtable target = new Hashtable();
            target["IntKey"] = 123;
            Assert.That(ValueGetter.GetValue(target, "IntKey"), Is.EqualTo(123));
        }

        [Test]
        public void Dictionary()
        {
            Dictionary<string, int> target = new Dictionary<string, int>();
            target["IntKey"] = 123;
            Assert.That(ValueGetter.GetValue(target, "IntKey"), Is.EqualTo(123));
        }

        [Test]
        public void DataRowView()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("IntColumn", typeof(int));
            dt.Rows.Add(new object[] { 123 });
            DataRowView target = dt.DefaultView[0];
            Assert.That(ValueGetter.GetValue(target, "IntColumn"), Is.EqualTo(123));
        }

        [Test]
        public void Enum()
        {
            ReadWriteEnums target = new ReadWriteEnums();
            target.EnumField = TestEnum.Bar;
            Assert.That(ValueGetter.GetValue(target, "EnumField"), Is.EqualTo(TestEnum.Bar));
        }

        [Test]
        public void NullableEnumWhenNull()
        {
            ReadWriteEnums target = new ReadWriteEnums();
            target.NullableEnumField = null;
            Assert.That(ValueGetter.GetValue(target, "NullableEnumField"), Is.EqualTo(null));
        }

        [Test]
        public void NullableEnumWhenNotNull()
        {
            ReadWriteEnums target = new ReadWriteEnums();
            target.NullableEnumField = TestEnum.Bar;
            Assert.That(ValueGetter.GetValue(target, "NullableEnumField"), Is.EqualTo(TestEnum.Bar));
        }

        public class ReadWriteInts
        {
            public int IntField = 0;
            public int IntProperty { get { return IntField; } set { IntField = value; } }
            public int IntMethod() { return IntField; }
            public void IntMethod(int value) { IntField = value; }
        }

        public class ReadOnlyInts
        {
            public readonly int IntField = 0;
            public int IntProperty { get { return IntField; } }
            public int IntMethod() { return IntField; }
        }

        public class WriteOnlyInts
        {
            private int IntField = 0;
            public int IntProperty { set { IntField = value; } }
            public void IntMethod(int value) { IntField = value; }
        }

        public enum TestEnum
        {
            Foo,
            Bar,
            BazQuux
        }

        public class ReadWriteEnums
        {
            public TestEnum EnumField;
            public TestEnum? NullableEnumField;
        }
    }
}