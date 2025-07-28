

public class PersistenceConfig
{
    public RedisConfig RedisConfig { get; set; }
    public MongoDbSettings MongoDbSettings { get; set; }
}
public class RedisConfig
{
    public string Url { get; set; }       
    public string Password { get; set; }       
    public int Database { get; set; } 
    public int ExpireTimeSpan { get; set; }
}