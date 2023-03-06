using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Trainline.PromocodeService.Host.Startup;

namespace Trainline.PromocodeService.Host.UnitTests
{
    [TestFixture]
    public class TrainlineMediaTypeApiVersionReaderTests
    {
        private TrainlineMediaTypeApiVersionReader _reader;


        [SetUp]
        public void Setup()
        {
            _reader = new TrainlineMediaTypeApiVersionReader();
        }

        [TestCase("application/vnd.trainline.promocode.v2+json", "2.0")]
        [TestCase("application/vnd.trainline.promocode.v1+json", "1.0")]
        [TestCase(null, null)]
        public void DetectCorrectVersionFromAcceptHeader(string header, string version)
        {
            var request = RequestWithAcceptHeader(header);

            var result = _reader.Read(request);

            Assert.AreEqual(version, result);
        }

        private HttpRequest RequestWithAcceptHeader(string header)
        {
            var mock = new Mock<HttpRequest>();
            mock.Setup(x => x.Headers)
                .Returns(() => new HeaderDictionary
                {
                    {"Accept", header}
                });

            return mock.Object;
        }
    }
}
