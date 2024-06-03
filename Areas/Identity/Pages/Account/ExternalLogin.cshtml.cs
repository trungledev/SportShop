// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace SportShop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        public bool IsExistEmail { get; set; } = false;
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; }
        }
        //Duoc goi thong qua page cua ExternalLogin.cshtml
        public IActionResult OnGet() => RedirectToPage("./Login");
        //submit tai 2 trang dang nhap va dang ky => chuyen den trang xac thuc tu provider(fb, google)
        public async Task<IActionResult> OnPost(string provider, string returnUrl = null)
        {
            // Kiểm tra yêu cầu dịch vụ provider tồn tại
            var listProvider = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var providerProcess = listProvider.Find(x => x.Name == provider);
            if (providerProcess == null)
            {
                return NotFound("Dịch vụ không chính xác: " + provider);
            }

            // Request a redirect to the external login provider.
            // redirectUrl - là Url sẽ chuyển hướng đến - sau khi CallbackPath (/dang-nhap-tu-google) thi hành xong
            // nó bằng identity/account/externallogin?handler=Callback
            // tức là gọi OnGetCallbackAsync
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            // Cấu hình
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            //Chuyển đến dịch vụ ngoài(Google, Facebook)
            return new ChallengeResult(provider, properties);
        }

        //Duoc goi khi da ket thuc trang xac thuc tu provider
        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            // Lấy thông tin do dịch vụ ngoài chuyển đến
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi thông tin từ dịch vụ đăng nhập.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            //Đã có  user tồn tại trong database và đã tạo liên kết
            // User nào có 2 thông tin này sẽ được đăng nhập - thông tin này lưu tại bảng UserLogins của Database
            // Trường LoginProvider và ProviderKey ---> tương ứng UserId 
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // User đăng nhập thành công vào hệ thống theo thông tin info
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                // Bị tạm khóa
                return RedirectToPage("./Lockout");
            }
            else
            {
                var userExisted = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (userExisted != null)
                {
                    // Đã có Acount, đã liên kết với tài khoản ngoài - nhưng không đăng nhập được
                    // có thể do chưa kích hoạt email => chuyển hướng sang page RegisterConfirm
                    return RedirectToPage("./RegisterConfirmation", new { Email = userExisted.Email });
                }
                //Kiểm tra đã tồn tại account chứa email với tài khoản ngoài chưa
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var existEmail = await _userManager.FindByEmailAsync(email);
                if (existEmail != null)
                {
                    //Xác định phương án cho trường hợp đã tồn tại account chứa email từ dv ngoài
                    ViewData["StatusAccount"] = "Đã có tài khoản được tạo bạn có muốn liên kết với toàn khoản " + existEmail.Email + ". Muốn liên kết hãy nhấn nút Register nếu không hãy quay lại.";
                    IsExistEmail = true;
                    Input = new InputModel
                    {
                        Email = existEmail.Email,
                        FullName = existEmail.FullName
                    };

                }
                else
                {

                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    {
                        Input = new InputModel
                        {
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FullName = info.Principal.FindFirstValue(ClaimTypes.Name)
                        };
                    }
                }
                // Chưa có Account liên kết với tài khoản ngoài
                // Hiện thị form để thực hiện bước tiếp theo ở OnPostConfirmationAsync
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;

                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Nhận thông tin về người dùng từ nhà cung cấp đăng nhập bên ngoài
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi khi tải thông tin đăng nhập bên ngoài trong khi xác nhận.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                //Các trường hợp khi đăng nhập/ký từ dv ngoài 
                /*
                    1,Đã có tài khoản trong database nhưng chưa tạo liên kết
                    2,Tạo mới tài khoản từ dịch vụ ngoài 
                */
                //Tạo liên kết với tài khoản trong database

                //  Tìm user có email là email từ provider
                string externalEmail = null;
                //Kiểm tra trong privider có claim loại email ?
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                }
                //Kiểm tra có tồn tại user có externalEmail ?
                var userExternal = (externalEmail != null) ? (await _userManager.FindByEmailAsync(externalEmail)) : null;
                if (userExternal != null)
                {

                    if (!userExternal.EmailConfirmed)
                    {
                        var codeActive = await _userManager.GenerateEmailConfirmationTokenAsync(userExternal);
                        await _userManager.ConfirmEmailAsync(userExternal, codeActive);
                    }
                    //Có user có externalEmail => tạo liên kết
                    var resultAdd = await _userManager.AddLoginAsync(userExternal, info);
                    if (resultAdd.Succeeded)
                    {
                        //Tạo liên kết thành công => đăng nhập
                        await _signInManager.SignInAsync(userExternal, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(returnUrl);

                    }
                }

                //Tạo mới tài khoản từ dịch vụ ngoài 
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FullName = Input.FullName;
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //Set role default for user
                    string nameRoleDefault = "Guest";
                    var roleAdd = new IdentityRole(nameRoleDefault);
                    await _roleManager.CreateAsync(roleAdd);
                    await _userManager.AddToRoleAsync(user, nameRoleDefault);
                    //Tạo liên kết với _userManager.AddLoginAsync(user, info);
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        //Liên kết thành công
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        //Xác thực email
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        //Nếu cần xác nhận tài khoản, chúng tôi cần hiển thị liên kết nếu chúng tôi không có người gửi email thực
                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
