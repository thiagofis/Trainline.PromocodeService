using System;

namespace Trainline.PromocodeService.Service
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime DefaultRetentionDate => UtcNow.AddMonths(43);
    }
}
