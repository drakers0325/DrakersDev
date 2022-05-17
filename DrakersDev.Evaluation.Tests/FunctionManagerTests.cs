using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace DrakersDev.Evaluation.Tests
{
    public class FunctionManagerTests
    {
        [Fact]
        public void Can_Setup_Method_That_Returns_Double_Array_Static_Method_From_Type()
        {
            var funcManager = new FunctionManager();
            funcManager.SetupFunctionList(typeof(TestFunctionClass));
            var names = typeof(TestFunctionClass).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.ReturnType == typeof(Double[]))
                .Select(m => m.Name).Distinct().ToArray();
            funcManager.FunctionNames.Should().Equal(names);
        }
    }
}
