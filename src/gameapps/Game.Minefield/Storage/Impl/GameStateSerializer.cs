using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Game.Minefield.Contracts.Model;

namespace Game.Minefield.Storage.Impl
{
    public static class GameStateSerializer
    {
        public static string Serialize(State state)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
                stream.Flush();
                stream.Position = 0L;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static State Deserialize(string data)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(data)))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0L, SeekOrigin.Begin);
                var state = formatter.Deserialize(stream) as State;
                return state;
            }
        }
    }
}