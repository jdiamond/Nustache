using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Moq;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_ValueGetter
    {
        [Test]
        public void It_returns_null_when_it_cant_get_a_value()
        {
            Assert.IsNull(ValueGetter.GetValue(this, "x"));
        }

        [Test]
        public void It_gets_field_values()
        {
            ReadWriteInts target = new ReadWriteInts();
            target.IntField = 123;
            Assert.AreEqual(123, ValueGetter.GetValue(target, "IntField"));
        }

        [Test]
        public void It_gets_property_values()
        {
            ReadWriteInts target = new ReadWriteInts();
            target.IntField = 123;
            Assert.AreEqual(123, ValueGetter.GetValue(target, "IntProperty"));
        }

        [Test]
        public void It_gets_method_values()
        {
            ReadWriteInts target = new ReadWriteInts();
            target.IntField = 123;
            Assert.AreEqual(123, ValueGetter.GetValue(target, "IntMethod"));
        }

        [Test]
        public void It_cant_get_values_from_write_only_properties()
        {
            WriteOnlyInts target = new WriteOnlyInts();
            Assert.IsNull(ValueGetter.GetValue(target, "IntProperty"));
        }

        [Test]
        public void It_cant_get_values_from_write_only_methods()
        {
            WriteOnlyInts target = new WriteOnlyInts();
            Assert.IsNull(ValueGetter.GetValue(target, "IntMethod"));
        }

        [Test]
        public void It_gets_Hashtable_values()
        {
            Hashtable target = new Hashtable();
            target["IntKey"] = 123;
            Assert.AreEqual(123, ValueGetter.GetValue(target, "IntKey"));
        }

        [Test]
        public void It_gets_Dictionary_values()
        {
            Dictionary<string, int> target = new Dictionary<string, int>();
            target["IntKey"] = 123;
            Assert.AreEqual(123, ValueGetter.GetValue(target, "IntKey"));
        }

        [Test]
        public void It_gets_GenericDictionary_values()
        {
            var mock = new Mock<IDictionary<string, int>>();
            mock.Setup(x => x.ContainsKey("Key")).Returns(true);
            mock.Setup(x => x["Key"]).Returns(123);
            IDictionary<string, int> target = mock.Object;
            Assert.AreEqual(123, ValueGetter.GetValue(target, "Key"));
        }

        [Test]
        public void It_gets_DataRowView_values()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("IntColumn", typeof(int));
            dt.Rows.Add(new object[] { 123 });
            DataRowView target = dt.DefaultView[0];
            Assert.AreEqual(123, ValueGetter.GetValue(target, "IntColumn"));
        }

        [Test]
        public void It_gets_XmlNode_attribute_values()
        {
            XmlDocument target = new XmlDocument();
            target.LoadXml("<doc attr='val'></doc>");
            Assert.AreEqual("val", ValueGetter.GetValue(target.DocumentElement, "attr"));
        }

        [Test]
        public void It_gets_XmlNode_single_child_element_values()
        {
            XmlDocument target = new XmlDocument();
            target.LoadXml("<doc attr='val'><child>text</child></doc>");
            Assert.AreEqual("text", ValueGetter.GetValue(target.DocumentElement, "child"));
        }

        [Test]
        public void It_gets_XmlNode_multiple_child_element_values()
        {
            XmlDocument target = new XmlDocument();
            target.LoadXml("<doc attr='val'><child>text1</child><child>text2</child></doc>");
            XmlNodeList elements = (XmlNodeList)ValueGetter.GetValue(target.DocumentElement, "child");
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("text1", elements[0].InnerText);
            Assert.AreEqual("text2", elements[1].InnerText);
        }

        public class ReadWriteInts
        {
            public int IntField = -1;
            public int IntProperty { get { return IntField; } set { IntField = value; } }
            public int IntMethod() { return IntField; }
            public void IntMethod(int value) { IntField = value; }
        }

        public class ReadOnlyInts
        {
            public readonly int IntField = -1;
            public int IntProperty { get { return IntField; } }
            public int IntMethod() { return IntField; }
        }

        public class WriteOnlyInts
        {
            public int IntField; // Write only?
            public int IntProperty { set { IntField = value; } }
            public void IntMethod(int value) { IntField = value; }
        }
    }
}