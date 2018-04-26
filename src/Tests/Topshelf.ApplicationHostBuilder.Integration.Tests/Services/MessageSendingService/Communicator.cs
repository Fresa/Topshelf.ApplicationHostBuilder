using System;
using System.Collections.Generic;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests.Services.MessageSendingService
{
    internal class Communicator
    {
        private readonly List<Action<string>> _subscriptions = new List<Action<string>>();

        public void On(Action<string> subscription)
        {
            _subscriptions.Add(subscription);
        }

        public void Send(string message)
        {
            foreach (var subscription in _subscriptions)
            {
                subscription(message);
            }
        }
    }
}