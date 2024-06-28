using Microsoft.EntityFrameworkCore;
using provasemestral.Data;
using provasemestral.Models;

namespace provasemestral.Services
{
    public class UsuarioService
    {
        private readonly ProvaDbContext _dbContext;
        public UsuarioService(ProvaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Usuario>> GetAllUsuarios()
        {
            return await _dbContext.Usuarios.ToListAsync();
        }
        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            var usuario = await _dbContext.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuario com o ID {id} não encontrado");
            }
            return usuario;
        }

        public async Task<Usuario> GetUsuarioByEmailAsync(string? email)
        {
            //var usuario = await _dbContext.Usuarios.FindAsync(email);
            var usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuario com o email {email} não encontrado");
            }
            return usuario;
        }
        public async Task AddUsuarioAsync(Usuario usuario)
        {
            _dbContext.Usuarios.Add(usuario);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            _dbContext.Entry(usuario).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _dbContext.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _dbContext.Usuarios.Remove(usuario);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
