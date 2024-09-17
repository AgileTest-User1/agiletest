using Xunit;

namespace MathOperationsTests
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }
    }

    public class CalculatorTests  
    
    {
        private Calculator _calculator;

        public CalculatorTests()
        {
            _calculator = new Calculator();
        }

        [Fact]
        public void Add_ShouldReturnSum()
        {
            int a = 5;
            int b = 3;

            int result = _calculator.Add(a, b);

            Assert.Equal(8, result);
        }

        [Fact]
        public void Subtract_ShouldReturnDifference()
        {
            int a = 5;
            int b = 3;

            int result = _calculator.Subtract(a, b);

            Assert.Equal(2, result);
        }
    }
}