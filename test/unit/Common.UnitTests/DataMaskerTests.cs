using NUnit.Framework;
using Trainline.PromocodeService.Common.Formatting;

namespace Common.UnitTests
{
    public class DataMaskerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("thiagoms@thetrainline.com.pt", "t******s@t***********.com.pt")]
        [TestCase("thiago.marcolino@thetrainline.com", "t**************o@t***********.com")]
        public void GivenAEmail_WhenMasked_ThenTheMaskedEmailIsReturned(string email, string expected)
        {
            var result = email.MaskEmail();

            Assert.AreEqual(expected, result);
        }
    }
}
