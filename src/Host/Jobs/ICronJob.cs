namespace Trainline.PromocodeService.Host.Jobs
{
    public interface ICronJob
    {
        public string Cron { get; }
        void Run();
    }
}
