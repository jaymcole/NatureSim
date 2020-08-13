using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.GameObjects.BaseControls
{
    public interface INotifier
    {

        void Subscribe(IGameEvent gameEvent, ISubscriber sub);
        void UnSubscribe(IGameEvent gameEvent, ISubscriber sub, bool unsubscribeFromMaster = true);
        void Notify(IGameEvent gameEvent);

    }
}
