using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Hutzper.Library.Common.Net
{
    public class JsonSerializer
    {
        /// <summary>
        /// クラスオブジェクトをJSON文字列に変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classObj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T classObj)
        {
            try
            {
                string? Obj = System.Text.Json.JsonSerializer.Serialize<T>(classObj);

                return Obj;

            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// JSON文字列をクラスオブジェクトに変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static T? DeSerialize<T>(string response)
        {
            try
            {
                var Obj = System.Text.Json.JsonSerializer.Deserialize<T>(response);

                return Obj;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// JSON文字列をリストオブジェクトに変換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static List<T> DeSerializeToList<T>(string response)
        {
            try
            {
                var listObj = System.Text.Json.JsonSerializer.Deserialize<List<T>>(response);

                return listObj ?? new List<T>();

            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// 通常用
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <returns></returns>
        public static DataContractJsonSerializer Serializer<T>() => new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings
        {
            DateTimeFormat = new DateTimeFormat("yyyy-MM-dd hh:mm:ss"),
        });

        /// <summary>
        /// Listオブジェクト用
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <returns></returns>
        public static DataContractJsonSerializer SerializerList<T>() => new DataContractJsonSerializer(typeof(List<T>), new DataContractJsonSerializerSettings
        {
            DateTimeFormat = new DateTimeFormat("yyyy-MM-dd hh:mm:ss"),
        });

        /// <summary>
        /// Dictionaryオブジェクト用
        /// </summary>
        /// <typeparam name="TKey">任意の型</typeparam>
        /// <typeparam name="TVal">任意の型</typeparam>
        /// <returns></returns>
        private static DataContractJsonSerializer SerializerDictionary<TKey, TVal>() where TKey : notnull
        {
            return new DataContractJsonSerializer(typeof(Dictionary<TKey, TVal>), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd hh:mm:ss"),
            });
        }
    }
}