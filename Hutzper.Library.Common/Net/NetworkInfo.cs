using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Hutzper.Library.Common.Net
{
    /// <summary>
    /// ネットワーク情報
    /// </summary>
    [Serializable]
    public static class NetworkInfo
    {
        /// <summary>
        /// PING
        /// </summary>
        /// <param name="hostNameOrAddress">ICMP エコー メッセージの送信先コンピュータを識別する String。このパラメータの値には、ホスト名または IP アドレスの文字列形式を指定できます</param>
        /// <returns>PING成功でtrue</returns>
        public static bool Ping(string hostNameOrAddress)
        {
            return NetworkInfo.Ping(hostNameOrAddress, -1);
        }

        /// <summary>
        /// PING
        /// </summary>
        /// <param name="hostNameOrAddress">ICMP エコー メッセージの送信先コンピュータを識別する String。このパラメータの値には、ホスト名または IP アドレスの文字列形式を指定できます</param>
        /// <param name="timeoutMilliseconds">エコー メッセージを送信してから ICMP エコー応答メッセージを待つ時間の最大値 (ミリ秒単位) を指定する </param>
        /// <returns>PING成功でtrue</returns>
        public static bool Ping(string hostNameOrAddress, int timeoutMilliseconds)
        {
            var isSuccess = false;

            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    try
                    {
                        PingReply reply;

                        if (0 > timeoutMilliseconds)
                        {
                            reply = ping.Send(hostNameOrAddress);
                        }
                        else
                        {
                            reply = ping.Send(hostNameOrAddress, timeoutMilliseconds);
                        }

                        if (System.Net.NetworkInformation.IPStatus.Success == reply.Status)
                        {
                            isSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return isSuccess;
        }
    }
}