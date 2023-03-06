using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Constants;

namespace Trainline.PromocodeService.Host.Mappers
{
    public class PromocodeMapper : IPromocodeMapper
    {
        private readonly IUrlHelper _urlHelper;

        public PromocodeMapper(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public Promocode Map(Model.Promocode promocode)
        {
            return new Promocode
            {
                PromocodeId = promocode.PromocodeId,
                Code = promocode.Code,
                ValidityStartDate = promocode.ValidityStartDate.ToString("O"),
                ValidityEndDate = promocode.ValidityEndDate.ToString("O"),
                CurrencyCode = promocode.CurrencyCode,
                ProductType = promocode.ProductType,
                CampaignName = promocode.CampaignName,
                Discount = new Discount
                {
                    Amount = promocode.Discount.Amount,
                    Type = promocode.Discount.Type
                },
                Links = CreateLinks(promocode),
                ValidationRules = promocode.ValidationRules.Select(x => new ValidationRule(x.Name, x.Value)),
                Redemption = new PromocodeRedemption
                {
                    RedeemedQuantity = promocode.RedeemedQuantity,
                    RedemptionQuantity = promocode.RedemptionQuantity
                },
                State = promocode.State.ToString("G"),
                Type = promocode.Type.ToString("G")
            };
        }

        private Dictionary<string, Link> CreateLinks(Model.Promocode promocode)
        {
            var routeValues = new { promocodeId = promocode.PromocodeId };

            return new Dictionary<string, Link>
            {
                ["self"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.GetPromocode, routeValues)),
                    Method = HttpMethod.Get.Method
                },
                ["apply"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.ApplyPromocode, routeValues)),
                    Method = HttpMethod.Post.Method
                },
                ["redeem"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.RedeemPromocode, routeValues)),
                    Method = HttpMethod.Post.Method
                },
                ["redemptions"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.GetAllRedemption, routeValues)),
                    Method = HttpMethod.Get.Method
                }
            };
        }
    }
}
