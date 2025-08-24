using CatiphyWeb.Components;
using CatiphyWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });


builder.Services.AddHttpClient<CatiphyApi>(c =>
{
    c.BaseAddress = new Uri("https://localhost:44360/"); //API
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
app.UseAntiforgery();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
