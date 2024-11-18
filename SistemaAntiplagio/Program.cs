using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SistemaAntiplagio.Models.Repositorio;
using SistemaAntiplagio.Models.Repositorio.Entidades;
using SistemaAntiplagio.Services;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

//Serviço de Anotações do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

// Adiciona o serviço de controladores e configura o Swagger para documentação
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT desta forma: Bearer {seu_token}"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurações JWT
// Esta é a chave secreta usada para assinar os tokens JWT.
// OBs: Use uma chave forte.
var key = Encoding.ASCII.GetBytes("sistema_antiplagio_tccsantiago2024!#20181021");

// Configuração do serviço de autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Permite testes em HTTP (mude para true em produção)
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Configura a validação da chave de assinatura do token (usa a chave secreta)
        ValidateIssuerSigningKey = true, //lê o token e verifica se a assinatura corresponde à chave fornecida em IssuerSigningKey.
        IssuerSigningKey = new SymmetricSecurityKey(key), //Chave que será do outro sistema.

        // Configurações opcionais para emissor e audiência:
        ValidateIssuer = false, 
        ValidateAudience = false
    };
});

// Registro do SqlConnectionFactory para gerenciar a conexão com o banco
builder.Services.AddSingleton<ISqlConnectionFactory>(sp =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("Connection")));

// Registro dos repositórios
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IAtividadeRepositorio, AtividadeRepositorio>();
builder.Services.AddScoped<IArquivoRepositorio, ArquivoRepositorio>();
builder.Services.AddScoped<IResultadoRepositorio, ResultadoRepositorio>();
builder.Services.AddScoped<DeteccaoPlagioService>();


var app = builder.Build();

// Configuração do pipeline da aplicação
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ativa a autenticação e autorização no pipeline
app.UseAuthentication(); // Esta linha habilita a autenticação JWT para a aplicação
app.UseAuthorization();

app.MapControllers();

app.Run();
