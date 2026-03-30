namespace Toro.Testes.BuildingBlocks.Constants;

public static class ApplicationConstants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Investor = "Investor";
    }

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string InvestorAccess = "InvestorAccess";
    }

    public static class Claims
    {
        public const string CustomerId = "customer_id";
        public const string Email = "email";
    }

    public static class Messaging
    {
        public const string Exchange = "toro.investments";
        public const string OrderCreatedQueue = "investment-order-created";
        public const string OrderCreatedRoutingKey = "investment-order-created";
        public const string OrderProcessedRoutingKey = "investment-order-processed";
        public const string OrderFailedRoutingKey = "investment-order-failed";
        public const string DeadLetterQueue = "investment-order-created-dlq";
        public const string WorkerConsumerName = "Toro-Testes.Worker";
    }
}
