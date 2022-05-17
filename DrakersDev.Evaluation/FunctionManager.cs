using System.Reflection;

namespace DrakersDev.Evaluation
{
    public class FunctionManager
    {
        private enum ArgumentValueType
        {
            Integer,
            IntegerArray,
            FloatingPoint,
            FloatingPointArray,
            Object,
            ObjectArray
        }

        private readonly Dictionary<String, List<MethodInfo>> methodMap = new();
        public String[] FunctionNames { get => this.methodMap.Keys.ToArray(); }

        public void SetupFunctionList(Type staticClassType)
        {
            foreach (var eachMethod in staticClassType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (eachMethod.ReturnType == typeof(Double[]))
                {
                    List<MethodInfo>? methodList;
                    if (!methodMap.TryGetValue(eachMethod.Name, out methodList))
                    {
                        methodList = new List<MethodInfo>();
                        methodMap[eachMethod.Name] = methodList;
                    }
                    methodList.Add(eachMethod);
                }
            }
        }

        internal MethodInfo? GetMethodInfo(FunctionToken token)
        {
            if (methodMap.TryGetValue(token.FunctionName, out var methodList))
            {
                foreach (var eachInfo in methodList)
                {
                    var methodArgTypes = eachInfo.GetParameters().Select(p => p.ParameterType).ToArray();
                    var funcArgs = token.Arguments.ToArray();
                    if (methodArgTypes.Length == funcArgs.Length)
                    {
                        Boolean isOk = true;
                        for (Int32 index = 0; index < funcArgs.Length; index++)
                        {
                            var tokenArg = funcArgs[index];
                            var funcArgType = methodArgTypes[index];
                            if (!IsAcceptableArgument(tokenArg.EvalType, funcArgType))
                            {
                                isOk = false;
                                break;
                            }
                        }
                        if (isOk)
                        {
                            return eachInfo;
                        }
                    }
                }
            }
            return null;
        }

        private static Boolean IsAcceptableArgument(Type tokenEvalType, Type funcArgType)
        {
            var tokenValueType = GetArgumentValueType(tokenEvalType);
            var funcValueType = GetArgumentValueType(funcArgType);
            if (tokenValueType != funcValueType)
            {
                return false;
            }

            return (tokenValueType != ArgumentValueType.Object && tokenValueType != ArgumentValueType.ObjectArray)
                || tokenEvalType == funcArgType;
        }

        private static ArgumentValueType GetArgumentValueType(Type type)
        {
            var elementType = type.IsArray ? type.GetElementType() : type;
            if (elementType == typeof(Byte) || elementType == typeof(Int16) ||
                elementType == typeof(Int32) || elementType == typeof(Int64))
            {
                return type.IsArray ? ArgumentValueType.IntegerArray : ArgumentValueType.Integer;
            }

            if (elementType == typeof(Single) || elementType == typeof(Double) || elementType == typeof(Decimal))
            {
                return type.IsArray ? ArgumentValueType.FloatingPointArray : ArgumentValueType.FloatingPoint;
            }
            return type.IsArray ? ArgumentValueType.ObjectArray : ArgumentValueType.Object;
        }
    }
}
