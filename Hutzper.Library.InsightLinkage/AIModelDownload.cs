using Newtonsoft.Json;
using System.Net;
using System.Web;

namespace Hutzper.Library.InsightLinkage
{
    static public class Request
    {
        static public async Task<List<Project>?> ProjectList(string user_token)
        {
            try
            {
                HttpClient client = new HttpClient();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://hutzper-insight.com/api/v1/ai_projects"),
                    Method = HttpMethod.Get,
                    Headers = {
                        { HttpRequestHeader.Authorization.ToString(),user_token },
                    },
                };
                HttpResponseMessage response = await client.SendAsync(request);
                string result = await response.Content.ReadAsStringAsync();
                List<Project>? data = null;
                if (!(result.Contains("アクセストークンが無効です")))
                {
                    data = JsonConvert.DeserializeObject<List<Project>>(result);
                }
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public async Task<List<Model>?> ModelList(string user_token, string ProjectID)
        {
            try
            {
                HttpClient client = new HttpClient();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://hutzper-insight.com/api/v1/ai_project_models?uuid={ProjectID}"),
                    Method = HttpMethod.Get,
                    Headers = {
                    { HttpRequestHeader.Authorization.ToString(),user_token },
                }
                };

                HttpResponseMessage response = await client.SendAsync(request);
                string result = await response.Content.ReadAsStringAsync();
                List<Model>? data = JsonConvert.DeserializeObject<List<Model>>(result);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public async Task<string> Download(string onnx_uuid, string filename, string user_token, string aiAlgorithm)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = TimeSpan.FromMinutes(15)
                };
                var builder = new UriBuilder($"https://hutzper-insight.com/api/v1/ai_project_models/{onnx_uuid}/onnx_presigned_url");
                var query = HttpUtility.ParseQueryString(builder.Query);
                if (aiAlgorithm == "セグメンテーション")
                {
                    query["model_type"] = "segmentation_model_image";
                }
                builder.Query = query.ToString();
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = builder.Uri,
                    Headers = { { HttpRequestHeader.Authorization.ToString(), user_token } },
                };

                HttpResponseMessage initialResponse = await client.SendAsync(request);

                if (initialResponse.IsSuccessStatusCode)
                {
                    string jsonResponse = await initialResponse.Content.ReadAsStringAsync();
                    var jsonParsed = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    string? downloadUri = jsonParsed?["presigned_url"].ToString();

                    HttpResponseMessage downloadResponse = await client.GetAsync(downloadUri);

                    if (downloadResponse.IsSuccessStatusCode)
                    {
                        byte[] bytes = await downloadResponse.Content.ReadAsByteArrayAsync();
                        string directoryPath = Path.GetDirectoryName(filename)!;
                        if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
                        await File.WriteAllBytesAsync(filename, bytes);
                        Console.WriteLine("ダウンロード成功");
                    }
                    else { Console.WriteLine("ダウンロード失敗: " + downloadResponse.StatusCode); }
                }
                else { Console.WriteLine("request失敗: " + initialResponse.StatusCode); }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error! ダウンロード中に問題が発生しました";
            }
            FileInfo fileInfo = new FileInfo(filename);
            long fileSize = fileInfo.Length;
            if (fileSize <= 1024)
            {
                File.Delete(fileInfo.FullName);
                return "Error! onnxファイルが見つかりません。Hutzper Insight上で作り直してください。";
            }
            return "ダウンロードが完了しました";
        }

        static public async Task<List<string>?> ClassNames(string user_token, string modelUuid)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new("Bearer", user_token);
            HttpResponseMessage response = await client.GetAsync($"https://hutzper-insight.com/api/v1/ai_project_models/{modelUuid}/class_names");

            if (response.IsSuccessStatusCode)
            {
                Dictionary<string, List<string>>? dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(await response.Content.ReadAsStringAsync());
                if (dictionary == null) return null;
                return dictionary?.GetValueOrDefault("class_names", new());
            }
            return null;
        }

        static public async Task<List<string>?> ProjectClassNames(string user_token, string ProjectUuid)
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new("Bearer", user_token);
            HttpResponseMessage response = await client.GetAsync($"https://hutzper-insight.com/api/v1/ai_projects/{ProjectUuid}/class_names");
            if (response.IsSuccessStatusCode)
            {
                Dictionary<string, List<string>>? dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(await response.Content.ReadAsStringAsync());
                return dictionary?.GetValueOrDefault("class_names", new());
            }
            return null;
        }
    }
}