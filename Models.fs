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
