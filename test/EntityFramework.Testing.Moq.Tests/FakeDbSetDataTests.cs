using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Testing.Moq.Tests
{
    [TestClass]
    public class FakeDbSetDataTests
    {
        public void Data_is_addded_to_set()
        {
            var data = new List<Blog> { new Blog(), new Blog() };

            var set = new MockDbSet<Blog>()
                .SetupSeedData(data);

            var result = set.Data.ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(data[0], result[0]);
            Assert.AreSame(data[1], result[1]);
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
        }
    }
}
