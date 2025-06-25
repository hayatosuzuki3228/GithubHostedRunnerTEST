using Hutzper.Library.Common.TestHelper;
using Hutzper.Library.DigitalIO.Device;
using Hutzper.Library.DigitalIO.Device.Plc.Mitsubishi;
using Xunit.Abstractions;

namespace Hutzper.Library.DigitalIO.Test
{
    public class MitsubishiPlcDeviceTest(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
    {
        #region Open/Close

        [Theory]
        [InlineData("192.168.3.250", 8000, true)]  // IPとPortが有効
        [InlineData("192.168.3.250", 0, false)]    // Portが無効
        [InlineData("1.2.3.4", 8000, false)]       // IPが無効
        public void OpenCloseTest(string ipAddress, int port, bool expectedSuccess)
        {
            // デバイスを作成
            IDigitalIODevice device = new MitsubishiPlcDevice();
            IDigitalIODeviceParameter parameter = new MitsubishiPlcDeviceParameter(new Hutzper.Library.Common.Drawing.Point(0, 0))
            {
                DeviceID = "FX5UC",
                OutputChannels = new int[] { 7 },
                IpAddress = ipAddress,
                PortNumber = port
            };

            device.SetParameter(parameter); // パラメータを設定

            // オープン
            var openResult = device.Open();

            // オープンの結果が期待値と一致するか検証
            Assert.Equal(expectedSuccess, openResult);

            if (true == openResult)
            {
                // オープンに成功した場合、クローズ
                var closeResult = device.Close();

                // クローズが成功したことを検証
                Assert.True(closeResult);
            }
        }

        #endregion

        #region WriteOutput

        private MitsubishiPlcDevice SetupDevice()
        {
            var device = new MitsubishiPlcDevice();
            var parameter = new MitsubishiPlcDeviceParameter(new Hutzper.Library.Common.Drawing.Point(0, 0))
            {
                DeviceID = "FX5UC",
                OutputChannels = new int[] { 7, 15 }, // Y7, Y15
                InputChannels = new int[] { 1, 2 }, // X1, X2
                IpAddress = "192.168.3.250",
                PortNumber = 8000
            };

            device.SetParameter(parameter);
            device.Open();
            return device;
        }

        [Fact]
        public void TestWriteOutputUnit_Success()
        {
            var device = this.SetupDevice();

            bool value = false;
            bool result = device.WriteOutput(7, value);
            device.Close();

            Assert.True(result);
        }

        [Fact]
        public void TestWriteOutputUnit_Fail()
        {
            var device = this.SetupDevice();

            int value = 1;
            bool result = device.WriteOutput(1000000, value); // 無効な値
            device.Close();

            Assert.False(result);
        }

        [Fact]
        public void TestWriteOutputAll_Success()
        {
            var device = this.SetupDevice();

            bool[] values = { true, true };
            bool result = device.WriteOutput(values);
            device.Close();

            Assert.True(result);
        }

        [Fact]
        public void TestWriteOutputAll_Fail()
        {
            var device = this.SetupDevice();

            int[] values = { 1, 2 }; // 無効な値を設定
            var result = device.WriteOutput(values);
            device.Close();

            Assert.False(result);
        }

        [Fact]
        public void TestReadOutputUnit_Success()
        {
            var device = this.SetupDevice();

            bool value;
            bool result = device.ReadOutput(7, out value);
            device.Close();

            Assert.True(result);
        }

        [Fact]
        public void TestReadOutputAll_Success()
        {
            var device = this.SetupDevice();

            bool[] values;
            bool result = device.ReadOutput(out values);
            device.Close();

            Assert.True(result);
        }

        [Fact]
        public void TestReadInputUnit_Success()
        {
            var device = this.SetupDevice();

            bool value;
            bool result = device.ReadInput(1, out value);
            device.Close();

            Assert.True(result);
        }

        [Fact]
        public void TestReadInputAll_Success()
        {
            var device = this.SetupDevice();

            bool[] values;
            bool result = device.ReadInput(out values);
            device.Close();

            Assert.True(result);
        }

        #endregion
    }
}