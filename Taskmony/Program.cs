using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Taskmony.Auth;
using Taskmony.Data;
using Taskmony.Emails;
using Taskmony.Errors;
using Taskmony.GraphQL;
using Taskmony.GraphQL.Comments;
using Taskmony.GraphQL.Converters;
using Taskmony.GraphQL.Directions;
using Taskmony.GraphQL.Errors;
using Taskmony.GraphQL.Ideas;
using Taskmony.GraphQL.Notifications;
using Taskmony.GraphQL.Subscriptions;
using Taskmony.GraphQL.Tasks;
using Taskmony.GraphQL.Users;
using Taskmony.Repositories;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;

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

builder.Services.Configure<EmailCommunicationSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Authentication:Schemes:Bearer"));
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ITaskRepository, TaskRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<IIdeaRepository, IdeaRepository>();
builder.Services.AddTransient<IDirectionRepository, DirectionRepository>();
builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddTransient<IVerificationTokenRepository, VerificationTokenRepository>();
builder.Services.AddTransient<IMessageTemplateRepository, MessageTemplateRepository>();

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
builder.Services.AddTransient<ITimeConverter, TimeConverter>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ISecurityService, SecurityService>();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IIdeaService, IdeaService>();
builder.Services.AddTransient<IDirectionService, DirectionService>();
builder.Services.AddTransient<IUserIdentifierProvider, UserIdentifierProvider>();
builder.Services.AddTransient<IEmailService, EmailCommunicationService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddPooledDbContextFactory<TaskmonyDbContext>(opt => opt.UseNpgsql(
    builder.Configuration.GetConnectionString("TaskmonyDatabase")));

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services
    .AddGraphQLServer()
    .AddErrorFilter(provider => new ErrorFilter(
        provider.GetRequiredService<ILogger<ErrorFilter>>(),
        builder.Environment))
    .AddAuthorization()
    .AddHttpRequestInterceptor<HttpRequestInterceptor>()
    .AddQueryType<Query>()
    .AddType<TaskType>()
    .AddType<IdeaType>()
    .AddType<UserType>()
    .AddType<DirectionType>()
    .AddType<CommentType>()
    .AddType<NotificationType>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<TaskMutations>()
    .AddTypeExtension<IdeaMutations>()
    .AddTypeExtension<CommentMutations>()
    .AddTypeExtension<DirectionMutations>()
    .AddTypeExtension<UserMutations>()
    .AddTypeExtension<SubscriptionMutations>()
    .BindRuntimeType<Guid, IdType>()
    .AddTypeConverter<StringToGuidConverter>()
    .AddTypeConverter<GuidToStringConverter>()
    .AddTypeConverter<ValueObjectToStringConverter>()
    .AddTypeConverter<DateTimeToStringConverter>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.MapControllers();

app.Run();