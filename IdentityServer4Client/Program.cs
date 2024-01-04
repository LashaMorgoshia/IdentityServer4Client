using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

ConfigureApp(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


void ConfigureApp(WebApplicationBuilder app)
{
    app.Services.AddIdentityServer()
        .AddInMemoryApiResources(new ApiResource[]
        {
            new ApiResource("api1", "My API")
        })
        .AddInMemoryClients(new Client[]
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = { "api1" },
                AccessTokenLifetime = 3600, // Set token lifetime in seconds
                // Enable JWT token support
                AccessTokenType = AccessTokenType.Jwt,
                RequireClientSecret = false // For client credentials flow, you may not need a client secret
            }
        })
        .AddDeveloperSigningCredential();

    app.Services.Configure<MvcOptions>(options =>
    {
        options.Filters.Add(new RequireHttpsAttribute());
    });

    app.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "https://localhost:7074"; // IdentityServer URL
            options.RequireHttpsMetadata = false; // Only for development; use true in production
            options.Audience = "api1"; // API resource name
        });
}