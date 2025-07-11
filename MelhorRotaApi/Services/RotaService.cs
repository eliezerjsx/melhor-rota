using MelhorRotaApi.Data;
using MelhorRotaApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MelhorRotaApi.Services
{
    public class RotaService
    {
        private readonly RotaDbContext _context;

        public RotaService(RotaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rota>> GetAllRotasAsync()
        {
            return await _context.Rotas.ToListAsync();
        }

        public async Task<Rota?> GetRotaByIdAsync(int id)
        {
            return await _context.Rotas.FindAsync(id);
        }

        public async Task<Rota> AddRotaAsync(Rota rota)
        {
            _context.Rotas.Add(rota);
            await _context.SaveChangesAsync();
            return rota;
        }

        public async Task<bool> DeleteRotaAsync(int id)
        {
            var rota = await _context.Rotas.FindAsync(id);
            if (rota == null) return false;

            _context.Rotas.Remove(rota);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> CalcularMelhorRotaAsync(string origem, string destino)
        {
            var rotas = await _context.Rotas.ToListAsync();
            var grafo = new Dictionary<string, List<(string destino, decimal custo)>>();

            foreach (var rota in rotas)
            {
                if (!grafo.ContainsKey(rota.Origem))
                    grafo[rota.Origem] = new List<(string, decimal)>();

                grafo[rota.Origem].Add((rota.Destino, rota.Valor));
            }

            var menorCusto = new Dictionary<string, decimal>();
            var predecessores = new Dictionary<string, string?>();
            var visitados = new HashSet<string>();
            var fila = new PriorityQueue<string, decimal>();

            foreach (var key in grafo.Keys)
                menorCusto[key] = decimal.MaxValue;

            menorCusto[origem] = 0;
            fila.Enqueue(origem, 0);

            while (fila.Count > 0)
            {
                var atual = fila.Dequeue();

                if (!grafo.ContainsKey(atual)) continue;

                foreach (var vizinho in grafo[atual])
                {
                    var custoTotal = menorCusto[atual] + vizinho.custo;
                    if (custoTotal < menorCusto.GetValueOrDefault(vizinho.destino, decimal.MaxValue))
                    {
                        menorCusto[vizinho.destino] = custoTotal;
                        predecessores[vizinho.destino] = atual;
                        fila.Enqueue(vizinho.destino, custoTotal);
                    }
                }
            }

            if (!menorCusto.ContainsKey(destino) || menorCusto[destino] == decimal.MaxValue)
                return $"Rota de {origem} para {destino} não encontrada.";

            var caminho = new Stack<string>();
            var atualRota = destino;

            while (atualRota != origem)
            {
                caminho.Push(atualRota);
                atualRota = predecessores.GetValueOrDefault(atualRota) ?? origem;
            }
            caminho.Push(origem);

            return $"{string.Join(" - ", caminho)} ao custo de ${menorCusto[destino]}";
        }
    }
}