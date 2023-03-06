using System;
using NUnit.Framework;
using Trainline.PromocodeService.Host.Vortex;

namespace Trainline.PromocodeService.Host.UnitTests.Vortex
{
    [TestFixture]
    public class PromocodeGenericTests
    {
        [Explicit]
        [TestCase(typeof(PromocodeCreated))]
        [TestCase(typeof(PromocodeValidated))]
        [TestCase(typeof(PromocodeRedeemed))]
        [TestCase(typeof(PromocodeReinstated))]
        public void GenerateSchema(Type eventType)
        {
            var publishMethod = typeof(SchemaPublisher).GetMethod("Publish");
            var fooRef = publishMethod.MakeGenericMethod(eventType);
            fooRef.Invoke(new SchemaPublisher(), new object[]{ "dietcode@thetrainline.com", null, null });
        }
    }
}
