using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.AI.Behaviour
{
    public class BehaviourBlackboard
    {
        public enum NotificationType
        {
            ADD,
            REMOVE,
            CHANGE
        }
        private struct Notification
        {
            public string key;
            public NotificationType type;
            public object value;
            public Notification(string key, NotificationType type, object value)
            {
                this.key = key;
                this.type = type;
                this.value = value;
            }
        }
        private Dictionary<string, object> m_data = new Dictionary<string, object>();
        private Dictionary<string, List<System.Action<NotificationType, object>>> observers = new Dictionary<string, List<System.Action<NotificationType, object>>>();
        private bool isNotifiyng = false;
        private Dictionary<string, List<System.Action<NotificationType, object>>> addObservers = new Dictionary<string, List<System.Action<NotificationType, object>>>();
        private Dictionary<string, List<System.Action<NotificationType, object>>> removeObservers = new Dictionary<string, List<System.Action<NotificationType, object>>>();
        private List<Notification> m_notifications = new List<Notification>();
        private List<Notification> m_notificationsDispatch = new List<Notification>();
        public BehaviourBlackboard Parent { get; private set; }
        private HashSet<BehaviourBlackboard> m_children = new HashSet<BehaviourBlackboard>();

        public void SetParent(BehaviourBlackboard parent)
        {
            Parent = parent;
            Parent?.AddChild(this);
        }

        public void AddChild(BehaviourBlackboard child)
        {
            if (!m_children.Contains(child))
            {
                m_children.Add(child);
                child.SetParent(this);
            } 
        }
        public void RemoveChild(BehaviourBlackboard child)
        {
            child.SetParent(null);
            m_children.Remove(child);
        }

        private void AddNotifications(List<Notification> notifications)
        {
            m_notifications.AddRange(notifications);
        }

        public bool IsSet(string key)
        {
            return this.m_data.ContainsKey(key) || (Parent != null && Parent.IsSet(key));
        }

        public void Set(string key, object value)
        {
            if (Parent != null && Parent.IsSet(key))
            {
                Parent.Set(key, value);
            }
            else
            {
                if (!this.m_data.ContainsKey(key))
                {
                    m_data[key] = value;
                    m_notifications.Add(new Notification(key, NotificationType.ADD, value));
                    // this.clock.AddTimer(0f, 0, NotifiyObservers);
                }
                else
                {
                    if ((this.m_data[key] == null && value != null) || (this.m_data[key] != null && !this.m_data[key].Equals(value)))
                    {
                        this.m_data[key] = value;
                        m_notifications.Add(new Notification(key, NotificationType.CHANGE, value));
                        // this.clock.AddTimer(0f, 0, NotifiyObservers);
                    }
                }
            }
        }

        public void Unset(string key)
        {
            if (m_data.ContainsKey(key))
            {
                m_data.Remove(key);
                m_notifications.Add(new Notification(key, NotificationType.REMOVE, null));
                // this.clock.AddTimer(0f, 0, NotifiyObservers);
            }
        }

        public object Get(string key)
        {
            if (m_data.ContainsKey(key))
            {
                return m_data[key];
            }
            else if (Parent != null)
            {
                return Parent.Get(key);
            }
            else
            {
                return null;
            }
        }

        public T Get<T>(string key)
        {
            var result = Get(key);
            if (result == null)
            {
                return default(T);
            }
            return (T)result;
        }
        private List<System.Action<NotificationType, object>> GetObserverList(Dictionary<string,
            List<System.Action<NotificationType, object>>> target, string key)
        {
            List<System.Action<NotificationType, object>> result;
            if (target.ContainsKey(key))
            {
                result = target[key];
            }
            else
            {
                result = new List<System.Action<NotificationType, object>>();
                target[key] = result;
            }
            return result;
        }

        public void AddObserver(string key, System.Action<NotificationType, object> observer)
        {
            List<System.Action<NotificationType, object>> observers = GetObserverList(this.observers, key);
            if (!isNotifiyng)
            {
                if (!observers.Contains(observer))
                {
                    observers.Add(observer);
                }
            }
            else
            {
                if (!observers.Contains(observer))
                {
                    List<System.Action<NotificationType, object>> addObservers = GetObserverList(this.addObservers, key);
                    if (!addObservers.Contains(observer))
                    {
                        addObservers.Add(observer);
                    }
                }

                List<System.Action<NotificationType, object>> removeObservers = GetObserverList(this.removeObservers, key);
                if (removeObservers.Contains(observer))
                {
                    removeObservers.Remove(observer);
                }
            }
        }

        public void Update()
        {
            NotifiyObservers();
        }

        private void NotifiyObservers()
        {
            if (m_notifications.Count == 0)
            {
                return;
            }

            m_notificationsDispatch.Clear();
            m_notificationsDispatch.AddRange(m_notifications);
            foreach (var child in m_children)
            {
                child.AddNotifications(m_notifications);//.m_notifications.AddRange(m_notifications);
            }
            m_notifications.Clear();

            isNotifiyng = true;
            foreach (Notification notification in m_notificationsDispatch)
            {
                if (!this.observers.ContainsKey(notification.key))
                {
                    //                Debug.Log("1 do not notify for key:" + notification.key + " value: " + notification.value);
                    continue;
                }

                List<System.Action<NotificationType, object>> observers = GetObserverList(this.observers, notification.key);
                foreach (System.Action<NotificationType, object> observer in observers)
                {
                    if (this.removeObservers.ContainsKey(notification.key) && this.removeObservers[notification.key].Contains(observer))
                    {
                        continue;
                    }
                    observer(notification.type, notification.value);
                }
            }

            foreach (string key in this.addObservers.Keys)
            {
                GetObserverList(this.observers, key).AddRange(this.addObservers[key]);
            }
            foreach (string key in this.removeObservers.Keys)
            {
                foreach (System.Action<NotificationType, object> action in removeObservers[key])
                {
                    GetObserverList(this.observers, key).Remove(action);
                }
            }
            this.addObservers.Clear();
            this.removeObservers.Clear();

            isNotifiyng = false;
        }

    }
}
