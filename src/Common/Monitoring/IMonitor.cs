namespace Trainline.PromocodeService.Common.Monitoring
{
    public interface IMonitor
    {
        void AddCustomAttribute(string key, string value);
    }
}
