namespace Shared.Contracts
{
    public class RabbitMQSettings
    {
        public const string Worker_ContactInfoAddedEventQueue = "worker-contact-info-added-event-queue";
        public const string Worker_ContactInfoRemovedEventQueue = "worker-contact-info-removed-event-queue";
        public const string Worker_ContactDeletedEventQueue = "worker-contact-deleted-event-queue";
    }
}
