using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace Restaurant.Models.Tests
{
    public class FoodTypeTests
    {
        [Fact]
        public void FoodType_IsInvalid_WhenNameIsNull()
        {
            // Arrange
            var model = new FoodType
            {
                Name = null
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
        public void FoodType_IsValid_WhenNameIsProvided()
        {
            // Arrange
            var model = new FoodType
            {
                Name = "Pizza"
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
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

    }

}
