﻿using System;
using System.Threading.Tasks;
using TauCode.Mq.Abstractions;

// todo clean
namespace TauCode.Mq.Testing
{
    public static class TestMqMediaExtensions
    {
        public static void Publish<TMessage>(this ITestMqMedia media, TMessage message) where TMessage : IMessage
        {
            media.Publish(typeof(TMessage), message);
        }

        //public static void Publish<TMessage>(this ITestMqMedia media, TMessage message, string topic) where TMessage : IMessage
        //{
        //    media.Publish(typeof(TMessage), message, topic);
        //}

        public static IDisposable Subscribe<TMessage>(this ITestMqMedia media, Func<TMessage, Task> handler)
        {
            return media.Subscribe(typeof(TMessage), message => handler((TMessage)message));
        }

        public static IDisposable Subscribe<TMessage>(this ITestMqMedia media, Action<TMessage> handler)
        {
            return media.Subscribe(
                typeof(TMessage),
                message =>
                {
                    handler((TMessage)message);
                    return Task.CompletedTask;
                });
        }

        public static IDisposable Subscribe<TMessage>(this ITestMqMedia media, Func<TMessage, Task> handler, string topic)
        {
            return media.Subscribe(typeof(TMessage), message => handler((TMessage)message), topic);
        }

        public static IDisposable Subscribe<TMessage>(this ITestMqMedia media, Action<TMessage> handler, string topic)
        {
            return media.Subscribe(
                typeof(TMessage),
                message =>
                {
                    handler((TMessage)message);
                    return Task.CompletedTask;
                },
                topic);
        }
    }
}