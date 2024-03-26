
using System.Web;

public partial class APSService
{
	public async Task<dynamic> getExchange(string exchangeFileUrn, Tokens tokens)
	{
		var client = new HttpClient();
		string exchangeUrnParameter = $"=={exchangeFileUrn}";
		var request = new HttpRequestMessage
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri($"https://developer.api.autodesk.com/exchange/v1/exchanges?filters=attribute.exchangeFileUrn{HttpUtility.UrlEncode(exchangeUrnParameter)}"),
			Headers =
		{
				{ "Authorization", $"Bearer {tokens.AccessToken}" },
		},
		};
		using (var response = await client.SendAsync(request))
		{
			response.EnsureSuccessStatusCode();
			var body = await response.Content.ReadAsStringAsync();
			return body;
		}
	}
}

