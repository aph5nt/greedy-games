using Newtonsoft.Json;

namespace WebApi.Infrastructure.Validation
{
    #region

    #endregion

    public class ValidationError
    {
        public ValidationError(string name, string description)
        {
            this.Name = name != string.Empty ? name : null;
            this.Description = description;
        }

        public string Description { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; }
    }
}