using System;

namespace Trainline.PromocodeService.Service.Exceptions
{
    public class CustomerIsNotEligibleForTheCampaignException : Exception
    {
        public CustomerIsNotEligibleForTheCampaignException(string message) : base(message)
        {
        }
    }
}
