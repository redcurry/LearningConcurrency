using System.Threading;
using System.Threading.Tasks;

namespace Snippets.Tests.AsyncBasics
{
    public interface ISomeService
    {
        Task<int> ExecuteAsync();
        Task<int> ExecuteWithCancellationAsync(CancellationToken cancellationToken);
    }
}