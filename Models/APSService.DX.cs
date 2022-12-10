
using RestSharp;

public partial class APSService
{
	public async Task<dynamic> getExchange(string exchangeFileUrn, Tokens tokens)
	{
		var client = new HttpClient();
		var request = new HttpRequestMessage
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri("https://developer.api.autodesk.com/exchange/v1/exchanges?filters=attribute.exchangeFileUrn%3D%3Durn%3Aadsk.wipprod%3Adm.lineage%3AZIYFqO1lSbej5yZvgYpOYw"),
			Headers =
		{
				{ "Authorization", $"Bearer {tokens.InternalToken}" },
		},
		};
		using (var response = await client.SendAsync(request))
		{
			response.EnsureSuccessStatusCode();
			var body = await response.Content.ReadAsStringAsync();
			return body;
		}
		//var client = new RestClient("https://developer.api.autodesk.com/exchange/v1/exchanges");
		//var request = new RestRequest("", Method.Get);
		//request.AddParameter("filters", $"attribute.exchangeFileUrn=={exchangeFileUrn}", ParameterType.QueryString, true);
		//request.AddHeader("Authorization", $"Bearer {tokens.InternalToken}");
		//var response = await client.ExecuteAsync(request);
		//return response;
	}
}

