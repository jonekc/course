using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Projekt.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Projekt.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<string> Login(AuthenticateRequest request)
        {
            HttpResponseMessage post = await _httpClient.PostAsJsonAsync("login", request);
            AuthenticateResponse response = await post.Content.ReadFromJsonAsync<AuthenticateResponse>();
            if ((int)post.StatusCode >= 500)
            {
                return "Nie można się zalogować. Spróbuj ponownie później";
            }

            if (string.IsNullOrEmpty(response.Token))
            {
                return "Login lub hasło jest niepoprawne";
            }
            await _localStorage.SetItemAsync("jwtToken", response.Token);
            ((ClientAuthenticationStateProvider)_authenticationStateProvider).MarkAuthenticated();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
            return string.Empty;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("jwtToken");
            ((ClientAuthenticationStateProvider)_authenticationStateProvider).MarkLoggedOut();
        }
    }

    public interface IAuthService
    {
        public Task<string> Login(AuthenticateRequest request);
        public Task Logout();
    }
}
