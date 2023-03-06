namespace Trainline.PromocodeService.Acceptance.Helpers.TestContracts.Diagnostics
{
    public class ServiceInformationContract : ResponseBase
    {
        public string ApplicationId { get; set; }
        public string ServiceName { get; set; }
        public string Version { get; set; }
        public string Slice { get; set; }
        public bool IsHealthy { get; set; }
    }
}
