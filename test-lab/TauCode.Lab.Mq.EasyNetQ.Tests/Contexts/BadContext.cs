﻿using System;
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Bye.Sync;
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Sync;
using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Contexts
{
    public class BadContext : IMessageHandlerContext
    {
        private readonly bool _throwOnBegin;
        private readonly bool _throwOnEnd;

        private readonly bool _throwOnGetService;
        private readonly bool _returnNullOnGetService;
        private readonly bool _returnsWrongService;

        private readonly bool _throwOnDispose;

        public BadContext(
            bool throwOnBegin,
            bool throwOnEnd,
            bool throwOnGetService,
            bool returnNullOnGetService,
            bool returnsWrongService,
            bool throwOnDispose)
        {
            _throwOnBegin = throwOnBegin;
            _throwOnEnd = throwOnEnd;

            _throwOnGetService = throwOnGetService;
            _returnNullOnGetService = returnNullOnGetService;
            _returnsWrongService = returnsWrongService;

            _throwOnDispose = throwOnDispose;
        }

        public void Begin()
        {
            if (_throwOnBegin)
            {
                throw new NotSupportedException("Failed to begin.");
            }
        }

        public void End()
        {
            if (_throwOnEnd)
            {
                throw new NotSupportedException("Failed to end.");
            }
        }

        public object GetService(Type serviceType)
        {
            if (_throwOnGetService)
            {
                throw new NotSupportedException("Failed to get service.");
            }

            if (_returnNullOnGetService)
            {
                return null;
            }

            if (serviceType == typeof(HelloHandler))
            {
                if (_returnsWrongService)
                {
                    return new ByeHandler();
                }

                return new HelloHandler();
            }

            throw new NotSupportedException($"Service of type '{serviceType.FullName}' not supported.");
        }

        public void Dispose()
        {
            if (_throwOnDispose)
            {
                throw new NotSupportedException("Failed to dispose.");
            }
        }
    }
}