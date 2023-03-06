
using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Caches;

namespace Service.UnitTests.Caches
{
    public class InMemoryCacheTests
    {
        const string Key = "cache_key_1";
        private Mock<IDateTimeProvider> _dateTimeProviderMock;
        private InMemoryCache<string> _cache;
        private readonly DateTime _testNow = new DateTime(2021, 11, 1, 11, 01, 01);

        [SetUp]
        public void Setup()
        {
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _dateTimeProviderMock.Setup(x => x.UtcNow)
                .Returns(_testNow);
            _cache = new InMemoryCache<string>(_dateTimeProviderMock.Object);
        }
        
        [Test]
        public async Task GetOrAdd_RetrievesDataWhenCacheEmpty()
        {
            int retrievalCounter = 0;
            var expected = "expectedString";
            var result = await _cache.GetOrAdd(Key, () =>
            {
                retrievalCounter++;
                return Task.FromResult(expected);
            });

            Assert.NotNull(result);
            Assert.AreSame(expected, result);
            Assert.AreEqual(1, retrievalCounter);
        }
        
        [Test]
        public async Task GetOrAdd_ReturnsStoredValue()
        {
            int retrievalCounter = 0;
            var expected = "expectedString";
            Task<string> Get() { 
                retrievalCounter++;
                return Task.FromResult(expected);
            };

            var result1 = await _cache.GetOrAdd(Key, Get);
            var result2 = await _cache.GetOrAdd(Key, Get);
            var result3 = await _cache.GetOrAdd(Key, Get);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.AreSame(expected, result1);
            Assert.AreSame(expected, result2);
            Assert.AreSame(expected, result3);
            Assert.AreEqual(1, retrievalCounter);
        }

        [Test]
        public async Task GetOrAdd_WhenCacheDataExpired_ReturnsRefreshedData()
        {
            int retrievalCounter = 0;
            var expired = "expiredString";
            var expiredResult = await _cache.GetOrAdd(Key, () => {
                retrievalCounter++;
                return Task.FromResult(expired);
            });
            _dateTimeProviderMock.Setup(x => x.UtcNow)
                .Returns(_testNow.AddHours(1));

            var expected = "expectedString";
            Task<string> Get()
            {
                retrievalCounter++;
                return Task.FromResult(expected);
            };
            var result1 = await _cache.GetOrAdd(Key, Get);
            var result2 = await _cache.GetOrAdd(Key, Get);

            Assert.NotNull(expiredResult);
            Assert.AreSame(expired, expiredResult);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.AreSame(expected, result1);
            Assert.AreSame(expected, result2);
            Assert.AreEqual(2, retrievalCounter);
        }

        [Test]
        public async Task GetOrAdd_CacheExpiresExactlyAtTheBeginningOfTheHour()
        {
            int retrievalCounter = 0;
            var expected = "expectedString";
            Task<string> Get()
            {
                retrievalCounter++;
                return Task.FromResult(expected);
            };

            await _cache.GetOrAdd(Key, Get);
            var expectedExpiredNow = new DateTime(2021, 11, 1, 12, 00, 00);
            _dateTimeProviderMock.Setup(x => x.UtcNow)
                .Returns(expectedExpiredNow);

            await _cache.GetOrAdd(Key, Get);

            Assert.AreEqual(2, retrievalCounter);
        }
    }
}
