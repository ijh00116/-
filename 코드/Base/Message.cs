using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;


namespace BlackTree.Core
{
    public class Message
    {

        private static Dictionary<string, List<Delegate>> handlers = new Dictionary<string, List<Delegate>>();

        public static void AddListener<T>(string messageName, Action<T> callback) where T : Message
        {
            RegisterListener(typeof(T).ToString() + messageName, callback);
        }
        public static void AddListener<T>(Action<T> callback) where T : Message
        {
            RegisterListener(typeof(T).ToString(), callback);
        }

        public static void RemoveListener<T>(string messageName, Action<T> callback) where T : Message
        {
            UnRegisterListener(typeof(T).ToString() + messageName, callback);
        }
        public static void RemoveListener<T>(Action<T> callback) where T : Message
        {
            UnRegisterListener(typeof(T).ToString(), callback);
        }

        public static void Send<T>(string messageName, T e) where T : Message
        {
            SendMessage<T>(typeof(T).ToString() + messageName, e);
        }
        public static void Send<T>(T e) where T : Message
        {
            SendMessage<T>(typeof(T).ToString(), e);
        }

        private static void RegisterListener(string messageName, Delegate callback)
        {
            if (callback == null)
                return;
            if (!handlers.ContainsKey(messageName))
            {
                handlers.Add(messageName, new List<Delegate>());
            }

            List<Delegate> messagelst = handlers[messageName];
            Delegate ms = messagelst.Find(o => o.Method == callback.Method && o.Target == callback.Target);
            if (ms != null)
            {
                throw new ArgumentException("Callback method is already exist!!", messageName);
            }

            messagelst.Add(callback);
        }

        private static void UnRegisterListener(string messageName, Delegate callback)
        {
            if (callback == null)
                return;
            if (!handlers.ContainsKey(messageName))
                return;

            List<Delegate> messagelst = handlers[messageName];
            Delegate ms = messagelst.Find(o => o.Method == callback.Method && o.Target == callback.Target);
            if (ms == null)
                return;
            messagelst.Remove(ms);
        }

        private static void SendMessage<T>(string messageName, T e) where T : Message
        {
            if (!handlers.ContainsKey(messageName))
                return;

            List<Delegate> messagelst = handlers[messageName];

            for (int i = 0; i < messagelst.Count; i++)
            {
                if (messagelst[i].GetType() != typeof(Action<T>))
                    continue;

                var _event = (Action<T>)messagelst[i];
                _event(e);
            }
        }
    }

    #region messages
    public class KillDungeonEnemy:Message
    {

    }
    public class DungeonLevelUp:Message
    {
        public int currentLevel;
    }

    public class DungeonWaveUp : Message
    {
        public Model.DungeonType dungeonType;
    }

    #endregion
}

