using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using provasemestral.Data;
using provasemestral.Models;
using provasemestral.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using provasemestral.Utils;

var builder = WebApplication.CreateBuilder(args);

var methodsUtil = new MethodsUtil();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("abc"))
        };
    });

// Add services to the container.
builder.Services.AddScoped<UsuarioService>(); 
builder.Services.AddScoped<ServicoService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ProvaDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

//private readonly ProvaDbContext _provaBbContex;

var app = builder.Build();

// app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.MapPost("/createUsuario", async (Usuario usuario, UsuarioService usuarioService) =>
{
    await usuarioService.AddUsuarioAsync(usuario);
    return Results.Created($"/createUsuario/{usuario.Id}", usuario);
});

app.MapPost("/login", async (HttpContext context, UsuarioService usuarioService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();

    var json = JsonDocument.Parse(body);
    //var username = json.RootElement.GetProperty("username").GetString();
    var email = json.RootElement.GetProperty("email").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();

    var usuario = await usuarioService.GetUsuarioByEmailAsync(email);

    var token = "";
    if (senha == usuario.Senha)
    {
        token = methodsUtil.GenerateToken(email);
    }
    //return token;
    await context.Response.WriteAsync(token);
});

app.MapPost("/createServico", async (Servico servico, ServicoService servicoService, HttpContext context) =>
{
    bool validated = await methodsUtil.ValidateToken(context);

    if (validated)
    {
        try
        {
            await servicoService.AddServicoAsync(servico);
            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsync($"Serviço criado com sucesso. Id: {servico.Id}");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync($"Erro ao criar serviço: {ex.Message}");
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token inválido");
    }
});

app.MapPut("/updateServico/{id}", async (string id, Servico updateServico, ServicoService servicoService, HttpContext context) =>
{
    bool validated = await methodsUtil.ValidateToken(context);

    if (validated)
    {
        try
        {
            if (id != updateServico.Id)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Mismatch between provided Id and Servico Id.");
                return Results.BadRequest("Servico ID mismatch.");
            }

            await servicoService.UpdateServicoAsync(updateServico);
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync($"Serviço atualizado com sucesso. Id: {updateServico.Id}");
            return Results.Ok($"Serviço atualizado com sucesso. Id: {updateServico.Id}");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync($"Erro ao atualizar serviço: {ex.Message}");
            return Results.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao atualizar serviço: {ex.Message}");
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token inválido");
        return Results.Unauthorized();
    }
});


app.MapGet("/ServicoId/{id}", async (string id, ServicoService servicoService) =>
{
    var produtos = await servicoService.GetServicoByIdAsync(id);

    if (produtos == null)
    {
        return Results.NotFound($"Servico with ID {id} not found");
    }
    return Results.Ok(produtos);
});


app.Run();