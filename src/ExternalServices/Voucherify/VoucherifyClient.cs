using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Trainline.PromocodeService.Common.Exceptions;
using Trainline.PromocodeService.ExternalServices.Extensions;
using Trainline.PromocodeService.ExternalServices.Http.Constants;
using Trainline.PromocodeService.ExternalServices.Voucherify.Contract;

namespace Trainline.PromocodeService.ExternalServices.Voucherify
{
    public class VoucherifyClient : IVoucherifyClient
    {
        private const string RedeemQuantityExceeded = "quantity_exceeded";
        private const string AlreadyRolledBack = "already_rolled_back";
        private const string RedemptionTotalLimitReached = "redemption_rules_violated";
        private const string CustomerNotNew = "customer_rules_violated";
        private const string VoucherExpired = "voucher_expired";
        private readonly HttpClient _httpClient;
        private readonly VoucherifySettings _settings;
        private readonly JsonSerializerSettings _serializerSettings;

        public VoucherifyClient(HttpClient httpClient, IOptions<VoucherifySettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public async Task<Voucher> GetVoucher(string voucherCode)
        {
            var uri = new Uri($"{_settings.BaseUri}/vouchers/{voucherCode}");

            using var request = CreateRequest(HttpMethod.Get, uri);
            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new VoucherifyPromocodeNotFoundException($"{voucherCode} is not a valid promocode");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new VoucherifyException(await response.Content.ReadAsAsync<Error>(_serializerSettings));
            }

            return await response.Content.ReadAsAsync<Voucher>(_serializerSettings);
        }

        public async Task<ValidationContainer> GetValidationRules(string validationRuleId)
        {
            var uri = new Uri($"{_settings.BaseUri}/validation-rules/{validationRuleId}");
            using var request = CreateRequest(HttpMethod.Get, uri);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new VoucherifyException(await response.Content.ReadAsAsync<Error>(_serializerSettings));
            }
            return await response.Content.ReadAsAsync<ValidationContainer>(_serializerSettings);

        }

        public async Task<Validated> ValidateVoucher(string voucherCode, Validation validation)
        {
            var uri = new Uri($"{_settings.BaseUri}/vouchers/{voucherCode}/validate");

            using var request = CreateRequest(HttpMethod.Post, uri);
            string json = JsonConvert.SerializeObject(validation, _serializerSettings);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new VoucherifyException(await response.Content.ReadAsAsync<Error>(_serializerSettings));
            }

            var validated = await response.Content.ReadAsAsync<Validated>(_serializerSettings);
            validated.EnsureSuccess();
            return validated;
        }

        public async Task<Redeemed> RedeemVoucher(string voucherCode, Redeem redeem)
        {
            var uri = new Uri($"{_settings.BaseUri}/vouchers/{voucherCode}/redemption");

            using var request = CreateRequest(HttpMethod.Post, uri);
            string json = JsonConvert.SerializeObject(redeem, _serializerSettings);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<FailedResponse>(_serializerSettings);

                if (error.Key == RedeemQuantityExceeded)
                {
                    throw new QuantityExceededException();
                }

                if (error.Key == RedemptionTotalLimitReached)
                {
                    throw new RedemptionTotalLimitReachedException();
                }

                if (error.Key == CustomerNotNew)
                {
                    throw new CustomerNotNewException("Customer is not new.");
                }

                if (error.Key == VoucherExpired)
                {
                    throw new PromocodeExpiredException("Promocode has expired.");
                }

                throw new VoucherifyRedeemException(error.Error);
            }
            
            var redeemed =  await response.Content.ReadAsAsync<Redeemed>(_serializerSettings);
            redeemed.EnsureSuccess();
            return redeemed;

        }

        public async Task<RedemptionRollback> RollbackVoucher(string code, string redemptionId)
        {
            var uri = new Uri($"{_settings.BaseUri}/redemptions/{redemptionId}/rollback");

            using var request = CreateRequest(HttpMethod.Post, uri);
            request.Content = new StringContent(string.Empty, Encoding.UTF8, MediaTypes.ApplicationJson);
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsAsync<Error>(_serializerSettings);
                if (error.Key == AlreadyRolledBack)
                {
                    throw new AlreadyRolledBackException();
                }
                throw new VoucherifyException(error);
            }

            return await response.Content.ReadAsAsync<RedemptionRollback>(_serializerSettings);
        }

        private HttpRequestMessage CreateRequest(HttpMethod httpMethod, Uri requestUri)
        {
            var request = new HttpRequestMessage(httpMethod, requestUri);
            request.Headers.Add(Headers.AppId, _settings.AppId);
            request.Headers.Add(Headers.AppToken, _settings.AppToken);
            request.Headers.Add(Headers.Accept, MediaTypes.ApplicationJson);

            return request;
        }

        //TODO - Move to root Headers class
        public static class Headers
        {
            public const string AppId = "X-App-Id";
            public const string AppToken = "X-App-Token";
            public const string Accept = "Accept";
        }
    }
}
