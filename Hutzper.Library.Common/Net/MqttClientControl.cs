using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace Hutzper.Library.Common.Net
{
    public class MqttClientControl
    {
        #region メンバ変数

        private string clientId = string.Empty;

        private MqttClient? mqttClient = null;

        #endregion

        #region プロパティ

        public bool IsConnect
        {
            get
            {
                if (mqttClient == null)
                {
                    return false;
                }
                else
                {
                    return this.mqttClient.IsConnected;
                }

            }

        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MqttClientControl(string broker, string caname, string pfxname, string pfxpassword, int port = 8883)
        {
            try
            {
                var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mqtt");
                var caCert = X509Certificate.CreateFromCertFile(Path.Combine(folderPath, caname));
                var clientCert = new X509Certificate2(Path.Combine(folderPath, pfxname), pfxpassword);
                this.mqttClient = new MqttClient(broker, port, true, caCert, clientCert, MqttSslProtocols.TLSv1_2);
            }
            catch (Exception)
            {

            }
        }

        #endregion


        #region パブリックメソッド

        public bool Connect(string clientId)
        {
            if (this.mqttClient != null)
            {
                if (this.mqttClient.IsConnected == false)
                {
                    try
                    {
                        this.mqttClient?.Connect(clientId);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool DisConnect()
        {
            if (this.mqttClient != null)
            {
                if (this.mqttClient.IsConnected == true)
                {
                    try
                    {
                        this.mqttClient?.Disconnect();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Publish(string topic, string message)
        {
            this.mqttClient?.Publish(topic, Encoding.UTF8.GetBytes($"{message}"), 0, false);
        }

        #endregion
    }
}