using Business.Abstract;
using Core.Entities.Concrete;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class AuthController : Controller
    {
        private IAuthService _authService;
        private IUserOperationClaimService _userOperationClaimService;
        private IOperationClaimService _operationClaimService;

        public AuthController(IAuthService authService, IUserOperationClaimService userOperationClaimService, IOperationClaimService operationClaimService)
        {
            _authService = authService;
            _userOperationClaimService = userOperationClaimService;
            _operationClaimService = operationClaimService;
        }

        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

    }
}
