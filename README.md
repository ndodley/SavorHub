<div align="center">

# 🍽️ SavorHub

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
| **Customer** | Menu browsing by category, featured items carousel, item detail with reviews, quantity stepper cart controls, favourites, Stripe checkout, order history — cart, favourite, and review actions update instantly without a page reload |
| **Admin** *(Manager)* | CRUD for categories, food types, and menu items; TinyMCE rich-text editor; image upload; bulk CSV import/export; review moderation |
| **Operations** *(Manager / Front Desk / Kitchen)* | Live order queue with status filter pills; progress orders Submitted → In Process → Ready; complete, cancel, and refund actions |

---

## 🏗️ Architecture

Eight-project solution organized as:

| Project | Role |
|:---|:---|
| `SavorHub.Web` | Razor Pages presentation layer, controllers, app startup |
| `SavorHub.Data` | EF Core `DbContext`, repositories, Unit of Work |
| `SavorHub.Models` | Domain entities (`MenuItem`, `OrderHeader`, `Review`, ...) |
| `SavorHub.Utilities` | Shared role and status constants (`SD.cs`) |
| `SavorHub.Web.Tests` | xUnit tests for controllers (Moq) |
| `SavorHub.Data.Tests` | xUnit tests for repositories (EF Core InMemory) |
| `SavorHub.Models.Tests` | xUnit tests for domain entity validation |
| `SavorHub.Utilities.Tests` | xUnit tests for shared constants |

**Patterns used:** Repository Pattern · Unit of Work · Dependency Injection · `ApplicationUser : IdentityUser`

---

## 🚀 Getting Started

