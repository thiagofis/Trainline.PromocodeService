using System;

namespace Trainline.PromocodeService.Service
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }

        DateTime DefaultRetentionDate { get; }
    }
}
