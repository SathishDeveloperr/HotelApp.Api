# HotelApp.Api - Usage Guide

## 1) Overview
HotelApp.Api is a Restaurant Management backend built with ASP.NET Core Web API and MongoDB.

Main modules:
- Authentication (register/login with JWT)
- Table management
- Reservation management
- Menu management
- Order management
- Payment processing

## 2) Prerequisites
- .NET SDK 9.0+
- Internet access to reach MongoDB Atlas
- MongoDB connection string configured in `appsettings.json`

## 3) Configuration
File: `D:\Freelancing\RestaurentManagement_ilavenil\RestaurentBackend\HotelApp.Api\appsettings.json`

Configured values:
- `MongoDbSettings:ConnectionString`
- `MongoDbSettings:DatabaseName`
- `JwtSettings:Issuer`
- `JwtSettings:Audience`
- `JwtSettings:SecretKey`
- `JwtSettings:ExpiryMinutes`

Important:
- Replace `JwtSettings:SecretKey` with a strong secret before production.
- Keep MongoDB credentials private.

## 4) Run the API
From terminal:

```powershell
dotnet run --project "D:\Freelancing\RestaurentManagement_ilavenil\RestaurentBackend\HotelApp.Api\HotelApp.Api.csproj"
```

Swagger (Development):
- `https://localhost:<port>/swagger`
- `http://localhost:<port>/swagger`

## 5) Authentication Flow
1. Register a user: `POST /api/Auth/register`
2. Login user: `POST /api/Auth/login`
3. Copy returned JWT token.
4. Authorize Swagger with:
   - Header: `Authorization`
   - Value: `Bearer <your_token>`

Endpoints except auth are protected with `[Authorize]`.

## 6) API Endpoints

### AuthController
- `POST /api/Auth/register`
- `POST /api/Auth/login`

Register request:
```json
{
  "userName": "manager1",
  "email": "manager1@example.com",
  "password": "Pass@123",
  "role": "Admin"
}
```

Login request:
```json
{
  "email": "manager1@example.com",
  "password": "Pass@123"
}
```

---

### TableController
- `GET /api/Table`
- `GET /api/Table/{id}`
- `POST /api/Table`
- `PATCH /api/Table/{id}/status?status=Available|Occupied|Reserved`

Create table request:
```json
{
  "tableNumber": 1,
  "capacity": 4,
  "status": "Available"
}
```

---

### ReservationController
- `GET /api/Reservation`
- `GET /api/Reservation/{id}`
- `POST /api/Reservation`
- `PATCH /api/Reservation/{id}/status?status=Pending|Confirmed|Cancelled`

Create reservation request:
```json
{
  "tableId": "<table_object_id>",
  "customerName": "Arun",
  "phoneNumber": "9876543210",
  "reservationTime": "2026-03-20T13:00:00Z",
  "numberOfGuests": 3,
  "notes": "Window seat"
}
```

---

### MenuController
- `GET /api/Menu`
- `GET /api/Menu/{id}`
- `POST /api/Menu`
- `PUT /api/Menu/{id}`
- `DELETE /api/Menu/{id}`

Create menu item request:
```json
{
  "name": "Veg Fried Rice",
  "category": "Main Course",
  "price": 180,
  "isAvailable": true
}
```

---

### OrderController
- `GET /api/Order`
- `GET /api/Order/{id}`
- `POST /api/Order`
- `PATCH /api/Order/{id}/status?status=Placed|Preparing|Completed|Paid`

Create order request:
```json
{
  "tableId": "<table_object_id>",
  "reservationId": "<reservation_object_id>",
  "items": [
    {
      "menuItemId": "<menu_item_id>",
      "quantity": 2
    }
  ]
}
```

---

### PaymentController
- `GET /api/Payment`
- `GET /api/Payment/{id}`
- `POST /api/Payment`

Create payment request:
```json
{
  "orderId": "<order_object_id>",
  "amount": 0,
  "paymentMethod": "Card",
  "status": "Completed"
}
```

Notes:
- If `amount <= 0`, API auto-uses order total.
- On payment success, order status is set to `Paid`.

## 7) Standard Response Format
All endpoints return:

```json
{
  "success": true,
  "message": "Success",
  "data": {}
}
```

Error example:

```json
{
  "success": false,
  "message": "Table not found.",
  "data": null
}
```

## 8) Common Workflow (Quick Start)
1. Register + login and get JWT.
2. Create tables.
3. Create menu items.
4. Create reservation for a table.
5. Create order using table + menu items.
6. Create payment for the order.

## 9) Production Recommendations
- Move secrets to environment variables or secret manager.
- Restrict CORS for known frontend domains.
- Add request validation attributes for DTOs.
- Add role-based policies (`Admin`, `Staff`, `Customer`).
- Add indexes in MongoDB for common queries (email, table number, reservation time).
