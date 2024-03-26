using Autodesk.Authentication;
using Autodesk.Authentication.Model;

public partial class APSService
{
	public string GetAuthorizationURL()
	{
		var authenticationClient = new AuthenticationClient(_sdkManager);
        return authenticationClient.Authorize(_clientId, ResponseType.Code, _callbackUri, TokenScopes);
	}

	public async Task<Tokens> GenerateTokens(string code)
	{
		var authenticationClient = new AuthenticationClient(_sdkManager);
		var auth = await authenticationClient.GetThreeLeggedTokenAsync(_clientId, _clientSecret, code, _callbackUri);
		return new Tokens
		{
			AccessToken = auth.AccessToken,
			RefreshToken = auth.RefreshToken,
			ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds((double)auth.ExpiresIn)
		};
	}

	public async Task<Tokens> RefreshTokens(Tokens tokens)
	{
		var authenticationClient = new AuthenticationClient(_sdkManager);
		var auth = await authenticationClient.GetRefreshTokenAsync(_clientId, _clientSecret, tokens.RefreshToken, TokenScopes);
		return new Tokens
		{
			AccessToken = auth.AccessToken,
			RefreshToken = auth._RefreshToken,
			ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds((double)auth.ExpiresIn)
		};
	}

	public async Task<UserInfo> GetUserProfile(Tokens tokens)
	{
		var authenticationClient = new AuthenticationClient(_sdkManager);
        UserInfo userInfo = await authenticationClient.GetUserInfoAsync(tokens.AccessToken);
        return userInfo;
	}
}