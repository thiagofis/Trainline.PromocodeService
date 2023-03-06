namespace Trainline.PromocodeService.Model
{
    public class ValidationRule
    {
        public ValidationRule(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public string Value { get; }
    }
}
