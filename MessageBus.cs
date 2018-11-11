using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Salsa {

    /// <summary>
    /// A simple message bus to implement publish/subscribe based on a message type 
    /// </summary>
    public sealed class MessageBus {

        /// <summary>
        /// Prevent instantiation.
        /// </summary>
        private MessageBus() { }

        /// <summary>
        /// Subscribe to all messages of type `TMessage` that are sent to the bus.
        /// </summary>
        public static void Subscribe<TMessage>(object subscriber, Action<object, TMessage> callback)
        {
            Subscribe(subscriber, null, callback);
        }

        /// <summary>
        /// Subscribe to messages of type `TMessage` that are sent to the bus filtered by only those sent by objects of type `senderFilter`.
        /// </summary>
        public static void Subscribe<TMessage>(object subscriber, Type senderFilter, Action<object, TMessage> callback)
        {
            var info = FindOrCreateInfo(typeof(TMessage), senderFilter);
            info.subscribers.Add(new SubscriberInfo { subscriber = subscriber, callback = (s, m) => callback(s, (TMessage)m) });
        }

        /// <summary>
        /// Publish a message to the bus, sender should be `this`.
        /// </summary>
        public static void Publish<TMessage>(object sender, TMessage message)
        {
            var infoAll = FindOrCreateInfo(typeof(TMessage), null);
            PublishImmediate(sender, infoAll.subscribers, message);
            if(sender != null) {
                var infoSpecific = FindOrCreateInfo(typeof(TMessage), sender.GetType());
                PublishImmediate(sender, infoSpecific.subscribers, message);
            }
        }

        private static void PublishImmediate<TMessage>(object sender, IList<SubscriberInfo> subscribers, TMessage message)
        {
            for(int i = 0; i < subscribers.Count(); ++i) {
                var action = subscribers[i];
                action.callback(sender, message);
            }
        }

        /// <summary>
        /// Unsubscribe the specific subscriber from all messages.
        /// </summary>
        public static void Unsubscribe(object subscriber)
        {
            foreach(var subscription in subscriptions.Values) {
                var toDelete = subscription.subscribers.Where(e => e.subscriber == subscriber).ToList();
                foreach(var existingSubscriber in toDelete) {
                    subscription.subscribers.Remove(existingSubscriber);
                }
            }
        }

        /// <summary>
        /// Unsubscribe the specific `subscriber` from all messages of type `TMessage`.
        /// </summary>
        public static void Unsubscribe<TMessage>(object subscriber)
        {
            var subscriptionsForType = subscriptions.Values.Where(e => e.messageType == typeof(TMessage));
            foreach(var subscriptionInfo in subscriptionsForType) {
                var allSubscribers = subscriptionInfo.subscribers;
                var subscriptions = allSubscribers.Where(e => e.subscriber == subscriber);
                var toDelete = subscriptions.ToList();
                foreach(var subscription in toDelete) {
                    allSubscribers.Remove(subscription);
                }
            }
        }

        /// <summary>
        /// Unsubscribe the specific `subscriber` from messages of type `TMessage` that are sent from senders of type `senderType`.
        /// </summary>
        public static void Unsubscribe<TMessage>(object subscriber, Type senderType)
        {
            var info = FindOrCreateInfo(typeof(TMessage), senderType);
            var toDelete = info.subscribers.Where(e => e.subscriber == subscriber).ToList();
            foreach(var delete in toDelete) {
                info.subscribers.Remove(delete);
            }
            if(!info.subscribers.Any()) {
                var key = CalculateKey(typeof(TMessage), senderType);
                subscriptions.Remove(key);
            }
        }

        /// <summary>
        /// Unsubscribe the specific `subscriber` from _all_ messages that are sent from senders of type `senderType`.
        /// </summary>
        public static void Unsubscribe(object subscriber, Type senderType)
        {
            var subscriptionsForType = subscriptions.Values.Where(e => e.senderType == senderType);
            foreach(var subscriptionInfo in subscriptionsForType) {
                var allSubscribers = subscriptionInfo.subscribers;
                var subscriptions = allSubscribers.Where(e => e.subscriber == subscriber);
                var toDelete = subscriptions.ToList();
                foreach(var subscription in toDelete) {
                    allSubscribers.Remove(subscription);
                }
            }
        }

        #region Implementation Details

        private static SubscriptionInfo FindOrCreateInfo(Type messageType, Type senderType)
        {
            var key = CalculateKey(messageType, senderType);
            if(subscriptions.ContainsKey(key)) {
                return subscriptions[key];
            }
            else {
                var info = new SubscriptionInfo { messageType = messageType, senderType = senderType };
                subscriptions.Add(key, info);
                return info;
            }
        }

        private static string CalculateKey(Type senderType, Type messageType)
        {
            if(messageType == null) {
                return senderType.FullName;
            }
            else {
                return $"{senderType.FullName}::{messageType.FullName}";
            }
        }

        private static Dictionary<string, SubscriptionInfo> subscriptions = new Dictionary<string, SubscriptionInfo>();

        /// <summary>
        /// Inner class that represents a single type of subscription, contains a list of a specific subscriptions.
        /// </summary>
        private class SubscriptionInfo {

            /// <summary>
            /// The type of message that defines this subscription
            /// </summary>
            public Type messageType;

            /// <summary>
            /// The type of the sender that defines this subscription.
            /// </summary>
            public Type senderType;

            /// <summary>
            ///  The list of subscribers and callbacks that are the actual subscriptions.
            /// </summary>
            public List<SubscriberInfo> subscribers = new List<SubscriberInfo>();
        }

        /// <summary>
        /// Inner class that represents a single subscription.
        /// </summary>
        private class SubscriberInfo {
            /// <summary>
            /// The specific object that is subscribing, retained so that Unsubscribe can work.
            /// </summary>
            public object subscriber;

            /// <summary>
            /// The action callback, with parameters (sender, message).
            /// Note, the user sees the message as strongly typed but we store it weakly typed.
            /// </summary>
            public Action<object, object> callback;
        }

    }

    #endregion

}
