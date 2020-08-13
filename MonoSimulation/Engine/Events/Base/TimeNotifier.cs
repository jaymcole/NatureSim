using Microsoft.Xna.Framework;
using MonoSimulator.GameObjects.BaseControls;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Events
{
    public class FastOnTimeNode : StablePriorityQueueNode
    {
        public OnTime TimeEvent;
    }


    public class TimeNotifier : INotifier
    {


        public StablePriorityQueue<FastOnTimeNode> TimeEvents { get; }
        public Stack<FastOnTimeNode> AvailableNodes { get; private set; }
        private static int MaxNodes = 100000;
        public TimeNotifier ()
        {
            AvailableNodes = new Stack<FastOnTimeNode>();
            for (int i = 0; i < MaxNodes; i++)
                AvailableNodes.Push(new FastOnTimeNode());
            TimeEvents = new StablePriorityQueue<FastOnTimeNode>(MaxNodes);
        }

        public void Update(GameTime time)
        {
            while (TimeEvents.Count > 0 && TimeEvents.First.TimeEvent.FireTime.TotalMilliseconds <= time.TotalGameTime.TotalMilliseconds)
            {
                var node = TimeEvents.Dequeue();
                Notify(node.TimeEvent);
                AvailableNodes.Push(node);
            }
        }

        public void Notify(IGameEvent gameEvent)
        {
            if (gameEvent.GetType().IsEquivalentTo(typeof(OnTime)) || gameEvent.GetType().IsSubclassOf(typeof(OnTime)))
                (gameEvent as OnTime).EventTarget.OnNotify(gameEvent);
        }

        public void Subscribe(IGameEvent gameEvent, ISubscriber sub)
        {
            if (gameEvent.GetType().IsEquivalentTo(typeof(OnTime)) || gameEvent.GetType().IsSubclassOf(typeof(OnTime)))
            {
                var nextNode = AvailableNodes.Pop();
                nextNode.TimeEvent = gameEvent as OnTime;
                TimeEvents.Enqueue(nextNode, (float)nextNode.TimeEvent.FireTime.TotalMilliseconds);
            } 
            else
            {
                Game1.messages.AddLogMessage($"[ERROR] Tried to subscribe to TimeNotifier with non-time based event. ({gameEvent.GetType()})");
            }
        }

        public void UnSubscribe(IGameEvent gameEvent, ISubscriber sub, bool unsubscribeFromMaster = true)
        {
            Game1.messages.AddLogMessage($"[ERROR] Cannot currently unsubscribe from time events.");
        }



        /*
        private SimplePriorityQueue<OnTime> TimeEvents;


        public TimeNotifier ()
        {
            TimeEvents = new SimplePriorityQueue<OnTime>();
        }

        public void Update(GameTime time)
        {
            if (time.TotalGameTime.TotalMilliseconds > 5000)
            { }
            while (TimeEvents.Count > 0 && TimeEvents.First.FireTime.TotalMilliseconds <= time.TotalGameTime.TotalMilliseconds)
            {
                Notify(TimeEvents.Dequeue());
            }
        }

        public void Notify(IGameEvent gameEvent)
        {
            if (gameEvent.GetType().IsEquivalentTo(typeof(OnTime)) || gameEvent.GetType().IsSubclassOf(typeof(OnTime)))
                (gameEvent as OnTime).EventTarget.OnNotify(gameEvent);
        }

        public void Subscribe(IGameEvent gameEvent, ISubscriber sub)
        {
            if (gameEvent.GetType().IsEquivalentTo(typeof(OnTime)) || gameEvent.GetType().IsSubclassOf(typeof(OnTime)))
            {
                var timeEvent = gameEvent as OnTime;
                TimeEvents.Enqueue(timeEvent, (float)timeEvent.FireTime.TotalMilliseconds);
            } 
            else
            {
                Game1.messages.AddLogMessage($"[ERROR] Tried to subscribe to TimeNotifier with non-time based event. ({gameEvent.GetType()})");
            }
        }

        public void UnSubscribe(IGameEvent gameEvent, ISubscriber sub, bool unsubscribeFromMaster = true)
        {
            Game1.messages.AddLogMessage($"[ERROR] Cannot currently unsubscribe from time events.");
        }
         */
    }
}
