using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Nustache.Core.Tests
{
    [TestFixture]
    public class Example_Trees
    {
        [Test]
        public void Flat_lists_must_be_transformed_into_trees()
        {
            var list = GetNodeList();
            var tree = GetNodeTree(list);
            var template =
@"{{<node}}
    <li>{{Description}}
        {{#HasChildren}}
            <ul>
                {{#Children}}
                    {{>node}}
                {{/Children}}
            </ul>
        {{/HasChildren}}
    </li>
{{/node}}

<ul>
    {{#.}}
        {{>node}}
    {{/.}}
</ul>";

            var result = Render.StringToString(template, tree);

            Assert.AreEqual(
@"<ul>
    <li>A</li>
    <li>B
        <ul>
            <li>C
                <ul>
                    <li>D</li>
                </ul>
            </li>
            <li>E</li>
        </ul>
    </li>
    <li>F</li>
</ul>".RemoveWhitespace(), result.RemoveWhitespace());
        }

        private class ListNode
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public int ParentId { get; set; }
        }

        private IEnumerable<ListNode> GetNodeList()
        {
            return new[]
                   {
                       new ListNode { Id = 1, Description = "A" },
                       new ListNode { Id = 2, Description = "B" },
                       new ListNode { Id = 3, Description = "C", ParentId = 2 },
                       new ListNode { Id = 4, Description = "D", ParentId = 3 },
                       new ListNode { Id = 5, Description = "E", ParentId = 2 },
                       new ListNode { Id = 6, Description = "F" },
                   };
        }

        private class TreeNode
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public int ParentId { get; set; }
            public IEnumerable<TreeNode> Children { get; set; }
            public bool HasChildren { get { return Children.Any(); } }
        }

        private IEnumerable<TreeNode> GetNodeTree(IEnumerable<ListNode> listNodes)
        {
            var treeNodes = listNodes.Select(n => new TreeNode
                                                  {
                                                      Id = n.Id,
                                                      Description = n.Description,
                                                      ParentId = n.ParentId
                                                  })
                                     .ToArray();

            var lookup = treeNodes.ToLookup(n => n.ParentId);

            foreach (var treeNode in treeNodes)
            {
                treeNode.Children = lookup[treeNode.Id].ToArray();
            }

            return lookup[0].ToArray();
        }
    }

    internal static class StringExtensions
    {
        public static string RemoveWhitespace(this string input)
        {
            return Regex.Replace(input, @"\s", "");
        }
    }
}