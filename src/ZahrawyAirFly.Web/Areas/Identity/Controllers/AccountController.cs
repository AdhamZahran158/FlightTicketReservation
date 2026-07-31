using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZahrawyAirFly.Domain.Entities;
using ZahrawyAirFly.Domain.Interfaces;
using ZahrawyAirFly.Infrastructure.Repositories;
using ZahrawyAirFly.Infrastructure.Utilities;
using ZahrawyAirFly.Web.Models;
using ZahrawyAirFly.Web.ViewModels;

namespace ZahrawyAirFly.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class AccountController : Controller
{
    private readonly UserManager<Tenant> _userManager;
    private readonly SignInManager<Tenant> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<Otp> _applicationUserOTPRepository;

    public AccountController(
        UserManager<Tenant> userManager,
        SignInManager<Tenant> signInManager,
        IEmailSender emailSender,
        IRepository<Otp> applicationUserOTPRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _applicationUserOTPRepository = applicationUserOTPRepository;
    }

    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpVM model)
    {
        try
        {

        if (!ModelState.IsValid)
            return View(model);

        var user = new Tenant
        {
            Name = model.Name,
            CompanyName = model.CompanyName,
            Subdomain = model.Subdomain,
            Currency = model.Currency,
            Email = model.Email,
            UserName = model.UserName,
            PhoneNumber = model.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            PassportNumber = model.Passport
        };

        var result = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, SD.USER_ROLE);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            TempData["error"] = string.Join("<br>", result.Errors);

            return View(model);
        }

        // Send Email Confirmation
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);

        await _emailSender.SendEmailAsync(user.Email, "ZahrawyAirFly - Please Confirm Your Email",
            $"<h1>Welcome to ZahrawyAirFly!</h1>" +
            $"<p>Please confirm your email by clicking <a href='{link}'>here</a></p>" +
            $"<p>Your subdomain: <strong>{user.Subdomain}</strong></p>" +
            $"<p>Your company: <strong>{user.CompanyName}</strong></p>");

        TempData["success"] = "Account created successfully! Please check your email to confirm your account.";

        //await _userManager.AddToRoleAsync(user, "TenantAdmin");

        return RedirectToAction(nameof(Login));
        }
        catch(Exception ex)
        {
            TempData["error"] = ex.ToString();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            TempData["error"] = string.Join(", ", result.Errors.Select(e => e.Description));
        }
        else
        {
            TempData["success"] = "Email confirmed successfully! You can now login.";
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult VerifyEmail()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyEmail(VerifyEmailVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            TempData["error"] = "User not found.";
            return View(model);
        }

        if (user.EmailConfirmed)
        {
            TempData["info"] = "Email already confirmed. Please login.";
            return RedirectToAction(nameof(Login));
        }

        // Generate and send OTP
        var otp = new Random().Next(100000, 999999).ToString();

        await _applicationUserOTPRepository.AddAsync(new Otp
        {
            OTP = otp,
            TenantId = user.Id,
            IsValid = true,
            ValidTo = DateTime.UtcNow.AddMinutes(15),
            CreatedAt = DateTime.UtcNow
        });
        await _applicationUserOTPRepository.CommitAsync();

        await _emailSender.SendEmailAsync(user.Email, "ZahrawyAirFly - Email Verification OTP",
            $"<h1>Email Verification</h1>" +
            $"<p>Your OTP code is: <strong>{otp}</strong></p>" +
            $"<p>This code will expire in 15 minutes.</p>" +
            $"<p>Please enter this code to verify your email address.</p>");

        TempData["success"] = "Verification code sent to your email.";
        return RedirectToAction(nameof(ValidateOTP), new { userId = user.Id, purpose = "email" });
    }

    [HttpGet]
    public IActionResult ValidateOTP(string userId, string purpose)
    {
        if (string.IsNullOrEmpty(userId))
            return NotFound();

        ViewBag.Purpose = purpose;
        return View(new ValidateOtpVM { UserId = userId });
    }

    [HttpPost]
    public async Task<IActionResult> ValidateOTP(ValidateOtpVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user is null)
            return NotFound();

        var otpRecord = await _applicationUserOTPRepository.GetOneAsync(e =>
            e.TenantId == user.Id &&
            e.OTP == model.OTP &&
            e.IsValid &&
            e.ValidTo > DateTime.UtcNow);

        if (otpRecord is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid or expired OTP code.");
            return View(model);
        }

        // Mark OTP as used
        otpRecord.IsValid = false;
        await _applicationUserOTPRepository.CommitAsync();

        // If purpose is email verification
        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            TempData["success"] = "Email verified successfully! You can now login.";
            return RedirectToAction(nameof(Login));
        }

        // For password reset
        return RedirectToAction(nameof(ResetPassword), new { userId = user.Id });
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByNameAsync(model.UsernameOrEmail) ??
                   await _userManager.FindByEmailAsync(model.UsernameOrEmail);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
            TempData["error"] = "Invalid username/email or password.";
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Too many failed attempts. Please try again later.");
                TempData["error"] = "Account locked. Please try again later.";
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Please confirm your email before logging in.");
                TempData["error"] = "Email not confirmed. Please check your email for verification link.";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
                TempData["error"] = "Invalid username/email or password.";
            }
            return View(model);
        }
        if (!string.IsNullOrEmpty(ReturnUrl))
        {
            return Redirect(ReturnUrl);
        }
        if (await _userManager.IsInRoleAsync(user,SD.ADMIN_ROLE))
        {
            return RedirectToAction("Index", "Home", new
            {
                area="Admin"
            });
        }
        TempData["success"] = $"Welcome back, {user.CompanyName}!";
        return RedirectToAction("Index", "Home", new { area = "User" });
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            // Don't reveal that the user doesn't exist
            TempData["InfoMessage"] = "If an account with this email exists, a password reset link will be sent.";
            return RedirectToAction(nameof(Login));
        }

        // Check for too many OTP requests (max 3 per 24 hours)
        var recentOTPs = await _applicationUserOTPRepository.CountAsync(e =>
            e.TenantId == user.Id &&
            e.CreatedAt > DateTime.UtcNow.AddHours(-24));

        if (recentOTPs > 5)  
        {
            TempData["error"] = "Too many password reset attempts. Please try again later.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();

        await _applicationUserOTPRepository.AddAsync(new Otp
        {
            OTP = otp,
            TenantId = user.Id,
            IsValid = true,
            ValidTo = DateTime.UtcNow.AddMinutes(15),
            CreatedAt = DateTime.UtcNow
        });
        await _applicationUserOTPRepository.CommitAsync();

        await _emailSender.SendEmailAsync(user.Email, "ZahrawyAirFly - Password Reset OTP",
            $"<h1>Password Reset Request</h1>" +
            $"<p>Your OTP code is: <strong>{otp}</strong></p>" +
            $"<p>This code will expire in 15 minutes.</p>" +
            $"<p>Please enter this code to reset your password.</p>" +
            $"<p>If you didn't request this, please ignore this email.</p>");

        TempData["success"] = "Password reset code sent to your email.";
        return RedirectToAction(nameof(ValidateOTP), new { userId = user.Id, purpose = "reset" });
    }

    [HttpGet]
    public IActionResult ResetPassword(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return NotFound();

        return View(new ResetPasswordVM { UserId = userId });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user is null)
            return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        TempData["success"] = "Password reset successfully! You can now login with your new password.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["success"] = "You have been logged out.";
        return RedirectToAction(nameof(Login));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}