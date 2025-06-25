using Hutzper.Library.Common.Diagnostics;
using Xunit.Abstractions;

namespace Hutzper.Library.Common.TestHelper;

public abstract class TestBase
{
    public TestBase(ITestOutputHelper testOutputHelper)
    {
        UseSerilog.StartLogger(useFileLogger: false, output: testOutputHelper);
    }
}