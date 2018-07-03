using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TECHIS.Cloud.ActivityMetrics.Reactions;

namespace TECHIS.Cloud.ActivityMetrics.Reactions.Test
{
    [TestClass]
    public class ActivityTests
    {
        [TestInitialize]
        public void Init()
        {
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        }

        [TestMethod]
        public void TestMethod1()
        {
            long mx = long.MaxValue;
            var mxString = mx.ToString("D19");
            var mxStringHex = mx.ToString("X19");

            var l1 = mxString.Length;
            var l2 = mxStringHex.Length;

            Assert.AreEqual(l1, l2);
        }
        [TestMethod]
        public void TestMaxKey()
        {
            Settings settings = new Settings();
            var maxKey = settings.GetActivityPartitionKey(0);
            var currentKey = settings.GetActivityPartitionKey(DateTime.UtcNow.Ticks);

            Assert.IsTrue(string.CompareOrdinal( maxKey, currentKey)>0);
        }
        [TestMethod]
        public void TestConvert()
        {

            Guid g = Guid.Parse("{C6EF9E84-CEA8-48F1-A582-2A5390146CC8}");
            var b = new byte[] { 0, 1 };
            var t = DateTime.UtcNow;
            var inputs = new List<DynamicTableEntity> {
                new DynamicTableEntity("p0","r",string.Empty,  new Dictionary<string, EntityProperty>
                                                                {
                                                                    { EdmType.String.ToString(),    new EntityProperty(nameof(EntityProperty.StringValue)) },
                                                                    { EdmType.Double.ToString(),    new EntityProperty( double.Epsilon) },
                                                                    { EdmType.Guid.ToString(),      new EntityProperty(g) },
                                                                    { EdmType.Binary.ToString(),    new EntityProperty(b) },
                                                                    { EdmType.Boolean.ToString(),   new EntityProperty(true) },
                                                                    { EdmType.DateTime.ToString(),  new EntityProperty(t)},
                                                                    { EdmType.Int32.ToString(),     new EntityProperty(int.MinValue) },
                                                                    { EdmType.Int64.ToString(),     new EntityProperty(Int64.MinValue) }
                                                                } ),
                new DynamicTableEntity("p1","r",string.Empty,  new Dictionary<string, EntityProperty>
                                                                {
                                                                    { EdmType.String.ToString(),    new EntityProperty(nameof(EntityProperty.StringValue)) },
                                                                    { EdmType.Double.ToString(),    new EntityProperty( double.Epsilon) },
                                                                    { EdmType.Guid.ToString(),      new EntityProperty(g) },
                                                                    { EdmType.Binary.ToString(), new EntityProperty(b) },
                                                                    { EdmType.Boolean.ToString(), new EntityProperty(true) },
                                                                    { EdmType.DateTime.ToString(), new EntityProperty(t)},
                                                                    { EdmType.Int32.ToString(), new EntityProperty(int.MinValue) },
                                                                    { EdmType.Int64.ToString(), new EntityProperty(Int64.MinValue) }
                                                                } ),
                new DynamicTableEntity("p2","r",string.Empty,  new Dictionary<string, EntityProperty>
                                                                {
                                                                    { EdmType.String.ToString(), new EntityProperty(nameof(EntityProperty.StringValue)) },
                                                                    { EdmType.Double.ToString(), new EntityProperty( double.Epsilon) },
                                                                    { EdmType.Guid.ToString(), new EntityProperty(g) },
                                                                    { EdmType.Binary.ToString(), new EntityProperty(b) },
                                                                    { EdmType.Boolean.ToString(), new EntityProperty(true) },
                                                                    { EdmType.DateTime.ToString(), new EntityProperty(t)},
                                                                    { EdmType.Int32.ToString(), new EntityProperty(int.MinValue) },
                                                                    { EdmType.Int64.ToString(), new EntityProperty(Int64.MinValue) }
                                                                } ),
            };

            var r = inputs.Convert();

            Assert.IsTrue(r?.Count == 3);

            Assert.IsTrue(r[0][EdmType.String.ToString()]   == nameof(EntityProperty.StringValue));
            Assert.IsTrue(r[0][EdmType.Double.ToString()]   == double.Epsilon.ToString());
            Assert.IsTrue(r[0][EdmType.Guid.ToString()]     == g.ToString());
            Assert.IsTrue(r[0][EdmType.Binary.ToString()]   == Convert.ToBase64String(b));
            Assert.IsTrue(r[0][EdmType.Boolean.ToString()]  == true.ToString());
            Assert.IsTrue(r[0][EdmType.DateTime.ToString()] == new DateTimeOffset(t).ToString());
            Assert.IsTrue(r[0][EdmType.Int32.ToString()]    == int.MinValue.ToString());
            Assert.IsTrue(r[0][EdmType.Int64.ToString()]    == Int64.MinValue.ToString());

            Assert.IsTrue(r[1][EdmType.String.ToString()]   == nameof(EntityProperty.StringValue));
            Assert.IsTrue(r[1][EdmType.Double.ToString()]   == double.Epsilon.ToString());
            Assert.IsTrue(r[1][EdmType.Guid.ToString()]     == g.ToString());
            Assert.IsTrue(r[1][EdmType.Binary.ToString()]   == Convert.ToBase64String(b));
            Assert.IsTrue(r[1][EdmType.Boolean.ToString()]  == true.ToString());
            Assert.IsTrue(r[1][EdmType.DateTime.ToString()] == new DateTimeOffset(t).ToString());
            Assert.IsTrue(r[1][EdmType.Int32.ToString()]    == int.MinValue.ToString());
            Assert.IsTrue(r[1][EdmType.Int64.ToString()]    == Int64.MinValue.ToString());

            Assert.IsTrue(r[2][EdmType.String.ToString()]   == nameof(EntityProperty.StringValue));
            Assert.IsTrue(r[2][EdmType.Double.ToString()]   == double.Epsilon.ToString());
            Assert.IsTrue(r[2][EdmType.Guid.ToString()]     == g.ToString());
            Assert.IsTrue(r[2][EdmType.Binary.ToString()]   == Convert.ToBase64String(b));
            Assert.IsTrue(r[2][EdmType.Boolean.ToString()]  == true.ToString());
            Assert.IsTrue(r[2][EdmType.DateTime.ToString()] == new DateTimeOffset(t).ToString());
            Assert.IsTrue(r[2][EdmType.Int32.ToString()]    == int.MinValue.ToString());
            Assert.IsTrue(r[2][EdmType.Int64.ToString()]    == Int64.MinValue.ToString());
        }
    }
}
