using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Trainline.ProductJsonApiDeserialisation.AsyncModel;
using Trainline.ProductJsonApiDeserialisation.ProductService;
using Trainline.PromocodeService.Acceptance.Helpers.Steps;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.CustomerAttribute.Contract;
using Trainline.PromocodeService.ExternalServices.DiscountCard;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify.Extensions;
using Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Mappers;
using Trainline.PromocodeService.Service.Repository.Entities;
using Promocode = Trainline.PromocodeService.Service.Repository.Entities.Promocode;
using Redemption = Trainline.PromocodeService.Service.Repository.Entities.Redemption;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Steps
{
    public class InMemoryGivenSteps : GivenSteps
    {
        private readonly InMemoryVoucherifyClient _inMemoryVoucherifyClient;
        private readonly InMemoryPromocodeRepository _inMemoryPromocodeRepository;
        private readonly InMemoryCustomerServiceClient _inMemoryCustomerServiceClient;
        private readonly InMemoryCustomerAttributeClient _inMemoryCustomerAttributeClient;
        private readonly InMemoryRedemptionRepository _inMemoryRedemptionRepository;
        private readonly InMemoryCampaignRepository _inMemoryCampaignRepository;
        private readonly InMemorySupportedProtocolsService _inMemorySupportedProtocolsService;
        private readonly InMemoryDiscountCardService _inMemoryDiscountCardService;
        private readonly InMemoryCardTypeClient _inMemoryCardTypeClient;
        private readonly InMemoryTravelProductClient _inMemoryTravelProductClient;
        private readonly PromocodeMapper _mapper;

        public InMemoryGivenSteps(Fixture fixture, InMemoryVoucherifyClient inMemoryVoucherifyClient, InMemoryPromocodeRepository inMemoryPromocodeRepository,
            InMemoryCustomerServiceClient inMemoryCustomerServiceClient, InMemoryCustomerAttributeClient inMemoryCustomerAttributeClient,
            InMemoryCampaignRepository inMemoryCampaignRepository, InMemorySupportedProtocolsService inMemorySupportedProtocolsService,
            InMemoryDiscountCardService inMemoryDiscountCardService, InMemoryCardTypeClient inMemoryCardTypeClient,
            InMemoryTravelProductClient inMemoryTravelProductClient) : base(fixture)
        {
            _inMemoryVoucherifyClient = inMemoryVoucherifyClient;
            _inMemoryPromocodeRepository = inMemoryPromocodeRepository;
            _inMemoryCampaignRepository = inMemoryCampaignRepository;
            _inMemorySupportedProtocolsService = inMemorySupportedProtocolsService;
            _inMemoryCustomerServiceClient = inMemoryCustomerServiceClient;
            _inMemoryCustomerAttributeClient = inMemoryCustomerAttributeClient;
            _inMemoryDiscountCardService = inMemoryDiscountCardService;
            _inMemoryCardTypeClient = inMemoryCardTypeClient;
            _inMemoryTravelProductClient = inMemoryTravelProductClient;
            _mapper = new PromocodeMapper(new ValidationRuleMapper());
        }

        public ValidationContainer AValidationRule(string ruleId)
        {
            var validationContainer = new ValidationContainer
            {
                Id = ruleId,
                Name = "Fake Test Rule",
                Rules = new ValidationRulesContainer
                {
                    RuleDetails = new List<ValidationRuleDetails>
                    {
                        new ValidationRuleDetails
                        {
                            Name = "product.metadata",
                            Error = new ValidationError
                            {
                                Message = "VendorNotMatching_ATOC"
                            },
                            Rules = new ValidationRulesContainer
                            {
                                RuleDetails = new List<ValidationRuleDetails>
                                {
                                    new ValidationRuleDetails
                                    {
                                        Name = "product.metadata.aggregated_amount",
                                        Error = new ValidationError
                                        {
                                            Message = "OrderMinimumSpend_100"
                                        }
                                    }
                                }
                            }
                        },
                        new ValidationRuleDetails
                        {
                            Name = "order.metadata",
                            Error = new ValidationError
                            {
                                Message = "CurrencyCodeNotMatching_GBP"
                            }
                        }
                    }
                }
            };
            _inMemoryVoucherifyClient.AddValidationRules(validationContainer);

            return validationContainer;
        }

        public void ValidationWillFail(string voucherCode, Exception voucherifyClientException)
        {
            _inMemoryVoucherifyClient.SetupValidationFailure(voucherCode, voucherifyClientException);
        }

        public Voucher AVoucherWithCode(string code, string ruleId = "ruleId123", int? redeemedCount = null, int? redemptionCount = null,
            int? tierThreshold = null, decimal? tierDiscountAmount = null)
        {
            var voucher = _fixture.Create<Voucher>();
            voucher.Code = code;
            voucher.Discount.Type = DiscountTypeDefinitions.Amount;
            voucher.Metadata.Add(MetadataKeys.CurrencyCode, "GBP");
            voucher.Metadata.Add(MetadataKeys.ProductType, "travel");
            voucher.Metadata.Add(MetadataKeys.ValidityPeriod, "P1Y");
            voucher.Metadata.Add(MetadataKeys.RailcardCode, "urn:trainline:atoc:card:NEW");
            voucher.Metadata.Add(MetadataKeys.CarrierCode, "urn:trainline:atoc:carrier:LD");
            voucher.Metadata.Add(MetadataKeys.TicketTypeCode, "urn:trainline:atoc:fare:L9H");


            if (tierThreshold.HasValue && tierDiscountAmount.HasValue)
            {
                voucher.Metadata.Add(InvoiceGenerator.TiersEnableMetadataKey, "true");
                voucher.Metadata.Add(InvoiceGenerator.TierTwoThresholdMetadataKey, tierThreshold.ToString());
                voucher.Metadata.Add(InvoiceGenerator.TierTwoDiscountMetadataKey, tierDiscountAmount.ToString());
            }

            if (redeemedCount != null)
                voucher.Redemption.RedeemedQuantity = redeemedCount.Value;
            if (redemptionCount != null)
                voucher.Redemption.Quantity = redemptionCount;

            if (ruleId != null)
            {
                voucher.ValidationRulesAssignments = new ValidationRulesAssignments
                {
                    Object = "list",
                    Total = 1,
                    Data = new List<ValidationRulesAssignment>
                    {
                        new ValidationRulesAssignment
                        {
                            RuleId = ruleId
                        }
                    }
                };
            }
            else
            {
                voucher.ValidationRulesAssignments = null;
            }
            voucher.StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
            voucher.ExpirationDate = DateTime.UtcNow.Add(TimeSpan.FromDays(30));
            voucher.CampaignId = "testCampaign";
            voucher.Active = true;

            _inMemoryVoucherifyClient.AddVoucher(voucher);

            return voucher;
        }

        public Voucher AVoucherForRailcardWithCode(string code, string ruleId = "ruleId123", int? redeemedCount = null, int? redemptionCount = null)
        {
            var voucher = _fixture.Create<Voucher>();
            voucher.Code = code;
            voucher.Discount.Type = DiscountTypeDefinitions.Amount;
            voucher.Metadata.Add(MetadataKeys.CurrencyCode, "GBP");
            voucher.Metadata.Add(MetadataKeys.ProductType, "railcard");
            voucher.Metadata.Add(MetadataKeys.ValidityPeriod, "P1Y");
            voucher.Metadata.Add(MetadataKeys.RailcardCode, "urn:trainline:atoc:card:NEW");
            if (redeemedCount != null)
                voucher.Redemption.RedeemedQuantity = redeemedCount.Value;
            if (redemptionCount != null)
                voucher.Redemption.Quantity = redemptionCount;

            if (ruleId != null)
            {
                voucher.ValidationRulesAssignments = new ValidationRulesAssignments
                {
                    Object = "list",
                    Total = 1,
                    Data = new List<ValidationRulesAssignment>
                    {
                        new ValidationRulesAssignment
                        {
                            RuleId = ruleId
                        }
                    }
                };
            }
            else
            {
                voucher.ValidationRulesAssignments = null;
            }
            voucher.StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
            voucher.ExpirationDate = DateTime.UtcNow.Add(TimeSpan.FromDays(30));
            voucher.CampaignId = "testCampaign";
            voucher.Active = true;

            _inMemoryVoucherifyClient.AddVoucher(voucher);

            return voucher;
        }

        public DiscountCard ADiscountCard(string validityPeriod)
        {
            var discountCard = _fixture.Create<DiscountCard>();
            discountCard.Id = "testid";
            discountCard.CardDetails.ValidityEndDate = DateTime.UtcNow.Add(TimeSpan.FromDays(30));
            discountCard.CardDetails.ValidityStartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
            discountCard.CardDetails.ValidityPeriod = validityPeriod;
            discountCard.CardDetails.CardType.Id = "cardTypeId";
            discountCard.CardDetails.CardType.Url = "https://cm1-ukrailreferencedataapi.service.ttlnonprod.local/cardtype/62116b49-c298-413a-a8d9-c85d836bd8ee";

            return discountCard;
        }

        public Voucher AnInactiveVoucherWithCode(string code, string ruleId = "ruleId123", int? redeemedCount = null, int? redemptionCount = null)
        {
            var voucher = _fixture.Create<Voucher>();
            voucher.Code = code;
            voucher.Discount.Type = DiscountTypeDefinitions.Amount;
            voucher.Metadata.Add(MetadataKeys.CurrencyCode, "GBP");
            if (redeemedCount != null)
                voucher.Redemption.RedeemedQuantity = redeemedCount.Value;
            if (redemptionCount != null)
                voucher.Redemption.Quantity = redemptionCount;

            if (ruleId != null)
            {
                voucher.ValidationRulesAssignments = new ValidationRulesAssignments
                {
                    Object = "list",
                    Total = 1,
                    Data = new List<ValidationRulesAssignment>
                    {
                        new ValidationRulesAssignment
                        {
                            RuleId = ruleId
                        }
                    }
                };
            }
            else
            {
                voucher.ValidationRulesAssignments = null;
            }
            voucher.StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
            voucher.ExpirationDate = DateTime.UtcNow.Add(TimeSpan.FromDays(30));
            voucher.CampaignId = "testCampaign";
            voucher.Active = false;

            _inMemoryVoucherifyClient.AddVoucher(voucher);

            return voucher;
        }

        public Validated AValidatedWithCode(string code, IEnumerable<Contract.Item> applicableTo, decimal discountAmount, int? tierThreshold = null, decimal? tierDiscountAmount = null)
        {
            var validated = _fixture.Create<Validated>();
            validated.Code = code;
            validated.Valid = true;
            validated.ApplicableTo = new ApplicableTo
            {
                Data = applicableTo.Select(x => new ApplicableInfo
                {
                    Object = "product",
                    Id = Guid.NewGuid().ToString(),
                    SourceId = x.ProductId
                }).ToList()
            };
            validated.Discount = new ExternalServices.Voucherify.Contract.Discount
            {
                AmountOff = discountAmount.ToVoucherifyPrice(),
                Type = DiscountTypeDefinitions.Amount
            };

            if (tierThreshold.HasValue && tierDiscountAmount.HasValue)
            {
                validated.Metadata.Add(InvoiceGenerator.TiersEnableMetadataKey, "true");
                validated.Metadata.Add(InvoiceGenerator.TierTwoThresholdMetadataKey, tierThreshold.ToString());
                validated.Metadata.Add(InvoiceGenerator.TierTwoDiscountMetadataKey, tierDiscountAmount.ToString());
            }

            _inMemoryVoucherifyClient.AddValidated(validated);

            return validated;
        }

        public Validated AnInvaliddWithCode(string code, IEnumerable<Contract.Item> applicableTo, decimal discountAmount)
        {
            var validated = _fixture.Create<Validated>();
            validated.Code = code;
            validated.Valid = false;
            validated.ApplicableTo = new ApplicableTo
            {
                Data = applicableTo.Select(x => new ApplicableInfo
                {
                    Object = "product",
                    Id = Guid.NewGuid().ToString(),
                    SourceId = x.ProductId
                }).ToList()
            };
            validated.Discount = new ExternalServices.Voucherify.Contract.Discount
            {
                AmountOff = discountAmount.ToVoucherifyPrice(),
                Type = DiscountTypeDefinitions.Amount
            };
            validated.Reason = "redemption does not match validation rules";

            _inMemoryVoucherifyClient.AddValidated(validated);

            return validated;
        }

        public Validated ACodeInvalidForProductType(string code, IEnumerable<Contract.Item> applicableTo, decimal discountAmount)
        {
            var validated = _fixture.Create<Validated>();
            validated.Code = code;
            validated.Valid = false;
            validated.ApplicableTo = new ApplicableTo
            {
                Data = applicableTo.Select(x => new ApplicableInfo
                {
                    Object = "product",
                    Id = Guid.NewGuid().ToString(),
                    SourceId = x.ProductId
                }).ToList()
            };
            validated.Discount = new ExternalServices.Voucherify.Contract.Discount
            {
                AmountOff = discountAmount.ToVoucherifyPrice(),
                Type = DiscountTypeDefinitions.Amount
            };
            validated.Reason = "order does not match validation rules";
            validated.Error.Message = "ExcludedProductTypeMatched";

            _inMemoryVoucherifyClient.AddValidated(validated);

            return validated;
        }

        public Validated ACodeInvalidForSpecificRailcard(string code, IEnumerable<Contract.Item> applicableTo, decimal discountAmount)
        {
            var validated = _fixture.Create<Validated>();
            validated.Code = code;
            validated.Valid = false;
            validated.ApplicableTo = new ApplicableTo
            {
                Data = applicableTo.Select(x => new ApplicableInfo
                {
                    Object = "product",
                    Id = Guid.NewGuid().ToString(),
                    SourceId = x.ProductId
                }).ToList()
            };
            validated.Discount = new ExternalServices.Voucherify.Contract.Discount
            {
                AmountOff = discountAmount.ToVoucherifyPrice(),
                Type = DiscountTypeDefinitions.Amount
            };
            validated.Reason = "order does not match validation rules";
            validated.Error.Message = "ExcludedProductTypeMatched";

            _inMemoryVoucherifyClient.AddValidated(validated);

            return validated;
        }

        public Validated AVoucherNotValidForCarrierCode(string code, IEnumerable<Contract.Item> applicableTo, decimal discountAmount)
        {
            var validated = _fixture.Create<Validated>();
            validated.Code = code;
            validated.Valid = false;
            validated.ApplicableTo = new ApplicableTo
            {
                Data = applicableTo.Select(x => new ApplicableInfo
                {
                    Object = "product",
                    Id = Guid.NewGuid().ToString(),
                    SourceId = x.ProductId
                }).ToList()
            };
            validated.Discount = new ExternalServices.Voucherify.Contract.Discount
            {
                AmountOff = discountAmount.ToVoucherifyPrice(),
                Type = DiscountTypeDefinitions.Amount
            };
            validated.Reason = "order does not match validation rules";
            validated.Error.Message = "ExcludedProductTypeMatched";

            _inMemoryVoucherifyClient.AddValidated(validated);

            return validated;
        }

        public Validated AnInvalidWithInactiveCode(string code, IEnumerable<Contract.Item> applicableTo, decimal discountAmount)
        {
            var validated = _fixture.Create<Validated>();
            validated.Code = code;
            validated.Valid = false;
            validated.ApplicableTo = new ApplicableTo
            {
                Data = applicableTo.Select(x => new ApplicableInfo
                {
                    Object = "product",
                    Id = Guid.NewGuid().ToString(),
                    SourceId = x.ProductId
                }).ToList()
            };
            validated.Discount = new ExternalServices.Voucherify.Contract.Discount
            {
                AmountOff = discountAmount.ToVoucherifyPrice(),
                Type = DiscountTypeDefinitions.Amount
            };
            validated.Reason = "voucher is disabled";

            _inMemoryVoucherifyClient.AddValidated(validated);

            return validated;
        }

        public RedemptionRollback ARollback(string code, string redemptionId)
        {
            var redemptionRollback = _fixture.Create<RedemptionRollback>();
            redemptionRollback.Result = "SUCCESS";
            redemptionRollback.Voucher.Code = code;
            redemptionRollback.Voucher.Discount.Type = DiscountTypeDefinitions.Amount;
            redemptionRollback.Voucher.Metadata.Add(MetadataKeys.CurrencyCode, "GBP");
            redemptionRollback.Voucher.Metadata.Add(MetadataKeys.ProductType, "travel");

            _inMemoryVoucherifyClient.AddRollback(redemptionId, redemptionRollback);

            return redemptionRollback;
        }

        public async Task APromocode(Promocode code)
        {
            await _inMemoryPromocodeRepository.Add(code);
        }

        public async Task<Promocode> APromocodeFromVoucher(Voucher voucher)
        {
            var promocode = _mapper.Map(voucher);
            promocode.PromocodeId = Guid.NewGuid().ToString();
            await _inMemoryPromocodeRepository.Add(promocode);

            return promocode;
        }

        public async Task ARedemption(Redemption redemption)
        {
            await _inMemoryRedemptionRepository.Add(redemption);
        }

        public void AValidRedemptionForInvoices(string voucherCode, Invoice[] invoices)
        {
            _inMemoryVoucherifyClient.AddRedemptionFromInvoices(voucherCode, invoices);
        }

        public void AnInvalidRedemptionForInvoices(string voucherCode, Invoice[] invoices)
        {
            _inMemoryVoucherifyClient.AddInvalidRedemptionFromInvoices(voucherCode, invoices);
        }

        public void AnInvalidProductRedemptionForInvoices(string voucherCode, Invoice[] invoices)
        {
            _inMemoryVoucherifyClient.AddInvalidProductRedemptionFromInvoices(voucherCode, invoices);
        }

        public async Task<CampaignEntity> AnInvalidValidCampaign(string campaignId)
        {
            var campaign = new CampaignEntity{CampaignId = campaignId, Redeemable = false};
            await _inMemoryCampaignRepository.Add(campaign);

            return campaign;
        }

        public void ACustomerThatIsDefinedAsANewCustomer(Uri customerUri, ExternalServices.Customer.Contract.Customer customer)
        {
            var customerAttributes = new CustomerAttributes
            {
                CustomerId = customer.Id,
                Attributes = new CustomerAttributeDetails[]
                {
                    new CustomerAttributeDetails
                {
                    Name = "isNewCustomer",
                    DataType = "boolean",
                    Value = true,
                    LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                    ProvenanceSource = "c99.kronos.private.Smtm",
                    ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                }
                }
            };

            _inMemoryCustomerServiceClient.AddCustomer(customerUri, customer);
            _inMemoryCustomerAttributeClient.SetCustomerAttributes(customer.Id, customerAttributes.Attributes);
        }

        public void ACustomerThatIsDefinedAsANotNewCustomer(Uri customerUri, ExternalServices.Customer.Contract.Customer customer)
        {
            var customerAttributesDetails = new CustomerAttributeDetails[]
            {
                new CustomerAttributeDetails
                {
                    Name = "isNewCustomer",
                    DataType = "boolean",
                    Value = false,
                    LastUpdated = Convert.ToDateTime("2022-01-17T14:31:34.640Z"),
                    ProvenanceSource = "c99.kronos.private.Smtm",
                    ProvenanceId = "d7eaeb2d1c0106e4e9a60546158b8785"
                }
            };
            _inMemoryCustomerServiceClient.AddCustomer(customerUri, customer);
            _inMemoryCustomerAttributeClient.SetCustomerAttributes(customer.Id, customerAttributesDetails);
        }

        public void AProtocolForProductsInInvoice(Invoice[] invoices, params string[] protocols)
        {
            var productUri = invoices.SelectMany(x => x.ProductItems.Select(x => x.ProductUri));

            foreach (var pUri in productUri)
            {
                _inMemorySupportedProtocolsService.SetProductSupportedProtocols(pUri, protocols);
            }
        }

        public void ADiscountCardDetailsForRailcardInInvoice(Invoice[] invoices, DiscountCard discountCard)
        {
            var productUri = invoices.SelectMany(x => x.ProductItems.Select(x => x.ProductUri));

            foreach (var pUri in productUri)
            {
                _inMemoryDiscountCardService.SetDiscountCardDetails(pUri, discountCard);
            }
        }

        public void ADiscountCardCodeForRailcardInInvoice(Uri discountCardUri, string railcardCode)
        {
                _inMemoryCardTypeClient.SetCardTypeCodeAsync(discountCardUri, railcardCode);
        }

        internal void ATravelProtocolProduct(Uri productUri, string fareTypeCode = default, string carrierCode = default)
        {
            fareTypeCode ??= _fixture.Create<string>();
            carrierCode ??= _fixture.Create<string>();

            var travelProtocolProduct = new AsyncTravelProtocolProduct
            {
                Fares = Task.FromResult(new[]{new AsyncFare
                {
                    FareType = Task.FromResult(new FareType{Code = fareTypeCode}),
                    FareLegs = Task.FromResult(new[]{new AsyncFareLeg{Leg = Task.FromResult(new AsyncLeg{Carrier = Task.FromResult(new Carrier{Code = carrierCode})})}})
                }})
            };

            _inMemoryTravelProductClient.SetAsyncProduct(productUri, travelProtocolProduct);
        }
    }
}
