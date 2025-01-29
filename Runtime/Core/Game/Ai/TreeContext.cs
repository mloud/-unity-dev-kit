using System.Collections.Generic;

namespace OneDay.Core.Game.Ai
{
    public class TreeContext
    {
        private Dictionary<string, object> dataContext = new();
        
        public void Add(string key, object value)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext[key] = value;
                return;
            }
            dataContext.Add(key, value);
        }
        
        public void Remove(string key)
        {
            dataContext.Remove(key);
        }
        
        public T Get<T>(string key)
        {
            return (T)dataContext[key];
        }
    }
}