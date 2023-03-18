using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace WebApplication.Controllers
{
    public class LoginController : Controller
    {
        private IAuthService _authService;
        private IUserService _userService;
        private IUserOperationClaimService _userOperationClaimService;
        private IOperationClaimService _operationClaimService;


        public LoginController(IAuthService authService, IUserOperationClaimService userOperationClaimService, IOperationClaimService operationClaimService, IUserService userService)
        {
            _authService = authService;
            _userOperationClaimService = userOperationClaimService;
            _operationClaimService = operationClaimService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult LoginUser(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                var claimsUser = _userService.GetClaims(userToLogin.Data);

                object responseObj = new { };

                if (claimsUser != null)
                {
                    if (claimsUser.Where(x => x.Name == "Admin").Count() > 0)
                    {
                        responseObj = new
                        {
                            TokenInfo = result.Data,
                            UserClaim = "Admin",
                            AllUserInfo = _userService.GetAllUser()
                        };

                        return Ok(responseObj);
                    }
                    else if (claimsUser.Where(x => x.Name == "Manager").Count() > 0)
                    {
                        responseObj = new
                        {
                            TokenInfo = result.Data,
                            UserClaim = "Manager",
                            UserInfo = userToLogin.Data
                        };

                        return Ok(responseObj);
                    }
                    else if (claimsUser.Where(x => x.Name == "Customer").Count() > 0)
                    {
                        responseObj = new
                        {
                            TokenInfo = result.Data,
                            UserClaim = "Customer",
                            UserInfo = userToLogin.Data
                        };

                        return Ok(responseObj);
                    }
                    else return Ok(result.Data);
                }
                else
                {
                    return Ok(result.Data);
                }

            }

            return BadRequest(result.Message);
        }
    }
}
