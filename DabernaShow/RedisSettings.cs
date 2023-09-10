namespace DabernaShow
{
    using Microsoft.Extensions.Configuration;

    public class RedisSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }

        public static RedisSettings FromConfiguration(IConfiguration configuration)
        {
            var redisSettings = new RedisSettings();
            configuration.GetSection("Redis").Bind(redisSettings);
            return redisSettings;
        }
    }

}
