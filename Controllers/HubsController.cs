using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class HubsController : ControllerBase
{
	private readonly ILogger<HubsController> _logger;
	private readonly APSService _apsService;

	public HubsController(ILogger<HubsController> logger, APSService forgeService)
	{
		_logger = logger;
		_apsService = forgeService;
	}

	[HttpGet("exchanges/{exchangeFileUrn}")]
	public async Task<ActionResult<string>> ListVersions(string exchangeFileUrn)
	{
		var tokens = await AuthController.PrepareTokens(Request, Response, _apsService);
		if (tokens == null)
		{
			return Unauthorized();
		}
		var exchangeInfo = await _apsService.getExchange(exchangeFileUrn, tokens);
		return JsonConvert.SerializeObject(exchangeInfo);
	}
}