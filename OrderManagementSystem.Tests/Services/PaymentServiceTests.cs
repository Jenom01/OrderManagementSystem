using FluentAssertions;
using OrderManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementSystem.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly PaymentService _service = new();

        [Theory]
        [InlineData("CreditCard")]
        [InlineData("PayPal")]
        public async Task ProcessPaymentAsync_ShouldReturnTrue_ForValidMethods(string method)
        {
            var result = await _service.ProcessPaymentAsync(method, 100);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnFalse_ForInvalidMethod()
        {
            var result = await _service.ProcessPaymentAsync("Cash", 100);
            result.Should().BeFalse();
        }
    }
}
