﻿using MonoSimulator.GameObjects.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class NotifierBase : INotifier
    {
        public static void RemoveAllSubs(ISubscriber sub)
        {
            if (AllSubs.ContainsKey(sub))
            {
                foreach (IGameEvent key in AllSubs[sub].Keys)
                    AllSubs[sub][key].UnSubscribe(key, sub, false);
                AllSubs.Remove(sub);
            }
        }

        private void AddToMasterSubList(IGameEvent gameEvent, ISubscriber sub)
        {
            if (AllSubs.ContainsKey(sub))
            {
                if (!AllSubs[sub].ContainsKey(gameEvent))
                {
                    AllSubs[sub][gameEvent] = this;
                    TotalSubScriptions += 1;
                }
            }
            else
            {
                Dictionary < IGameEvent, INotifier > dict = new Dictionary<IGameEvent, INotifier>();
                dict[gameEvent] = this;
                AllSubs[sub] = dict;
                TotalSubScriptions += 1;
            }
        }

        private static void RemoveFromMasterSubList(INotifier notifier, ISubscriber sub, IGameEvent gameEvent)
        {
            if (AllSubs.ContainsKey(sub))
            {
                AllSubs[sub].Remove(gameEvent);
                TotalSubScriptions -= 1;
            }
        }

        public static int TotalSubScribers { 
            get { return AllSubs.Count; }
        }

        public static int TotalSubScriptions { get; private set; }

        private static Dictionary<ISubscriber, Dictionary<IGameEvent, INotifier>> AllSubs = new Dictionary<ISubscriber, Dictionary<IGameEvent, INotifier>>();
        private Dictionary<Type, List<ISubscriber>> Subscribed;
        
        public NotifierBase ()
        {
            Subscribed = new Dictionary<Type, List <ISubscriber>>();
            PendingRemoval = new List<(IGameEvent gameEvent, ISubscriber subscriber)>();
        }
            
        public void Subscribe(IGameEvent gameEvent, ISubscriber sub)
        {
            if (Subscribed.ContainsKey(gameEvent.GetType())) {
                List<ISubscriber> CurrentSubs = Subscribed[gameEvent.GetType()];
                if (!CurrentSubs.Contains(sub))
                {
                    CurrentSubs.Add(sub);
                    AddToMasterSubList(gameEvent, sub);
                }
            }
            else
            {
                List<ISubscriber> subList = new List<ISubscriber>();
                subList.Add(sub);
                Subscribed[gameEvent.GetType()] = subList;
                AddToMasterSubList(gameEvent, sub);
            }
            
        }

        public void UnSubscribe(IGameEvent gameEvent, ISubscriber sub, bool unsubscribeFromMaster = true)
        {
            if (IsBusyNotifying)
                PendingRemoval.Add((gameEvent, sub));
            else
            {
                if (Subscribed.ContainsKey(gameEvent.GetType()))
                {
                    Subscribed[gameEvent.GetType()].Remove(sub);
                    if (unsubscribeFromMaster)
                        RemoveFromMasterSubList(this, sub, gameEvent);
                }
            }
        }

        private void UnsubscribePending()
        {
            foreach ((IGameEvent gameEvent, ISubscriber subscriber) remove in PendingRemoval)
            {
                UnSubscribe(remove.gameEvent, remove.subscriber);
            }
        }

        private bool IsBusyNotifying = false;
        private List<(IGameEvent gameEvent, ISubscriber subscriber)> PendingRemoval;
        public void Notify(IGameEvent gameEvent)
        {
            try
            {
                IsBusyNotifying = true;
                if (Subscribed.ContainsKey(gameEvent.GetType()))
                {
                    foreach (ISubscriber sub in Subscribed[gameEvent.GetType()])
                    {
                        
                        if (sub.OnNotify(gameEvent))
                            UnSubscribe(gameEvent, sub);
                    }
                }

            } 
            catch (Exception e)
            {
                Game1.messages.AddLogMessage($"[ERROR] Failed while notifying subscribers");
                Game1.messages.AddLogMessage($"[ERROR]{e.Message}");
            } finally
            {
                IsBusyNotifying = false;
                UnsubscribePending();
            }
        }
    }
}