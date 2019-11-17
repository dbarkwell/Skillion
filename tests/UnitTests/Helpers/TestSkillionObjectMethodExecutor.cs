using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SkillionUnitTests.Helpers
{
    // https://github.com/aspnet/Extensions/blob/ff87989d893b000aac1bfef0157c92be1f04f714/shared/Microsoft.Extensions.ObjectMethodExecutor.Sources/ObjectMethodExecutor.cs
    internal class TestSkillionObjectMethodExecutor
    {
        private readonly object[] _parameterDefaultValues;
        private readonly MethodExecutor _executor;
        
        private TestSkillionObjectMethodExecutor(MethodInfo methodInfo, TypeInfo targetTypeInfo, object[] parameterDefaultValues)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            MethodInfo = methodInfo;
            MethodParameters = methodInfo.GetParameters();
            TargetTypeInfo = targetTypeInfo;
            MethodReturnType = methodInfo.ReturnType;
            
            // Upstream code may prefer to use the sync-executor even for async methods, because if it knows
            // that the result is a specific Task<T> where T is known, then it can directly cast to that type
            // and await it without the extra heap allocations involved in the _executorAsync code path.
            _executor = GetExecutor(methodInfo, targetTypeInfo);
            
            _parameterDefaultValues = parameterDefaultValues;
        }
        
        private delegate object MethodExecutor(object target, object[] parameters);

        private delegate void VoidMethodExecutor(object target, object[] parameters);

        public MethodInfo MethodInfo { get; }

        public ParameterInfo[] MethodParameters { get; }

        public TypeInfo TargetTypeInfo { get; }
        
        public bool IsMethodAsync { get; set; }
        
        // This field is made internal set because it is set in unit tests.
        public Type MethodReturnType { get; internal set; }
        
        public static TestSkillionObjectMethodExecutor Create(MethodInfo methodInfo, TypeInfo targetTypeInfo)
        {
            return new TestSkillionObjectMethodExecutor(methodInfo, targetTypeInfo, null);
        }
        
        /// <summary>
        /// Executes the configured method on <paramref name="target"/>. This can be used whether or not
        /// the configured method is asynchronous.
        /// </summary>
        /// <remarks>
        /// Even if the target method is asynchronous, it's desirable to invoke it using Execute rather than
        /// ExecuteAsync if you know at compile time what the return type is, because then you can directly
        /// "await" that value (via a cast), and then the generated code will be able to reference the
        /// resulting awaitable as a value-typed variable. If you use ExecuteAsync instead, the generated
        /// code will have to treat the resulting awaitable as a boxed object, because it doesn't know at
        /// compile time what type it would be.
        /// </remarks>
        /// <param name="target">The object whose method is to be executed.</param>
        /// <param name="parameters">Parameters to pass to the method.</param>
        /// <returns>The method return value.</returns>
        public object Execute(object target, object[] parameters)
        {
            return _executor(target, parameters);
        }
        
        private static MethodExecutor GetExecutor(MethodInfo methodInfo, TypeInfo targetTypeInfo)
        {
            // Parameters to executor
            var targetParameter = Expression.Parameter(typeof(object), "target");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            var parameters = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                var paramInfo = paramInfos[i];
                var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }

            // Call method
            var instanceCast = Expression.Convert(targetParameter, targetTypeInfo.AsType());
            var methodCall = Expression.Call(instanceCast, methodInfo, parameters);

            // methodCall is "((Ttarget) target) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                var lambda = Expression.Lambda<VoidMethodExecutor>(methodCall, targetParameter, parametersParameter);
                var voidExecutor = lambda.Compile();
                return WrapVoidMethod(voidExecutor);
            }
            else
            {
                // must coerce methodCall to match ActionExecutor signature
                var castMethodCall = Expression.Convert(methodCall, typeof(object));
                var lambda = Expression.Lambda<MethodExecutor>(castMethodCall, targetParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        private static MethodExecutor WrapVoidMethod(VoidMethodExecutor executor)
        {
            return delegate (object target, object[] parameters)
            {
                executor(target, parameters);
                return null;
            };
        }
    }
}