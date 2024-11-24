using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PeculiarCardGame.Data;
using PeculiarCardGame.Services.Authentication;
using PeculiarCardGame.Services.DeckManagement;
using PeculiarCardGame.Services.Users;

namespace PeculiarCardGame.Tests
{
    internal class ApiTestsWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
        where TEntryPoint : class
    {
        private readonly IAuthenticationService? _authenticationService;
        private readonly IUsersService? _usersService;
        private readonly IDeckManagementService? _deckManagementService;

        public ApiTestsWebApplicationFactory(IAuthenticationService? authenticationService, IUsersService? usersService, IDeckManagementService? deckManagementService)
        {
            _authenticationService = authenticationService;
            _usersService = usersService;
            _deckManagementService = deckManagementService;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("ApiTests");
            builder.ConfigureServices(services =>
            {
                services.Remove(services.Single(x => x.ServiceType == typeof(PeculiarCardGameDbContext)));
                services.Remove(services.Single(x => x.ServiceType == typeof(IAuthenticationService)));
                services.Remove(services.Single(x => x.ServiceType == typeof(IUsersService)));
                services.Remove(services.Single(x => x.ServiceType == typeof(IDeckManagementService)));

                if (_authenticationService is not null)
                    services.AddScoped(_ => _authenticationService);
                if (_usersService is not null)
                    services.AddScoped(_ => _usersService);
                if (_deckManagementService is not null)
                    services.AddScoped(_ => _deckManagementService);
            });
        }
    }
}
