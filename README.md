# VelureShop

An ASP.NET web application for an online clothing store with an admin panel for managing products, categories, orders, and users.

---

## Features

- Browse and shop men's and women's clothing products
- User registration and login
- Shopping cart and checkout
- Order and payment tracking
- Admin panel to create, edit, and manage products
- Category management
- Address and user management

---

## Technologies Used

- ASP.NET (C#)
- SQL Server
- HTML / CSS
- Microsoft SQL Server Management Studio (SSMS)

---

## Getting Started

### Prerequisites

- Visual Studio
- SQL Server
- SQL Server Management Studio (SSMS)

### Database Setup

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your local SQL Server instance
3. Open the file `VelureShop.sql` from the project folder
4. Click **Execute** to run the script
5. The **VelureShop** database will be created with all tables and data

### Running the Project

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/VelureShop.git
   ```
2. Open the project in **Visual Studio**
3. Update the database connection string in `Web.config` to match your SQL Server instance:
   ```xml
   <connectionStrings>
     <add name="VelureShopDB" connectionString="Server=YOUR_SERVER_NAME;Database=VelureShop;Integrated Security=True;" />
   </connectionStrings>
   ```
4. Build and run the project

---

## Database Tables

| Table | Description |
|---|---|
| Users | Registered customer accounts |
| Products | Store products with price and stock |
| Categories | Product categories |
| Cart | Customer shopping carts |
| CartItems | Individual items in a cart |
| Orders | Customer orders |
| OrderItems | Items within each order |
| Payments | Payment records |
| Addresses | Customer delivery addresses |

---

## Admin Panel

The admin panel allows store managers to:

- Add, edit, and delete products
- Upload product images to `/Images/products/`
- Manage categories
- View orders and users

---

## Notes

- Product images should be placed in the `/Images/products/` folder
- The database file `VelureShop.sql` must be run before launching the application

---

## Author

Developed as part of a web development project.
