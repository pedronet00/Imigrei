using Imigrei.API.Authentication;
using Imigrei.API.Middleware;
using Imigrei.API.OpenApi;
using Imigrei.Application;
using Imigrei.Application.Interfaces;
using Imigrei.Domain.Constants;
using Imigrei.Infrastructure;
using Imigrei.Infrastructure.Authentication;
using Imigrei.Infrastructure.Data.Context;
using Imigrei.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Seção 'Jwt' não configurada.");

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
    throw new InvalidOperationException(
        "Jwt:SecretKey é obrigatória e precisa de pelo menos 32 caracteres. " +
        "Use user-secrets ou variáveis de ambiente — nunca versione a chave.");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PerfisUsuario.Administrador, p => p.RequireRole(PerfisUsuario.Administrador));
    options.AddPolicy("Operacao", p => p.RequireRole(PerfisUsuario.Administrador, PerfisUsuario.Atendente));

    // Por padrão toda rota exige autenticação; use [AllowAnonymous] para abrir.
    options.FallbackPolicy = options.DefaultPolicy;
});


string[] rotasDeCredencial =
[
    "/api/auth/login",
    "/api/auth/registrar",
    "/api/auth/refresh",
    "/api/auth/alterar-senha",
];

static string IpDoCliente(HttpContext ctx) =>
    ctx.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
        PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        {
            var usuarioId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? ctx.User.FindFirstValue("sub");

            var chave = usuarioId is not null ? $"usuario:{usuarioId}" : $"ip:{IpDoCliente(ctx)}";

            return RateLimitPartition.GetFixedWindowLimiter(chave, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            });
        }),
        PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        {
            var caminho = ctx.Request.Path.Value ?? string.Empty;
            var ehRotaDeCredencial = rotasDeCredencial.Any(r =>
                caminho.Equals(r, StringComparison.OrdinalIgnoreCase));

            if (!ehRotaDeCredencial)
                return RateLimitPartition.GetNoLimiter("livre");

            return RateLimitPartition.GetFixedWindowLimiter($"credencial:{IpDoCliente(ctx)}", _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                });
        }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        var resposta = context.HttpContext.Response;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            resposta.Headers.RetryAfter = ((int)Math.Ceiling(retryAfter.TotalSeconds))
                .ToString(CultureInfo.InvariantCulture);

        await resposta.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = "Muitas requisições",
            Detail = "Você fez muitas requisições em pouco tempo. Aguarde um instante e tente novamente.",
        }, cancellationToken);
    };
});

builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();


var origensPermitidas = builder.Configuration.GetSection("Cors:Origens").Get<string[]>()
    ?.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (origensPermitidas.Length > 0)
            policy.WithOrigins(origensPermitidas);
        else
            policy.AllowAnyOrigin();

        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Retry-After");
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

var app = builder.Build();

app.UseForwardedHeaders();

app.UseExceptionHandler();

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference().AllowAnonymous();

app.UseCors("AllowAll");

// Autenticação antes do rate limit, para particionar pelo usuário do token.
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

using (var escopo = app.Services.CreateScope())
{
    var db = escopo.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

await IdentityDataSeeder.SeedAsync(app.Services);

app.Run();