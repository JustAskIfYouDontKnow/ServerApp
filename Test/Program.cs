namespace Test;

public static class IntExtension
{
    public static int Addition(this int num, int num2)
    {
        return num + 2;
    }
    public static int Multiply(this int num, int factor)
    {
        return num*factor;
    }
}

public static class Program
{
    private delegate int Calculator(int valueA, int valueB);

    static int Addition(this int x, int y) => x + y;
    static int Multiply(this int x, int y) => x * y;

    public static void Main()
    {
        Calculator calculator; 
        
        calculator = Multiply;
        var result = calculator(1, 2);
        Console.WriteLine(result);
        
        calculator += Addition;
        result = calculator(1, 2);
        Console.WriteLine(result);

    }
}