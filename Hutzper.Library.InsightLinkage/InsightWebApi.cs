using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Hutzper.Library.InsightLinkage
{
    public class InsightWebApi
    {
        private delegate bool ParamValidator(string? param);

        private static readonly Dictionary<Type, (string endpointTemplate, ParamValidator validator)> TypeToEndpointMap = new Dictionary<Type, (string, ParamValidator)>
        {
            { typeof(ProjectClassNamesResponse), ("ai_projects/{0}/class_names", UuidValidator) },
            { typeof(ProjectListResponse), ("ai_projects", NoValidation) },
        };

        static public async Task<T?> GetApiResponse<T>(string user_token, string? param) where T : class
        {
            if (TypeToEndpointMap.TryGetValue(typeof(T), out var mapping))
            {
                // UUIDのバリデーション
                if (false == mapping.validator(param))
                {
                    // 不正なUUIDの場合はdefaultを返す
                    return default;
                }
            }
            else
            {
                // マッピングが見つからない場合
                return default;
            }

            // リクエストパラメータの作成
            string requestParameter = string.Format(mapping.endpointTemplate, param);

            // リクエスト
            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user_token);
            string requestString = $"https://hutzper-insight.com/api/v1/{requestParameter}";

            HttpResponseMessage response = await client.GetAsync(requestString);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                T? result = JsonConvert.DeserializeObject<T>(responseContent);
                return result;
            }
            return default;
        }

        public class ProjectClassNamesResponse
        {
            [JsonProperty("class_names")]
            public List<string> ProjectClassNames { get; set; } = new List<string>();
        }

        public class ProjectListResponse
        {
            [JsonProperty("")]
            public List<Project> Projects { get; set; } = new List<Project>();
        }

        private static bool UuidValidator(string? param)
        {
            return Guid.TryParse(param, out _);
        }

        private static bool NoValidation(string? param)
        {
            return true; // 常にtrueを返す
        }
    }
}