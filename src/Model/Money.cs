namespace Trainline.PromocodeService.Model
{
    public class Money
    {
        public decimal Amount { get; }

        public string CurrencyCode { get; }

        public Money(decimal amount, string currencyCode)
        {
            Amount = amount;
            CurrencyCode = currencyCode;
        }

        public override bool Equals(object obj)
            => obj is Money other
                && this.Amount == other.Amount
                && this.CurrencyCode == other.CurrencyCode;

        public override int GetHashCode()
            => (Amount, CurrencyCode).GetHashCode();

        public override string ToString()
            => $"{Amount}{CurrencyCode}";
    }
}
