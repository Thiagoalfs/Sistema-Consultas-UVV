using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Controllers
{
    [Authorize] // Bloqueia todas as ações para usuários não autenticados
    public class ConsultasController : Controller
    {
        private readonly AppDbContext _context;

        public ConsultasController(AppDbContext context)
        {
            _context = context;
        }

        private int ObterUsuarioLogadoId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!);
        }

        // GET: Consultas
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int usuarioId = ObterUsuarioLogadoId();

            var consultas = await _context.Consultas
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.DataHora)
                .ToListAsync();

            return View(consultas);
        }

        // GET: Retorna o formulário em branco para agendar consulta
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Processa os dados preenchidos e salva no banco
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Consulta consulta)
        {
            consulta.UsuarioId = ObterUsuarioLogadoId();

            if (ModelState.IsValid)
            {
                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(consulta);
        }

        // GET: Busca os dados da consulta e preenche a tela para edição
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int usuarioId = ObterUsuarioLogadoId();

            var consulta = await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // POST: Grava as alterações feitas na consulta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Consulta consulta)
        {
            if (id != consulta.Id)
            {
                return NotFound();
            }

            int usuarioId = ObterUsuarioLogadoId();

            // Valida novamente no banco se a consulta de fato pertence ao usuário antes de salvar
            var registroExistente = await _context.Consultas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (registroExistente == null)
            {
                return Unauthorized();
            }

            consulta.UsuarioId = usuarioId;

            if (ModelState.IsValid)
            {
                _context.Consultas.Update(consulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(consulta);
        }

        // GET: Exibe a tela de confirmação de exclusão
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int usuarioId = ObterUsuarioLogadoId();

            var consulta = await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // POST: Efetiva a exclusão no banco após a confirmação do usuário
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int usuarioId = ObterUsuarioLogadoId();

            var consulta = await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (consulta != null)
            {
                _context.Consultas.Remove(consulta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}