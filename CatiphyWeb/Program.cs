using CatiphyWeb.Components;
using CatiphyWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;   // <-- a�ade esto
    });


builder.Services.AddHttpClient<CatiphyApi>(c =>
{
    c.BaseAddress = new Uri("https://localhost:44360/"); // <-- tu API
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Si tienes auth:
// app.UseAuthentication();
// app.UseAuthorization();

// >>> Agrega esta l�nea:
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
