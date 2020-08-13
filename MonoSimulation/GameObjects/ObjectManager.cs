using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSimulation.Engine.Events;
using MonoSimulation.Globals;
using MonoSimulator;
using MonoSimulator.GameObjects.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.GameObjects
{
    public class ObjectManager : ISubscriber
    {

        public List<GameObject> Objects;
        public List<GameObject> ObjectsToAdd;
        public List<GameObject> ObjectsToRemoveFromActive;
        public List<GameObject> ObjectsToRemoveFromGame;
        public List<GameObject> ObjectsSleeping;

        public Effect ObjectEffect;

        public ObjectManager ()
        {
            ObjectsToAdd = new List<GameObject>();

            Objects = new List<GameObject>();
            ObjectsToRemoveFromActive = new List<GameObject>();
            ObjectsSleeping = new List<GameObject>();
            ObjectsToRemoveFromGame = new List<GameObject>();
        }


        public void Update(GameTime time)
        {
            foreach (GameObject go in ObjectsToAdd)
            {
                Objects.Add(go);
            }
            ObjectsToAdd.Clear();

            foreach (GameObject go in ObjectsToRemoveFromActive)
            {
                Objects.Remove(go);
            }
            ObjectsToRemoveFromActive.Clear();

            foreach (GameObject go in ObjectsToRemoveFromGame)
            {
                if (!Objects.Remove(go))
                    ObjectsSleeping.Remove(go);
                NotifierBase.RemoveAllSubs(go);
            }
            ObjectsToRemoveFromGame.Clear();
            
            foreach (GameObject go in Objects)
            {
                go.Update(time);
            }

        }

        public void AddObject(GameObject obj)
        {
            obj.Subscribe(new OnDeath(null), this);
            obj.Subscribe(new OnSleep(null), this);
            obj.Subscribe(new OnWake(null), this);
            ObjectsToAdd.Add(obj);
        }

        public void Render(Effect effect, GraphicsDeviceManager graphics, SpriteBatch batch)
        {
            foreach (GameObject go in Objects)
            {
                go.Render(effect, graphics, batch);
            }

            foreach (GameObject go in ObjectsSleeping)
            {
                go.Render(effect, graphics, batch);
            }
        }

        public bool OnNotify(IGameEvent gameEvent)
        {
            var eventBase = gameEvent as EventBase;
            if (eventBase.EventCreater.GetType().IsSubclassOf(typeof(GameObject)))
            {
                GameObject go = (gameEvent as EventBase).EventCreater as GameObject;
                if (gameEvent.GetType().IsEquivalentTo(typeof(OnWake)))
                {
                    ObjectsSleeping.Remove(go);
                    ObjectsToAdd.Add(go);
                    return false;
                }
                else if (gameEvent.GetType().IsEquivalentTo(typeof(OnSleep)))
                {
                    ObjectsSleeping.Add(go);          
                    ObjectsToRemoveFromActive.Add(go);
                    return false;
                }
                else if (gameEvent.GetType().IsEquivalentTo(typeof(OnDeath)))
                {
                    ObjectsToRemoveFromGame.Add(go);
                    return false;
                }
            }
            return true;
        }
    }
}
