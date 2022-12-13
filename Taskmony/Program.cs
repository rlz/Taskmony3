using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Taskmony.Auth;
using Taskmony.Data;
using Taskmony.GraphQL;
using Taskmony.GraphQL.Comments;
using Taskmony.GraphQL.Directions;
using Taskmony.GraphQL.Ideas;
using Taskmony.GraphQL.Notifications;
using Taskmony.GraphQL.Tasks;
using Taskmony.GraphQL.Users;
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
builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Authentication:Schemes:Bearer"));
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddPooledDbContextFactory<TaskmonyDbContext>(opt => opt.UseNpgsql(
    builder.Configuration.GetConnectionString("TaskmonyDatabase")));

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddType<TaskType>()
    .AddType<IdeaType>()
    .AddType<UserType>()
    .AddType<DirectionType>()
    .AddType<CommentType>()
    .AddType<NotificationType>()
    .BindRuntimeType<Guid, IdType>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapGraphQL());

app.MapControllers();

app.Run();