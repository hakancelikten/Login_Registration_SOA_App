using Business.Abstract;
using Core.Utilities.Security.Captcha;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    public class RegisterController : Controller
    {
        private IAuthService _authService;
        private IUserOperationClaimService _userOperationClaimService;
        private IOperationClaimService _operationClaimService;
        private IReferralLinkService _referralLinkService;

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
        public async Task<IActionResult> RegisterUserAsync(UserForRegisterDto userForRegisterDto)
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

                var operationClaim = _operationClaimService.Get("Customer");

                if (userForRegisterDto.ReferralLink != null)
                {
                    var referralLink = _referralLinkService.GetByLink(userForRegisterDto.ReferralLink);
                    if (referralLink != null)
                        operationClaim = _operationClaimService.Get("Manager");
                }

                var userExists = _authService.UserExists(userForRegisterDto.Email);
                if (!userExists.Success)
                {
                    return BadRequest(userExists.Message);
                }

                var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);

                _userOperationClaimService.Add(new UserOperationClaim()
                {
                    UserId = registerResult.Data.Id,
                    OperationClaimId = operationClaim.Id
                });

                var result = _authService.CreateAccessToken(registerResult.Data);
                if (result.Success)
                {
                    return Ok(result.Data);
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
