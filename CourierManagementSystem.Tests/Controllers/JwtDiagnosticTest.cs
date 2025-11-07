using CourierManagementSystem.Api.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Xunit;

namespace CourierManagementSystem.Tests.Controllers;

public class JwtDiagnosticTest : BaseIntegrationTest
{
    public JwtDiagnosticTest(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public void Diagnostic_CheckJwtSettings()
    {
        // Get JWT settings from DI
        var scope = Factory.Services.CreateScope();
        var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;

        // Output settings
        Console.WriteLine($"JWT Secret: {jwtSettings.Secret}");
        Console.WriteLine($"JWT Issuer: {jwtSettings.Issuer}");
        Console.WriteLine($"JWT Audience: {jwtSettings.Audience}");
        Console.WriteLine($"JWT ExpirationMinutes: {jwtSettings.ExpirationMinutes}");

        Assert.NotNull(jwtSettings.Secret);
        Assert.NotEmpty(jwtSettings.Secret);
    }

    [Fact]
    public void Diagnostic_DecodeTokens()
    {
        var handler = new JwtSecurityTokenHandler();

        // Decode admin token
        var adminJwt = handler.ReadJwtToken(AdminToken);
        Console.WriteLine($"\nAdmin Token:");
        Console.WriteLine($"  Issuer: {adminJwt.Issuer}");
        Console.WriteLine($"  Audience: {string.Join(", ", adminJwt.Audiences)}");
        Console.WriteLine($"  Claims:");
        foreach (var claim in adminJwt.Claims)
        {
            Console.WriteLine($"    {claim.Type}: {claim.Value}");
        }

        // Decode manager token
        var managerJwt = handler.ReadJwtToken(ManagerToken);
        Console.WriteLine($"\nManager Token:");
        Console.WriteLine($"  Issuer: {managerJwt.Issuer}");
        Console.WriteLine($"  Audience: {string.Join(", ", managerJwt.Audiences)}");
        Console.WriteLine($"  Claims:");
        foreach (var claim in managerJwt.Claims)
        {
            Console.WriteLine($"    {claim.Type}: {claim.Value}");
        }

        // Decode courier token
        var courierJwt = handler.ReadJwtToken(CourierToken);
        Console.WriteLine($"\nCourier Token:");
        Console.WriteLine($"  Issuer: {courierJwt.Issuer}");
        Console.WriteLine($"  Audience: {string.Join(", ", courierJwt.Audiences)}");
        Console.WriteLine($"  Claims:");
        foreach (var claim in courierJwt.Claims)
        {
            Console.WriteLine($"    {claim.Type}: {claim.Value}");
        }
    }

    [Fact]
    public async Task Diagnostic_TestAuthEndpoint()
    {
        var response = await GetWithAuthAsync("/auth/debug", CourierToken);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"\nAuth Debug Response (Courier Token):");
        Console.WriteLine($"  Status: {response.StatusCode}");
        Console.WriteLine($"  Content: {content}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
