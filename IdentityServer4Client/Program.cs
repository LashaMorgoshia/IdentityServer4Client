using IdentityServer4.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryApiResources(new List<ApiResource>
    {
        new ApiResource("api1", "My API")
    })
    .AddInMemoryClients(new List<Client>
    {
        new Client
        {
            ClientId = "client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets =
            {
                new Secret("secret".Sha256())
            },
            AllowedScopes = { "api1" }
        }
    });

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:5000";
        options.RequireHttpsMetadata = false;
        options.Audience = "api1";
    });


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

