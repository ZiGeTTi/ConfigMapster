

using System;

public class BaseException : Exception
{
    protected BaseException(int code, string message)
    {
        Message = message;
        Code = code;
    }

    public int Code { get; }
    public override string Message { get; }
}