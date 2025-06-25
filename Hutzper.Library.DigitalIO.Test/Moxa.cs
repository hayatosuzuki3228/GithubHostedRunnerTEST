using Hutzper.Library.Common.TestHelper;
using Hutzper.Library.DigitalIO.Device.Moxa;
using Xunit.Abstractions;

namespace Hutzper.Library.DigitalIO.Test
{
    public class Moxa(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
    {
        private MoxaE1200DigitalIORemoteDevice InitializeMoxaDevice()
        {
            var moxaDevice = new MoxaE1200DigitalIORemoteDevice();
            var moxaParameter = new MoxaE1200DigitalIORemoteDeviceParameter(new Hutzper.Library.Common.Drawing.Point(0, 0));
            moxaParameter.OutputChannels = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            moxaParameter.InputChannels = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            moxaDevice.SetParameter(moxaParameter);
            return moxaDevice;
        }

        [Fact]
        [Trait("Category", "Moxa")]
        public void OpenAndCloseMoxaDevice()
        {
            var moxaDevice = this.InitializeMoxaDevice();
            Assert.True(moxaDevice.Open(), "Failed to open MOXA device.");
            Assert.True(moxaDevice.Close(), "Failed to close MOXA device.");
        }

        [Fact]
        [Trait("Category", "Moxa")]
        public void ReadAndWriteOutput()
        {
            var moxaDevice = this.InitializeMoxaDevice();
            moxaDevice.Open();
            bool writeValue = true;
            bool outputValue = false;
            Assert.True(moxaDevice.WriteOutput(0, writeValue), "Failed to write output.");
            Assert.True(moxaDevice.ReadOutput(0, out outputValue), "Failed to read output.");
            Assert.True(outputValue == writeValue, "Unexpected value.");
            moxaDevice.Close();
        }

        [Fact]
        [Trait("Category", "Moxa")]
        public void ReadInput()
        {
            var moxaDevice = this.InitializeMoxaDevice();
            moxaDevice.Open();
            bool inputValue = false;
            Assert.True(moxaDevice.ReadInput(0, out inputValue), "Failed to read input.");
            moxaDevice.Close();
        }
    }
}