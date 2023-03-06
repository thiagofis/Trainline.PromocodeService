namespace Trainline.PromocodeService.Acceptance.Helpers.TestContracts
{
    public class ResponseBase
    {

        public Link[] Links { get; set; }

        public Error[] Errors { get; set; }
    }
}
