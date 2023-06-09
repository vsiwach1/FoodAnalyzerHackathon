using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("FoodAnalyzer", httpClient =>
{
    httpClient.BaseAddress = new Uri("Your Api base address");
    httpClient.DefaultRequestHeaders.Add("Prediction-Key", "your prediction project key");
});
builder.Services.AddHttpClient("FoodDectector", httpClient =>
{
    httpClient.BaseAddress = new Uri("Your Api base address");
    httpClient.DefaultRequestHeaders.Add("Prediction-Key", "your prediction project key");
});
builder.Services.AddHttpClient("FoodNutrition", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://trackapi.nutritionix.com/v2/natural/nutrients");
    httpClient.DefaultRequestHeaders.Add("x-app-id", "your application id");
    httpClient.DefaultRequestHeaders.Add("x-app-key", "your application key");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
