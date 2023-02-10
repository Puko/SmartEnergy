namespace SmartEnergy.Extensions
{
    public static class AppExtenstions
    {
        public static T? GetResource<T>(this Application app, string key)
        {
            if (app.Resources.TryGetValue(key, out var resource))
                return (T)resource;

            foreach (var dict in app.Resources.MergedDictionaries)
            {
                if (dict.TryGetValue(key, out resource))
                {
                    return (T)resource;
                }
            }

            return default;
        }
    }
}
