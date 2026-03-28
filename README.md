# Savour Table

> *A modern restaurant ordering & management platform — à la carte for the digital age.*

A full-stack Razor Pages app built with **.NET 10**. Customers browse the menu, add items to a cart, and place orders. Staff manage menu content via Admin pages and process live orders via Operations pages. The UI supports **Light/Dark mode** with a persistent theme toggle.

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 10 + ASP.NET Core Razor Pages |
| ORM | Entity Framework Core (SQL Server / SQLite) |
| Auth | ASP.NET Core Identity |
| UI | Bootstrap 5 + Bootswatch (Lux) |
| JS Libraries | DataTables, SweetAlert2, Toastr, TinyMCE |

## Key Features

**Customer**
- Browse menu by category with a per-category carousel
- Featured items carousel on the home page
- Item detail page with reviews and add-to-cart
- Cart, checkout, and order history
- Favourites and personal reviews

**Admin** *(Manager role)*
- Manage Categories, Food Types, and Menu Items
- Rich-text item editing via TinyMCE with image upload
- Bulk CSV import/export for menu items
- Moderate customer reviews

**Operations** *(Manager / Front Desk / Kitchen)*
- Live order queue with status filter pills
- Progress orders: Submitted → In Process → Ready
- Order detail view with complete, cancel, and refund actions

**UI**
- Persistent Light/Dark mode toggle
- Dark-mode-safe styling across all admin and operations pages

## Project Structure

```
RestaurantFinal/          # Razor Pages web app
├── Pages/
│   ├── Customer/         # Customer-facing pages
│   ├── Admin/            # Admin + Operations pages
│   └── Shared/           # Layout and partials
├── wwwroot/
│   ├── css/site.css      # Theme tokens and utilities
│   └── js/               # UI scripts (theme toggle, DataTables, etc.)
Restaurant.Data/          # EF Core, repositories, UnitOfWork
Restaurant.Utility/       # Shared constants (roles, statuses)
```

## Architecture

The solution follows an **N-tier architecture** split across four projects:

| Tier | Project | Responsibility |
|---|---|---|
| Presentation | `RestaurantFinal` | Razor Pages, UI, controllers |
| Data Access | `Restaurant.Data` | EF Core DbContext, repositories, migrations |
| Domain Models | `Restaurant.Models` | Entity classes (MenuItem, Order, Review, etc.) |
| Utilities | `Restaurant.Utility` | Shared constants (roles, order statuses) |

**Design patterns & principles used:**

- **Repository Pattern** — A generic `IRepository<T>` base interface provides common CRUD operations. Each entity (Category, MenuItem, Order, etc.) has its own typed interface and implementation, keeping data access concerns isolated from the presentation layer.
- **Unit of Work** — `IUnitOfWork` aggregates all repositories behind a single interface and exposes a `Save()` method to commit changes atomically. Page models depend only on `IUnitOfWork`, never on concrete repository classes.
- **Dependency Injection** — `IUnitOfWork` is registered as a scoped service in `Program.cs` (`builder.Services.AddScoped<IUnitOfWork, UnitOfWork>()`). Razor Page models receive it via constructor injection, keeping them loosely coupled and testable.
- **OOP principles** — Abstraction via interfaces, generics via the base repository, and inheritance via `ApplicationUser : IdentityUser` for extended Identity fields.

## Getting Started

1. Clone the repo and open the solution in Visual Studio.
2. Restore NuGet packages.
3. Set the startup project to `RestaurantFinal`.
4. Update the connection string in `appsettings.json`.
5. Apply EF Core migrations: `Update-Database` in Package Manager Console.
6. Run the app.

## Pages

### Customer

| Page | URL | Description |
|---|---|---|
| Home | `/` | Hero, featured items carousel, Why Choose Us |
| Menu | `/Customer/Menu` | All items grouped by category |
| Menu Item Details | `/Customer/Menu/Details?id=` | Image, description, price, reviews |
| Cart | `/Customer/Cart` | Cart contents, quantities, remove items |
| Checkout Summary | `/Customer/Cart/Summary` | Review order before placing |
| Order Confirmation | `/Customer/Cart/OrderConfirmation` | Post-checkout confirmation |
| My Orders | `/Customer/Order/MyOrders` | Customer order history |
| Order Details | `/Customer/Order/OrderDetails?id=` | Individual order summary |
| Favourites | `/Customer/Favorites` | Saved favourite menu items |
| My Reviews | `/Customer/Reviews/MyReviews` | Reviews the customer has written |

### Admin *(Manager role required)*

