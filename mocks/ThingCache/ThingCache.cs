using System.Collections.Generic;
using NUnit.Framework;
using FakeItEasy;
using FluentAssertions;

namespace MockFramework
{
    public class ThingCache
    {
        private IDictionary<string, Thing> dictionary
            = new Dictionary<string, Thing>();
        private readonly IThingService thingService;

        public ThingCache(IThingService thingService)
        {
            this.thingService = thingService;
        }

        public Thing Get(string thingId)
        {
            Thing thing;
            if (dictionary.TryGetValue(thingId, out thing))
                return thing;
            if (thingService.TryRead(thingId, out thing))
            {
                dictionary[thingId] = thing;
                return thing;
            }
            return null;
        }
    }

    [TestFixture]
    public class ThingCache_Should
    {
        private IThingService thingService;
        private ThingCache thingCache;

        private const string thingId1 = "TheDress";
        private Thing thing1 = new Thing(thingId1);

        private const string thingId2 = "CoolBoots";
        private Thing thing2 = new Thing(thingId2);

        [SetUp]
        public void SetUp()
        {
            thingService = A.Fake<IThingService>();
            thingService.TryRead(thingId1, out thing1);
            thingService.TryRead(thingId2, out thing2);

            thingCache = new ThingCache(thingService);
        }
        
        [Test]
        public void TestGetThing1() {
            A.CallTo(() => thingService.TryRead(thingId1, out thing1))
                .Returns(true);

            var actualThing = thingCache.Get(thingId1);
            Assert.AreEqual(thingId1, actualThing.ThingId);
        }

        [Test]
        public void TestGetThing2() {
            A.CallTo(() => thingService.TryRead(thingId2, out thing2))
                .Returns(true);

            Assert.AreEqual(thingId2, thingCache.Get(thingId2).ThingId);
        }
        
        [Test]
        public void TestReturnNullIfEmpty() {
            thingService.TryRead(thingId1, out thing1);
            Assert.IsNull(thingCache.Get(""));
        }
        
        [Test, Timeout(1000)]
        public void Test() {
            A.CallTo(() => thingService.TryRead(thingId2, out thing2))
                .Returns(true);

            Assert.AreEqual(thingId2, thingCache.Get(thingId2).ThingId);
        }

    }
}