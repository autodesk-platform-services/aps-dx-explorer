using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly ILogger<AuthController> _logger;
	private readonly APSService _forgeService;

	public AuthController(ILogger<AuthController> logger, APSService forgeService)
	{
		_logger = logger;
		_forgeService = forgeService;
	}

	public static async Task<Tokens> PrepareTokens(HttpRequest request, HttpResponse response, APSService forgeService)
	{
		if (!request.Cookies.ContainsKey("access_token"))
		{
			return null;
		}
		var tokens = new Tokens
		{
			AccessToken = request.Cookies["access_token"],
			RefreshToken = request.Cookies["refresh_token"],
			ExpiresAt = DateTime.Parse(request.Cookies["expires_at"])
		};
		if (tokens.ExpiresAt < DateTime.Now.ToUniversalTime())
		{
			tokens = await forgeService.RefreshTokens(tokens);
			response.Cookies.Append("access_token", tokens.AccessToken);
			response.Cookies.Append("refresh_token", tokens.RefreshToken);
			response.Cookies.Append("expires_at", tokens.ExpiresAt.ToString());
		}
		return tokens;
	}

	[HttpGet("login")]
	public ActionResult Login()
	{
		var redirectUri = _forgeService.GetAuthorizationURL();
		return Redirect(redirectUri);
	}

	[HttpGet("logout")]
	public ActionResult Logout()
	{
		Response.Cookies.Delete("access_token");
		Response.Cookies.Delete("refresh_token");
		Response.Cookies.Delete("expires_at");
		return Redirect("/");
	}

	[HttpGet("callback")]
	public async Task<ActionResult> Callback(string code)
	{
		var tokens = await _forgeService.GenerateTokens(code);
		Response.Cookies.Append("access_token", tokens.AccessToken);
		Response.Cookies.Append("refresh_token", tokens.RefreshToken);
		Response.Cookies.Append("expires_at", tokens.ExpiresAt.ToString());
		return Redirect("/");
	}

	[HttpGet("profile")]
	public async Task<dynamic> GetProfile()
	{
		var tokens = await PrepareTokens(Request, Response, _forgeService);
		if (tokens == null)
		{
			return Unauthorized();
		}
		var profile = await _forgeService.GetUserProfile(tokens);
		return new
		{
			name = profile.Name
		};
	}

	[HttpGet("token")]
	public async Task<dynamic> GetPublicToken()
	{
		var tokens = await PrepareTokens(Request, Response, _forgeService);
		if (tokens == null)
		{
			return Unauthorized();
		}
		return new
		{
			access_token = tokens.AccessToken,
			token_type = "Bearer",
			expires_in = Math.Floor((tokens.ExpiresAt - DateTime.Now.ToUniversalTime()).TotalSeconds)
		};
	}
}