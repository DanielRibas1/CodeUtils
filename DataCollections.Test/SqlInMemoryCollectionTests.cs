using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataCollections.Test
{    
    public class SqlInMemoryCollectionTests
    {
        private SqlInMemoryCollection<int, SqlMockEntityItem, SqlEntitiesMock> _sup;
                
        public SqlInMemoryCollectionTests()
        {
            _sup = new SqlInMemoryCollection<int, SqlMockEntityItem, SqlEntitiesMock>(
                new ContextStarter<SqlEntitiesMock>(() => GenerateMock()),
                (c) => c().Entities.Select(x => new KeyValuePair<int, SqlMockEntityItem>(x.Id, x)));                
        }

        [Fact]
        public void LoadAndGetVoloteaStaff()
        {
            _sup.Refresh();
            Assert.True(_sup.Loaded);
            Assert.Equal(3, _sup.GetAllKeys().Count());
            Assert.Equal(3, _sup.GetAll().Count());
            SqlMockEntityItem response;
            var result = _sup.TryGet(1, out response);
            Assert.True(result);
            Assert.Equal("Second", response.FirstName);
        }

        private SqlEntitiesMock GenerateMock()
        {
            return new SqlEntitiesMock
            {
                Entities = new List<SqlMockEntityItem>
                {
                    new SqlMockEntityItem
                    {
                        Id = 0,
                        CultureCode = "en-GB",
                        Email = "test1@mail.com",
                        FirstName = "First",
                        LastName = "LastName",
                        PhoneNumber = "600121212"
                    },
                    new SqlMockEntityItem
                    {
                        Id = 1,
                        CultureCode = "es*ES",
                        Email = "test3@mail.com",
                        FirstName = "Second",
                        LastName = "LastName",
                        PhoneNumber = "600121213"
                    },
                    new SqlMockEntityItem
                    {
                        Id = 2,
                        CultureCode = "fr-FR",
                        Email = "test3@mail.com",
                        FirstName = "Third",
                        LastName = "LastName",
                        PhoneNumber = "600121214"
                    }
                }
            };                
        }

        private class SqlMockEntityItem
        {
            public int Id { get; set; }
            public string CultureCode { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
        }

        private class SqlEntitiesMock
        {
            public IEnumerable<SqlMockEntityItem> Entities { get; set; }
        }
    }
}
