using FinalTercero;
using FinalTercero.Data;
using FinalTercero.Interfaces;
using FinalTercero.Models;
using FinalTercero.Repositories;
using FinalTercero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<IUserRepository, UsersInMemoryRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<DBPrimerParcialJorgeCuevas>(Options => Options.UseNpgsql(connectionString));



var info = new OpenApiInfo
{
    Title = "Curso JWT"
};
var security = new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT para curso ua"
};
var requirement = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id   = "Bearer"
            }
        },
        new List<string>()
    }
};
builder.Services.AddSwaggerGen(config =>
{
    config.AddSecurityDefinition("Bearer", security);
    config.AddSecurityRequirement(requirement);
    config.EnableAnnotations();
});

builder.Services.Configure<TokenSettings>(
    builder.Configuration.GetSection(nameof(TokenSettings)));

builder.Services.AddAuthentication()
    .AddJwtBearer("CURSO-UA", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["TokenSettings:Issuer"],
            ValidAudience = builder.Configuration["TokenSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder
                .Configuration["TokenSettings:Secret"]))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("CURSO-UA")
        .Build();
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", async (IUserService userService, IAuthenticationService authenticationService, AuthenticationRequest request) =>
{
    var isValidAuthentication = await authenticationService
        .Authenticate(request.Username, request.Password);

    if (isValidAuthentication)
    {
        var user = await userService.GetByCredentials(request.Username, request.Password);

        var token = await authenticationService.GenerateJwt(user);

        return Results.Ok(new { AccessToken = token });
    }

    return Results.Forbid();

});
app.MapGet("/cliente/{id:int}",
[Authorize]
[SwaggerOperation(Summary = "Retorna el objeto cliente", Description = "Retorna un objeto de tipo `Cliente` de la base de datos")]
[SwaggerResponse(statusCode: 200, description: "Success. Cliente enviado correctamente")]
[SwaggerResponse(statusCode: 400, description: "BadRequest. Solicitud Incorrecta")]
[SwaggerResponse(statusCode: 404, description: "NotFound. Solicitud no encontrada")]
[SwaggerResponse(statusCode: 500, description: "InternalServerError. Ocurrió un error al realizar la consulta")]
async (int id, DBPrimerParcialJorgeCuevas db) =>
{
    return await db.Cliente.FindAsync(id) is Cliente cliente ? Results.Ok(cliente) : Results.NotFound("Solicitud no encontrada");
});

app.MapGet("/clientes/",
[Authorize]
[SwaggerOperation(Summary = "Retorna todos los clientes", Description = "Retorna un arreglo de objetos de tipo `Cliente` de la base de datos")]
[SwaggerResponse(statusCode: 200, description: "Success. Cliente enviado correctamente")]
[SwaggerResponse(statusCode: 400, description: "BadRequest. Solicitud Incorrecta")]
[SwaggerResponse(statusCode: 404, description: "NotFound. Solicitud no encontrada")]
[SwaggerResponse(statusCode: 500, description: "InternalServerError. Ocurrió un error al realizar la consulta")]
async (DBPrimerParcialJorgeCuevas db) =>
{
    return db.Cliente.ToList();
});


app.MapPost("/cliente/",
[Authorize]
[SwaggerOperation(Summary = "Crea un objeto de tipo cliente", Description = "Crea y almacena en la BD un objeto de tipo `Cliente`")]
[SwaggerResponse(statusCode: 200, description: "Success. Cliente creado correctamente")]
[SwaggerResponse(statusCode: 400, description: "BadRequest. Solicitud Incorrecta")]
[SwaggerResponse(statusCode: 404, description: "NotFound. Solicitud no encontrada")]
[SwaggerResponse(statusCode: 500, description: "InternalServerError. Ocurrió un error al realizar la consulta")]
async (Cliente clientes, DBPrimerParcialJorgeCuevas db) =>
{
    db.Cliente.Add(clientes);
    await db.SaveChangesAsync();

    return Results.Created($"/cliente/{clientes.Id}", clientes);
});

app.MapPut("/modificar-cliente/{id:int}",
[Authorize]
[SwaggerOperation(Summary = "Modifica un registro de tipo cliente", Description = "Modifica un registro `Cliente` por su id")]
[SwaggerResponse(statusCode: 200, description: "Success. Cliente editado correctamente")]
[SwaggerResponse(statusCode: 400, description: "BadRequest. Solicitud Incorrecta")]
[SwaggerResponse(statusCode: 404, description: "NotFound. Solicitud no encontrada")]
[SwaggerResponse(statusCode: 500, description: "InternalServerError. Ocurrió un error al realizar la consulta")]
async (Cliente clientes, int id, DBPrimerParcialJorgeCuevas db) =>
{

    if (id != clientes.Id)
    {
        return Results.BadRequest();
    }

    if (!await db.Cliente.AnyAsync(x => x.Id == id))
    {
        return Results.NotFound();
    }

    db.Update(clientes);
    await db.SaveChangesAsync();

    return Results.Ok("Cliente editado correctamente");
});

app.MapDelete("/eliminar-cliente/{id}",
[Authorize]
[SwaggerOperation(Summary = "Elimina un registro de tipo cliente", Description = "Elimina un registro `Cliente` por su id")]
[SwaggerResponse(statusCode: 200, description: "Success. Cliente eliminado correctamente")]
[SwaggerResponse(statusCode: 400, description: "BadRequest. Solicitud Incorrecta")]
[SwaggerResponse(statusCode: 404, description: "NotFound. Solicitud no encontrada")]
[SwaggerResponse(statusCode: 500, description: "InternalServerError. Ocurrió un error al realizar la consulta")]
async (int id, DBPrimerParcialJorgeCuevas db) =>
{
    var cliente = await db.Cliente.FindAsync(id);
    if (cliente is null)
    {
        return Results.NotFound();
    }

    db.Cliente.Remove(cliente);
    await db.SaveChangesAsync();

    return Results.Ok("Cliente eliminado correctamente");
});



internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}