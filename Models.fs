module Skypay.Models

// ==========================================
// MODEL LAYER (Models.fs)
// ==========================================
// This file defines the "Shape" of my data (The Aircraft Type).
// Ideally this would connect to a SQL database, but for this project
// I am using a "Mock Data" list in memory to make it easier to run.

open System

// ==========================================
// UNITS OF MEASURE
// ==========================================
// This is a unique F# feature. By declaring [<Measure>] types,
// we give numbers a "physical unit" that the compiler enforces.
// This means you can NEVER accidentally mix a plain decimal with a price,
// or add a USD price to an SGD price without converting first.
// It's zero cost at runtime — purely a compile-time safety check.
[<Measure>] type USD   // US Dollars
[<Measure>] type seats // Passenger seat capacity

// This is the blueprint for every aircraft object
type Aircraft = {
    Id: Guid
    Model: string
    Manufacturer: string
    Price: decimal<USD>  // <-- Compiler now knows this MUST be a USD value
    Description: string
    ImageUrl: string
    SalesCount: int
}

// A helper function I made to create aircraft easily without typing everything out.
// Note: 'price' is typed as decimal<USD> — the compiler will reject any non-USD value here.
let createAircraft model manufacturer (price: decimal<USD>) desc sales = 
    {
        Id = Guid.NewGuid()
        Model = model
        Manufacturer = manufacturer
        Price = price
        Description = desc
        ImageUrl = "https://via.placeholder.com/300x200?text=" + (model.Replace(" ", "+"))
        SalesCount = sales
    }

// THE DATABASE
// Since I don't have a real SQL server set up, I just put all the data here.
// THE DATABASE
// Prices are written as decimal<USD> using the 1.0m<USD> syntax.
// The <USD> tag is the Unit of Measure annotation. The compiler reads this!
let aircraftDatabase = [
    // A320 Family
    createAircraft "A320" "Airbus" 101000000m<USD> "Standard single-aisle jet. 150 seats." 150
        |> fun a -> { a with ImageUrl = "/images/a320.jpg" }
    createAircraft "A321" "Airbus" 118000000m<USD> "Larger A320 version. 185-220 seats." 124
        |> fun a -> { a with ImageUrl = "/images/a321.jpg" }

    // A350 Family
    createAircraft "A350-900" "Airbus" 317000000m<USD> "Long range wide-body. Efficient." 80
        |> fun a -> { a with ImageUrl = "/images/a350-900.jpg" }
    createAircraft "A350-1000" "Airbus" 366000000m<USD> "Largest A350 model. High capacity." 40
        |> fun a -> { a with ImageUrl = "/images/a350-1000.jpg" }

    // A380
    createAircraft "A380" "Airbus" 445000000m<USD> "Double decker superjumbo. Very large." 30
        |> fun a -> { a with ImageUrl = "/images/a380.jpg" }

    // A220 Family
    createAircraft "A220-100" "Airbus" 81000000m<USD> "Small jet for 100-135 seats." 65
        |> fun a -> { a with ImageUrl = "/images/a220-100.png" }
    createAircraft "A220-300" "Airbus" 91000000m<USD> "Mid-size A220 variant." 75
        |> fun a -> { a with ImageUrl = "/images/A220-300  .jpg" }

    // A330 Family
    createAircraft "A330-800" "Airbus" 260000000m<USD> "Newer engine A330 (smaller)." 45
        |> fun a -> { a with ImageUrl = "/images/a330-800.jpg" }
    createAircraft "A330-900" "Airbus" 296000000m<USD> "Newer engine A330 (larger)." 90
        |> fun a -> { a with ImageUrl = "/images/A330-900.png" }

    // Boeing 737 MAX
    createAircraft "737 MAX 7" "Boeing" 99000000m<USD> "Smallest MAX model." 130
        |> fun a -> { a with ImageUrl = "/images/737 MAX 7.jpg" }
    createAircraft "737 MAX 8" "Boeing" 121000000m<USD> "Popular mid-size jet." 220
        |> fun a -> { a with ImageUrl = "/images/737 MAX 8.jpeg" }
    createAircraft "737 MAX 9" "Boeing" 128000000m<USD> "Longer fuselage MAX." 100
        |> fun a -> { a with ImageUrl = "/images/737 MAX 9.jpg" }
    createAircraft "737 MAX 10" "Boeing" 134000000m<USD> "Biggest 737 model available." 110
        |> fun a -> { a with ImageUrl = "/images/737 MAX 10.jpg" }

    // Boeing 787
    createAircraft "787-8 Dreamliner" "Boeing" 248000000m<USD> "Efficient composite aircraft." 140
        |> fun a -> { a with ImageUrl = "/images/787-8 Dreamliner.avif" }

    // Boeing 777
    createAircraft "777-300ER" "Boeing" 375000000m<USD> "Large twin-engine jet." 180
        |> fun a -> { a with ImageUrl = "/images/b777-300er.jpg" }
    createAircraft "777-9" "Boeing" 442000000m<USD> "New generation large 777." 60
        |> fun a -> { a with ImageUrl = "/images/b777-9.jpg" }

    // COMAC
    createAircraft "C909" "COMAC" 38000000m<USD> "Regional jet (ARJ21)." 115
        |> fun a -> { a with ImageUrl = "/images/c909.jpg" }
    createAircraft "C919" "COMAC" 99000000m<USD> "Standard narrow-body airliner." 190
        |> fun a -> { a with ImageUrl = "/images/c919.jpg" }

    // Embraer
    createAircraft "E170" "Embraer" 46000000m<USD> "Small regional jet. 70 seats." 90
        |> fun a -> { a with ImageUrl = "/images/E170.jpg" }
    createAircraft "C-390 Millennium" "Embraer" 85000000m<USD> "Military transport aircraft." 15
        |> fun a -> { a with ImageUrl = "/images/C-390 Millennium.jpg" }

    // Bombardier
    createAircraft "CRJ900" "Bombardier" 48000000m<USD> "Regional jet. 90 seats." 110
        |> fun a -> { a with ImageUrl = "/images/CRJ900.jpg" }
    createAircraft "CRJ1000" "Bombardier" 51000000m<USD> "Extended CRJ. 100 seats." 60
        |> fun a -> { a with ImageUrl = "/images/CRJ1000.jpg" }
]

