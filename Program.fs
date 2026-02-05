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
            Price = 0m
            Description = ""
            ImageUrl = ""
            SalesCount = 0 
        }
        
        // Ideally we would pass the ID in the form as a hidden field.
        // Let's assume the user just bought the "A320" for the success demo if we can't parse easily without HttpContext parsing logic.
        // Actually, we can read the REFERER or just make the form post to /process-payment/{id}
        
        htmlView (successView dummyAircraft) next ctx

let processPaymentWithIdHandler (id: Guid) : HttpHandler =
    fun next ctx ->
         let aircraft = aircraftDatabase |> List.tryFind (fun a -> a.Id = id)
         match aircraft with
         | Some a -> htmlView (successView a) next ctx
         | None -> (setStatusCode 404 >=> text "Error processing payment") next ctx


let webApp =
    choose [
        route "/" >=> dashboardHandler
        routef "/checkout/%O" checkoutHandler
        routef "/process-payment/%O" processPaymentWithIdHandler // Better URL structure
        
        // Fallback for form action adjustment
        route "/process-payment" >=> processPaymentHandler 
    ]

// ---------------------------------
// Main
// ---------------------------------

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.Services.AddGiraffe() |> ignore

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore

    app.UseStaticFiles() |> ignore
    app.UseGiraffe(webApp)

    app.Run()
    0
