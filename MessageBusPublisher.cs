using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Salsa {

    /// <summary>
    /// A message bus publisher that can be connected to UGUI items to translate their events onto the MessageBus.
    /// </summary>
    public class MessageBusPublisher : MonoBehaviour {

        private Selectable source;

        [Tooltip("The type of message that is published to the message bus.")]
        public MessageType message;

        [Tooltip("The message that is sent, if and only if message is `CustomString`.")]
        public string customMessage;

        internal void Awake()
        {
            source = GetComponent<Selectable>();
            if(source == null) {
                Debug.LogWarning("No selectable on game object, MessageBusPublisher will not publish any messages.");
                return;
            }
            if(source is Button) {
                var button = source as Button;
                button.onClick.AddListener(new UnityAction(SelectableClicked));
            }
            
        }

        private void SelectableClicked()
        {
            switch(message) {
                case MessageType.CustomString:
                    MessageBus.Publish(source, "Clicked");
                    break;
                case MessageType.EventTypeAsEnum:
                    MessageBus.Publish(source, SelectableEvent.Clicked);
                    break;
                case MessageType.EventTypeAsString:
                    MessageBus.Publish(source, customMessage);
                    break;
            }
        }

        public enum MessageType {
            EventTypeAsString,
            EventTypeAsEnum,
            CustomString,
        }

        public enum SelectableEvent {
            Clicked,
        }

    }

}
