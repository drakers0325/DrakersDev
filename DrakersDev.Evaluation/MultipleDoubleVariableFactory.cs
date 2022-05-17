namespace DrakersDev.Evaluation
{
    public class MultipleDoubleVariableFactory : IVariableTokenFactory
    {
        public VariableToken CreateVariableToken(String variableName)
        {
            return new MultipleValueVariable<Double>(variableName);
        }
    }
}
