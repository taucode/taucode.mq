using Microsoft.Extensions.Logging;
using System;
using TauCode.Mq.EasyNetQ.IntegrationTests.Contexts;

namespace TauCode.Mq.EasyNetQ.IntegrationTests.ContextFactories
{
    public class BadContextFactory : IMessageHandlerContextFactory
    {
        private readonly ILogger _logger;

        private readonly bool _throwsOnCreateContext;
        private readonly bool _returnsNullOnCreateContext;

        private readonly bool _contextThrowsOnBegin;
        private readonly bool _contextThrowsOnEnd;

        private readonly bool _contextThrowsOnGetService;
        private readonly bool _contextReturnsNullOnGetService;
        private readonly bool _contextReturnsWrongServiceOnGetService;

        private readonly bool _contextThrowsOnDispose;

        public BadContextFactory(
            ILogger logger,
            bool throwsOnCreateContext,
            bool returnsNullOnCreateContext,
            bool contextThrowsOnBegin,
            bool contextThrowsOnEnd,
            bool contextThrowsOnGetService,
            bool contextReturnsNullOnGetService,
            bool contextReturnsWrongServiceOnGetService,
            bool contextThrowsOnDispose)
        {
            _logger = logger;

            _throwsOnCreateContext = throwsOnCreateContext;
            _returnsNullOnCreateContext = returnsNullOnCreateContext;

            _contextThrowsOnBegin = contextThrowsOnBegin;
            _contextThrowsOnEnd = contextThrowsOnEnd;

            _contextThrowsOnGetService = contextThrowsOnGetService;
            _contextReturnsNullOnGetService = contextReturnsNullOnGetService;
            _contextReturnsWrongServiceOnGetService = contextReturnsWrongServiceOnGetService;

            _contextThrowsOnDispose = contextThrowsOnDispose;
        }

        public IMessageHandlerContext CreateContext()
        {
            if (_throwsOnCreateContext)
            {
                throw new NotSupportedException("Failed to create context.");
            }

            if (_returnsNullOnCreateContext)
            {
                return null;
            }

            return new BadContext(
                _logger,
                _contextThrowsOnBegin,
                _contextThrowsOnEnd,
                _contextThrowsOnGetService,
                _contextReturnsNullOnGetService,
                _contextReturnsWrongServiceOnGetService,
                _contextThrowsOnDispose);
        }
    }
}
