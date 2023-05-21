namespace DistributedCashingredisWebapi;

public interface ICacheService
{
   T GetData<T>(string key );
    bool SetData<T>(string key , T value , DateTimeOffset expiryTime );

    object RemoveData (string key ); 
}