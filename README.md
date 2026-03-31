<div align="center">

# 🍽️ Savour Table

A full-stack restaurant ordering and management platform built with **.NET 10** and ASP.NET Core Razor Pages.  
Customers browse the menu, add items to a cart, and pay via Stripe.  
Staff manage menu content and process live orders through role-based Operations pages.  
Supports persistent **Light / Dark mode**.

<br>

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Razor_Pages-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-SQL_Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Stripe](https://img.shields.io/badge/Stripe-Checkout-635BFF?logo=stripe&logoColor=white)
![Bootstrap 5](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white)

</div>

---

## 🛠️ Tech Stack

| Layer | Technology |
|:---|:---|
| Framework | .NET 10 · ASP.NET Core Razor Pages |
| ORM | Entity Framework Core (SQL Server) |
| Auth | ASP.NET Core Identity |
| Payments | Stripe Checkout |
| UI | Bootstrap 5 · Bootswatch Lux |
| JS | DataTables · SweetAlert2 · Toastr · TinyMCE |

---

## ✨ Features

| Area | Highlights |
|:---|:---|
| **Customer** | Menu browsing by category, featured items carousel, item detail with reviews, cart, Stripe checkout, order history, favourites |
| **Admin** *(Manager)* | CRUD for categories, food types, and menu items; TinyMCE rich-text editor; image upload; bulk CSV import/export; review moderation |
| **Operations** *(Manager / Front Desk / Kitchen)* | Live order queue with status filter pills; progress orders Submitted → In Process → Ready; complete, cancel, and refund actions |

---

## 🏗️ Architecture

Four-project N-tier solution:

| Project | Role |
|:---|:---|
| `RestaurantFinal` | Razor Pages presentation layer, controllers |
| `Restaurant.Data` | EF Core `DbContext`, Repository Pattern, Unit of Work |
| `Restaurant.Models` | Domain entities (`MenuItem`, `OrderHeader`, `Review`, …) |
| `Restaurant.Utility` | Shared role and status constants (`SD.cs`) |

**Patterns used:** Repository Pattern · Unit of Work · Dependency Injection · `ApplicationUser : IdentityUser`

---

## 🚀 Getting Started

```bash
# 1. Clone and open the solution in Visual Studio
# 2. Set RestaurantFinal as the startup project
# 3. Update the connection string in appsettings.json
# 4. Apply migrations (Package Manager Console)
Update-Database
# 5. Run
```

> ⚠️ Never commit real API keys or connection strings to source control.

---

## 📄 Pages

<details>
<summary><strong>Customer pages</strong></summary>
<br>

| Page | URL |
|:---|:---|
| Home | `/` |
| Menu | `/Customer/Menu` |
| Item Details | `/Customer/Menu/Details?id=` |
| Cart | `/Customer/Cart` |
| Checkout Summary | `/Customer/Cart/Summary` |
| Order Confirmation | `/Customer/Cart/OrderConfirmation` |
| My Orders | `/Customer/Order/MyOrders` |
| Order Details | `/Customer/Order/OrderDetails?id=` |
| Favourites | `/Customer/Favorites` |
| My Reviews | `/Customer/Reviews/MyReviews` |

</details>

<details>
<summary><strong>Admin pages</strong> <em>(Manager role)</em></summary>
<br>

| Page | URL |
|:---|:---|
| Categories | `/Admin/Categories` |
| Food Types | `/Admin/FoodTypes` |
| Menu Items | `/Admin/MenuItems` |
| Menu Item Create/Edit | `/Admin/MenuItems/Upsert` |
| Reviews | `/Admin/Reviews` |
| Register Employee | `/Identity/Account/Register` |

</details>

<details>
<summary><strong>Operations pages</strong> <em>(Manager / Front Desk / Kitchen)</em></summary>
<br>

| Page | URL |
|:---|:---|
| Order List | `/Admin/Order/OrderList` |
| Manage Orders | `/Admin/Order/ManageOrder` |
| Order Details | `/Admin/Order/OrderDetails?id=` |

</details>

---

## 📸 Screenshots

<details>
<summary><strong>🏠 Public — Home &amp; Menu</strong></summary>
<br>

**Home page** — hero banner, featured items carousel, Why Choose Us section.

<img width="1916" alt="Home page" src="https://github.com/user-attachments/assets/27f1cc88-087c-45a9-befb-1c5bbab62811" />

<img width="1911" alt="Home page scrolled" src="https://github.com/user-attachments/assets/553cdd78-a7f7-4f19-8a7b-40b750923378" />

<br>

**Menu page** — items grouped by category, per-category carousel, food type badge, star rating, and favourites toggle.

<img width="1916" alt="Menu" src="https://github.com/user-attachments/assets/c73b0284-d10a-49a1-bf1b-0bf68913f8be" />

<img width="1915" alt="Menu carousel" src="https://github.com/user-attachments/assets/2d51acb4-be28-4f5b-883c-c3b2507bbb1a" />

<img width="1916" alt="Menu dark mode" src="https://github.com/user-attachments/assets/c34c44d9-3aeb-479e-9049-08b1cc23d9fe" />

<img width="1916" alt="Menu dark mode scrolled" src="https://github.com/user-attachments/assets/48c92168-3dad-4c3e-93cf-3e5f3a966adc" />

<br>

**Item details** — image, description, category / food-type badges, price, quantity selector, and customer reviews.

<img width="1916" alt="Item details" src="https://github.com/user-attachments/assets/6f5838a9-22a7-4c84-b704-54e092a000d2" />

</details>

---

<details>
<summary><strong>🔐 Login &amp; Register</strong></summary>
<br>

<img width="1917" alt="Login" src="https://github.com/user-attachments/assets/0b5613ef-b164-4331-b812-11b08e41738a" />

<img width="1917" alt="Register" src="https://github.com/user-attachments/assets/2dc084ab-01c4-402d-a20e-051a4f4688d2" />

</details>

---

<details>
<summary><strong>🛒 Customer — Ordering Flow</strong></summary>
<br>

**Home (signed in)** — navbar shows cart icon, account menu, and role-based links.

<img width="1919" alt="Home signed in" src="https://github.com/user-attachments/assets/9bed6817-3862-4191-9f92-eb3773a9a2f4" />

<br>

**Cart** — item list with quantity controls and running order total.

<img width="1918" alt="Cart" src="https://github.com/user-attachments/assets/285e9093-2fce-4437-8a3d-a95a6fede635" />

<br>

**Order summary** — contact details, pickup time, optional comments, and itemised total before placing.

<img width="1918" alt="Order summary" src="https://github.com/user-attachments/assets/58185fc0-651f-4d98-8cf2-bf341434a335" />

<br>

**Stripe checkout** — card payment form.

<img width="1916" alt="Stripe checkout" src="https://github.com/user-attachments/assets/5c4b8ca8-1ab5-4be4-a898-f4da4b00a5f1" />

<br>

**Order confirmation** — assigned order ID, purchased items with clickable images, and links to order history.

<img width="1915" alt="Order confirmation" src="https://github.com/user-attachments/assets/7ee7de1e-b19d-4dc8-b707-0f53a7ccbef2" />

<img width="1915" alt="Order confirmation items" src="https://github.com/user-attachments/assets/fcb67e5f-e879-45ad-bc70-5adf8219436f" />

</details>

---

<details>
<summary><strong>📦 Customer — Orders, Favourites &amp; Reviews</strong></summary>
<br>

**My orders** — chronological order history with ID, date, total, and status.

<img width="1916" alt="My orders" src="https://github.com/user-attachments/assets/0a18467c-27e0-4ca6-9cb6-c5c4399ddf88" />

<br>

**Order details** — full summary with pickup time, comments, and itemised list. Thumbnails link to menu detail pages.

<img width="1916" alt="Order details" src="https://github.com/user-attachments/assets/48823ec2-191d-4e68-9749-9c17c7c79afe" />

<br>

**Favourites** — saved items in a category carousel. Heart button removes an item.

<img width="1917" alt="Favourites" src="https://github.com/user-attachments/assets/fdae8fbe-fa4e-4585-89dc-f135fd14312b" />

<img width="1916" alt="Favourites dark" src="https://github.com/user-attachments/assets/8f0f2441-6ee7-4f3b-9c61-28280e93ffa4" />

<br>

**My reviews** — all reviews written by the customer with edit and delete actions.

<img width="1920" alt="My reviews" src="https://github.com/user-attachments/assets/5c0efb39-6957-4e1e-8ab2-50f4c03dfe79" />

</details>

---

<details>
<summary><strong>⚙️ Admin</strong> <em>(Manager role)</em></summary>
<br>

**Categories** — DataTables list and create/edit form with display-order control.

<img width="1916" alt="Categories list" src="https://github.com/user-attachments/assets/27ce7efe-efc3-4072-bd3f-8b488a9921d7" />

<img width="1916" alt="Categories edit" src="https://github.com/user-attachments/assets/29ecf19b-043a-4269-a754-f2ce4fca31ef" />

<br>

**Food types** — manage dietary tags (Vegetarian, Vegan, Gluten-Free, etc.).

<img width="1916" alt="Food types list" src="https://github.com/user-attachments/assets/c57539cf-7e79-41e5-b990-061837e1c3f5" />

<img width="1916" alt="Food types edit" src="https://github.com/user-attachments/assets/1ad3927f-10ab-4b57-96ef-95d0c2e42d67" />

<br>

**Menu items** — searchable DataTables list with CSV import/export and a combined create/edit form (TinyMCE, image upload).

<img width="1916" alt="Menu items list" src="https://github.com/user-attachments/assets/a6565111-281e-4504-b63b-17cafc5c8fc6" />

<img width="1916" alt="Menu items CSV" src="https://github.com/user-attachments/assets/ac01fb1e-c2c1-416e-9a33-b5895eda57ec" />

<img width="1917" alt="Menu item upsert" src="https://github.com/user-attachments/assets/ffc14342-9623-4a62-8f8f-2984d66a25b4" />

<img width="1917" alt="Menu item upsert rich text" src="https://github.com/user-attachments/assets/16a9668b-bd90-42b8-bded-55fb3e5bac11" />

<br>

**Reviews** — all customer reviews; Managers can edit or delete any entry.

<img width="1915" alt="Reviews list" src="https://github.com/user-attachments/assets/c1920f01-3e66-4662-be02-cd70a6d02386" />

<br>

**Register employee** — Manager creates staff accounts and assigns roles.

<img width="1917" alt="Register employee" src="https://github.com/user-attachments/assets/01bbffcb-7295-46ee-8b50-e2a1f19a0f9f" />

</details>

---

<details>
<summary><strong>🧑‍🍳 Operations</strong> <em>(Manager / Front Desk / Kitchen)</em></summary>
<br>

**Order list** — all orders with status filter pills (Submitted, In Process, Ready, Completed, Cancelled).

<img width="1918" alt="Order list" src="https://github.com/user-attachments/assets/d8153b7f-3434-40a9-8adc-3c4b858f778a" />

<br>

**Order details** — full summary with customer info, pickup time, and itemised list. Authorised roles can complete, cancel, or refund.

<img width="1916" alt="Order details ops" src="https://github.com/user-attachments/assets/8d38b958-89b8-45e1-be8c-b49d37b12636" />

<br>

**Manage orders** — progress orders through the workflow. Action buttons adapt to the current status and the user's role.

<img width="1914" alt="Manage orders" src="https://github.com/user-attachments/assets/c0a51106-ea41-42cc-af99-48f388e53769" />

<img width="1917" alt="Manage orders in process" src="https://github.com/user-attachments/assets/d6095ef4-16e4-4a0c-bf58-f9926598b900" />

</details>
