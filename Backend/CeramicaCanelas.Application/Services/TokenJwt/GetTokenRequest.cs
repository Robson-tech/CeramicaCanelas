using Microsoft.AspNetCore.Http;


namespace CeramicaCanelas.Application.Services.TokenJwt
{
    public class GetTokenRequest
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTokenRequest(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? GetToken()
        {
            var headers = _httpContextAccessor.HttpContext?.Request.Headers;

            if (headers != null && headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                var token = authorizationHeader.ToString();

                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return token["Bearer ".Length..].Trim();
                }
            }

            return null; // Retorna null se não houver token ou estiver malformado
        }

    }
}
