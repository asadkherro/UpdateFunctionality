var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();


const string latestVersion = "1.2.0";

// GET /check-version?currentVersion=1.0.0
app.MapGet("/check-version", (string currentVersion) =>
{
    var response = new
    {
        currentVersion,
        latestVersion,
        isUpToDate = currentVersion == latestVersion
    };

    return Results.Json(response);
});

app.Run();

