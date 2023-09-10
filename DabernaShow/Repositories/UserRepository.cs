using System;
using StackExchange.Redis;
using Newtonsoft.Json;
using DabernaShow.Models;
using Microsoft.AspNetCore.Mvc;
using DabernaShow;

public class UserRepository
{
    private readonly IDatabase _database;

    public UserRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public void UpdateOrCreateUser(User user)
    {
        var existingUserJson = _database.StringGet("user:" + user.Username);

        User existingUser;

        if (!string.IsNullOrEmpty(existingUserJson))
        {
            existingUser = JsonConvert.DeserializeObject<User>(existingUserJson);
        }
        else
        {
            existingUser = new User
            {
                Username = user.Username,
            };
        }

        existingUser.Stats = user.Stats;
        existingUser.Score = user.Score;

        var updatedUserJson = JsonConvert.SerializeObject(existingUser);

        string statValue = user.Stats?.ToString();
        _database.SortedSetAdd("stats:" + statValue, existingUser.Username, existingUser.Score);
    }

    public List<UserDTO> GetTopUsersByStats(string stat)
    {
        var topUsers = _database.SortedSetRangeByRankWithScores("stats:" + stat, 0, 19, Order.Descending);

        List<UserDTO> users = topUsers.Select(entry => new UserDTO
        {
            Score = entry.Score,
            Username = entry.Element
        }).ToList();

        return users;
    }



}
