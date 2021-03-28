using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application.Database
{
    public class AadTokenInjectorDbInterceptor : DbConnectionInterceptor
    {
        private readonly AzureServiceTokenProvider _tokenProvider;

        public AadTokenInjectorDbInterceptor() => _tokenProvider = new AzureServiceTokenProvider();

        public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = default)
        {
            var sqlConnection = (SqlConnection)connection;
            sqlConnection.AccessToken = await GetAccessTokenAsync(cancellationToken);
            return result;
        }

        private Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
            => _tokenProvider.GetAccessTokenAsync("https://database.windows.net/", null, cancellationToken);
    }
}
