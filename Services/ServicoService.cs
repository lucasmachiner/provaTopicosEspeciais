using Microsoft.EntityFrameworkCore;
using provasemestral.Data;
using provasemestral.Models;

namespace provasemestral.Services
{
    public class ServicoService
    {
        private readonly ProvaDbContext _dbContext;
        public ServicoService(ProvaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Servico>> GetAllServicos()
        {
            return await _dbContext.Servicos.ToListAsync();
        }
        public async Task<Servico> GetServicoByIdAsync(string id)
        {
            var servico = await _dbContext.Servicos.FindAsync(id);
            if (servico == null)
            {
                throw new KeyNotFoundException($"Serviço com o ID {id} não encontrado");
            }
            return servico;
        }

        public async Task AddServicoAsync(Servico servico)
        {
            _dbContext.Servicos.Add(servico);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateServicoAsync(Servico servico)
        {
            _dbContext.Entry(servico).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteServicoAsync(int id)
        {
            var servico = await _dbContext.Servicos.FindAsync(id);
            if (servico != null)
            {
                _dbContext.Servicos.Remove(servico);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
