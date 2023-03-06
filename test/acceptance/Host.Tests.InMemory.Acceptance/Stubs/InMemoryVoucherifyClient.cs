using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainline.PromocodeService.Contract;
using Trainline.PromocodeService.ExternalServices.Voucherify;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.Exceptions;
using Redeemed = Trainline.PromocodeService.ExternalServices.Voucherify.Contract.Redeemed;

namespace Trainline.PromocodeService.Host.Tests.InMemory.Acceptance.Stubs
{
    public class InMemoryVoucherifyClient : IVoucherifyClient
    {
        private readonly Dictionary<string, Voucher> _vouchers;
        private readonly Dictionary<string, ValidationContainer> _validationRules;
        private readonly Dictionary<string, Validated> _validated;
        private readonly Dictionary<string, RedemptionRollback> _rollbacks;
        private readonly Dictionary<string, Redeemed> _redemptions;
        private readonly Dictionary<string, FailedResponse> _failedResponse;
        private readonly List<ValidationRequest> _validationRequests;
        private readonly Dictionary<string, Exception> _validationsToFail = new Dictionary<string, Exception>();

        public InMemoryVoucherifyClient()
        {
            _vouchers = new Dictionary<string, Voucher>();
            _validationRules = new Dictionary<string, ValidationContainer>();
            _validated = new Dictionary<string, Validated>();
            _rollbacks = new Dictionary<string, RedemptionRollback>();
            _redemptions = new Dictionary<string, Redeemed>();
            _validationRequests = new List<ValidationRequest>();
            _failedResponse = new Dictionary<string, FailedResponse>();
        }

        public void AddVoucher(Voucher voucher)
        {
            _vouchers[voucher.Code] = voucher;
        }

        public void AddRedemptionFromInvoices(string voucherCode, Invoice[] invoices)
        {
            var voucher = _vouchers[voucherCode];

            var redeemed = new Redeemed
            {
                Id = Guid.NewGuid().ToString(),
                Result = "SUCCESS",
                Voucher = new RedeemedVoucher
                {
                    Code = voucher.Code,
                    Discount = voucher.Discount,
                    StartDate = voucher.StartDate,
                    ExpirationDate = voucher.ExpirationDate,
                    Redemption = voucher.Redemption,
                    Metadata = voucher.Metadata,
                    ValidationRulesAssignments = voucher.ValidationRulesAssignments,
                    ApplicableTo = new ApplicableTo
                    {
                        Data = invoices.SelectMany(x => x.ProductItems).Select(x => new ApplicableInfo
                        {
                            SourceId = x.ProductId
                        }).ToList()
                    }
                },
            };

            _redemptions[voucherCode] = redeemed;
        }
        public void AddInvalidRedemptionFromInvoices(string voucherCode, Invoice[] invoices)
        {
            var voucher = _vouchers[voucherCode];

            var failedResponse = new FailedResponse {Error = null, Key = "redemption_rules_violated"};

            _failedResponse[voucherCode] = failedResponse;
        }

        public void AddInvalidProductRedemptionFromInvoices(string voucherCode, Invoice[] invoices)
        {
            var voucher = _vouchers[voucherCode];

            var failedResponse = new FailedResponse { Error = new RedeemedError { Message = "ExcludedProductTypeMatched" }, Key = "order_rules_violated" };
            _failedResponse[voucherCode] = failedResponse;
        }

        public Task<Voucher> GetVoucher(string voucherCode)
            => Task.FromResult(_vouchers[voucherCode]);


        public void AddValidationRules(ValidationContainer validationContainer)
        {
            _validationRules[validationContainer.Id] = validationContainer;
        }

        public void SetupValidationFailure(string voucherCode, Exception exception)
        {
            _validationsToFail[voucherCode] = exception;
        }

        public Task<ValidationContainer> GetValidationRules(string validationRuleId)
            => Task.FromResult(validationRuleId != null &&_validationRules.ContainsKey(validationRuleId) ? _validationRules[validationRuleId]
                : new ValidationContainer()
                {
                    Rules = new ValidationRulesContainer()
                    {
                        RuleDetails = new List<ValidationRuleDetails>()
                    }
                });

        public void AddValidated(Validated validated)
        {
            _validated[validated.Code] = validated;
        }

        public void AddRollback(string redemptionId, RedemptionRollback redemptionRollback)
        {
            _rollbacks[redemptionId] = redemptionRollback;
        }

        public Task<Validated> ValidateVoucher(string voucherCode, Validation validation)
        {
            if (_validationsToFail.TryGetValue(voucherCode, out var exception))
            {
                throw exception;
            }

            _validationRequests.Add(new ValidationRequest(voucherCode, validation));
            return Task.FromResult(_validated[voucherCode]);
        }

        public Task<Redeemed> RedeemVoucher(string voucherCode, Redeem redeem)
        {
            var voucher = _vouchers[voucherCode];
            var redeemProductType = redeem.Order.Items.SelectMany(x => x.Product.Metadata.Where(x => x.Key == MetadataKeys.ProductType).Select(x => x.Value));

            if (voucher.Metadata.TryGetValue(MetadataKeys.ProductType, out var productType))
            {
                if (redeemProductType.Any(x => x != productType))
                {
                    throw new VoucherifyRedeemException(new RedeemedError { Message = "ExcludedProductTypeMatched_Travel" });
                }
            }

            if (_failedResponse.TryGetValue(voucherCode, out var failedResponse))
            {
                throw new RedemptionTotalLimitReachedException();
            }

            return Task.FromResult(_redemptions[voucherCode]);
        }

        public Task<RedemptionRollback> RollbackVoucher(string code, string redemptionId)
            => Task.FromResult(_rollbacks[redemptionId]);

        public IEnumerable<ValidationRequest> ValidationRequests => _validationRequests;
    }

    public class ValidationRequest
    {
        public ValidationRequest(string voucherCode, Validation validation)
        {
            VoucherCode = voucherCode;
            Validation = validation;
        }

        public string VoucherCode { get; }

        public Validation Validation { get; }
    }
}
