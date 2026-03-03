using System.Threading.Tasks;

namespace WebsiteAIAssistant
{
    public interface IWebsiteAIAssistantCreateModelService
    {
        Task<bool> CreateModelAsync();
    }
}