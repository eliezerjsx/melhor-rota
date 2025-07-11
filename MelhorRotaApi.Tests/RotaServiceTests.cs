using FluentAssertions;
using MelhorRotaApi.Data;
using MelhorRotaApi.Models;
using MelhorRotaApi.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MelhorRotaApi.Tests
{
    public class RotaServiceTests
    {
        private async Task<RotaService> GetServiceWithSeededDataAsync()
        {
            var options = new DbContextOptionsBuilder<RotaDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;

            var context = new RotaDbContext(options);

            context.Rotas.AddRange(new List<Rota>
            {
                new Rota { Origem = "GRU", Destino = "BRC", Valor = 10 },
                new Rota { Origem = "BRC", Destino = "SCL", Valor = 5 },
                new Rota { Origem = "GRU", Destino = "CDG", Valor = 75 },
                new Rota { Origem = "GRU", Destino = "SCL", Valor = 20 },
                new Rota { Origem = "GRU", Destino = "ORL", Valor = 56 },
                new Rota { Origem = "ORL", Destino = "CDG", Valor = 5 },
                new Rota { Origem = "SCL", Destino = "ORL", Valor = 20 },
            });

            await context.SaveChangesAsync();

            return new RotaService(context);
        }

        [Fact]
        public async Task CalcularMelhorRota_DeveRetornarRotaMaisBarata()
        {
            var service = await GetServiceWithSeededDataAsync();
            var resultado = await service.CalcularMelhorRotaAsync("GRU", "CDG");

            resultado.Should().Be("GRU - BRC - SCL - ORL - CDG ao custo de $40");
        }

        [Fact]
        public async Task CalcularMelhorRota_DeveRetornarMensagemParaRotaInexistente()
        {
            var service = await GetServiceWithSeededDataAsync();
            var resultado = await service.CalcularMelhorRotaAsync("AAA", "ZZZ");

            Console.WriteLine("Mensagem retornada:");
            Console.WriteLine(resultado); 

            resultado.Should().Contain("não encontrada");
        }
    }
}
