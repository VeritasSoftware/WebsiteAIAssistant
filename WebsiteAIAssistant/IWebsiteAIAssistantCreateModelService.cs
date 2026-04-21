using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public interface IWebsiteAIAssistantCreateModelService
    {
        Task<bool> CreateModelAsync();
        Task<bool> CreateModelAsync<TModelInput>()
            where TModelInput : ModelInput, new();
    }
}