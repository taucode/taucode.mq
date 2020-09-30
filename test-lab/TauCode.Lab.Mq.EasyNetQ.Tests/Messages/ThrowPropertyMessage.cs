﻿using System;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests.Messages
{
    public class ThrowPropertyMessage : IMessage
    {
        private string _badProperty;

        public string BadProperty
        {
            get
            {
                if (_badProperty == "bad")
                {
                    throw new NotSupportedException("Property is bad!");
                }

                return _badProperty;
            }
            set => _badProperty = value;
        }

        public string CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}