using System;
using System.Collections.Generic;
using Trainline.NetStandard.StandardHeaders.Extensions;
using Trainline.NetStandard.StandardHeaders.Services;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Vortex;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Extensions;
using Trainline.VortexPublisher.EventPublishing;
using Promocode = Trainline.PromocodeService.Model.Promocode;

namespace Trainline.PromocodeService.Host.Extensions
{
    public static class VortexEventPublisherExtensions
    {
        public static void NotifyVortexOfPromocodeCreated(this IVortexEventPublisher publisher, Promocode promocode, IHeaderService headerService)
        {
            if (publisher == null)
                throw new InvalidOperationException(
                    "Incorrect constructor used - Vortex client is null - the default ctor only exists for unit tests!");

            var promocodeCreated = promocode.ToCreatedVortexEvent();
            promocodeCreated.Header.ConversationId = headerService.GetConversationId();
            promocodeCreated.Header.ContextUri = headerService.GetContextUri();
            publisher.Publish(promocodeCreated);
        }

        public static void NotifyVortexOfPromocodeValidated(this IVortexEventPublisher publisher, string promocodeId, IEnumerable<DiscountInvoiceInfo> invoiceInfos, IHeaderService headerService, string campaignName)
        {
            if (publisher == null)
                throw new InvalidOperationException(
                    "Incorrect constructor used - Vortex client is null - the default ctor only exists for unit tests!");

            var promocodeValidated = invoiceInfos.ToValidatedVortexEvent(promocodeId, campaignName);
            promocodeValidated.Header.ConversationId = headerService.GetConversationId();
            promocodeValidated.Header.ContextUri = headerService.GetContextUri();
            promocodeValidated.CustomerUri = headerService.GetCustomerUri()?.ToString();

            publisher.Publish(promocodeValidated);
        }

        public static void NotifyVortexOfPromocodeRedeemed(this IVortexEventPublisher publisher, string promocodeId, ICollection<DiscountInvoiceInfo> invoiceInfos, IHeaderService headerService, string campaignName)
        {
            if (publisher == null)
                throw new InvalidOperationException(
                    "Incorrect constructor used - Vortex client is null - the default ctor only exists for unit tests!");

            var promocodeRedeemed = invoiceInfos.ToRedeemedVortexEvent(promocodeId, campaignName);
            promocodeRedeemed.Header.ConversationId = headerService.GetConversationId();
            promocodeRedeemed.Header.ContextUri = headerService.GetContextUri();
            promocodeRedeemed.CustomerUri = headerService.GetCustomerUri()?.ToString();

            publisher.Publish(promocodeRedeemed);
        }

        public static void NotifyVortexOfPromocodeReinstate(this IVortexEventPublisher publisher, string promocodeId, string redemptionId, IHeaderService headerService)
        {
            if (publisher == null)
                throw new InvalidOperationException(
                    "Incorrect constructor used - Vortex client is null - the default ctor only exists for unit tests!");

            var promocodeReinstated = new PromocodeReinstated
            {
                PromocodeId = promocodeId,
                RedemptionId = redemptionId,
                CustomerUri = headerService.GetCustomerUri()?.ToString(),
                Header =
                {
                    ConversationId = headerService.GetConversationId(),
                    ContextUri = headerService.GetContextUri()
                }
            };

            publisher.Publish(promocodeReinstated);
        }

        public static void NotifyVortexOfNewCustomerPromocode(this IVortexEventPublisher publisher, NewCustomerCampaignEligibilityData eligibilityData, IHeaderService headerService)
        {
            if (publisher == null)
                throw new InvalidOperationException(
                    "Incorrect constructor used - Vortex client is null - the default ctor only exists for unit tests!");

            var newCustomerPromocode = new NewCustomerPromocode
            {
                Email = eligibilityData.Email,
                FirstName = eligibilityData.FirstName,
                LastName = eligibilityData.LastName,
                CustomerId = eligibilityData.CustomerId,
                ExternalCampaignId = eligibilityData.ExternalCampaignId,
                Locale = eligibilityData.Locale,
                Header =
                {
                    ConversationId = headerService.GetConversationId(),
                    ContextUri = headerService.GetContextUri()
                }
            };

            publisher.Publish(newCustomerPromocode);
        }
    }
}
