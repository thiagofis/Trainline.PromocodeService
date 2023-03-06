using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.ProductJsonApiDeserialisation.Extensions;
using Trainline.ProductJsonApiDeserialisation.ProductService;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.CardType;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.ExternalServices.TravelProduct;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;

namespace Trainline.PromocodeService.Host.Mappers
{
    public class InvoiceMapper : IInvoiceMapper
    {
        private readonly IProductTypeClient _productTypeClient;
        private readonly IDiscountCardClient _discountCardClient;
        private readonly ICardTypeClient _cardTypeClient;
        private readonly ITravelProductClient _travelProductClient;
        public const string Railcard = "railcard";
        public const string Travel = "travel";

        public InvoiceMapper(IProductTypeClient productTypeClient, IDiscountCardClient discountCardClient, ICardTypeClient cardTypeClient, ITravelProductClient travelProductClient)
        {
            _productTypeClient = productTypeClient;
            _discountCardClient = discountCardClient;
            _cardTypeClient = cardTypeClient;
            _travelProductClient = travelProductClient;
        }
        

        public async Task<ICollection<Model.InvoiceInfo>> Map(ICollection<Invoice> invoices)
        {
            var result = new List<Model.InvoiceInfo>();
            foreach (var invoice in invoices)
            {
                var productItems = new List<Model.ProductItem>();
                if (invoice.ProductItems != null)
                {
                    productItems = (await Task.WhenAll(invoice.ProductItems.Select(Map)))
                        .ToList();
                }

                result.Add(new InvoiceInfo
                {
                    Id = invoice.Id,
                    CurrencyCode = invoice.CurrencyCode,
                    ProductItems = productItems
                });
            }

            return result;
        }

        public ICollection<Contract.DiscountItem> MapDiscounts(IEnumerable<DiscountInvoiceInfo> discountedInvoices)
        {
            var result = new List<Contract.DiscountItem>();
            foreach (var discountedInvoice in discountedInvoices)
            {
                result.AddRange(discountedInvoice.Items.Select(x => new Contract.DiscountItem
                {
                    InvoiceId = discountedInvoice.Id,
                    CurrencyCode = discountedInvoice.CurrencyCode,
                    ProductId = x.ProductId,
                    ProductUri = x.ProductUri,
                    Amount = x.Amount,
                    Vendor = x.Vendor,
                    ProductType = x.ProductType,
                    ValidityPeriod = x.ValidityPeriod
                }));
            }

            return result;
        }

        private Contract.Item Map(Model.ProductItem productItem)
        {
            return new Item
            {
                Amount = productItem.Amount,
                ProductId = productItem.ProductId,
                ProductUri = productItem.ProductUri,
                Vendor = productItem.Vendor,
            };
        }

        private async Task<Model.ProductItem> Map(Contract.Item item)
        {
            var productItem = new Model.ProductItem
            {
                Amount = item.Amount,
                ProductId = item.ProductId,
                ProductUri = item.ProductUri,
                Vendor = item.Vendor,
                ProductType = await _productTypeClient.GetProductType(item.ProductUri),
            };

            if (productItem.ProductType == Railcard)
            {
                var discountCard = await _discountCardClient.GetDiscountCardDetailsAsync(item.ProductUri);
                productItem.ValidityPeriod = discountCard.CardDetails.ValidityPeriod;
                productItem.RailcardCode = await _cardTypeClient.GetCardTypeCodeAsync(new Uri(discountCard.CardDetails.CardType.Url));

                return productItem;
            }

            if (productItem.ProductType == Travel)
            {
                var travelProduct = await _travelProductClient.GetAsyncProductAsync(item.ProductUri);

                var carriers = (await (await (await (await travelProduct.Fares).SelectAsync(fare => fare.FareLegs))
                        .SelectManyAsync(fareLegs => fareLegs.SelectAsync(fareLeg => fareLeg.Leg)))
                    .SelectAsync(leg => leg.Carrier)) ?? Enumerable.Empty<Carrier>();
                productItem.CarrierCode = carriers.Select(carrier => carrier?.Code).Where(code => !string.IsNullOrEmpty(code)).Distinct().ToList();

                var fareTypes = await (await travelProduct.Fares).SelectAsync(fare => fare.FareType) ?? Enumerable.Empty<FareType>();
                productItem.TicketTypeCode = fareTypes.Select(fareType => fareType?.Code).Where(code => !string.IsNullOrEmpty(code)).Distinct().ToList();

                return productItem;
            }

            return productItem;
        }

    }
}