// For the graph: Get top 5 selling aircraft
let topSellingAircraft = 
    aircraftDatabase 
    |> List.sortByDescending (fun a -> a.SalesCount)
    |> List.take 10

// ==========================================
// ORDER TRACKING
// ==========================================
// A Discriminated Union (DU) represents the 5 stages of an aircraft delivery.
// Unlike an enum, each case can carry data. The compiler enforces that every
// pattern match handles ALL cases — making this type-safe by design.
type DeliveryStatus =
    | OrderConfirmed    // Step 1: Payment received, order logged
    | InProduction      // Step 2: Aircraft being built at the factory
    | ReadyForDelivery  // Step 3: Quality checked, awaiting shipment
    | InTransit         // Step 4: En route to the buyer's airport
    | Delivered         // Step 5: Handed over to the buyer

// A record type for a placed order
type OrderRecord = {
    OrderNumber  : string
    AircraftModel: string
    Manufacturer : string
    Destination  : string
    Status       : DeliveryStatus
    EstimatedDate: string
    OrderDate    : string
}

// Mock order database — Map<orderNumber, OrderRecord>
// Map.tryFind gives us O(log n) lookup with a safe Option result.
// In production this would be a SQL query via a repository layer.
let mockOrderDatabase : Map<string, OrderRecord> =
    Map.ofList [
        "SKY-2026-001",
            { OrderNumber = "SKY-2026-001"; AircraftModel = "A320"; Manufacturer = "Airbus"
              Destination = "Changi Airport, Singapore"; Status = InTransit
              EstimatedDate = "June 15, 2026"; OrderDate = "May 10, 2026" }
        "SKY-2026-002",
            { OrderNumber = "SKY-2026-002"; AircraftModel = "737 MAX 8"; Manufacturer = "Boeing"
              Destination = "Heathrow Airport, UK"; Status = InProduction
              EstimatedDate = "August 20, 2026"; OrderDate = "May 9, 2026" }
        "SKY-2026-003",
            { OrderNumber = "SKY-2026-003"; AircraftModel = "A380"; Manufacturer = "Airbus"
              Destination = "Dubai International, UAE"; Status = Delivered
              EstimatedDate = "May 1, 2026"; OrderDate = "January 5, 2026" }
        "SKY-2026-004",
            { OrderNumber = "SKY-2026-004"; AircraftModel = "777-9"; Manufacturer = "Boeing"
              Destination = "JFK Airport, USA"; Status = ReadyForDelivery
              EstimatedDate = "May 25, 2026"; OrderDate = "March 12, 2026" }
        "SKY-2026-005",
            { OrderNumber = "SKY-2026-005"; AircraftModel = "C919"; Manufacturer = "COMAC"
              Destination = "Beijing Capital Airport, China"; Status = OrderConfirmed
              EstimatedDate = "December 1, 2026"; OrderDate = "May 10, 2026" }
    ]

// ==========================================
// CURRENCY CONVERSION (Units of Measure)
// ==========================================
// Additional currency measure types — zero runtime cost, pure compile-time safety.
[<Measure>] type EUR  // Euro
[<Measure>] type SGD  // Singapore Dollar
[<Measure>] type GBP  // British Pound Sterling

// Reference exchange rates from USD (2026)
let usdToEurRate = 0.92m
let usdToSgdRate = 1.35m
let usdToGbpRate = 0.79m

