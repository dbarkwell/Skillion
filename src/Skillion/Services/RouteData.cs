namespace Skillion.Services
{
    public sealed class RouteData
    {
        public RouteData(string controller, string action)
        {
            Controller = controller;
            Action = action;
        }

        public string Controller { get; }

        public string Action { get; }
    }
}