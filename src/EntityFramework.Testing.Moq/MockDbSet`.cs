using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EntityFramework.Testing.Moq
{
    public class MockDbSet<TEntity> : Mock<DbSet<TEntity>>
           where TEntity : class
    {
        private IQueryable<TEntity> _queryable;
        private List<TEntity> _data;

        public MockDbSet()
        {
            _data = new List<TEntity>();
            _queryable = _data.AsQueryable();
        }

        public ICollection<TEntity> Data
        {
            get { return _data; }
        }

        internal bool IsLinqSetup { get; set; }

        internal IQueryable<TEntity> Queryable
        {
            get { return _queryable; }
        }

        internal void AddData(ICollection<TEntity> data)
        {
            _data.AddRange(data);
        }
    }
}
