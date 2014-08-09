using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace EntityFramework.Testing.Moq
{
    public static class MockDbSetExtenstions
    {
        public static Mock<DbSet<TEntity>> SetupData<TEntity>(this Mock<DbSet<TEntity>> mock, ICollection<TEntity> data = null, Func<object[], TEntity> find = null) where TEntity : class
        {
            data = data ?? new List<TEntity>();
            find = find ?? (o => null);

            var query = new InMemoryAsyncQueryable<TEntity>(data.AsQueryable());

            mock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(query.Provider);
            mock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(query.Expression);
            mock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(query.ElementType);
            mock.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(query.GetEnumerator());
#if !NET40
            mock.As<IDbAsyncEnumerable<TEntity>>().Setup(m => m.GetAsyncEnumerator()).Returns(query.GetAsyncEnumerator());
#endif
            mock.Setup(m => m.Include(It.IsAny<string>())).Returns(mock.Object);
            mock.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(find);

            mock.Setup(m => m.Remove(It.IsAny<TEntity>())).Callback<TEntity>(t =>
            {
                data.Remove(t);
                mock.SetupData(data, find);
            });

            mock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>())).Callback<IEnumerable<TEntity>>(r =>
            {
                foreach (var t in r)
                {
                    data.Remove(t);
                }

                mock.SetupData(data, find);
            });

            mock.Setup(m => m.Add(It.IsAny<TEntity>())).Callback<TEntity>(t =>
            {
                data.Add(t);
                mock.SetupData(data, find);
            });

            mock.Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>())).Callback<IEnumerable<TEntity>>(t =>
            {
                foreach (var e in t)
                {
                    data.Add(e);
                };

                mock.SetupData(data, find);
            });

            return mock;
        }
    }
}
