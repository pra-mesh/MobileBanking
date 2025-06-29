using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MobileBanking.Application.Services;
using MobileBanking.Models.Request.ISmart;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace MobileBanking.Tests.Integration;

public class ISmartControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ISmartControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        
        // Add required headers
        _client.DefaultRequestHeaders.Add("tenant", "test-tenant");
        _client.DefaultRequestHeaders.Add("X-API-Key", "test-api-key");
    }

    [Fact]
    public async Task BalanceInquiry_WithMockedServices_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing services
                var balanceInquiryDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBalanceInquiry));
                if (balanceInquiryDescriptor != null)
                    services.Remove(balanceInquiryDescriptor);

                // Add mock
                var mockBalanceInquiry = new Mock<IBalanceInquiry>();
                services.AddSingleton(mockBalanceInquiry.Object);
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Add("tenant", "test-tenant");
        client.DefaultRequestHeaders.Add("X-API-Key", "test-api-key");

        var request = new BalanceInquiryRequest
        {
            accountNumber = "123456",
            branchId = "01"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/api/transaction/BalanceInquiry", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task BalanceInquiry_WithoutTenantHeader_ShouldReturnForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-Key", "test-api-key");

        var request = new BalanceInquiryRequest
        {
            accountNumber = "123456",
            branchId = "01"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/api/transaction/BalanceInquiry", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task BalanceInquiry_WithoutApiKey_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant", "test-tenant");

        var request = new BalanceInquiryRequest
        {
            accountNumber = "123456",
            branchId = "01"
        };

        // Act
        var response = await client.PostAsJsonAsync("/v1/api/transaction/BalanceInquiry", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}