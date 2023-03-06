using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service.Exceptions;

namespace Trainline.PromocodeService.Service.Mappers
{
    public class VoucherifyMapper : IVoucherifyMapper
    {
        private readonly ICustomerVoucherifyMapper _customerVoucherifyMapper;

        public VoucherifyMapper(ICustomerVoucherifyMapper customerVoucherifyMapper)
        {
            _customerVoucherifyMapper = customerVoucherifyMapper;
        }

        public async Task<Validation> Map(ICollection<InvoiceInfo> invoices, Uri? customerUri)
        {
            var currencyCodes = invoices.Select(x => x.CurrencyCode).Distinct().ToList();
            var currencyCode = currencyCodes.SingleOrDefault();

            if (string.IsNullOrEmpty(currencyCode))
            {
                throw new MultipleCurrencyNotSupportedException(currencyCodes);
            }

            return new Validation
            {
                Customer = await _customerVoucherifyMapper.GetCustomer(customerUri),
                Order = GetOrder(currencyCode, invoices)
            };
        }

        public async Task<Redeem> MapRedeem(ICollection<InvoiceInfo> invoices, Uri? customerUri)
        {
            var currencyCodes = invoices.Select(x => x.CurrencyCode).Distinct().ToList();
            var currencyCode = currencyCodes.SingleOrDefault();

            if (string.IsNullOrEmpty(currencyCode))
            {
                throw new MultipleCurrencyNotSupportedException(currencyCodes);
            }

            return new Redeem
            {
                Customer = await _customerVoucherifyMapper.GetCustomer(customerUri),
                Order = GetOrder(currencyCode, invoices)
            };
        }

        private Order GetOrder(string currencyCode, ICollection<InvoiceInfo> invoices) => new Order
        {
            SourceId = Guid.NewGuid().ToString(),
            Items = MapItems(invoices.SelectMany(x => x.ProductItems)).ToList(),
            Amount = invoices.Sum(x => x.ProductItems.Sum(y => y.Amount)).ToVoucherifyPrice(),
            Metadata = new Dictionary<string, object> { { MetadataKeys.CurrencyCode, currencyCode } }
        };

        private IEnumerable<Item> MapItems(IEnumerable<ProductItem> lineItems)
        {
            foreach (var productLineItem in lineItems)
            {
                yield return new Item
                {
                    Price = productLineItem.Amount.ToVoucherifyPrice(),
                    Quantity = 1.ToString(),
                    ProductId = productLineItem.ProductId,
                    Product = new ExternalServices.Voucherify.Contract.Product
                    {
                        Override = false,
                        Metadata = new Dictionary<string, object> {
                            { MetadataKeys.Vendor, productLineItem.Vendor },
                            { MetadataKeys.ProductType, productLineItem.ProductType },
                            { MetadataKeys.ValidityPeriod, productLineItem.ValidityPeriod },
                            { MetadataKeys.RailcardCode, productLineItem.RailcardCode },
                            { MetadataKeys.CarrierCode, productLineItem.CarrierCode },
                            { MetadataKeys.TicketTypeCode, productLineItem.TicketTypeCode }}
                    },
                };
            }
        }
    }
}
