module Skypay.App

// ==========================================
// CONTROLLER LAYER (Program.fs)
// ==========================================
// Handles all HTTP routes: dashboard, checkout, payment, order tracking,
// shopping cart (session-based), and currency conversion.

open System
open System.Text.Json
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Skypay.Models
open Skypay.Views

// ---------------------------------
// Lambda Calculus: Discount System
// ---------------------------------
// Curried lambda: takes a rate, returns a function that discounts a price.
// applyDiscount : decimal -> decimal<USD> -> decimal<USD>
let applyDiscount = fun rate -> fun (price: decimal<USD>) -> price * (1.0m - rate)

// Partial application — specialised discount lambdas
let vipDiscount   = applyDiscount 0.20m  // 20% off
let staffDiscount = applyDiscount 0.50m  // 50% off

// Higher-Order Function: takes a promo code, RETURNS the correct lambda
let getDiscountFn = fun (code: string) ->
    match code.Trim().ToUpper() with
    | "VIP20"   -> Some (vipDiscount,   "VIP20 — 20% Off Applied! ✈️")
    | "STAFF50" -> Some (staffDiscount, "STAFF50 — 50% Off Applied! 🎉")
    | _         -> None

// ---------------------------------
// Cart Session Helpers
// ---------------------------------
let private cartKey = "skypay_cart"

let getCartItems (ctx: HttpContext) : CartItem list =
    let json = ctx.Session.GetString(cartKey)
    if isNull json then []
    else
        try JsonSerializer.Deserialize<CartItem list>(json)
        with _ -> []

let saveCartItems (ctx: HttpContext) (items: CartItem list) =
    ctx.Session.SetString(cartKey, JsonSerializer.Serialize(items))

let getCartCount (ctx: HttpContext) : int =
    getCartItems ctx |> List.length

// ---------------------------------
// Existing Page Handlers (updated with cartCount)
// ---------------------------------

let dashboardHandler : HttpHandler =
    fun next ctx ->
        let searchQuery =
            match ctx.TryGetQueryStringValue "q" with
            | Some q -> q.Trim()
            | None   -> ""
        let brandFilter =
            match ctx.TryGetQueryStringValue "brand" with
            | Some b -> b
            | None   -> ""
        let allAircraft =
            aircraftDatabase
            |> List.filter (fun a ->
                let matchesName =
                    if String.IsNullOrWhiteSpace searchQuery then true
                    else a.Model.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0
                let matchesBrand =
                    if String.IsNullOrWhiteSpace brandFilter then true
                    else a.Manufacturer.Equals(brandFilter, StringComparison.OrdinalIgnoreCase)
                matchesName && matchesBrand)
        let cartCount = getCartCount ctx
        htmlView (dashboardView allAircraft topSellingAircraft searchQuery brandFilter cartCount) next ctx

let checkoutHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        let aircraft = aircraftDatabase |> List.tryFind (fun a -> a.Id = id)
        match aircraft with
        | Some a ->
            let cartCount = getCartCount ctx
            htmlView (paymentView a cartCount) next ctx
        | None ->
            (setStatusCode 404 >=> text "Aircraft not found") next ctx

let processPaymentHandler : HttpHandler =
    fun next ctx ->
        let dummy = { Id = Guid.Empty; Model = "Selected Aircraft"; Manufacturer = "Unknown"
                      Price = 0m<USD>; Description = ""; ImageUrl = ""; SalesCount = 0 }
        htmlView (successView dummy 0m<USD> "" (getCartCount ctx)) next ctx

let processPaymentWithIdHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        task {
            let! form      = ctx.Request.ReadFormAsync()
            let cardNumber = if form.ContainsKey("cardNumber") then form.["cardNumber"].ToString() else ""
            let cvc        = if form.ContainsKey("cvc")        then form.["cvc"].ToString()        else ""
            let promoCode  = if form.ContainsKey("promoCode")  then form.["promoCode"].ToString()  else ""
            let aircraft   = aircraftDatabase |> List.tryFind (fun a -> a.Id = id)
            match aircraft with
            | Some a ->
                if String.IsNullOrWhiteSpace(cardNumber) || cardNumber.Replace(" ", "").Length < 15 then
                    return! (setStatusCode 400 >=> text "Payment Failed: Invalid Card Number (must be 15–16 digits)") next ctx
                elif String.IsNullOrWhiteSpace(cvc) || cvc.Length < 3 then
                    return! (setStatusCode 400 >=> text "Payment Failed: Invalid CVC (must be 3+ digits)") next ctx
                else
                    // Apply discount lambda if a valid promo code was supplied
                    let finalPrice, discountMsg =
                        match getDiscountFn promoCode with
                        | Some (fn, msg) -> fn a.Price, msg
                        | None           -> a.Price, ""
                    // Remove purchased item from cart (if it was there)
                    let updatedCart = getCartItems ctx |> List.filter (fun c -> c.AircraftId <> a.Id.ToString())
                    saveCartItems ctx updatedCart
                    return! htmlView (successView a finalPrice discountMsg updatedCart.Length) next ctx
            | None ->
                return! (setStatusCode 404 >=> text "Error: Aircraft not found") next ctx
        }

