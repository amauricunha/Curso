using curso.api.Business.Entities;
using curso.api.Business.Repositories;
using curso.api.Models.Cursos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace curso.api.Controllers
{
    [Route("api/v1/cursos")]
    [ApiController]
    [Authorize]
    public class CursoController : ControllerBase
    {


        private readonly ICursoRepository _cursoRepository;

        public CursoController(
            ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        /// <summary>
        ///  Este serviço permite cadastrar curso para o usuario autenticado
        /// </summary>
        /// <returns>Retorna status 201 e dados do curso do usuário</returns>
        [SwaggerResponse(statusCode: 200, description: "Sucesso ao cadastrar um curso")]
        [SwaggerResponse(statusCode: 401, description: "Não autorizado")]

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post(CursoViewModelInput cursoViewModelInput) 
        {

            Curso curso = new Curso();
            curso.Nome = cursoViewModelInput.Name;
            curso.Descricao = cursoViewModelInput.Description;

            var codigoUsuario = int.Parse(User.FindFirst(c =>
                c.Type ==ClaimTypes.NameIdentifier)?.Value
            );

            curso.CodigoUsuario = codigoUsuario;

            _cursoRepository.Adicionar(curso);
            _cursoRepository.Commit();

            return Created("", cursoViewModelInput);
        }


        /// <summary>
        ///  Este serviço permite obter todos os cursos ativos do usuario
        /// </summary>
        /// <returns>Retorna status ok, dados do curso do usuário</returns>
        [SwaggerResponse(statusCode: 200, description: "Sucesso ao cadastrar Curso")]
        [SwaggerResponse(statusCode: 401, description: "Não autorizado")]

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var codigoUsuario = int.Parse(User.FindFirst(c =>
            c.Type == ClaimTypes.NameIdentifier)?.Value
            );

            var cursos = _cursoRepository.ObterPorUsuario(codigoUsuario)
                .Select(s => new CursoViewModelOutput()
                {
                    Name = s.Nome,
                    Description = s.Descricao,
                    Login = s.Usuario.Login
                });

            return Ok(cursos);
        }
    }
}
