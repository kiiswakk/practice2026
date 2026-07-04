using task11;
using Xunit;

namespace task11tests;

public class CalculatorTests
{
     private const string row = @"
        public class Calculator
        {
            public int Add(int a, int b) => a + b;
            public int Minus(int a, int b) => a - b;
            public int Mul(int a, int b) => a * b;
            public int Div(int a, int b) => a / b;
        }";
    private readonly ICalculator _calculator = Calculator.CreateCalculator(row);

    [Fact]
    public void AddTest()
    {
        Assert.Equal(13, _calculator.Add(6, 7));
    }

    [Fact]
    public void MinusTest()
    {
        Assert.Equal(50, _calculator.Minus(70, 20));
    }

    [Fact]
    public void MulTest()
    {
        Assert.Equal(225, _calculator.Mul(15, 15));
    }

    [Fact]
    public void DivTest()
    {
        Assert.Equal(4, _calculator.Div(20, 5));
    }
}
