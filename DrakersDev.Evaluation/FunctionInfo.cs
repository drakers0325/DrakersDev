using System.Reflection;

namespace DrakersDev.Evaluation
{
    public class FunctionInfo
    {
        private readonly MethodInfo method;

        public String Name { get; private set; }
        private readonly Type[] argTypes;
        public Type[] ArgumentTypes => this.argTypes;

        public FunctionInfo(MethodInfo method)
        {
            this.method = method;
            this.Name = this.method.Name;
            this.argTypes = this.method.GetGenericArguments();
        }
    }
}