using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Projekt.Client.Services
{
    public class ClientAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navManager;

        public ClientAuthenticationStateProvider(ILocalStorageService localStorage, NavigationManager navManager)
        {
            _localStorage = localStorage;
            _navManager = navManager;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _localStorage.GetItemAsync<string>("jwtToken");
            AuthenticationState anonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));

            if (string.IsNullOrWhiteSpace(token))
            {
                return anonymousState;
            }

            IEnumerable<Claim> claims = ParseClaimsFromJwt(token);
            // Checks the exp field of the token
            Claim expiry = claims.Where(claim => claim.Type.Equals("exp")).FirstOrDefault();
            if (expiry == null)
            {
                return anonymousState;
            }
            // The exp field is in Unix time
            var datetime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry.Value));
            if (datetime.UtcDateTime <= DateTime.UtcNow)
            {
                await _localStorage.RemoveItemAsync("jwtToken");
                return anonymousState;
            }

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "WebApiAuth"))));
        }

        public void MarkAuthenticated()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkLoggedOut()
        {
            ClaimsPrincipal anonymousUser = new(new ClaimsIdentity());
            Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<bool> IsPanel()
        {
            Task<AuthenticationState> authenticationStateTask = GetAuthenticationStateAsync();
            AuthenticationState authenticationState = await authenticationStateTask;
            bool isPanel = _navManager.ToBaseRelativePath(_navManager.Uri).StartsWith("panel") && authenticationState.User.IsInRole("Admin");
            return isPanel;
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var claims = new List<Claim>();
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
