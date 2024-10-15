using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Projekt.Client.Services;
using Projekt.Shared.Entities;
using Projekt.Shared.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Projekt.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/") });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<ClientAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ClientAuthenticationStateProvider>());
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddOptions();

            builder.Services.AddScoped<IHttpService<Category>, HttpService<Category>>(opt => ActivatorUtilities.CreateInstance<HttpService<Category>>(opt, "categories"));
            builder.Services.AddScoped<IHttpService<CourseModel>, HttpService<CourseModel>>(opt => ActivatorUtilities.CreateInstance<HttpService<CourseModel>>(opt, "courses"));
            builder.Services.AddScoped<IHttpService<ItemModel>, HttpService<ItemModel>>(opt => ActivatorUtilities.CreateInstance<HttpService<ItemModel>>(opt, "courses/items"));
            builder.Services.AddScoped<IHttpService<QuestionModel>, HttpService<QuestionModel>>(opt => ActivatorUtilities.CreateInstance<HttpService<QuestionModel>>(opt, "courses/items/questions"));
            builder.Services.AddScoped<IHttpService<SentItem>, HttpService<SentItem>>(opt => ActivatorUtilities.CreateInstance<HttpService<SentItem>>(opt, "courses/send"));
            builder.Services.AddScoped<IHttpService<SentQuestionModel>, HttpService<SentQuestionModel>>(opt => ActivatorUtilities.CreateInstance<HttpService<SentQuestionModel>>(opt, "courses/send/questions"));

            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
