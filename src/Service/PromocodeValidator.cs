using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Trainline.PromocodeService.Common;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Exceptions;
using Trainline.PromocodeService.Service.Repository;

namespace Trainline.PromocodeService.Service
{
    public class PromocodeValidator : IPromocodeValidator
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPromocodeService _promocodeService;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ILogger _logger;
        private static string[] _validCurrencies = new[] { "GBP", "EUR" };

        private Dictionary<Func<Promocode, bool>, string> _conditions;

        public PromocodeValidator(
            IDateTimeProvider dateTimeProvider,
            IPromocodeService promocodeService,
            ICampaignRepository campaignRepository,
            ILogger logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _promocodeService = promocodeService;
            _campaignRepository = campaignRepository;
            _logger = logger;
            _conditions = new Dictionary<Func<Promocode, bool>, string>
            {
                {(promocode) => promocode == null, ErrorCodes.PromocodeNotFound },
                {(promocode) => promocode?.ValidityStartDate > _dateTimeProvider.UtcNow, ErrorCodes.PromocodeHaveNotStarted },
                {(promocode) => promocode?.ValidityEndDate < _dateTimeProvider.UtcNow, ErrorCodes.PromocodeExpired },
                {(promocode) => promocode != null && !_validCurrencies.Contains(promocode?.CurrencyCode), ErrorCodes.PromocodeCurrencyNotApplicable },
                {(promocode) => promocode?.RedemptionQuantity != null && promocode.RedeemedQuantity >= promocode.RedemptionQuantity, ErrorCodes.PromocodeAlreadyRedeemed }
            };
        }

        public async Task Validate(string promocodeId)
        {
            var promocode = await _promocodeService.GetByPromocodeId(promocodeId);

            var failingConditions = _conditions
                .Where(x => x.Key(promocode));

            if (failingConditions.Any())
            {
                var errorCodes = failingConditions.Select(c => c.Value);
                throw new PromocodeValidatorException(errorCodes);
            }

            var campaign = await _campaignRepository.Get(promocode.CampaignName);
            if (campaign.ExpirationDate < DateTime.UtcNow)
            {
                _logger.LogError($"The promocode {promocode.Code} is invalid because the campaign {promocode.CampaignName} is expired.");
                throw new CustomerIsNotEligibleForTheCampaignException("Campaign is expired.");
            }
        }
    }
}
