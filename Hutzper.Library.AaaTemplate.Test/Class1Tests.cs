using Hutzper.Library.Common.TestHelper;
using Xunit.Abstractions;

namespace Hutzper.Library.AaaTemplate.Test;

public class Class1Tests(ITestOutputHelper testOutputHelper) : TestBase(testOutputHelper)
{
    [Fact(DisplayName = "Divide関数の正常系のテスト")]

    public void TestDivide1()
    {
        // Arrange
        var class1 = new Class1();

        // Act
        var result = class1.Divide(1, 2);

        // Assert
        Assert.Equal(0.5, result);
    }

    [Fact(DisplayName = "0で割ったときのテスト")]
    public void TestDivide2()
    {
        // Arrange
        var class1 = new Class1();

        // Act
        var result = class1.Divide(1, 0);

        // Assert
        Assert.Equal(float.PositiveInfinity, result);
    }

    [Fact(DisplayName = "Add関数の正常系のテスト")]

    public void TestAdd1()
    {
        // Arrange
        var class1 = new Class1();

        // Act
        var result = class1.Add(1, 2);

        // Assert
        Assert.Equal(3, result);
    }

}