| Page | URL | Description |
|---|---|---|
| Categories — List | `/Admin/Categories` | All categories in a DataTables table |
| Categories — Create | `/Admin/Categories/Create` | Add a new category |
| Categories — Edit | `/Admin/Categories/Edit?id=` | Edit an existing category |
| Categories — Delete | `/Admin/Categories/Delete?id=` | Confirm delete |
| Food Types — List | `/Admin/FoodTypes` | All food types in a table |
| Food Types — Create | `/Admin/FoodTypes/Create` | Add a new food type |
| Food Types — Edit | `/Admin/FoodTypes/Edit?id=` | Edit a food type |
| Food Types — Delete | `/Admin/FoodTypes/Delete?id=` | Confirm delete |
| Menu Items — List | `/Admin/MenuItems` | DataTables list + CSV import/export |
| Menu Items — Create/Edit | `/Admin/MenuItems/Upsert` | Add or edit a menu item |
| Reviews — List | `/Admin/Reviews` | All customer reviews |
| Reviews — Edit | `/Admin/Reviews/Edit?id=` | Moderate/edit a review |
| Reviews — Delete | `/Admin/Reviews/Delete?id=` | Confirm delete |
| Register Employee | `/Identity/Account/Register` | Create a staff account |

### Operations *(Manager / Front Desk / Kitchen roles)*

| Page | URL | Description |
|---|---|---|
| Order List | `/Admin/Order/OrderList` | All orders with status filter pills |
| Manage Orders | `/Admin/Order/ManageOrder` | Progress orders through workflow |
| Order Details | `/Admin/Order/OrderDetails?id=` | Full order summary, complete/cancel/refund |

## Screenshots

### Not Signed In

#### Home Page
Public landing page with a hero banner, featured items carousel, and "Why Choose Us" section.

<img width="1916" height="1029" alt="image" src="https://github.com/user-attachments/assets/27f1cc88-087c-45a9-befb-1c5bbab62811" />
<img width="1911" height="1032" alt="image" src="https://github.com/user-attachments/assets/553cdd78-a7f7-4f19-8a7b-40b750923378" />

#### Menu Page
All menu items grouped by category with a Bootstrap carousel per category. Each card shows the item image, name, price, food type badge, star rating, and a favourites toggle.

<img width="1916" height="1033" alt="image" src="https://github.com/user-attachments/assets/c73b0284-d10a-49a1-bf1b-0bf68913f8be" />
<img width="1915" height="1029" alt="image" src="https://github.com/user-attachments/assets/2d51acb4-be28-4f5b-883c-c3b2507bbb1a" />
<img width="1916" height="1030" alt="image" src="https://github.com/user-attachments/assets/c34c44d9-3aeb-479e-9049-08b1cc23d9fe" />
<img width="1916" height="1031" alt="image" src="https://github.com/user-attachments/assets/48c92168-3dad-4c3e-93cf-3e5f3a966adc" />

#### Menu Item Details Page
Full detail view with image, description, category/food-type badges, price, quantity selector, and customer reviews.

<img width="1916" height="1030" alt="image" src="https://github.com/user-attachments/assets/6f5838a9-22a7-4c84-b704-54e092a000d2" />

---

### Login / Register

#### Login Page
Standard ASP.NET Core Identity login form. Returns the user to their previous page after signing in.

<img width="1917" height="1031" alt="image" src="https://github.com/user-attachments/assets/0b5613ef-b164-4331-b812-11b08e41738a" />

#### Register Page
Customer self-registration form. Staff accounts are created by a Manager via the Register Employee page.

<img width="1917" height="1028" alt="image" src="https://github.com/user-attachments/assets/2dc084ab-01c4-402d-a20e-051a4f4688d2" />

---

### Customer (Signed In)

#### Home Page
Navbar updates to show the cart icon, account menu, and role-based links. Content is identical to the public view.

<img width="1919" height="1032" alt="image" src="https://github.com/user-attachments/assets/9bed6817-3862-4191-9f92-eb3773a9a2f4" />

#### Cart Page
Lists cart items with image, name, category, unit price, and quantity controls. Shows a running order total.

<img width="1917" height="1030" alt="image" src="https://github.com/user-attachments/assets/b64e1a6d-7cc9-4f67-9739-35821a109999" />

#### Checkout Summary Page
Order review before placing. Shows contact details, requested pickup time, optional comments, and a final itemised total.

<img width="1917" height="1030" alt="image" src="https://github.com/user-attachments/assets/fc67d6dd-f9d0-4eeb-83fa-ec9982660b94" />

#### Order Confirmation Page
Shown after a successful order is placed. Displays the assigned order ID and a link to view order history.

<img width="1916" height="1030" alt="image" src="https://github.com/user-attachments/assets/59820e20-a31f-4af0-aa58-6aecdebbbd35" />

#### My Orders Page
Chronological list of all orders for the logged-in customer, showing order ID, date, total, and status.

<img width="1916" height="1030" alt="image" src="https://github.com/user-attachments/assets/0a18467c-27e0-4ca6-9cb6-c5c4399ddf88" />

