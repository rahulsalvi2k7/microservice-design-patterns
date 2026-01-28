namespace ServiceRegistry
{
    public class RegistryDictionary
    {
        private readonly Dictionary<string, string> _registry = [];

        public void Register(ServiceRegistrationRequest serviceRegistrationRequest)
        {
            _registry[serviceRegistrationRequest.Name] = serviceRegistrationRequest.Location;
        }

        public void Unregister(string key)
        {
            _registry.Remove(key);
        }

        public string Get(string key)
        {
            return _registry[key];
        }
    }
}
