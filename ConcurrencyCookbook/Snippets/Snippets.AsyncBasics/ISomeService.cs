using System.Threading;
using System.Threading.Tasks;

namespace Snippets.AsyncBasics
{
    public interface ISomeService
    {
        Task<int> ExecuteAsync();
        Task<int> ExecuteWithCancellationAsync(CancellationToken cancellationToken);
    }
}