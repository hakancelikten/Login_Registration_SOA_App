using Business.Abstract;
using Core.Utilities.Security.Captcha;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    public class LoginController : Controller
    {
        private IAuthService _authService;
        private IUserService _userService;


        public LoginController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> LoginUserAsync(UserForLoginDto userForLoginDto)
        {

            var captchaImage = HttpContext.Request.Form["g-recaptcha-response"];

            if (string.IsNullOrEmpty(captchaImage))
            {
                return BadRequest("Captcha doğrulanamamıştır");
            }

            var verified = await CaptchaControl.CheckCaptcha(HttpContext.Connection.RemoteIpAddress.ToString(), captchaImage);

            if (!verified)
            {
                return BadRequest("Captcha yanlış doğrulanmış");
            }

            if (ModelState.IsValid)
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
            else
            {
                return BadRequest("Captcha doğrulanamamıştır.");
            }

        }
    }
}
