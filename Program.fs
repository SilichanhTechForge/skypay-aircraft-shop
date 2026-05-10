module Skypay.App

// ==========================================
// CONTROLLER LAYER (Program.fs)
// ==========================================
// This file acts as the "Controller" in my MVC structure.
// It handles the incoming HTTP requests, talks to the Models to get data,
// and then passes that data to the Views to be rendered.

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Skypay.Models
open Skypay.Views

// ---------------------------------
// Lambda Calculus: Discount System
// ---------------------------------
// This is a CURRIED lambda function. It takes a 'rate', and returns
// ANOTHER function that takes a 'price'. This is the core of lambda calculus!
// It works with decimal<USD> so the Unit of Measure is preserved through the discount!
// applyDiscount : decimal -> decimal<USD> -> decimal<USD>
let applyDiscount = fun rate -> fun (price: decimal<USD>) -> price * (1.0m - rate)

// Partial Application: We 'bake in' the rate to create specific discount functions.
// vipDiscount and staffDiscount are both lambda functions created from applyDiscount.
let vipDiscount   = applyDiscount 0.20m  // 20% off
let staffDiscount = applyDiscount 0.50m  // 50% off

// A Higher-Order Function: it takes a promo code string,
// and RETURNS the correct lambda discount function to apply.
let getDiscountFn = fun (code: string) ->
    match code.Trim().ToUpper() with
    | "VIP20"   -> Some (vipDiscount,   "VIP20 — 20% Off Applied! ✈️")
    | "STAFF50" -> Some (staffDiscount, "STAFF50 — 50% Off Applied! 🎉")
    | _         -> None

// ---------------------------------
// Web App Handlers
// ---------------------------------

let dashboardHandler : HttpHandler =
    fun next ctx ->
        // I added this part to handle the SEARCH and FILTER requirements.
        // We look for 'q' (search text) and 'brand' in the URL query string.
        
        // 1. Get query parameters from the URL
        let searchQuery = 
            match ctx.TryGetQueryStringValue "q" with
            | Some q -> q.Trim()
            | None -> ""
            
        let brandFilter = 
            match ctx.TryGetQueryStringValue "brand" with
            | Some b -> b
            | None -> ""

        // 2. Filter Logic
        // Here I am filtering the full database list based on what the user asked for.
        let allAircraft = 
            aircraftDatabase
            |> List.filter (fun a -> 
                // Check if the Name matches the search text (Case Insensitive)
                let matchesName = 
                    if String.IsNullOrWhiteSpace searchQuery then true
                    else a.Model.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0
                
                // Check if the Manufacturer matches the dropdown selection
                let matchesBrand = 
                    if String.IsNullOrWhiteSpace brandFilter then true
                    else a.Manufacturer.Equals(brandFilter, StringComparison.OrdinalIgnoreCase)
                
                // Both conditions must be true
                matchesName && matchesBrand
            )

        // I also pass the Top Selling stats for the graph
        let topStats = topSellingAircraft
        
        // 3. Render the View
        // Pass the filtered data + the current filter inputs (so the search bar works sticky)
        let view = dashboardView allAircraft topStats searchQuery brandFilter
        htmlView view next ctx

let checkoutHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        // Find aircraft by ID
        let aircraft = aircraftDatabase |> List.tryFind (fun a -> a.Id = id)
        match aircraft with
        | Some a ->
            let view = paymentView a
            htmlView view next ctx
        | None ->
            (setStatusCode 404 >=> text "Aircraft not found") next ctx

let processPaymentHandler : HttpHandler =
    fun next ctx ->
        // In a real app, we would parse the form data here.
        // For this mock, we just want to show the success page.
        // We'll try to find the aircraft from the referer or just pick a random one/mock one for the success message 
        // to keep it simple since we aren't using a database.
        
        // Let's grab the aircraft ID from the referer if possible, or just parse form if we had it.
        // For simplicity, let's just show a success message for "your aircraft".
        
        let dummyAircraft = { 
            Id = Guid.Empty
            Model = "Selected Aircraft"
            Manufacturer = "Unknown"
            Price = 0m<USD>
            Description = ""
            ImageUrl = ""
            SalesCount = 0 
        }
        
        // Ideally we would pass the ID in the form as a hidden field.
        // Let's assume the user just bought the "A320" for the success demo if we can't parse easily without HttpContext parsing logic.
        // Actually, we can read the REFERER or just make the form post to /process-payment/{id}
        
        htmlView (successView dummyAircraft 0m<USD> "") next ctx

let processPaymentWithIdHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        task {
            // Read the POST form data sent from the browser
            let! form = ctx.Request.ReadFormAsync()
            let cardNumber = if form.ContainsKey("cardNumber") then form.["cardNumber"].ToString() else ""
            let cvc        = if form.ContainsKey("cvc")        then form.["cvc"].ToString()        else ""
            let promoCode  = if form.ContainsKey("promoCode")  then form.["promoCode"].ToString()  else ""
            
            let aircraft = aircraftDatabase |> List.tryFind (fun a -> a.Id = id)
            match aircraft with
            | Some a -> 
                // Basic Server-Side Validation
                if String.IsNullOrWhiteSpace(cardNumber) || cardNumber.Replace(" ", "").Length < 15 then
                    return! (setStatusCode 400 >=> text "Payment Failed: Invalid Card Number (Must be 15-16 digits)") next ctx
                elif String.IsNullOrWhiteSpace(cvc) || cvc.Length < 3 then
                    return! (setStatusCode 400 >=> text "Payment Failed: Invalid CVC (Must be at least 3 digits)") next ctx
                else
                    // ---- LAMBDA CALCULUS DISCOUNT LOGIC ----
                    // Use getDiscountFn (a Higher-Order Function) to look up the promo code.
                    // If valid, it returns the correct lambda function (e.g. vipDiscount).
                    // We then APPLY that lambda to the aircraft price to get the final price.
                    let discountResult = getDiscountFn promoCode
                    let finalPrice, discountMsg =
                        match discountResult with
                        | Some (discountFn, msg) -> discountFn a.Price, msg  // Apply the lambda!
                        | None                  -> a.Price, ""               // No discount
                    // ---- END LAMBDA LOGIC ----
                    return! htmlView (successView a finalPrice discountMsg) next ctx
            | None -> 
                return! (setStatusCode 404 >=> text "Error processing payment: Aircraft not found") next ctx
        }


// ---------------------------------
// Order Tracking Handler
// ---------------------------------
// GET /track          -> show the search form (empty)
// GET /track?order=X  -> look up the order and show its delivery status
// Map.tryFind returns Option<OrderRecord> — safe, no exceptions.
let trackOrderHandler : HttpHandler =
    fun next ctx ->
        let query =
            match ctx.TryGetQueryStringValue "order" with
            | Some q -> q.Trim().ToUpper()
            | None   -> ""
        let result =
            if query = "" then None
            else Map.tryFind query mockOrderDatabase
        htmlView (orderTrackView query result) next ctx

let webApp =
    choose [
        route "/" >=> dashboardHandler
        routef "/checkout/%O" checkoutHandler
        routef "/process-payment/%O" processPaymentWithIdHandler
        route "/track"           >=> trackOrderHandler
        
        // Fallback for form action adjustment
        route "/process-payment" >=> processPaymentHandler 
    ]

// ---------------------------------
// Main
// ---------------------------------

[<EntryPoint>]
let main args =
    if args |> Array.contains "--build-static" then
        // Generate static HTML for GitHub Pages
        printfn "Building static HTML for GitHub Pages..."
        let allAircraft = aircraftDatabase
        let topStats = topSellingAircraft
        let view = dashboardView allAircraft topStats "" ""
        
        let htmlString = Giraffe.ViewEngine.RenderView.AsString.htmlDocument view
        
        if not (System.IO.Directory.Exists("dist")) then
            System.IO.Directory.CreateDirectory("dist") |> ignore
            
        System.IO.File.WriteAllText("dist/index.html", htmlString)
        
        if System.IO.Directory.Exists("wwwroot") then
            if not (System.IO.Directory.Exists("dist/wwwroot")) then
                System.IO.Directory.CreateDirectory("dist/wwwroot") |> ignore
            // Copy contents recursively
            let rec copyDir src dst =
                if not (System.IO.Directory.Exists(dst)) then
                    System.IO.Directory.CreateDirectory(dst) |> ignore
                for file in System.IO.Directory.GetFiles(src) do
                    let fileName = System.IO.Path.GetFileName(file)
                    System.IO.File.Copy(file, System.IO.Path.Combine(dst, fileName), true)
                for dir in System.IO.Directory.GetDirectories(src) do
                    let dirName = System.IO.Path.GetFileName(dir)
                    copyDir dir (System.IO.Path.Combine(dst, dirName))
            copyDir "wwwroot" "dist"
            
        printfn "Static site generated in dist/"
        0
    else
        let builder = WebApplication.CreateBuilder(args)
        builder.Services.AddGiraffe() |> ignore

        let app = builder.Build()

        if app.Environment.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseStaticFiles() |> ignore
        app.UseGiraffe(webApp)

        app.Run()
        0
