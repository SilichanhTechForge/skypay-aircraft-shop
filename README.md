# ✈️ Skypay — Aircraft Sales & Leasing Portal

A full-stack **F# web application** for browsing, searching, purchasing, and leasing commercial aircraft. Built using functional programming principles including **Lambda Calculus**, **Units of Measure**, and **Higher-Order Functions**.

**Created by: Silichanh SIPHANH**

> 🔗 **Live Demo:** [https://skypay-aircraft-shop.onrender.com](https://skypay-aircraft-shop.onrender.com)

---

## 🎯 Project Motivation & Real-World Problem (Grade 5 Criteria)

**The Problem:** Airlines in Southeast Asia (Laos, Cambodia, Myanmar) face highly seasonal travel demand. However, traditional Western aircraft manufacturers (like Boeing or Airbus) and major lessors force airlines into rigid, multi-year ownership or leasing contracts with heavy cancellation penalties. Furthermore, pricing is exclusively in USD, which exposes local operators to severe currency fluctuation risks (e.g., Lao Kip vs. USD).

**The Skypay Solution:** Skypay is a specialized B2B marketplace built explicitly for the Southeast Asian market. It solves this problem by offering:
1. **On-Demand Flexible Leasing:** Airlines can request short-term leases (e.g., just for the Nov-Apr tourism peak) with zero cancellation penalties before contract signing.
2. **Dual-Currency Type Safety:** Pricing is natively tracked in USD and converted to local currencies (LAK, SGD, etc.) in real-time. By utilizing **F# Units of Measure**, the backend mathematically guarantees that USD and LAK values are never erroneously mixed, ensuring flawless financial calculations.

Skypay delivers a polished, responsive, and professional B2B user experience that bridges the gap between massive aerospace manufacturers and regional airlines.

---

## 🖼 Screenshots

<img width="1214" height="867" alt="Dashboard" src="https://github.com/user-attachments/assets/9c1a0960-8032-4f73-b706-c40e21811b59" />

<img width="1206" height="916" alt="Checkout" src="https://github.com/user-attachments/assets/46ceb207-66e7-4ff7-946d-e82c4e53ab7d" />

---

## 📚 Course Material Integration (Grade 5 Demonstration)

This project heavily utilizes the core concepts taught throughout the "Introduction to Functional Programming in F#" course. Below is a direct mapping of the lecture materials to the codebase:

### 1. `02-ProgrammingParadigms.md` (Functional Paradigm)
Instead of using Object-Oriented `class` structures with hidden mutable state, this application enforces a strict Functional Paradigm:
- **Immutable Records:** The `Aircraft` type (`Models.fs:L24`) is an immutable record.
- **Discriminated Unions (DUs):** The `DeliveryStatus` type (`Models.fs:L132`) uses DUs rather than Enums, allowing for exhaustive pattern matching that the compiler verifies.

### 2. `03-ValuesAndFunctions.md` (Values, Lambdas, and Higher-Order Functions)
The core pricing engine in `Program.fs` (Line 21) is built entirely on these concepts:
- **Lambda Calculus:** `let applyDiscount = fun rate -> fun price -> ...`
- **Currying & Partial Application:** `let vipDiscount = applyDiscount 0.20m` creates a new function by partially applying the rate.
- **Higher-Order Functions:** `getDiscountFn` takes a promo code string and *returns a lambda function* to be executed later.

### 3. `04-WorkingWithLists.md` (Working With Lists)
F# `List` module functions and pipelines (`|>`) are used extensively for data manipulation:
- **`List.filter`:** Used in the `dashboardHandler` (`Program.fs:L155`) to build the search engine (filtering by Model and Manufacturer).
- **`List.sortByDescending` & `List.take`:** Used to functionally extract the "Top Selling Aircraft" for the dashboard chart (`Models.fs:L121`).
- **`List.tryFind`:** Used for safe `O(log n)` lookups in the order database (`Program.fs:L163`).

### 4. Advanced F# Features (Syllabus: May 9)
- **Units of Measure:** Defined `[<Measure>] type USD` and `[<Measure>] type LAK` to ensure zero-cost, compile-time safety for all financial transactions, guaranteeing that US Dollars and Lao Kip are never accidentally added together.
- **Domain-Specific Languages (DSLs):** The entire HTML frontend (`Views.fs`) is built using `Giraffe.ViewEngine`, an internal F# DSL that guarantees structurally valid HTML at compile time.

---

## 🏗 Architecture — MVC Pattern

The project is organized using the **Model-View-Controller (MVC)** pattern:

| File | Role | Responsibility |
|---|---|---|
| `Models.fs` | **Model** | Defines `Aircraft` type, Units of Measure, and the mock database |
| `Views.fs` | **View** | Generates all HTML using `Giraffe.ViewEngine` DSL |
| `Program.fs` | **Controller** | Handles routes, filtering logic, discount lambdas |

---

## 📐 F# Feature 1: Units of Measure

**Units of Measure** is a unique F# compiler feature that attaches a physical unit to a number. The compiler will **reject any code that mixes up different units** — completely at compile time, with zero runtime cost.

```fsharp
// Declared in Models.fs
[<Measure>] type USD    // US Dollars currency unit
[<Measure>] type seats  // Passenger seat capacity

// The Aircraft type uses decimal<USD> — not just decimal!
type Aircraft = {
    Price: decimal<USD>   // <-- compiler enforces this is ALWAYS USD
    ...
}

// Every price in the database is annotated:
createAircraft "A320" "Airbus" 101000000m<USD> "Standard single-aisle." 150
//                              ^^^^^^^^^^^^^
//                              The <USD> tag is the Unit of Measure

// This would be a COMPILE ERROR — the compiler rejects it:
// let wrong = aircraft.Price + 50m  // ERROR: decimal<USD> + decimal mismatch!
```

**Why this matters:** In a real financial system, accidentally mixing SGD and USD prices would be a bug. With Units of Measure, the compiler catches this before the program even runs.

---

## λ F# Feature 2: Lambda Calculus & Functional Programming

### Lambda Abstraction (`fun x ->`)
Every `fun` keyword is a lambda expression — the building block of all functional programming.

```fsharp
// Used throughout the app for filtering and mapping:
aircraftDatabase |> List.filter (fun a -> a.Manufacturer = "Airbus")
aircrafts        |> List.map    (fun item -> div [] [...])
```

### Currying
A function that takes one argument and **returns another function**.

```fsharp
// applyDiscount : decimal -> decimal<USD> -> decimal<USD>
// Takes a rate, returns a NEW function waiting for a price
let applyDiscount = fun rate -> fun (price: decimal<USD>) -> price * (1.0m - rate)
```

### Partial Application
Calling a curried function with only *some* of its arguments to create a specialized function.

```fsharp
// We "bake in" the rate. Now vipDiscount is a ready-to-use lambda.
let vipDiscount   = applyDiscount 0.20m   // type: decimal<USD> -> decimal<USD>
let staffDiscount = applyDiscount 0.50m   // type: decimal<USD> -> decimal<USD>

// Usage — apply the lambda to any price:
let finalPrice = vipDiscount aircraft.Price  // → original price × 0.80
```

### Higher-Order Functions (HOF)
Functions that take or return other functions.

```fsharp
// getDiscountFn: takes a string, RETURNS a lambda function
let getDiscountFn = fun (code: string) ->
    match code.Trim().ToUpper() with
    | "VIP20"   -> Some (vipDiscount,   "VIP20 — 20% Off Applied!")
    | "STAFF50" -> Some (staffDiscount, "STAFF50 — 50% Off Applied!")
    | _         -> None
```

### Function Composition (`|>` Pipe Operator)
```fsharp
// The |> chains functions together — output of one feeds into the next
aircraftDatabase
|> List.sortByDescending (fun a -> a.SalesCount)   // step 1
|> List.take 10                                     // step 2
```

---

## 🎟️ Feature: Live Promo Code Validator

The checkout page has a **real-time promo code validator** that shows feedback instantly as you type. The browser-side JavaScript mirrors the exact same lambda calculus structure as the F# server code.

| F# Server (`Program.fs`) | JS Client (inline `<script>`) |
|---|---|
| `let applyDiscount = fun rate -> fun price -> ...` | `const applyDiscount = rate => price => ...` |
| `let vipDiscount = applyDiscount 0.20m` | `const vipDiscount = applyDiscount(0.20)` |
| `getDiscountFn` (HOF) | `getDiscountFn` (HOF) |

**Valid codes:**
| Code | Discount |
|---|---|
| `VIP20` | 20% off |
| `STAFF50` | 50% off |

---

## 🛠 Libraries Used

| Library | Purpose |
|---|---|
| **Giraffe** | F# web framework built on ASP.NET Core. Handles all HTTP routing. |
| **Giraffe.ViewEngine** | Write HTML directly in F# as a typed DSL. No separate template files. |
| **Chart.js** | Client-side bar chart for the "Most Popular Models" dashboard graph. |
| **ASP.NET Static Files** | Serves aircraft images from the `wwwroot/images/` folder. |

---

## 🚀 How to Run

```bash
# Clone the repo
git clone https://github.com/SilichanhTechForge/skypay-aircraft-shop.git
cd skypay-aircraft-shop

# Run locally
dotnet run

# Open browser at:
# http://localhost:5000
```

---

## 🐳 Docker

```bash
docker build -t skypay .
docker run -p 5000:5000 skypay
```

---

## 📊 Aircraft Inventory

22 aircraft across 5 manufacturers: **Airbus**, **Boeing**, **COMAC**, **Embraer**, **Bombardier**

Price range: **$38,000,000 USD** (COMAC C909) → **$445,000,000 USD** (Airbus A380)
