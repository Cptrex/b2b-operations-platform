using Microsoft.AspNetCore.Mvc;

namespace Platform.Auth.Business.Api;

[ApiController]
[Route("api/v1/keys")]
public class KeysController : ControllerBase
{
    private readonly IConfiguration _config;

    public KeysController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("client-public")]
    public IActionResult GetClientPublicKey()
    {
        var publicKeyPath = _config["ClientJwt:PublicKeyPath"] ?? "business_public.pem";

        if (!System.IO.File.Exists(publicKeyPath))
        {
            return NotFound("Client public key not found");
        }

        var key = System.IO.File.ReadAllText(publicKeyPath);

        return Ok(key);
    }
}
