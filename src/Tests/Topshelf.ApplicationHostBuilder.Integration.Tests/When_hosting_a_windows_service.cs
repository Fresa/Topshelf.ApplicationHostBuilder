using System.Collections.Generic;
using Shouldly;
using Test.It;
using Topshelf.ApplicationHostBuilder.Integration.Tests.Services.MessageSendingService;
using Topshelf.ApplicationHostBuilder.Integration.Tests.TestFramework;
using Xunit;

namespace Topshelf.ApplicationHostBuilder.Integration.Tests
{
    namespace Given_a_topshelf_hosted_windows_service
    {
        public class When_starting_and_stopping_the_service : 
            XUnitWindowsServiceSpecification<
                TopshelfHostedWindowsServiceBuilder<
                    MessageSendingService>>
        {
            private readonly List<string> _messagesFromService = new List<string>();

            protected override void Given(IServiceContainer configurer)
            {
                var communicator = new Communicator();
                communicator.On(s =>
                {
                    _messagesFromService.Add(s);
                    if (_messagesFromService.Count == 1)
                    {
                        ServiceController.Stop();
                    }
                });

                configurer.Register(() => communicator);
            }

            [Fact]
            public void It_should_have_sent_messages()
            {
                _messagesFromService.Count.ShouldBe(2);
            }

            [Fact]
            public void It_should_have_sent_start_message()
            {
                _messagesFromService.ShouldContain("Starting!");
            }

            [Fact]
            public void It_should_have_sent_stop_message()
            {
                _messagesFromService.ShouldContain("Stopping!");
            }
        }
    }
}