// Lambda Calculus: A CURRIED Higher-Order Function for currency conversion.
// convertFromUsd : decimal -> decimal<USD> -> decimal
// Takes a rate, and RETURNS a new function that converts any USD price.
let convertFromUsd = fun rate -> fun (price: decimal<USD>) -> (price / 1m<USD>) * rate

// Partial Application: "bake in" the rate to create specialized converters.
// Each is a lambda waiting for a decimal<USD> price.
let toEur = convertFromUsd usdToEurRate  // decimal<USD> -> decimal (EUR amount)
let toSgd = convertFromUsd usdToSgdRate  // decimal<USD> -> decimal (SGD amount)
let toGbp = convertFromUsd usdToGbpRate  // decimal<USD> -> decimal (GBP amount)

// ==========================================
// SHOPPING CART
// ==========================================
// CartItem stores what we need to display the cart and compute totals.
// PriceUsd is plain decimal (not decimal<USD>) for JSON serialisation compat.
type CartItem = {
    AircraftId   : string   // Guid stored as string
    Model        : string
    Manufacturer : string
    PriceUsd     : decimal
    ImageUrl     : string
}

// ==========================================
// USER AUTHENTICATION (SQLite)
// ==========================================
open System.Security.Cryptography
open System.Text

// User record — stored in the SQLite users table
type User = {
    Id           : int
    Username     : string
    Email        : string
    PasswordHash : string
    CreatedAt    : string
}

// Database module encapsulates all SQLite operations.
// Uses Microsoft.Data.Sqlite — the official lightweight .NET SQLite driver.
module Database =
    open Microsoft.Data.Sqlite

    let private connStr = "Data Source=skypay.db"

    // SHA-256 password hashing — zero runtime overhead, no extra package needed.
    // hashPassword : string -> string (hex)
    let hashPassword (password: string) =
        use sha = SHA256.Create()
        sha.ComputeHash(Encoding.UTF8.GetBytes(password))
        |> Convert.ToHexString
        |> fun s -> s.ToLower()

    // Creates the users table on first run (idempotent).
    let initDb () =
        use conn = new SqliteConnection(connStr)
        conn.Open()
        let cmd = conn.CreateCommand()
        cmd.CommandText <-
            """CREATE TABLE IF NOT EXISTS users (
                id            INTEGER PRIMARY KEY AUTOINCREMENT,
                username      TEXT    NOT NULL UNIQUE,
                email         TEXT    NOT NULL UNIQUE,
                password_hash TEXT    NOT NULL,
                created_at    TEXT    NOT NULL
            )"""
        cmd.ExecuteNonQuery() |> ignore

    let private readUser (r: SqliteDataReader) =
        { Id = r.GetInt32(0); Username = r.GetString(1); Email = r.GetString(2)
          PasswordHash = r.GetString(3); CreatedAt = r.GetString(4) }

    let findByUsername (username: string) : User option =
        use conn = new SqliteConnection(connStr)
        conn.Open()
        let cmd = conn.CreateCommand()
        cmd.CommandText <- "SELECT id,username,email,password_hash,created_at FROM users WHERE username=@u LIMIT 1"
        cmd.Parameters.AddWithValue("@u", username) |> ignore
        use r = cmd.ExecuteReader()
        if r.Read() then Some (readUser r) else None

    let findByEmail (email: string) : User option =
        use conn = new SqliteConnection(connStr)
        conn.Open()
        let cmd = conn.CreateCommand()
        cmd.CommandText <- "SELECT id,username,email,password_hash,created_at FROM users WHERE email=@e LIMIT 1"
        cmd.Parameters.AddWithValue("@e", email) |> ignore
        use r = cmd.ExecuteReader()
        if r.Read() then Some (readUser r) else None

    // Registers a new user. Returns Ok(User) or Error(reason).
    let registerUser (username: string) (email: string) (password: string) : Result<User, string> =
        match findByUsername username with
        | Some _ -> Error "Username already taken."
        | None ->
        match findByEmail email with
        | Some _ -> Error "Email already registered."
        | None ->
        try
            use conn = new SqliteConnection(connStr)
            conn.Open()
            let cmd = conn.CreateCommand()
            cmd.CommandText <- "INSERT INTO users (username,email,password_hash,created_at) VALUES (@u,@e,@p,@d)"
            let hash = hashPassword password
            let now  = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            cmd.Parameters.AddWithValue("@u", username) |> ignore
            cmd.Parameters.AddWithValue("@e", email)    |> ignore
            cmd.Parameters.AddWithValue("@p", hash)     |> ignore
            cmd.Parameters.AddWithValue("@d", now)      |> ignore
            cmd.ExecuteNonQuery() |> ignore
            Ok { Id = 0; Username = username; Email = email; PasswordHash = hash; CreatedAt = now }
        with ex -> Error ex.Message

    // Returns Some(User) if credentials are valid, None otherwise.
    let validateLogin (username: string) (password: string) : User option =
        match findByUsername username with
        | Some u when u.PasswordHash = hashPassword password -> Some u
        | _ -> None
