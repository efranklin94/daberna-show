using Hangfire;
using StackExchange.Redis;
using System;
using System.Linq;

public class ScoreResetService
{
    private readonly IDatabase _database;

    public ScoreResetService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    [AutomaticRetry(Attempts = 0)] // Disable automatic retries
    public void ResetScores()
    {
        try
        {
            var keys = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First())
                .Keys(pattern: "stats:*");

            foreach (var key in keys)
            {
                var stat = key.ToString().Split(':')[1];

                var userNames = _database.SortedSetRangeByRank("stats:" + stat, 0, -1);

                foreach (var userName in userNames)
                {
                    _database.SortedSetAdd("stats:" + stat, userName, 0);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw; 
        }
    }
}