#### Order Details Page
Full summary of a single order with pickup time, comments, and an itemised list. Each item thumbnail links back to its menu detail page.

<img width="1916" height="1028" alt="image" src="https://github.com/user-attachments/assets/48823ec2-191d-4e68-9749-9c17c7c79afe" />

#### Favourites Page
Saved items grouped by category in a carousel matching the Menu page. Heart button removes items from favourites.

<img width="1917" height="1030" alt="image" src="https://github.com/user-attachments/assets/fdae8fbe-fa4e-4585-89dc-f135fd14312b" />
<img width="1916" height="1032" alt="image" src="https://github.com/user-attachments/assets/8f0f2441-6ee7-4f3b-9c61-28280e93ffa4" />

#### My Reviews Page
All reviews submitted by the logged-in customer. Customers can edit or delete their own reviews.

<img width="1920" height="1031" alt="image" src="https://github.com/user-attachments/assets/5c0efb39-6957-4e1e-8ab2-50f4c03dfe79" />

---

### Admin *(Manager role)*

#### Categories List Page
DataTables table of all categories with create, edit, and delete actions.

<img width="1916" height="1028" alt="image" src="https://github.com/user-attachments/assets/27ce7efe-efc3-4072-bd3f-8b488a9921d7" />

#### Categories Create/Edit Page
Form to add or edit a category, including a display-order field that controls the order categories appear on the Menu page.

<img width="1916" height="1025" alt="image" src="https://github.com/user-attachments/assets/29ecf19b-043a-4269-a754-f2ce4fca31ef" />

#### Food Types List Page
Table of all food types (e.g. Vegetarian, Vegan, Gluten-Free) with create, edit, and delete actions.

<img width="1916" height="1029" alt="image" src="https://github.com/user-attachments/assets/c57539cf-7e79-41e5-b990-061837e1c3f5" />

#### Food Types Create/Edit Page
Form to add or edit a food type. Used to tag menu items for dietary filtering.

<img width="1916" height="1030" alt="image" src="https://github.com/user-attachments/assets/1ad3927f-10ab-4b57-96ef-95d0c2e42d67" />

#### Menu Items List Page
Full DataTables listing with search, sort, and pagination. Includes bulk CSV import and export.

<img width="1916" height="1029" alt="image" src="https://github.com/user-attachments/assets/a6565111-281e-4504-b63b-17cafc5c8fc6" />
<img width="1916" height="1030" alt="image" src="https://github.com/user-attachments/assets/ac01fb1e-c2c1-416e-9a33-b5895eda57ec" />

#### Menu Item Upsert Page
Combined create/edit form. Fields include name, description (TinyMCE rich-text), price, category, food type, and image upload.

<img width="1917" height="1029" alt="image" src="https://github.com/user-attachments/assets/ffc14342-9623-4a62-8f8f-2984d66a25b4" />
<img width="1917" height="1030" alt="image" src="https://github.com/user-attachments/assets/16a9668b-bd90-42b8-bded-55fb3e5bac11" />

#### Reviews List Page
All customer reviews across every menu item. Managers can edit or delete any review.

<img width="1915" height="1028" alt="image" src="https://github.com/user-attachments/assets/c1920f01-3e66-4662-be02-cd70a6d02386" />

#### Register Employee Page
Account creation form for staff. A Manager assigns the employee's role (Front Desk, Kitchen, or Manager) during registration.

<img width="1917" height="1027" alt="image" src="https://github.com/user-attachments/assets/01bbffcb-7295-46ee-8b50-e2a1f19a0f9f" />

---

### Operations *(Manager / Front Desk / Kitchen roles)*

#### Order List
All orders with status filter pills (Submitted, In Process, Ready, Completed, Cancelled).

<img width="1918" height="1028" alt="image" src="https://github.com/user-attachments/assets/d8153b7f-3434-40a9-8adc-3c4b858f778a" />

#### Order Details
Full order summary with customer info, pickup time, comments, and a clickable itemised list. Authorised roles can complete, cancel, or refund the order.

<img width="1916" height="1030" alt="image" src="https://github.com/user-attachments/assets/8d38b958-89b8-45e1-be8c-b49d37b12636" />

#### Manage Orders
Kitchen/front-desk view for progressing orders: Submitted → In Process → Ready. Action buttons adapt to the current status and the user's role.

<img width="1914" height="1028" alt="image" src="https://github.com/user-attachments/assets/c0a51106-ea41-42cc-af99-48f388e53769" />
<img width="1917" height="1030" alt="image" src="https://github.com/user-attachments/assets/d6095ef4-16e4-4a0c-bf58-f9926598b900" />

---

## Notes

- Role and status constants are defined in `Restaurant.Utility/SD.cs`.
- Do not commit real credentials or secret keys to source control.
