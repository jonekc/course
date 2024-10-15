#nullable enable
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Projekt.Client.Services
{
    public class HttpService<T> : IHttpService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private string path;
        private readonly NavigationManager _navManager;

        public HttpService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage, string path, NavigationManager navManager)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
            this.path = path;
            _navManager = navManager;
        }

        private async Task SetAuthorizationHeader()
        {
            string token = await _localStorage.GetItemAsync<string>("jwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task HandleUnauthorized(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorage.SetItemAsync("jwtToken", "");
                ((ClientAuthenticationStateProvider)_authenticationStateProvider).MarkLoggedOut();
                Uri uri = _navManager.ToAbsoluteUri(_navManager.Uri);
                bool success = QueryHelpers.ParseQuery(uri.Query).TryGetValue("url", out var _url);
                _navManager.NavigateTo($"/login{(success ? $"?url={_url}" : "")}");
            }
        }

        public async Task<List<T>?> Get(int id = 0)
        {
            await SetAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.GetAsync(id > 0 ? $"{path}/{id}" : path);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<T>>();
            }
            await HandleUnauthorized(response.StatusCode);
            return default;
        }

        public async Task<T?> GetOne(int id)
        {
            await SetAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.GetAsync($"{path}/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            await HandleUnauthorized(response.StatusCode);
            return default;
        }

        public async Task<T?> Add(T t)
        {
            await SetAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(path, t);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            await HandleUnauthorized(response.StatusCode);
            return default;
        }

        public async Task<List<T>?> AddRange(List<T>? list)
        {
            await SetAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(path, list);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<T>>();
            }
            await HandleUnauthorized(response.StatusCode);
            return default;
        }

        public async Task<T?> Edit(T t)
        {
            await SetAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(path, t);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            await HandleUnauthorized(response.StatusCode);
            return default;
        }

        public async Task<List<T>?> EditRange(List<T>? list)
        {
            await SetAuthorizationHeader();
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(path, list);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<T>>();
            }
            await HandleUnauthorized(response.StatusCode);
            return default;
        }

        public async Task<bool> Delete(int id)
        {
            await SetAuthorizationHeader();
            await _httpClient.DeleteAsync($"{path}/{id}");
            return true;
        }

        public void ChangePath(string path)
        {
            this.path = path;
        }
    }

    public interface IHttpService<T>
    {
        Task<List<T>?> Get(int id = 0);
        Task<T?> GetOne(int id);
        Task<T?> Add(T t);
        Task<List<T>?> AddRange(List<T>? list);
        Task<T?> Edit(T t);
        Task<List<T>?> EditRange(List<T>? list);
        Task<bool> Delete(int id);
        void ChangePath(string path);
    }
}
