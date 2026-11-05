using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavorHub.Utilities
{
    public static class SD
    {
        public const string ManagerRole = "Manager";
        public const string FrontDeskRole = "Front";
        public const string KitchenRole = "Kitchen";
        public const string CustomerRole = "Customer";

        public const string StatusPending = "Pending_Payment";
        public const string StatusSubmitted = "Submitted_PaymentApproved";
        public const string StatusRejected = "Rejected_Payment";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";
        //public const string SessionCart = "SessionCart";

        // Shared placeholder used when a menu item has no image (e.g. CSV upload
        // without an image path). It lives outside images/menuItems and must never
        // be deleted when an individual menu item is removed or its image replaced.
        public const string DefaultMenuItemImage = "/images/no-image.svg";

        public static bool IsDefaultMenuItemImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return false;
            }

            return string.Equals(NormalizeImagePath(imagePath), DefaultMenuItemImage, StringComparison.OrdinalIgnoreCase);
        }

        // "\images\menuItems\x.png" and "images/menuItems/x.png" -> "/images/menuItems/x.png"
        public static string NormalizeImagePath(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return string.Empty;
            }

            return "/" + imagePath.Replace('\\', '/').TrimStart('/');
        }

    }
}
