using FluentAssertions;
using OrderManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementSystem.Tests.Services
{
    public class DiscountServiceTests
    {
        private readonly DiscountService _service = new();

        [Fact]
        public void ApplyDiscount_ShouldReturn10Percent_WhenAbove200()
        {
            var result = _service.ApplyDiscount(300);
            result.Should().Be(30);
        }

        [Fact]
        public void ApplyDiscount_ShouldReturn5Percent_WhenBetween100And200()
        {
            var result = _service.ApplyDiscount(150);
            result.Should().Be(7.5m);
        }

        [Fact]
        public void ApplyDiscount_ShouldReturnZero_WhenBelow100()
        {
            var result = _service.ApplyDiscount(50);
            result.Should().Be(0);
        }
    }
}
