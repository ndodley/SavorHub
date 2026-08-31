using System;
using System.Collections.Generic;
using System.Text;
using SavorHub.Utilities;
using Xunit;

namespace SavorHub.Utilities.Tests
{
    public class SDTests
    {
        [Fact]
        public void SD_ManagerRole_HasExpectedValue()
        {
            Assert.Equal("Manager", SD.ManagerRole);
        }

        [Fact]
        public void SD_FrontDeskRole_HasExpectedValue()
        {
            Assert.Equal("Front", SD.FrontDeskRole);
        }

        [Fact]
        public void SD_KitchenRole_HasExpectedValue()
        {
            Assert.Equal("Kitchen", SD.KitchenRole);
        }

        [Fact]
        public void SD_CustomerRole_HasExpectedValue()
        {
            Assert.Equal("Customer", SD.CustomerRole);
        }

        [Fact]
        public void SD_StatusPending_HasExpectedValue()
        {
            Assert.Equal("Pending_Payment", SD.StatusPending);
        }

        [Fact]
        public void SD_StatusSubmitted_HasExpectedValue()
        {
            Assert.Equal("Submitted_PaymentApproved", SD.StatusSubmitted);
        }

        [Fact]
        public void SD_StatusRejected_HasExpectedValue()
        {
            Assert.Equal("Rejected_Payment", SD.StatusRejected);
        }

        [Fact]
        public void SD_StatusInProcess_HasExpectedValue()
        {
            Assert.Equal("Being Prepared", SD.StatusInProcess);
        }

        [Fact]
        public void SD_StatusReady_HasExpectedValue()
        {
            Assert.Equal("Ready for Pickup", SD.StatusReady);
        }

        [Fact]
        public void SD_StatusCompleted_HasExpectedValue()
        {
            Assert.Equal("Completed", SD.StatusCompleted);
        }

        [Fact]
        public void SD_StatusCancelled_HasExpectedValue()
        {
            Assert.Equal("Cancelled", SD.StatusCancelled);
        }

        [Fact]
        public void SD_StatusRefunded_HasExpectedValue()
        {
            Assert.Equal("Refunded", SD.StatusRefunded);
        }
    }
}
