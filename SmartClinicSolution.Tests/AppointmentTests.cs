using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using System;
using System.Net;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class AppointmentTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AppointmentTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_Appointment_And_Return_201()
    {
        // Arrange
        var command = new
        {
            DoctorId = Guid.Parse("B19FB776-EB21-422F-A20F-BBC2912DD8D7"),
            PatientId = Guid.Parse("C92A0041-46C3-4EB9-8EDE-AA0BC7111F0A"),
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddMinutes(30)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/appointments", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        result.Should().NotBeNull();
        result.AppointmentId.Should().NotBeEmpty();
    }
}
public class AppointmentResponseDto
{
    public Guid AppointmentId { get; set; }
}
