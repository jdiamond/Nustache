using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Describe_Template_Render
    {
        [Test]
        public void It_renders_empty_strings_as_empty_strings()
        {
            var result = Render.StringToString("", null);
            Assert.AreEqual("", result);
        }

        [Test]
        public void It_replaces_undefined_variables_with_empty_strings_when_there_is_no_data()
        {
            var result = Render.StringToString("before{{foo}}after", null);
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_replaces_undefined_variables_with_empty_strings_when_there_is_data()
        {
            var result = Render.StringToString("before{{foo}}after", new { bar = "baz" });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_replaces_defined_variables_with_values()
        {
            var result = Render.StringToString("before{{foo}}after", new { foo = "FOO" });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_handles_two_variables_correctly()
        {
            var result = Render.StringToString("before{{foo}}inside{{bar}}after", new { foo = "FOO", bar = "BAR" });
            Assert.AreEqual("beforeFOOinsideBARafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_false()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = false });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_true()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = true });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_null()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = (string)null });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_null()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = "bar" });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_does_not_render_sections_mapped_to_empty_collections()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new int[] { } });
            Assert.AreEqual("beforeafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_empty_collections()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new [] { 1 } });
            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_renders_sections_mapped_to_non_empty_collections_for_each_item_in_the_collection()
        {
            var result = Render.StringToString("before{{#foo}}FOO{{/foo}}after", new { foo = new [] { 1, 2, 3 } });
            Assert.AreEqual("beforeFOOFOOFOOafter", result);
        }

        [Test]
        public void It_changes_the_context_for_each_item_in_the_collection()
        {
            var result = Render.StringToString("before{{#foo}}{{.}}{{/foo}}after", new { foo = new[] { 1, 2, 3 } });
            Assert.AreEqual("before123after", result);
        }

        [Test]
        public void It_treats_generic_dictionaries_that_do_not_implement_IDictionary_as_dictionaries()
        {
            var result = Render.StringToString("before{{#Data}}{{#alpha}}{{one}}{{two}}{{/alpha}}{{#bravo}}{{three}}{{four}}{{/bravo}}{{#charlie}}{{five}}{{six}}{{/charlie}}{{/Data}}after", new
            {
                Data = new SneakyDictionary<String, object>
                    {
                        { "alpha", new SneakyDictionary<String, int>
                            {
                                { "one", 1 },
                                { "two", 2 }
                            }
                        },
                        { "bravo", new SneakyDictionary<String, int>
                            {
                                { "three", 3 },
                                { "four", 4 }
                            }
                        },
                        { "charlie", new SneakyDictionary<String, int>
                            {
                                { "five", 5 },
                                { "six", 6 }
                            }
                        },
                    }
            });
            Assert.AreEqual("before123456after", result);
        }

        [Test]
        public void It_can_render_arrays()
        {
            var result = Render.StringToString("before{{#.}}{{.}}{{/.}}after", new[] { 1, 2, 3 });
            Assert.AreEqual("before123after", result);
        }

        [Test]
        public void It_lets_you_reference_properties_of_items_in_the_collection()
        {
            var result = Render.StringToString(
                "before{{#foo}}{{bar}}{{/foo}}after",
                new { foo = new [] { new { bar = 1 }, new { bar = 2 }, new { bar = 3 } } });
            Assert.AreEqual("before123after", result);
        }

        [Test]
        public void It_looks_up_the_stack_for_properties()
        {
            var result = Render.StringToString(
                "{{#foo}}{{bar}}{{/foo}}",
                new { foo = new { /* no bar here */ }, bar = "baz" });
            Assert.AreEqual("baz", result);
        }

        [Test]
        public void It_doesnt_look_up_the_stack_for_properties_that_return_null()
        {
            var result = Render.StringToString(
                "{{#foo}}{{bar}}{{/foo}}",
                new { foo = new { bar = (string)null }, bar = "baz" });
            Assert.AreEqual("", result);
        }

        [Test]
        public void It_pushes_and_pops_contexts_correctly()
        {
            var result = Render.StringToString(
                "{{bar}}{{#foo}}{{bar}}{{/foo}}{{bar}}",
                new { foo = new { bar = "quux" }, bar = "baz" });
            Assert.AreEqual("bazquuxbaz", result);
        }

        [Test]
        public void It_can_include_templates()
        {
            var fooTemplate = new Template();
            fooTemplate.Load(new StringReader("FOO"));

            var result = Render.StringToString(
                "before{{>foo}}after", null, name => fooTemplate);

            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_throws_to_prevent_infinite_template_recursion()
        {
            var fooTemplate = new Template();
            fooTemplate.Load(new StringReader("{{>foo}}"));

            Assert.Throws<NustacheException>(
                () => Render.StringToString(
                    "before{{>foo}}after", null, name => fooTemplate));
        }

        [Test]
        public void It_can_include_templates_defined_in_templates()
        {
            var result = Render.StringToString(
                "{{<foo}}FOO{{/foo}}before{{>foo}}after", null);

            Assert.AreEqual("beforeFOOafter", result);
        }

        [Test]
        public void It_can_include_templates_defined_in_outer_templates()
        {
            var result = Render.StringToString(
                "{{<foo}}OUTSIDE{{/foo}}before{{#bar}}{{>foo}}{{/bar}}after",
                new { bar = "baz" });

            Assert.AreEqual("beforeOUTSIDEafter", result);
        }

		[Test]
		public void It_can_include_templates_over_three_levels()
		{
			var result = Render.StringToString("{{<t1}}One{{/t1}}{{<t2}}{{>t1}}Two{{/t2}}{{<t3}}{{>t2}}Three{{/t3}}{{>t3}}", null);

			Assert.AreEqual("OneTwoThree", result);
		}


		[Test]
		public void It_can_include_templates_over_three_levels_with_external_includes()
		{
			var baseTemplate = new Template("Base");
			baseTemplate.Load(new StringReader("Base{{>BaseContent}}"));

			var masterTemplate = new Template("Master");
			masterTemplate.Load(new StringReader("{{<BaseContent}}Master{{>MasterContent}}{{/BaseContent}}{{>Base}}"));

			var templates = new Dictionary<string, Template>();
			templates.Add("Base", baseTemplate);
			templates.Add("Master", masterTemplate);

			TemplateLocator locateTemplate =
				name =>
				{
					Template ret;
					templates.TryGetValue(name, out ret);
					if (ret == null) throw new KeyNotFoundException(string.Format("The view '{0}' could not be found.", name));
					return ret;
				};

			var result = Render.StringToString("{{<MasterContent}}Hello{{/MasterContent}}{{>Master}}", null, locateTemplate);

			Assert.AreEqual("BaseMasterHello", result);
		}

        [Test]
        public void It_allows_templates_to_be_overridden_in_sections()
        {
            var result = Render.StringToString(
                "{{<foo}}OUTSIDE{{/foo}}before{{#bar}}{{<foo}}INSIDE{{/foo}}{{>foo}}{{/bar}}after",
                new { bar = "baz" });

            Assert.AreEqual("beforeINSIDEafter", result);
        }

        private class SneakyDictionary<K, V> : IDictionary<K, V>
        {
            private Dictionary<K, V> underlying = new Dictionary<K, V>();
            public void Add(K key, V value)
            {
                underlying.Add(key, value);
            }

            public bool ContainsKey(K key)
            {
                return underlying.ContainsKey(key);
            }

            public ICollection<K> Keys
            {
                get { return underlying.Keys; }
            }

            public bool Remove(K key)
            {
                return underlying.Remove(key);
            }

            public bool TryGetValue(K key, out V value)
            {
                return underlying.TryGetValue(key, out value);
            }

            public ICollection<V> Values
            {
                get { return underlying.Values; }
            }

            public V this[K key]
            {
                get
                {
                    return underlying[key];
                }
                set
                {
                    underlying[key] = value;
                }
            }

            public void Add(KeyValuePair<K, V> item)
            {
                (underlying as IDictionary<K, V>).Add(item);
            }

            public void Clear()
            {
                underlying.Clear();
            }

            public bool Contains(KeyValuePair<K, V> item)
            {
                return (underlying as IDictionary<K, V>).Contains(item);
            }

            public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
            {
                (underlying as IDictionary<K, V>).CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return underlying.Count; }
            }

            public bool IsReadOnly
            {
                get { return (underlying as IDictionary<K, V>).IsReadOnly; }
            }

            public bool Remove(KeyValuePair<K, V> item)
            {
                return (underlying as IDictionary<K, V>).Remove(item);
            }

            public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
            {
                return underlying.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return (underlying as System.Collections.IEnumerable).GetEnumerator();
            }
        }
    }
}