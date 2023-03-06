using System.Linq;
using System.Net;
using NUnit.Framework;
using Trainline.Acceptance.Response;
using Trainline.PromocodeService.Acceptance.Helpers.TestContracts;

namespace Trainline.PromocodeService.Acceptance.Helpers.Steps
{
    public class ThenSteps
    {
        public void TheResponseCodeShouldBe(ApiResponse response, HttpStatusCode statusCode)
        {
            Assert.That(response.StatusCode, Is.EqualTo(statusCode));
        }

        public void TheResponseShouldIncludeALinkTo<TResponse>(ApiResponse<TResponse> response, string rel) where TResponse : ResponseBase
        {
            var link = response.Content.Links.SingleOrDefault(l => l.Rel == rel);

            Assert.That(link, Is.Not.Null);
            Assert.That(link.Href, Is.Not.Null);
            Assert.That(link.Method, Is.Not.Null);
        }

        public void TheResponseShouldHaveAnError<TResponse>(ApiResponse<TResponse> response, string errorCode) where TResponse : ResponseBase
        {
            var error = response.Content.Errors.FirstOrDefault(e => e.Code == errorCode);

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Detail, Is.Not.Null);
        }
    }
}
