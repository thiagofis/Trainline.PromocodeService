using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Trainline.NetStandard.Exceptions.Contracts;
using Trainline.PromocodeService.Host.Exceptions;

namespace Trainline.PromocodeService.Host.ActionFilters
{
    public sealed class ContractValidationAttribute : ActionFilterAttribute
    {
        private const string IndexRegexPattern = @"\[.*\]";
        private static readonly Regex IndexRegex = new Regex(IndexRegexPattern, RegexOptions.Compiled);


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.Values.Contains(null))
                throw new ValidationServiceException("Either request is empty or is it is syntactically invalid");

            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationServiceException = new ValidationServiceException("Request is invalid or missing required properties");

            var errors = context.ModelState
                .SelectMany(m => m.Value.Errors.Select(e => MapToError(m, e))).Distinct();

            var hash = new HashSet<string>();

            foreach (var error in errors)
            {
                if (hash.Add(error.Detail))
                {
                    validationServiceException.Errors.Add(error);
                }
            }

            throw validationServiceException;
        }

        private static Error MapToError(KeyValuePair<string, ModelStateEntry> state, ModelError modelError)
        {
            var propertyName = "Request";

            if (state.Key.Contains("."))
            {
                propertyName =
                    state.Key.Substring(state.Key.IndexOf(".", StringComparison.InvariantCultureIgnoreCase) + 1)
                        .Replace(".", "");
            }


            if (IndexRegex.IsMatch(propertyName))
            {
                propertyName = IndexRegex.Replace(propertyName, string.Empty);
            }

            var errorMessage = string.IsNullOrWhiteSpace(modelError.ErrorMessage)
                ? modelError.Exception?.Message
                : modelError.ErrorMessage;

            return new Error(Severity.Correctable, "InvalidRequest", $"Invalid{propertyName}: {errorMessage}");
        }
    }
}
