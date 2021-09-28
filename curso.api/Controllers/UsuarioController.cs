using curso.api.Business.Entities;
using curso.api.Business.Repositories;
using curso.api.Configurations;
using curso.api.Filters;
using curso.api.Models;
using curso.api.Models.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace curso.api.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    [ValidacaoModelStateCustomizado]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAuthenticationService _authenticationServices;

        public UsuarioController(
            IUsuarioRepository usuarioRepository, 
            IAuthenticationService authenticationServices)
        {
            _usuarioRepository = usuarioRepository;
            _authenticationServices = authenticationServices;
        }

        /// <summary>
        ///  Este serviço permite autenticar um usuario cadastrado e ativo
        /// </summary>
        /// <param name="loginViewModelInput">View model do login</param>
        /// <returns>Retorna status ok, dados do usuario e o token em caso de suacesso</returns>
        [SwaggerResponse(statusCode: 200, description: "Sucesso ao autenticar", Type = typeof(LoginViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campos obrigatórios", Type = typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro ao autenticar", Type = typeof(ErroGenericoViewModel))]

        [HttpPost]
        [Route("login")]
        [ValidacaoModelStateCustomizado]
        public IActionResult Login(LoginViewModelInput loginViewModelInput)
        {
            Usuario usuario =  _usuarioRepository.ObterUsuario(loginViewModelInput.Login);

            if (usuario == null)
            {
                return BadRequest("Houve um erro ao tentar acessar.");
            }

            //if (usuario.Senha != loginViewModel.Senha.GerarSenhaCriptografada())
            //{
            //    return BadRequest("Houve um erro ao tentar acessar.");
            //}

           var usuarioViewModelOutput = new UsuarioViewModelOutput() { 
                Codigo = usuario.Codigo,
                Login = loginViewModelInput.Login,
                Email = usuario.Email
            };

            
            var token = _authenticationServices.GerarToken(usuarioViewModelOutput);

            return Ok(new
            {
                Token = token,
                Usuario = usuarioViewModelOutput
            });
        }

        

        /// <summary>
        ///  Este serviço permite cadastrar um usuario se não existe
        /// </summary>
        /// <param name="registerViewModelInput">View model de registro de login</param>
        /// <returns>Retorna status ok, dados do usuario e o token em caso de suacesso</returns>
        [SwaggerResponse(statusCode: 200, description: "Sucesso ao cadastrar", Type = typeof(LoginViewModelInput))]
        [SwaggerResponse(statusCode: 400, description: "Campos obrigatórios", Type = typeof(ValidaCampoViewModelOutput))]
        [SwaggerResponse(statusCode: 500, description: "Erro ao cadastrar", Type = typeof(ErroGenericoViewModel))]


        [HttpPost]
        [Route("register")]
        [ValidacaoModelStateCustomizado]
        public IActionResult Register(RegisterViewModelInput registerViewModelInput)
        {
            

            //var migracoesPendentes = contexto.Database.GetPendingMigrations();

            //if (migracoesPendentes.Count() > 0)
            //{
            //    contexto.Database.Migrate();
            //}

            var usuario = new Usuario();
            usuario.Login = registerViewModelInput.Login;
            usuario.Senha = registerViewModelInput.Password;
            usuario.Email = registerViewModelInput.Email;
            _usuarioRepository.Adicionar(usuario);
            _usuarioRepository.Commit();

            return Created("", registerViewModelInput);
        }
    }
}
