using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Testing.Moq.Tests
{
    [TestClass]
    public class FakeDbSetDataOperationsTests
    {
        [TestMethod]
        public void Basic_add()
        {
            var set = new MockDbSet<Blog>()
                .SetupAddAndRemove();

            var blog = new Blog();
            var result = set.Object.Add(blog);

            Assert.AreSame(blog, result);
            Assert.AreEqual(1, set.Data.Count());
            Assert.IsTrue(set.Data.Contains(blog));
        }

        [TestMethod]
        public void Basic_remove()
        {
            var blog1 = new Blog();
            var blog2 = new Blog();
            var data = new List<Blog> { blog1, blog2 };
            var set = new MockDbSet<Blog>()
                .SetupSeedData(data)
                .SetupAddAndRemove();

            var result = set.Object.Remove(blog1);

            Assert.AreSame(blog1, result);
            Assert.AreEqual(1, set.Data.Count());
            Assert.IsFalse(set.Data.Contains(blog1));
            Assert.IsTrue(set.Data.Contains(blog2));
        }

        [TestMethod]
        public void Add_remove_work_with_enumeration()
        {
            var blog1 = new Blog();
            var blog2 = new Blog();
            var blog3 = new Blog();
            var data = new List<Blog> { blog1, blog2 };
            var set = new MockDbSet<Blog>()
                .SetupSeedData(data)
                .SetupLinq()
                .SetupAddAndRemove();

            set.Object.Remove(blog2);
            set.Object.Add(blog3);

            var result = set.Object.ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(blog3));
            Assert.IsTrue(result.Contains(blog1));
        }

        [TestMethod]
        public void Basic_find()
        {
            var blog = new Blog { BlogId = 1 };
            var data = new List<Blog> { blog, new Blog { BlogId = 2 } };
            var set = new MockDbSet<Blog>()
                .SetupSeedData(data)
                .SetupFind((keyValues, entity) => entity.BlogId == (int)keyValues.Single());

            var result = set.Object.Find(1);

            Assert.AreSame(blog, result);
        }

        [TestMethod]
        public void Find_returs_null_for_no_match()
        {
            var data = new List<Blog>{ new Blog { BlogId = 1 }, new Blog { BlogId = 2 } };
            var set = new MockDbSet<Blog>()
                .SetupSeedData(data)
                .SetupFind((keyValues, entity) => entity.BlogId == (int)keyValues.Single());

            var result = set.Object.Find(99);

            Assert.IsNull(result);
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
        }
    }
}
