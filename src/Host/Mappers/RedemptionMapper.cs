using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.Host.Constants;
using Trainline.PromocodeService.Model;
using Redeemed = Trainline.PromocodeService.Contract.Redeemed;

namespace Trainline.PromocodeService.Host.Mappers
{
    public class RedemptionMapper : IRedemptionMapper
    {
        private readonly IInvoiceMapper _invoiceMapper;
        private readonly IUrlHelper _urlHelper;

        public RedemptionMapper(IInvoiceMapper invoiceMapper, IUrlHelper urlHelper)
        {
            _invoiceMapper = invoiceMapper;
            _urlHelper = urlHelper;
        }

        public Redeemed Map(Model.Redeemed redeemed)
        {
            return new Redeemed
            {
                RedemptionId = redeemed.Id,
                Code = redeemed.Code,
                DiscountItems = _invoiceMapper.MapDiscounts(redeemed.InvoiceInfos),
                Links = CreateLinks(redeemed)
            };
        }

        public Contract.Redemption Map(Model.Redemption redemption)
        {
            return new Contract.Redemption
            {
                Id = redemption.Id,
                PromocodeId = redemption.PromocodeId,
                CampaignName = redemption.CampaignName,
                Links = CreateLinks(redemption)
            };
        }

        private Dictionary<string, Link> CreateLinks(IRedemption redemption)
        {
            var routeValues = new
            {
                redemptionId = redemption.Id,
                promocodeId = redemption.PromocodeId
            };

            return new Dictionary<string, Link>
            {
                ["self"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.GetRedemption, routeValues)),
                    Method = HttpMethod.Get.Method
                },
                ["reinstate"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.ReinstateRedemption, routeValues)),
                    Method = HttpMethod.Post.Method
                },
                ["ledger"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.GetLedger, routeValues)),
                    Method = HttpMethod.Get.Method
                },
                ["quotes"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.CreateQuote, routeValues)),
                    Method = HttpMethod.Post.Method
                },
                ["link"] = new Link
                {
                    Href = new Uri(_urlHelper.Link(RouteNames.CreateLink, routeValues)),
                    Method = HttpMethod.Post.Method
                }
            };
        }
    }
}
