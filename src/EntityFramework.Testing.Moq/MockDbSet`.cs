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

        public IEnumerable<TEntity> Data
        {
            get { return _data; }
        }

        internal bool IsLinqSetup { get; set; }

        internal IQueryable<TEntity> Queryable
        {
            get { return _queryable; }
        }

        internal void AddData(TEntity data)
        {
            _data.Add(data);
        }

        internal void AddData(IEnumerable<TEntity> data)
        {
            _data.AddRange(data);
        }

        internal void RemoveData(TEntity data)
        {
            _data.Remove(data);
        }
    }
}
