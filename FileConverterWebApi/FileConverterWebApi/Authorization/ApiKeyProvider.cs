namespace FileConverterWebApi.Authorization
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        const string ApiKey = "DummyApiKeyForConvertor";
        public string GetApiKey() => ApiKey;
    }

    public interface IApiKeyProvider
    {
        public string GetApiKey();
    }
}