
public class MongoRepoException : BaseException
{
    public MongoRepoException(int code = 500, string message = "Persistence: Mongo Repository Error") :
        base(code, message)
    {
    }
}
public class ExecuteException : BaseException
{
    public ExecuteException(int code, string? message) :
        base(code, string.Concat("Persistence: Dapper Execute Failed - ", message))

    {
    }
}

public class BulkAsyncException : BaseException
{
    public BulkAsyncException(int code, string? message) :
        base(code, string.Concat("Infrastructure: Elastic Bulking Error - ", message))

    {
    }
}