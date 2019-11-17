using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SkillionUnitTests.Helpers
{
    // https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.Core/src/Infrastructure/ActionMethodExecutor.cs
    internal abstract class TestSkillionActionMethodExecutor
    {
        public abstract ValueTask<IActionResult> Execute(
                    IActionResultTypeMapper mapper,
                    TestSkillionObjectMethodExecutor executor,
                    object controller,
                    object[] arguments);
        
        protected abstract bool CanExecute(TestSkillionObjectMethodExecutor executor);
        
        public static TestSkillionActionMethodExecutor GetExecutor(TestSkillionObjectMethodExecutor executor)
        {
            var testSkillionActionResultExecutor = new TestSkillionActionResultExecutor();
            if (testSkillionActionResultExecutor.CanExecute(executor))
            {
                return testSkillionActionResultExecutor;
            }
            
            Debug.Fail("Should not get here");
            throw new Exception();
        }
        
        private class TestSkillionActionResultExecutor : TestSkillionActionMethodExecutor
        {
            public override ValueTask<IActionResult> Execute(
                IActionResultTypeMapper mapper,
                TestSkillionObjectMethodExecutor executor,
                object controller,
                object[] arguments)
            {
                var returnValue = executor.Execute(controller, arguments);
                var actionResult = ConvertToActionResult(mapper, returnValue, executor.MethodReturnType);
                return new ValueTask<IActionResult>(actionResult);
            }

            protected override bool CanExecute(TestSkillionObjectMethodExecutor executor)
                => !executor.IsMethodAsync;
        }
        
        private IActionResult ConvertToActionResult(IActionResultTypeMapper mapper, object returnValue, Type declaredType)
        {
            var result = (returnValue as IActionResult) ?? mapper.Convert(returnValue, declaredType);
            if (result == null)
            {
                throw new InvalidOperationException();
            }

            return result;
        }
    }
}