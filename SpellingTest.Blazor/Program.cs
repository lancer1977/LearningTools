using PolyhydraGames.BlazorComponents; 
using SpellingTest.Web.Setup; 

//builder.Configuration.AddJsonFile("configs/secret.json", false, reloadOnChange: true).Build();
var builder = WebApplication.CreateBuilder(args);
PolyhydraGames.Core.ReactiveUI.ReactiveExtensions.RegisterErrorCallback(async (title, message) => { Console.WriteLine(title + ":" + message); });

builder.AddCors();
builder.Services.AddBlazorise();
builder.Services.AddBlazorComponents();
builder.Services.AddSignalR();
builder.Services.AddServerSideBlazor(); 
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHttpClient(); 
builder.AddOIDC();
builder.AddMiscServices();
builder.RegisterRest();
builder.RegisterViewModels();
builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{ 
    app.UseCors("AllowAllOrigins");
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();//Enforces HTTPS
app.UseStaticFiles(); // Allows serving static html/js
app.UseRouting(); 
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers(); // Maps all the controllers.
app.MapBlazorHub(); 
app.MapFallbackToPage("/_Host");
app.Run();