﻿using System.Runtime.Serialization;

namespace MultiplayerDemo.Logic.Exceptions;

public class InvalidMoveException : Exception
{
    public InvalidMoveException()
    {
    }

    public InvalidMoveException(string? message) : base(message)
    {
    }

    public InvalidMoveException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected InvalidMoveException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
