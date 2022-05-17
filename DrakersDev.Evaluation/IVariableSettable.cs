namespace DrakersDev.Evaluation
{
    public interface IVariableSettable
    {
        void SetVariableValue<T>(String variableName, T value);
        void SetVariableValues<T>(String variableName, T[] values);
        String[] GetVariableNames();

    }
}
