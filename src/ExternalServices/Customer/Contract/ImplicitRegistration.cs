using Trainline.PromocodeService.Common.Formatting;

namespace Trainline.PromocodeService.ExternalServices.Customer.Contract
{
    public class ImplicitRegistration
    {
        public ImplicitRegistration(string emailAddress, EntryParameter entryParameter)
        {
            EmailAddress = emailAddress;
            EntryParameter = entryParameter;
        }

        public string EmailAddress { get; }

        public EntryParameter EntryParameter { get; }

        public int ContactId => 0;

        public override string ToString()
        {
            return $"{nameof(EmailAddress)}: {EmailAddress.MaskEmail()}, {nameof(EntryParameter)}: {EntryParameter}, {nameof(ContactId)}: {ContactId}";
        }
    }
}
