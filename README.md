# QR Coupon Redemption & Wallet System

## Overview

This project is a backend system where users can redeem QR-based coupons and their wallet balance gets updated.
The system is designed to handle real-world challenges like duplicate requests, concurrency, and partial failures.

---

## Features

* Coupon validation (valid, expired, already redeemed)
* Wallet balance update
* Idempotent API handling (prevents duplicate requests)
* Concurrency-safe coupon redemption
* Transaction logging (success + failure)
* Reconciliation API to fix inconsistencies

---

## 🧠 System Design Highlights

* Uses **IdempotencyKey** to avoid duplicate processing
* Handles **concurrent requests** safely using atomic updates
* Uses **database transactions (ACID)** for consistency
* Logs **all outcomes** (success + failure) for audit
* Includes **reconciliation API** to recover from failures

---

## 🛠 Tech Stack

* .NET 8 Web API
* Entity Framework Core
* SQL Server

---

## Database Design

### Tables:

* Users → stores user data
* Wallets → stores wallet balance (1 user = 1 wallet)
* Coupons → stores coupon details
* Transactions → logs all attempts (success + failure)
* Campaigns → groups coupons

---

## APIs

### Redeem Coupon

POST /api/coupons/redeem

Request:
{
"userId": 1,
"qrCodeValue": "ABC123",
"idempotencyKey": "unique-guid"
}

---

### Get Wallet Balance

GET /api/wallet/{userId}

---

### Reconciliation

POST /api/admin/reconcile

---

## Edge Cases Handled

* Duplicate API requests
* Concurrent coupon redemption
* Expired coupons
* Already redeemed coupons
* Partial failure scenarios

---

## Setup Instructions

1. Clone the repository
2. Update connection string in appsettings.json
3. Run database scripts
4. Run the project
5. Test APIs using Swagger/Postman

---

## Documentation

* Decision Log → DECISION_LOG.md
* Failure Strategy → FAILURE_STRATEGY.md

---

## Demo

https://www.loom.com/share/5cdaf47ae45a4d06be5a05c535dff4e3

---

## Author

Sakshee Dubey