```bash
# 1. Clone and open the solution in Visual Studio
# 2. Set SavorHub as the startup project
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

<img width="1913" height="1030" alt="SavorHub home page" src="https://github.com/user-attachments/assets/a25e26af-09fa-4a41-9d52-dce1968a63d1" />

<img width="1917" height="1029" alt="SavorHub home page scrolled" src="https://github.com/user-attachments/assets/ca03ecfe-a0f2-449b-8d90-ee8adf6abeb6" />

<br>

**Menu page** — items grouped by category, per-category carousel, food type badge, star rating, favourites toggle, and an in-cart quantity stepper that updates instantly without reloading the page.

<img width="1915" height="1033" alt="SavorHub menu page" src="https://github.com/user-attachments/assets/1ba9ee32-90bf-40a9-8cf1-ee707bd6d99e" />

<img width="1917" height="1031" alt="SavorHub menu page category carousel" src="https://github.com/user-attachments/assets/9e4644c2-d066-4ee5-9088-e1a1fca5ecdc" />

<img width="1914" height="1027" alt="SavorHub menu page dark mode" src="https://github.com/user-attachments/assets/f1246d12-5e5c-46c8-b544-7a9f0394bd9d" />

<img width="1916" height="1031" alt="SavorHub menu page dark mode scrolled" src="https://github.com/user-attachments/assets/02246085-2bf2-465b-9edf-d92e46de429b" />

<br>

**Item details** — image, description, category / food-type badges, price, an in-cart quantity stepper, and customer reviews (multiple reviews per customer supported).

<img width="1913" height="1029" alt="SavorHub item details page" src="https://github.com/user-attachments/assets/3c2d8027-2c44-4e6e-8ef2-e491b30f969a" />

</details>

---

<details>
<summary><strong>🔐 Login &amp; Register</strong></summary>
<br>

<img width="1919" height="1030" alt="SavorHub login page" src="https://github.com/user-attachments/assets/651a2bdb-9fb1-4a31-9f8d-faecb070f8db" />

<img width="1917" height="1024" alt="SavorHub register page" src="https://github.com/user-attachments/assets/1b0df8b7-5652-4da2-9938-d27182d324cc" />

</details>

---

<details>
<summary><strong>🛒 Customer — Ordering Flow</strong></summary>
<br>

**Home (signed in)** — navbar shows cart icon, account menu, and role-based links.

<img width="1918" height="1030" alt="SavorHub signed-in home page" src="https://github.com/user-attachments/assets/cf44498c-1b43-454d-a141-2051ca824eb4" />

<br>

**Cart** — item list with quantity controls and running order total.

<img width="1917" height="1027" alt="SavorHub cart page" src="https://github.com/user-attachments/assets/b970ce2b-3d7f-47a3-8faa-6a48b44d2c40" />

<br>

**Order summary** — contact details, pickup time, optional comments, and itemised total before placing.

<img width="1918" height="1029" alt="SavorHub order summary page" src="https://github.com/user-attachments/assets/57226984-130b-4031-94ad-18889f6117af" />

<br>

**Stripe checkout** — card payment form.

<img width="1917" height="1029" alt="SavorHub Stripe checkout page" src="https://github.com/user-attachments/assets/db119ba6-b4be-4eed-8e06-10c74e639c4a" />

<br>

**Order confirmation** — assigned order ID, purchased items with clickable images, and links to order history.

<img width="1918" height="1030" alt="SavorHub order confirmation page" src="https://github.com/user-attachments/assets/a0477861-ec80-44d6-8ba1-781726d7e114" />

<img width="1917" height="1031" alt="SavorHub order confirmation purchased items" src="https://github.com/user-attachments/assets/c8c2667e-3d41-4d24-bd70-39265b04e780" />

</details>

---

<details>
<summary><strong>📦 Customer — Orders, Favourites &amp; Reviews</strong></summary>
<br>

**My orders** — chronological order history with ID, date, total, and status.

<img width="1920" height="1027" alt="SavorHub my orders page" src="https://github.com/user-attachments/assets/ca041a21-1dd5-423e-bc18-b3620af3c113" />

<br>

**Order details** — full summary with pickup time, comments, and itemised list. Thumbnails link to menu detail pages.

<img width="1916" height="1028" alt="SavorHub customer order details page" src="https://github.com/user-attachments/assets/b82b35b0-d955-4caf-aba8-d299c74d6d99" />

<br>

**Favourites** — saved items in a category carousel. Heart button removes an item instantly without reloading the page.

<img width="1914" height="1032" alt="SavorHub favourites page" src="https://github.com/user-attachments/assets/77de4d9c-2ead-40f3-a227-16976bb32bcd" />

<img width="959" height="517" alt="SavorHub favourites page dark mode" src="https://github.com/user-attachments/assets/bfead943-095a-4246-a28b-33da91e59da1" />

<br>

**My reviews** — all reviews written by the customer with edit and delete actions.

<img width="958" height="515" alt="SavorHub my reviews page" src="https://github.com/user-attachments/assets/ea37ea8d-4211-4e98-aa2a-35a4b7cc567f" />

</details>

---

<details>
<summary><strong>⚙️ Admin</strong> <em>(Manager role)</em></summary>
<br>

**Categories** — DataTables list and create/edit form with display-order control.

<img width="959" height="514" alt="SavorHub categories list page" src="https://github.com/user-attachments/assets/7b0a9999-fa3d-4271-907e-20939d3eff6b" />

<img width="959" height="515" alt="SavorHub category edit page" src="https://github.com/user-attachments/assets/61d5072e-6e24-4540-bffe-14fc26ff2183" />

<br>

**Food types** — manage dietary tags (Vegetarian, Vegan, Gluten-Free, etc.).

<img width="959" height="516" alt="SavorHub food types list page" src="https://github.com/user-attachments/assets/bbbe015b-b3ec-4f21-aa65-bf63c38d6735" />

<img width="958" height="515" alt="SavorHub food type edit page" src="https://github.com/user-attachments/assets/67cb80d3-c34a-416b-ba1e-e4e6338424b1" />

<br>

**Menu items** — searchable DataTables list with CSV import/export and a combined create/edit form (TinyMCE, image upload).

<img width="959" height="515" alt="SavorHub menu items list page" src="https://github.com/user-attachments/assets/30ae4702-b4fc-4d23-a1c6-ff3597352a4b" />

<img width="959" height="515" alt="SavorHub menu items CSV tools" src="https://github.com/user-attachments/assets/0a8d523c-f2ab-4477-8aeb-9fd6490fa5f0" />

<img width="959" height="515" alt="SavorHub menu item upsert page" src="https://github.com/user-attachments/assets/ae7e7737-3f2d-4ec5-8813-f648d081a7c3" />

<br>

**Reviews** — all customer reviews; Managers can edit or delete any entry.

<img width="959" height="515" alt="SavorHub admin reviews page" src="https://github.com/user-attachments/assets/7d660a43-b9f8-4f70-abe8-d0284a2bef10" />

<br>

**Register employee** — Manager creates staff accounts and assigns roles.

<img width="959" height="515" alt="SavorHub register employee page" src="https://github.com/user-attachments/assets/a53af9bf-c766-4a08-911b-0cad9badaaa8" />

</details>

---

<details>
<summary><strong>🧑‍🍳 Operations</strong> <em>(Manager / Front Desk / Kitchen)</em></summary>
<br>

**Order list** — all orders with status filter pills (Submitted, In Process, Ready, Completed, Cancelled).

<img width="958" height="515" alt="SavorHub operations order list page" src="https://github.com/user-attachments/assets/3db2805c-7ea3-47ca-bb57-174192117336" />

<br>

**Order details** — full summary with customer info, pickup time, and itemised list. Authorised roles can complete, cancel, or refund.

<img width="959" height="515" alt="SavorHub operations order details page" src="https://github.com/user-attachments/assets/ba951fbe-73b5-48ce-b023-b392a4ba5603" />

<br>

**Manage orders** — progress orders through the workflow. Action buttons adapt to the current status and the user's role.

<img width="959" height="515" alt="SavorHub manage orders page" src="https://github.com/user-attachments/assets/b9250488-3975-413a-a943-10c02253b0dd" />

<img width="958" height="515" alt="SavorHub manage orders in-process view" src="https://github.com/user-attachments/assets/549c4e04-1a11-4783-9033-7115c1dfc75f" />

</details>
