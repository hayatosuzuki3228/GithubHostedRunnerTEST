namespace Hutzper.Library.AaaTemplate;

public class Class1
{
    public float Divide(int a, int b)
    {
        if (b == 0)
        {
            return float.PositiveInfinity;
        }

        return (float)a / b;
    }

    public int Add(int a, int b)
    {
        return a + b;
    }
}
