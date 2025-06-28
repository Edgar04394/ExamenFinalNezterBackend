using ApiExamen.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔄 Agrega CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // <-- Tu app Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agregar servicios a la contenedor con el convertidor TimeSpan personalizado
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new ApiExamen.Converters.TimeSpanConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// ✅ Configuración de Swagger con JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Proyecto Examen", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando el esquema Bearer. 
                        Introduce el token así: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 🔐 Configuración de JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ✅ Registra tus servicios personalizados
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddScoped<PuestoService>();
builder.Services.AddScoped<ExamenService>();
builder.Services.AddScoped<ClasificacionService>();
builder.Services.AddScoped<PreguntaService>();
builder.Services.AddScoped<RespuestaService>();
builder.Services.AddScoped<AsignacionService>();
builder.Services.AddScoped<ResultadoService>();

// 🔧 Inicia app
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Proyecto Examen v1");
        c.RoutePrefix = string.Empty;
    });
}

// 🟢 HABILITA CORS ANTES DE AUTH
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthentication(); // JWT primero
app.UseAuthorization();

app.MapControllers();

app.Run();
