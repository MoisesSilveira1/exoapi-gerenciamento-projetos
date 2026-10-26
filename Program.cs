using System.Reflection;
using System.Text;
using ExoApi.Middleware;
using ExoApi.Models;
using ExoApi.Repositories;
using ExoApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Camadas: Controllers -> Services -> Repositories
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

// Autenticacao JWT (Desafio 2: restringir o acesso aos usuarios autenticados)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

// Documentacao com Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ExoApi - Gerenciamento de Projetos",
        Version = "v1",
        Description = "API REST para armazenar e gerenciar os projetos da empresa ExoApi."
    });

    var arquivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, arquivoXml));

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT obtido em /api/auth/login."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Tratamento global de excecoes (respostas padronizadas)
app.UseMiddleware<ErrorHandlingMiddleware>();

// Respostas de status sem corpo (ex.: 401 e 404 de rota) tambem em formato padronizado
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExoApi v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

// Cria o usuario de exemplo (admin / Admin@123) caso ainda nao exista
using (var escopo = app.Services.CreateScope())
{
    var logger = escopo.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var usuarios = escopo.ServiceProvider.GetRequiredService<IUsuarioRepository>();
        if (await usuarios.ObterPorLoginAsync("admin") is null)
        {
            await usuarios.InserirAsync(new Usuario
            {
                Login = "admin",
                SenhaHash = SenhaHasher.Gerar("Admin@123")
            });
            logger.LogInformation("Usuario de exemplo 'admin' criado.");
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning("Nao foi possivel preparar o usuario de exemplo (o banco ExoApi existe? execute Database/db.sql): {Mensagem}", ex.Message);
    }
}

app.Run();
