using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SavorHub.Models;
using Xunit;

namespace SavorHub.Models.Tests
{
    public class CategoryTests
    {
        [Fact]
        public void Category_IsInvalid_WhenNameIsNull()
        {
            // Arrange
            var model = new Category
            {
                Name = null,
                DisplayOrder = "1"
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(
                model,
                validationContext,
                validationResults,
                validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, result => result.MemberNames.Contains("Name"));
        }

        [Fact]
        public void Category_DisplayOrder_HasDisplayNameAttribute()
        {
            // Arrange
            var property = typeof(Category).GetProperty(nameof(Category.DisplayOrder));

            // Act
            var attribute = property?
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .Cast<DisplayAttribute>()
                .FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Display Order", attribute.Name);
        }

        [Fact]
        public void Category_DisplayOrder_HasRangeAttribute()
        {
            // Arrange
            var property = typeof(Category).GetProperty(nameof(Category.DisplayOrder));

            // Act
            var attribute = property?
                .GetCustomAttributes(typeof(RangeAttribute), false)
                .Cast<RangeAttribute>()
                .FirstOrDefault();

            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(1, attribute.Minimum);
            Assert.Equal(100, attribute.Maximum);
        }
    }
}
