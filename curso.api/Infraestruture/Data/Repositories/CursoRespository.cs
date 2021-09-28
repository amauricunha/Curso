using curso.api.Business.Entities;
using curso.api.Business.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace curso.api.Infraestruture.Data.Repositories
{
    public class CursoRespository : ICursoRepository
    {
        private readonly CursoDbContext _contexto;

        public CursoRespository(CursoDbContext contexto)
        {
            _contexto = contexto;
        }
        public void Adicionar(Curso curso)
        {
            _contexto.Curso.Add(curso);
        }

        public void Commit()
        {
            _contexto.SaveChanges();
        }

        public IList<Curso> ObterPorUsuario(int codigoUsuario)
        {
            return _contexto.Curso
                .Include(i => i.Usuario)
                .Where(u => u.CodigoUsuario == codigoUsuario)
                .ToList();
        }
    }
}
