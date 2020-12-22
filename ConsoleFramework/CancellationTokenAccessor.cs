using System.Threading;

namespace ConsoleFramework
{
    public class CancellationTokenAccessor
    {
        public CancellationToken CancellationToken { get; private set; }

        internal void SetCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }
    }
}