using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSimulation.Engine.Utilities
{
    public class DisplayMessages
    {

        private Dictionary<String, String> DataMessages = new Dictionary<String, String>();
        private List<String> LogMessages = new List<String>();

        private String DataMessagesString;
        private String LogMessagesString;
        private Vector2 LogMessageOffsetPosition;

        private int MaxLogMessage;

        private SpriteFont Font;

        public DisplayMessages (int maxLogMessages, SpriteFont font)
        {
            MaxLogMessage = maxLogMessages;
            LogMessageOffsetPosition = Vector2.Zero;
            Font = font;
        }

        public void AddLogMessage(string message)
        {
            LogMessages.Add(message);
            if (LogMessages.Count > MaxLogMessage)
            {
                LogMessages.RemoveAt(0);
            }
        }

        public void AddDataMessage (string key, string value)
        {
            if (DataMessages.ContainsKey(key))
            {
                DataMessages[key] = value;
            } else
            {
                DataMessages.Add(key, value);
            }
        }

        public void Update()
        {
            DataMessagesString = "Data Messages";
            foreach (String key in DataMessages.Keys)
            {
                DataMessagesString += $"\n [{key}] {DataMessages[key]}";
            }

            LogMessagesString = "Log Messages";
            foreach (String s in LogMessages)
            {
                LogMessagesString += $"\n {s}";
            }

            LogMessageOffsetPosition.Y = Font.MeasureString(DataMessagesString).Y;
        }

        public void Render(SpriteBatch spriteBatch, Vector2 fpsDisplayPosition, Color DataColor, Color LogColor)
        {
            spriteBatch.DrawString(Font, DataMessagesString, fpsDisplayPosition, DataColor);
            spriteBatch.DrawString(Font, LogMessagesString, fpsDisplayPosition + LogMessageOffsetPosition, LogColor);
        }
    }
}
