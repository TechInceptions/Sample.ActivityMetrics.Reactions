using Microsoft.Azure.CosmosDB.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TECHIS.Cloud.ActivityMetrics.Reactions.Test
{
    [TestClass]
   public class CountGeneratorTests
    {
        [TestInitialize]
        public void Init()
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        private const string _ValueName = "Count";

        private int[] _SubjectIds = { 101, 102, 103, 104, 106, 107, 108, 109, 110, 151, 152, 153, 154, 156, 157, 158, 159, 160, 1, 2, 3, 4, 6, 7,8,9,10, 51, 52, 53, 54, 56, 57, 58, 59, 60 };

        private string[] _SubjectTypeIds =  { "1", "2", "3", "4", "5" };

        private string[] _Actors = { "john", "manbij", "ike", "ari", "leena", "mary" };
        private string[] _ValueTypes = { "like", "smile", "dislike", "love", "hate", "upset" };

        [TestMethod]
        public async Task RandomPopulateAndUpdateAsync()
        {
            await PopulateJournalAsync(true);
            await ExecuteSummries(new MetricsCountWriter());
        }

        [TestMethod]
        public async Task UpdateAsync()
        {
            await ExecuteSummries(new MetricsCountWriter());
        }
        [TestMethod]
        public async Task PopulateAndUpdateBatchAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await PopulateJournalAsync(true);
            stopwatch.Stop();
            var popTIme = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
            
            stopwatch.Start();
                var results = await new MetricsCountWriter().Write();
            stopwatch.Stop();
            var batchTime = stopwatch.Elapsed.TotalSeconds;

            Assert.IsTrue(results.All(r => r.ValuePersisted));
        }

        [TestMethod]
        public async Task QueryRangeTest()
        {
            var reader = new MetricsReader();

            int maxResultCount = 5;
            var results = await reader.QueryAsync<DynamicTableEntity>(_SubjectIds[0], _SubjectIds[3], _SubjectTypeIds[0], maxResultCount);

            Assert.IsTrue(results.Count <= maxResultCount);
        }
        [TestMethod]
        public async Task QueryListTest()
        {

            var reader = new MetricsReader();

            int maxResultCount = 5;
            var results = await reader.QueryAsync<DynamicTableEntity>(new List<int>{ _SubjectIds[0], _SubjectIds[3] }, _SubjectTypeIds[0], maxResultCount);

            Assert.IsTrue( results.Count <= maxResultCount, $" {nameof(maxResultCount)} is '{maxResultCount}'. Count of results is '{results.Count}'");
        }
        [TestMethod]
        public async Task QueryPointsTest()
        {

            var reader = new MetricsReader();

            int maxResultCount = 5;
            var results = await reader.QueryPointsAsync<DynamicTableEntity>(new List<int>
            {
                _SubjectIds[0],
                _SubjectIds[3],
                _SubjectIds[20],
                _SubjectIds[19],
                _SubjectIds[18], 
                _SubjectIds[_SubjectIds.Length-1]
            }, _SubjectTypeIds[1], maxResultCount);

            Assert.IsTrue(results.Count <= maxResultCount, $" {nameof(maxResultCount)} is '{maxResultCount}'. Count of results is '{results.Count}'");
        }

        #region Private Methods 

        private async Task ExecuteSummries(MetricsCountWriter sum)
        {
            await sum.Write();
        }

        private string DeriveSummaryPartition(string subjectId, string subjectTypeId, decimal partitionPageSize = 50)
        {
            string partition = "1";
            if (int.TryParse(subjectId, out int result))
            {
                partition = $"{Math.Ceiling((result / partitionPageSize))}-{subjectTypeId}";
            }

            return partition;
        }
        private async Task<bool[]> PopulateJournalAsync(bool randomExecute=false, int randomChanceCount =3, int randomPoolSize=10)
        {
            var journal = new Activity();

            var numberOfCycles = 1;
            List<Task<bool>> tasks = new List<Task<bool>>();

            int bet = 0;
            if (randomExecute)
            {
                bet = (new Random()).Next(randomPoolSize);
            }
            for (int i = 0; i < numberOfCycles; i++)
            {
                foreach (var typeId in _SubjectTypeIds)
                {
                    foreach (var subjectId in _SubjectIds)
                    {
                        if ((!randomExecute) || RandomAllow(bet,randomChanceCount, randomPoolSize))
                        {
                            var partitionKey = $"{subjectId}-{typeId}";
                            tasks.Add(journal.AddAsync(subjectId, typeId, RandomSelect(new string[] { "1", "2", "3" }), RandomSelect(_ValueTypes), RandomSelect(_Actors)));
                        }
                    }
                }
            }

            return await Task.WhenAll(tasks);
        }

        private TResult RandomSelect<TResult>(IList<TResult> set)
        {
            return set[(new Random()).Next(set.Count)];
        }


        private bool RandomAllow(int chances = 3, int poolSize = 10)
        {
            Random rnd = new Random();

            int bet = rnd.Next(poolSize);

            List<int> numberPool = new List<int>(chances);
            for (int i = 0; i < chances; i++)
            {
                numberPool.Add( rnd.Next(poolSize));
            }

            if (numberPool.Contains(bet))
            {
                return true;
            }

            return false;
        }
        private bool RandomAllow(int bet, int chances = 3, int poolSize = 10)
        {
            Random rnd = new Random();

           
            List<int> numberPool = new List<int>(chances);
            for (int i = 0; i < chances; i++)
            {
                numberPool.Add(rnd.Next(poolSize));
            }

            if (numberPool.Contains(bet))
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
