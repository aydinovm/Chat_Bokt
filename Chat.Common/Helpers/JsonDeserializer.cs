namespace Chat.Common.Helpers
{
    public class JsonDeserializer
    {
        private readonly JsonSerializer _jsonSerializer;
        public JsonDeserializer()
        {
            _jsonSerializer = new JsonSerializer { ContractResolver = JsonSerializerSettings.ContractResolver };
        }

        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public async Task<T> TryDeserializeAsync<T>(Stream streamContent, CancellationToken cancellationToken)
              => await DeserializeAsync<T>(streamContent, cancellationToken);

        public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var loaded = await JToken.LoadAsync(jsonReader, cancellationToken);
                return loaded.ToObject<T>(_jsonSerializer);
            }
        }

    }
}
