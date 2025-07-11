using Microsoft.AspNetCore.Mvc;
using MelhorRotaApi.Models;
using MelhorRotaApi.Services;
using System.Threading.Tasks;

namespace MelhorRotaApi.Controllers
{
    [ApiController]
    [Route("api/rota")]
    public class RotaController : ControllerBase
    {
        private readonly RotaService _rotaService;

        public RotaController(RotaService rotaService)
        {
            _rotaService = rotaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _rotaService.GetAllRotasAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rota = await _rotaService.GetRotaByIdAsync(id);
            if (rota == null) return NotFound();
            return Ok(rota);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Rota rota)
        {
            var created = await _rotaService.AddRotaAsync(rota);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rotaService.DeleteRotaAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("melhor-rota")]
        public async Task<IActionResult> CalcularMelhorRota([FromQuery] string origem, [FromQuery] string destino)
        {
            var resultado = await _rotaService.CalcularMelhorRotaAsync(origem.ToUpper(), destino.ToUpper());
            return Ok(resultado);
        }
    }
}