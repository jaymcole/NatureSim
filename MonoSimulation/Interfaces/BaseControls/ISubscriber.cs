﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MonoSimulator.GameObjects.BaseControls
{
    public interface ISubscriber
    {
        bool OnNotify(IGameEvent gameEvent);
    }
}
