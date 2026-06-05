using Phantoms.API;
using Phantoms.Application;
using Phantoms.Infrastructure;
using Phantoms.Persistence;
using Phantoms.Persistence.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration from env vars too (used by Cloud Run)
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ──────────────────────────────────────────────────────────
// CORS: Universal access for hackathon – Ngrok, React, Mobile, Postman
// ──────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddPresentation(builder.Configuration);

var app = builder.Build();

// ──────────────────────────────────────────────────────────
// Global Exception Handling – FIRST middleware in the pipeline
// Catches ALL unhandled exceptions and returns standardized JSON
// ──────────────────────────────────────────────────────────
app.UseExceptionHandler();

// Swagger available in all environments (needed for hackathon judges)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Phantoms API v1");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
});

// Cloud Run terminates TLS at the load-balancer level; skip HTTPS redirect inside the container
if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

// CORS must come BEFORE Authentication/Authorization
app.UseCors();

// Rate limiting – after CORS, before auth (reject spammers early)
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Auto-migrate and seed on startup
await Phantoms.Persistence.DependencyInjection.ApplyMigrationsAsync(app.Services);
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();


