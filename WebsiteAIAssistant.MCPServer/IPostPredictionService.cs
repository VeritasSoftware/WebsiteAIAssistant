using System.Threading.Tasks;
namespace WebsiteAIAssistant.MCPServer
{
    public interface IPostPredictionService
    {
        Task<object> HandlePredictionAsync(ModelInput input, Prediction prediction);
    }
}
