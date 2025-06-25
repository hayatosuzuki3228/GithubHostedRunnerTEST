using Hutzper.Library.Common.TestHelper;
using Hutzper.Library.DigitalIO.Device.Y2;
using Xunit.Abstractions;
using YdciCs;

namespace Hutzper.Library.DigitalIO.Test
{
    public class Y2(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
    {
        private Y2DigitalIODevice InitializeY2Device()
        {
            var y2Device = new Y2DigitalIODevice();
            var y2DeviceParameter = new Y2DigitalIODeviceParameter(new Hutzper.Library.Common.Drawing.Point(0, 0));
            y2DeviceParameter.DeviceID = "DIO-8/8B-UBT";
            y2DeviceParameter.OutputChannels = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            y2DeviceParameter.InputChannels = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            y2Device.SetParameter(y2DeviceParameter);
            return y2Device;
        }

        [Fact]
        public void OpenAndCloseY2Device()
        {
            var y2Device = this.InitializeY2Device();
            Assert.True(y2Device.Open(), "Failed to open Y2 device.");
            Assert.True(y2Device.Close(), "Failed to close Y2 device.");
        }

        [Fact]
        public void ReadAndWriteOutput()
        {
            var y2Device = this.InitializeY2Device();
            y2Device.Open();
            bool writeValue = true;
            bool outputValue = false;
            Assert.True(y2Device.WriteOutput(0, writeValue), "Failed to write output.");
            Assert.True(y2Device.ReadOutput(0, out outputValue), "Failed to read output.");
            Assert.True(outputValue == writeValue, "Unexpected value.");
            y2Device.Close();
        }

        [Fact]
        public void ReadInput()
        {
            var y2Device = this.InitializeY2Device();
            y2Device.Open();
            bool inputValue = false;
            Assert.True(y2Device.ReadInput(0, out inputValue), "Failed to read input.");
            y2Device.Close();
        }

        [Fact]
        public void YdciOnly()
        {
            int result;
            ushort id;
            byte[] inputData = new byte[8];
            byte[] outputData = new byte[8];
            int i;

            result = Ydci.Open(0, "DIO-8/8B-UBT", out id);
            Assert.True(result == Ydci.YDCI_RESULT_SUCCESS);

            // IN0～7の入力をおこないます
            result = Ydci.DioInput(id, inputData, 0, 8);
            Assert.True(result == Ydci.YDCI_RESULT_SUCCESS);

            // OUT0～7の出力をONします
            for (i = 0; i < 8; i++)
            {
                outputData[i] = 1;
            }
            result = Ydci.DioOutput(id, outputData, 0, 8);
            Assert.True(result == Ydci.YDCI_RESULT_SUCCESS);

            byte[] inputData2 = new byte[1];
            result = Ydci.DioInput(id, inputData2, 0, 1);
            Assert.True(result == Ydci.YDCI_RESULT_SUCCESS);

            Assert.True(inputData2[0] == 0);
            // ボードをクローズします
            Ydci.Close(id);
        }

    }
}