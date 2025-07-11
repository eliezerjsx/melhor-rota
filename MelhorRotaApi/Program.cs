using MelhorRotaApi.Data;
using MelhorRotaApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Rotas",
        Version = "v1",
        Description = "API para calcular a melhor rota entre aeroportos.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Eliezer Souza",
            Email = "eliezerjs.df@email.com"
        }
    });
});
builder.Services.AddDbContext<RotaDbContext>(options =>
    options.UseSqlite("Data Source=rotas.db"));
builder.Services.AddScoped<RotaService>();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();



app.Run();