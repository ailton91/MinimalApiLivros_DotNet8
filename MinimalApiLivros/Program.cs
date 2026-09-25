using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MinimalApiLivros.Application.AppLivro.Commands;
using MinimalApiLivros.Application.AppLivro.Queries;
using MinimalApiLivros.Application.AppLivro.Services;
using MinimalApiLivros.Application.AppLivro.Validators;
using MinimalApiLivros.Application.IRepository;
using MinimalApiLivros.Domain.Entities;
using MinimalApiLivros.Infrastructure;
using MinimalApiLivros.Infrastructure.Data.DataBaseConfigurationMongo;
using MinimalApiLivros.Infrastructure.Repository;
using Serilog;
using Serilog.Filters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<CriarLivroCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AtualizarLivroCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ObterLivroQueryValidator>();

builder.Services.Configure<DatabaseMongoConfig>(builder.Configuration.GetSection(nameof(DatabaseMongoConfig)));
builder.Services.AddSingleton<IDatabaseMongoConfig>(sp => sp.GetRequiredService<IOptions<DatabaseMongoConfig>>().Value);

string? mySqlConnection = builder.Configuration.GetConnectionString("MySql_DefaultConnection"); //?? throw new Exception("A string de conexão não foi encontrada");

if (!string.IsNullOrEmpty(mySqlConnection) && builder.Environment.IsDevelopment())
{
    try
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .Filter.ByIncludingOnly(Matching.FromSource("MinimalApiLivros.Application"))
            .WriteTo.MySQL(
                connectionString: mySqlConnection,
                tableName: "LogsSistemaGeral"
            )
            .WriteTo.File("logs/erros_MinimalApiLivros.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
    catch
    {

    }
}

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

// Configuração do DbContext tratando a falta de banco no Azure
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrEmpty(mySqlConnection) && builder.Environment.IsDevelopment())
    {
        // Define uma versão fixa do MariaDB/MySQL em vez de AutoDetect para evitar Ping no Startup
        //options.UseMySql(mySqlConnection, new MySqlServerVersion(new Version(8, 0, 31)));
        options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));
    }
    else
    {
        // Fallback temporário para In-Memory se estiver sem MySQL no Azure (permite que o Swagger suba!)
        options.UseInMemoryDatabase("DbLivrosInMemory");
    }
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{

}

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API Livros v1");
    //c.RoutePrefix = string.Empty; // Isso faz o Swagger abrir direto na URL principal!
    c.InjectStylesheet("/css/swagger-dark.css");
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapPost("/Livros", async ([FromBody] CriarLivroCommand command, ILivroService livroService, CancellationToken cancellationToken = default) =>
{
    try
    {
        Livro retornoAdicionarLivro = await livroService.AdicionarLivroAsync(command, cancellationToken);
        return Results.Created($"{retornoAdicionarLivro.Id}", retornoAdicionarLivro);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
})
    .WithName("AddLivro")
    .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
    {
        Summary = "Adiciona um novo livro",
        Description = "Adiciona um novo livro ao banco de dados MySql - DbLivros",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Minha Biblioteca" } }
    });

app.MapGet("/Livros", async (ILivroService livroService, CancellationToken cancellationToken = default) =>
{
    try
    {
        var livros = await livroService.ObterTodosLivrosAsync(cancellationToken);
        return Results.Ok(livros);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
})
    .WithName("GetAllLivros")
    .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
    {
        Summary = "Obtém todos os livros",
        Description = "Obtém todos os livros do banco de dados MySql - DbLivros",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Minha Biblioteca" } }
    });

app.MapGet("/Livros/{id}", async ([AsParameters] ObterLivroQuery query, ILivroService livroService, CancellationToken cancellationToken = default) =>
{
    try
    {
        var livro = await livroService.ObterLivroPorIdAsync(query, cancellationToken);
        return livro is not null ? Results.Ok(livro) : Results.NotFound();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
})
    .WithName("GetLivroById")
    .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
    {
        Summary = "Obtém um livro pelo ID",
        Description = "Obtém um livro específico do banco de dados MySql - DbLivros pelo seu ID",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Minha Biblioteca" } }
    });

app.MapDelete("/Livros/{id}", async (int id, ILivroService livroService, CancellationToken cancellationToken = default) =>
{
    try
    {
        var livro = await livroService.DeletarLivroAsync(id, cancellationToken);
        return livro is not null ? Results.Ok(livro) : Results.NotFound();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
})
    .WithName("DeleteLivro")
    .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
    {
        Summary = "Exclui um livro pelo ID",
        Description = "Exclui um livro específico do banco de dados MySql - DbLivros pelo seu ID",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Minha Biblioteca" } }
    });

app.MapPut("/Livros/{id}", async (int id, [FromBody] AtualizarLivroCommand command, ILivroService livroService, CancellationToken cancellationToken = default) =>
{
    try
    {
        var updatedLivro = await livroService.AtualizarLivroAsync(id, command, cancellationToken);
        return updatedLivro is not null ? Results.Ok(updatedLivro) : Results.NotFound();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
})
    .WithName("UpdateLivro")
    .WithOpenApi(x => new Microsoft.OpenApi.Models.OpenApiOperation(x)
    {
        Summary = "Atualiza um livro pelo ID",
        Description = "Atualiza um livro específico do banco de dados MySql - DbLivros pelo seu ID",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Minha Biblioteca" } }
    });

app.Run();

[JsonSerializable(typeof(Livro))]
[JsonSerializable(typeof(List<Livro>))]
[JsonSerializable(typeof(Livro[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
