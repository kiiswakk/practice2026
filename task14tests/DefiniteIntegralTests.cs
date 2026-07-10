using System;
using Xunit;
using task14; 

namespace task14tests;
public class DefiniteIntegralTests
{
    private static readonly Func<double, double> X = (double x) => x;
    private static readonly Func<double, double> SIN = (double x) => Math.Sin(x);
    private static readonly Func<double, double> CONST = (double x) => 5.0;
    private static readonly Func<double, double> EXP = (double x) => Math.Exp(x);
    private static readonly Func<double, double> PARABOLA = (double x) => x * x;
    [Fact]
    public void Test_LinearSymmetric()
    {
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 1e-4);
    }
    [Fact]
    public void Test_SinSymmetric()
    {
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 1e-4);
    }
    [Fact]
    public void Test_LinearPositive()
    {
        Assert.Equal(10, DefiniteIntegral.Solve(0, 5, X, 1e-6, 8), 1e-5);
    }
    [Fact]
    public void Test_ConstantFunction()
    {
        Assert.Equal(25.0, DefiniteIntegral.Solve(-2, 3, CONST, 1e-4, 4), 1e-4);
    }
    [Fact]
    public void Test_QuadraticFunction()
    {
        Assert.Equal(9.0, DefiniteIntegral.Solve(0, 3, PARABOLA, 1e-5, 6), 1e-4);
    }
    [Fact]
    public void Test_ExponentFunction()
    {
        Assert.Equal(1.71828, DefiniteIntegral.Solve(0, 1, EXP, 1e-6, 5), 1e-4);
    }
}