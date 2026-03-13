using System.Reflection;

namespace WebsiteAIAssistant.Tests
{
    public class LoadModelBeforeTestAttribute : BeforeAsyncAfterSyncTestAttribute
    {
        public LoadModelBeforeTestAttribute(Type specificAttributeType, string stamp) : base(specificAttributeType, stamp)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            // Clean up resources after the test, if necessary
        }
    }

    public class SetModelPathBeforeTestAttribute : BeforeAsyncAfterSyncTestAttribute
    {
        public SetModelPathBeforeTestAttribute(Type specificAttributeType, string stamp) : base(specificAttributeType, stamp)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            // Clean up resources after the test, if necessary
        }
    }

    public class BuildLoadPredictDIContainerAttribute : BeforeAsyncAfterSyncTestAttribute
    {
        public BuildLoadPredictDIContainerAttribute(Type specificAttribute, Type returnFunctionClassType,
                                                    string returnFunctionName, string stamp)
                                                    : base(specificAttribute, returnFunctionClassType, returnFunctionName, stamp)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            // Clean up resources after the test, if necessary
        }
    }

    public class BuildCreateModelDIContainerAttribute : BeforeAsyncAfterSyncTestAttribute
    {
        public BuildCreateModelDIContainerAttribute(Type specificAttribute, Type returnFunctionClassType,
                                                    string returnFunctionName, string stamp)
                                                    : base(specificAttribute, returnFunctionClassType, returnFunctionName, stamp)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            // Clean up resources after the test, if necessary
        }
    }
}
