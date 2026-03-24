# SavourTable

> *A modern restaurant ordering & management platform — à la carte for the digital age.*

A full-stack Razor Pages app built with **.NET 10**. Customers browse the menu, add items to a cart, and place orders. Staff manage menu content via Admin pages and process live orders via Operations pages. The UI supports **Light/Dark mode** with a persistent theme toggle.

## Tech Stack

- **.NET 10** + **ASP.NET Core Razor Pages**
- **Entity Framework Core** (SQL Server / SQLite packages included)
- **ASP.NET Core Identity**
- **Bootstrap 5** + Bootswatch (Lux)
- JavaScript libraries used in the project: DataTables, SweetAlert2, Toastr, TinyMCE

## Key Features

### Customer

- Browse menu by categories
- Featured items carousel on the home page
- Menu item details page
- Cart / checkout flow
- Favorites and reviews (where enabled)

### Admin

- Manage Categories
- Manage Food Types
- Manage Menu Items (including CSV upload/download tooling)
- Manage Reviews

### Operations

- Order list with status filters
- Manage orders (start cooking, mark ready, cancel)
- Order details (complete, cancel, refund based on role/status)

### UI / Theme

- Global Light/Dark mode toggle
- Dark-mode-safe styling for Admin/Operations tables, forms, and cards
- Modern card/table styling using shared CSS utilities

## Project Structure (high level)

- `RestaurantFinal/` - Razor Pages web app
  - `Pages/Customer/` - customer-facing pages (menu, cart, favorites, etc.)
  - `Pages/Admin/` - admin + operations pages
  - `wwwroot/css/site.css` - app theme tokens + utilities
  - `wwwroot/js/` - UI behavior scripts (theme toggle, DataTables scripts, etc.)
- `Restaurant.Data/` - data access (EF Core, repositories, UnitOfWork)
- `Restaurant.Utility/` - shared constants/utilities (roles, statuses, etc.)

## Getting Started (local)

> Update this section to match your environment (SQL Server vs SQLite) and how you run migrations/seeding.

1. Open the solution in Visual Studio.
2. Restore NuGet packages.
3. Set the startup project to `RestaurantFinal`.
4. Configure the connection string in `appsettings.json`.
5. Apply EF Core migrations (if your project uses them).
6. Run the app.

## Suggested Screenshots

Store screenshots in `docs/screenshots/` and reference them from this README.

### Customer (Light + Dark)

1. Home page (Hero + Featured Carousel)
   - `docs/screenshots/home-light.png`
   - `docs/screenshots/home-dark.png`
2. Menu listing (cards + hover)
   - `docs/screenshots/menu-light.png`
   - `docs/screenshots/menu-dark.png`
3. Menu item details
   - `docs/screenshots/menu-details-light.png`
   - `docs/screenshots/menu-details-dark.png`
4. Cart
   - `docs/screenshots/cart-light.png`
   - `docs/screenshots/cart-dark.png`

### Admin (Dark mode emphasized)

5. Categories list + create/edit form
   - `docs/screenshots/admin-categories-dark.png`
   - `docs/screenshots/admin-category-form-dark.png`
6. Food Types list + create/edit form
   - `docs/screenshots/admin-foodtypes-dark.png`
   - `docs/screenshots/admin-foodtype-form-dark.png`
7. Menu Items list (DataTables) + Upsert form
   - `docs/screenshots/admin-menuitems-dark.png`
   - `docs/screenshots/admin-menuitem-upsert-dark.png`
8. Reviews list
   - `docs/screenshots/admin-reviews-dark.png`

### Operations (Dark mode emphasized)

9. Order list (status pills + table)
   - `docs/screenshots/ops-orderlist-dark.png`
10. Manage orders (start cooking / ready / cancel)
   - `docs/screenshots/ops-manageorders-dark.png`
11. Order details (summary + totals)
   - `docs/screenshots/ops-orderdetails-dark.png`

### Optional

12. Navbar showing theme toggle + dropdowns
   - `docs/screenshots/navbar-theme-toggle.png`
13. Mobile responsive view (DevTools device toolbar)
   - `docs/screenshots/mobile-menu.png`

## Notes

- Admin/Operations pages rely on role-based access (see `Restaurant.Utility/SD` for role/status constants).
- If you publish this repo publicly, do **not** commit real credentials or secret keys.
