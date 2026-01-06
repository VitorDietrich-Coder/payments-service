using static Payments.Microservice.Domain.Core.NotificationModel;

namespace Payments.Microservice.Domain.Core
{
    public interface INotification
    {
        NotificationModel NotificationModel { get; }
        bool HasNotification { get; }
        void AddNotification(string key, string message, ENotificationType notificationType);

    }
}
