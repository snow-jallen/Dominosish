using System.Runtime.Serialization;

namespace MultiplayerDemo.Logic.Exceptions
{
    [Serializable]
    internal class GameFullException : Exception
    {
        public GameFullException()
        {
        }

        public GameFullException(string? message) : base(message)
        {
        }

        public GameFullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected GameFullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}