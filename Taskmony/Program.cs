using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Taskmony.Data;
using Taskmony.GraphQL;
using Taskmony.Repositories;
using Taskmony.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:ValidIssuer"],
        ValidAudience = builder.Configuration["Authentication:Schemes:Bearer:ValidAudience"],
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Schemes:Bearer:Key"]!))
    };
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<TaskmonyDbContext>(opt => opt.UseNpgsql(
    builder.Configuration.GetConnectionString("TaskmonyDatabase")));

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
    .AddGraphQLServer()
    .AddProjections()
    .AddAuthorization()
    .AddQueryType<Query>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Playground url: http://localhost:%s/graphql/
app.UseEndpoints(endpoints => endpoints.MapGraphQL());

app.MapControllers();

app.Run();