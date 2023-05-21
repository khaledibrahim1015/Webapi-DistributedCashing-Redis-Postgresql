using StackExchange.Redis;
using System.Text.Json;

namespace DistributedCashingredisWebapi;

public class CacheService : ICacheService
{
    private readonly IDatabase _cacheDb;

    public CacheService(IConfiguration configuration)
    {
        //  Connect on Redis server 
        var redis = ConnectionMultiplexer.Connect(configuration.GetValue<string>("RedisSettings:IPANDPort"));
        // Connect on redis Database
        _cacheDb =redis.GetDatabase();
    }


    public T GetData<T>(string key)
    {
       
       var value =  _cacheDb.StringGet(key);

       if(!string.IsNullOrEmpty(value))
            return JsonSerializer.Deserialize<T>(value);
        return default;

    }

    public object RemoveData(string key)
    {
        // First check if key exist 
        var keyExist  = _cacheDb.KeyExists(key);

        if(keyExist)
            _cacheDb.KeyDelete(key);
        
        return false;



    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
       var expiryTime =  expirationTime.DateTime.Subtract(DateTime.Now);

       var isSet = _cacheDb.StringSet(key , JsonSerializer.Serialize<T>(value),expiryTime);
       if(isSet)
            return true;

        return false;

    }
}