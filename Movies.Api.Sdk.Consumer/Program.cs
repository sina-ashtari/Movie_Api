// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;

// var moviesApi = RestService.For<IMoviesApi>("http://localhost:7001");

var service = new ServiceCollection();
service
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(x => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async (message, token) => await x.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
    })
    .ConfigureHttpClient(x =>
{
    x.BaseAddress = new Uri("http://localhost:5000");
});
var provider  = service.BuildServiceProvider();
var moviesApi = provider.GetService<IMoviesApi>();


// var movie = await moviesApi.GetMovieAsync("idOrSlug");
var request = new GetAllMoviesRequest
{
    Title = null,
    Year = null,
    SortBy = null,
    Page = 1,
    PageSize = 3,
};
var movies = await moviesApi.GetMoviesAsync(request);
foreach (var movieResponse in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movieResponse));
}
