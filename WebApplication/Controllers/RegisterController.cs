using Business.Abstract;
using Core.Entities.Concrete;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class RegisterController : Controller
    {
        private IAuthService _authService;
        private IUserOperationClaimService _userOperationClaimService;
        private IOperationClaimService _operationClaimService;

        public RegisterController(IAuthService authService, IUserOperationClaimService userOperationClaimService, IOperationClaimService operationClaimService)
        {
            _authService = authService;
            _userOperationClaimService = userOperationClaimService;
            _operationClaimService = operationClaimService;
        }

        public IActionResult Index(string referralLink)
        {
            ViewBag.ReferralLink = referralLink;

            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(UserForRegisterDto userForRegisterDto)
        {
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }

            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);

            var operationClaim = _operationClaimService.Get("Manager");

            _userOperationClaimService.Add(new UserOperationClaim() { UserId = registerResult.Data.Id, OperationClaimId = operationClaim.Id });

            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }
    }
}
