using System.Text;
using static Hutzper.Library.Common.Net.ProtocolType;

namespace Hutzper.Library.Common.Net
{
    /// <summary>
    /// 通信プロトコル種類
    /// </summary>
    public enum ProtocolType
    {
        /// <summary>HTTP通信</summary>
        HTTP,

        /// <summary>HTTPS通信</summary>
        HTTPS,
    }

    /// <summary>
    /// REST API 通信クラス
    /// </summary>
    public class RestAPICom
    {
        /// <summary>APIのキー</summary>
        public string ApiKey { get; private set; }

        /// <summary>通信プロトコル種類</summary>
        public ProtocolType ProtocolTypeVal { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ApiKey">APIのキー</param>
        /// <param name="ProtocolType">通信プロトコル種類</param>
        public RestAPICom(string ApiKey, ProtocolType ProtocolTypeVal = HTTPS)
        {
            this.ApiKey = ApiKey;                 //  HTTPサーバーIPアドレス
            this.ProtocolTypeVal = ProtocolTypeVal;     //  通信プロトコル種類
        }

        /// <summary>
        /// GETコマンドを用いて 指定のURIのデータ取得
        /// </summary>
        /// <param name="strDir">ディレクトリ</param>
        /// <param name="strParams">パラメータ</param>
        /// <returns></returns>
        public async Task<string> GetCmd(List<string> strDirs, Dictionary<string, string>? strParams = null)
        {
            //  HTTPプロトコルのGETコマンドを用いて URLにアクセス
            try
            {
                string strUri = this.UriCreate(strDirs, strParams);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.ApiKey);

                    //  HTTPクライアントオブジェクト生成
                    return (await client.GetStringAsync(strUri));           //  GET通信
                }

            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// PUTコマンドを用いて JSON文字列を送信する
        /// </summary>
        /// <param name="strDirs">ディレクトリ</param>
        /// <param name="strParams">パラメータ</param>
        /// <param name="strPutData">送信データ</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutCmd(List<string> strDirs, Dictionary<string, string>? strParams = null, string strPutData = "")
        {
            //  HTTPプロトコルのPUTコマンドを用いて URLにアクセス
            try
            {
                string strUri = this.UriCreate(strDirs, strParams);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.ApiKey);

                    //  HTTPクライアントオブジェクト生成
                    var content = new StringContent(strPutData, Encoding.UTF8, @"application/json");

                    HttpResponseMessage response = await client.PutAsync(strUri, content);    //  PUT通信

                    return (response);
                }

            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// POSTコマンドを用いて JSON文字列を送信する
        /// </summary>
        /// <param name="strDirs">ディレクトリ</param>
        /// <param name="strParams">パラメータ</param>
        /// <param name="strPostData">送信データ</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostCmd(List<string> strDirs, Dictionary<string, string>? strParams = null, string strPostData = "")
        {
            //  HTTPプロトコルのPOSTコマンドを用いて URLにアクセス
            try
            {
                string strUri = this.UriCreate(strDirs, strParams);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.ApiKey);

                    //  HTTPクライアントオブジェクト生成
                    var content = new StringContent(strPostData, Encoding.UTF8, @"application/json");

                    HttpResponseMessage response = await client.PostAsync(strUri, content);       //  POST通信

                    return (response);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// URI文字列を生成する
        /// </summary>
        /// <param name="strDirs">ディレクトリ</param>
        /// <param name="strParams">パラメータ</param>
        /// <returns></returns>
        private string UriCreate(List<string> strDirs, Dictionary<string, string>? strParams = null)
        {
            //  URIディレクトリ文字列生成
            string DirVal = "";

            foreach (var str in strDirs)
            {
                DirVal += (str + "/");
            }

            //  パラメータ部文字列生成
            string ParamVal = "";

            if (strParams != null)
            {                      //  パラメータ指定がある場合
                ParamVal = "?";

                foreach (var paramElm in strParams)
                {
                    ParamVal += (paramElm.Key + "=" + paramElm.Value + "&");
                }

                ParamVal = ParamVal.TrimEnd('&');         //  末尾の & を取り除く
            }

            //  URI組み立て
            return (this.ProtocolTypeVal.ToString().ToLower() + "://" + DirVal + ParamVal);
        }
    }
}