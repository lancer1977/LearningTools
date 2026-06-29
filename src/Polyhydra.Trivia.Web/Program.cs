using Polyhydra.Trivia.Web.Components;
using Polyhydra.Trivia.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<TriviaApiClient>((provider, client) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["TriviaApi:BaseUrl"] ?? "http://localhost:5147";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddSingleton<PollResultPresenter>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