let trackOrderHandler : HttpHandler =
    fun next ctx ->
        let query =
            match ctx.TryGetQueryStringValue "order" with
            | Some q -> q.Trim().ToUpper()
            | None   -> ""
        let result = if query = "" then None else Map.tryFind query mockOrderDatabase
        htmlView (orderTrackView query result (getCartCount ctx)) next ctx

// ---------------------------------
// Cart Handlers
// ---------------------------------

let cartViewHandler : HttpHandler =
    fun next ctx ->
        let items = getCartItems ctx
        htmlView (cartView items items.Length) next ctx

// POST /cart/add/{id}
let addToCartHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        let aircraft = aircraftDatabase |> List.tryFind (fun a -> a.Id = id)
        match aircraft with
        | Some a ->
            let cart = getCartItems ctx
            // Idempotent — only add once
            if not (cart |> List.exists (fun c -> c.AircraftId = a.Id.ToString())) then
                let newItem = { AircraftId = a.Id.ToString(); Model = a.Model
                                Manufacturer = a.Manufacturer
                                PriceUsd = a.Price / 1m<USD>
                                ImageUrl = a.ImageUrl }
                saveCartItems ctx (cart @ [newItem])
            // Redirect back to where the user was
            let referer = ctx.Request.Headers.["Referer"].ToString()
            let back    = if String.IsNullOrWhiteSpace referer then "/" else referer
            (redirectTo false back) next ctx
        | None ->
            (setStatusCode 404 >=> text "Aircraft not found") next ctx

// GET /cart/remove/{id}  (id is the Guid string)
let removeFromCartHandler (id: string) : HttpHandler =
    fun next ctx ->
        getCartItems ctx
        |> List.filter (fun c -> c.AircraftId <> id)
        |> saveCartItems ctx
        (redirectTo false "/cart") next ctx

// GET /cart/clear
let clearCartHandler : HttpHandler =
    fun next ctx ->
        saveCartItems ctx []
        (redirectTo false "/cart") next ctx

// ---------------------------------
// Router
// ---------------------------------
let webApp =
    choose [
        GET  >=> route  "/"                    >=> dashboardHandler
        GET  >=> routef "/checkout/%O"              checkoutHandler
        POST >=> routef "/process-payment/%O"       processPaymentWithIdHandler
        POST >=> route  "/process-payment"      >=> processPaymentHandler
        GET  >=> route  "/track"                >=> trackOrderHandler
        GET  >=> route  "/cart"                 >=> cartViewHandler
        POST >=> routef "/cart/add/%O"              addToCartHandler
        GET  >=> routef "/cart/remove/%s"           removeFromCartHandler
        GET  >=> route  "/cart/clear"           >=> clearCartHandler
    ]

// ---------------------------------
// Entry Point
// ---------------------------------
[<EntryPoint>]
let main args =
    if args |> Array.contains "--build-static" then
        printfn "Building static HTML for GitHub Pages..."
        let view       = dashboardView aircraftDatabase topSellingAircraft "" "" 0
        let htmlString = Giraffe.ViewEngine.RenderView.AsString.htmlDocument view
        if not (System.IO.Directory.Exists("dist")) then
            System.IO.Directory.CreateDirectory("dist") |> ignore
        System.IO.File.WriteAllText("dist/index.html", htmlString)
        if System.IO.Directory.Exists("wwwroot") then
            if not (System.IO.Directory.Exists("dist/wwwroot")) then
                System.IO.Directory.CreateDirectory("dist/wwwroot") |> ignore
            let rec copyDir src dst =
                if not (System.IO.Directory.Exists(dst)) then
                    System.IO.Directory.CreateDirectory(dst) |> ignore
                for file in System.IO.Directory.GetFiles(src) do
                    System.IO.File.Copy(file, System.IO.Path.Combine(dst, System.IO.Path.GetFileName(file)), true)
                for dir in System.IO.Directory.GetDirectories(src) do
                    copyDir dir (System.IO.Path.Combine(dst, System.IO.Path.GetFileName(dir)))
            copyDir "wwwroot" "dist"
        printfn "Static site generated in dist/"
        0
    else
        let builder = WebApplication.CreateBuilder(args)
        builder.Services.AddGiraffe() |> ignore

        // Session services (required for shopping cart)
        builder.Services.AddDistributedMemoryCache() |> ignore
        builder.Services.AddSession(fun opts ->
            opts.Cookie.HttpOnly  <- true
            opts.Cookie.IsEssential <- true
            opts.IdleTimeout <- TimeSpan.FromHours(2.0)
        ) |> ignore

        let app = builder.Build()

        if app.Environment.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseStaticFiles() |> ignore
        app.UseSession()     |> ignore  // Must be before UseGiraffe
        app.UseGiraffe(webApp)

        app.Run()
        0
