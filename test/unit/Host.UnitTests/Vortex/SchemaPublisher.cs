using System;
using System.Collections.Generic;
using System.IO;
using Trainline.VortexPublisher;
using Trainline.VortexPublisher.Configuration;
using Trainline.VortexPublisher.SchemaRegistration;
using Trainline.VortexPublisher.Serialization.JsonDotNet;

namespace Trainline.PromocodeService.Host.UnitTests.Vortex
{
    public class SchemaPublisher
    {
        public string Publish<TType>(string owningCluster, List<VortexEnvironment> environments = null,
            string saveLocation = null) where TType : VortexEvent
        {
            environments = environments ??
                           new List<VortexEnvironment>
                           {
                               VortexEnvironment.Cluster,
                               VortexEnvironment.Nft
                           };

            MessageSchema schema = null;
            foreach (var destination in environments)
            {
                var cfg = VortexBuilderFactory.Configure()
                    .ForEnvironment(destination)
                    .AndSerializer(new JsonDotNetSerializer());

                var registrar = cfg.CreateSchemaRegistrar();
                schema = schema ?? registrar.GenerateSchema<TType>();

                try
                {
                    var result = registrar.Register(schema.Metadata.SchemaName, schema.Metadata.SchemaVersion,
                        schema.Body, owningCluster).Result;

                    Console.WriteLine(
                        $"{destination} {result.Message} - {schema.Metadata.SchemaName}-{schema.Metadata.SchemaVersion}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            saveLocation = saveLocation ?? Directory.GetCurrentDirectory();
            var fullPath = Path.Combine(saveLocation,
                schema.Metadata.SchemaName + "-" + schema.Metadata.SchemaVersion + ".json");

            File.WriteAllText(fullPath, schema.Body);
            Console.WriteLine("Saved to " + fullPath);

            return fullPath;
        }
    }
}
