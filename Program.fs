module Skypay.App

open System
open System.Text.Json
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Giraffe
open MailKit.Net.Smtp
open MailKit.Security
open MimeKit
open Skypay.Models
open Skypay.Views

// ── Email config (populated from appsettings.json at startup) ──────────────
let mutable private cfgSender   = ""
let mutable private cfgPassword = ""

// ── Lambda Calculus: Discount System ──────────────────────────────────────
let applyDiscount = fun rate -> fun (price: decimal<USD>) -> price * (1.0m - rate)
let vipDiscount   = applyDiscount 0.20m
let staffDiscount = applyDiscount 0.50m
let getDiscountFn = fun (code: string) ->
    match code.Trim().ToUpper() with
    | "VIP20"   -> Some (vipDiscount,   "VIP20 — 20% Off Applied!")
    | "STAFF50" -> Some (staffDiscount, "STAFF50 — 50% Off Applied!")
    | _         -> None

// ── Cart Session Helpers ───────────────────────────────────────────────────
let private cartKey = "skypay_cart"
let private userKey = "skypay_user"

let getCartItems (ctx: HttpContext) : CartItem list =
    let json = ctx.Session.GetString(cartKey)
    if isNull json then []
    else try JsonSerializer.Deserialize<CartItem list>(json) with _ -> []

let saveCartItems (ctx: HttpContext) (items: CartItem list) =
    ctx.Session.SetString(cartKey, JsonSerializer.Serialize(items))

let getCartCount (ctx: HttpContext) = getCartItems ctx |> List.length

// ── Auth Session Helpers ───────────────────────────────────────────────────
let getUserFromSession (ctx: HttpContext) : string option =
    let u = ctx.Session.GetString(userKey)
    if isNull u || u = "" then None else Some u

// Middleware: redirect to /login if no session user
let requireAuth : HttpHandler =
    fun next ctx ->
        match getUserFromSession ctx with
        | Some _ -> next ctx
        | None   -> (redirectTo false "/login") next ctx

// ── Email: Order Confirmation ──────────────────────────────────────────────
let sendConfirmationEmail (toEmail: string) (username: string) (aircraft: Aircraft) (finalPrice: decimal<USD>) (discountMsg: string) =
    task {
        if cfgSender = "" || cfgPassword = "" then
            printfn "[Email] Not configured — skipping."
        else
            try
                let msg = new MimeMessage()
                msg.From.Add(new MailboxAddress("Skypay Aircraft Shop", cfgSender))
                msg.To.Add(new MailboxAddress(username, toEmail))
                msg.Subject <- sprintf "Order Confirmed — %s | Skypay" aircraft.Model
                let priceStr    = (finalPrice / 1m<USD>).ToString("N0")
                let discountRow =
                    if discountMsg <> "" then
                        "<tr><td style='padding:10px;font-weight:bold;'>Promo Applied</td>" +
                        "<td style='padding:10px;color:#16a34a;'>" + discountMsg + "</td></tr>"
                    else ""
                let html =
                    "<div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;border:3px solid black;border-radius:12px;overflow:hidden;'>" +
                    "<div style='background:#fbbf24;padding:28px;text-align:center;'>" +
                    "<h1 style='margin:0;font-size:2rem;font-weight:900;'>SKYPAY</h1>" +
                    "<p style='margin:4px 0 0;font-weight:bold;'>Aircraft Sales Portal</p></div>" +
                    "<div style='background:white;padding:30px;'>" +
                    "<h2 style='margin-top:0;'>Order Confirmed, " + username + "!</h2>" +
                    "<p>Thank you for your purchase. Here are your order details:</p>" +
                    "<table style='width:100%;border-collapse:collapse;margin:20px 0;border:2px solid black;'>" +
                    "<tr style='background:#f8fafc;'><td style='padding:10px;font-weight:bold;'>Aircraft</td><td style='padding:10px;'>" + aircraft.Model + "</td></tr>" +
                    "<tr><td style='padding:10px;font-weight:bold;'>Manufacturer</td><td style='padding:10px;'>" + aircraft.Manufacturer + "</td></tr>" +
                    "<tr style='background:#f8fafc;'><td style='padding:10px;font-weight:bold;'>Amount Paid</td><td style='padding:10px;font-weight:900;color:#16a34a;'>$" + priceStr + " USD</td></tr>" +
                    discountRow + "</table>" +
                    "<p>We will contact you shortly about delivery arrangements.</p>" +
                    "<p style='text-align:center;margin-top:24px;'>" +
                    "<a href='https://skypay-aircraft-shop.onrender.com/track' style='background:#fbbf24;color:black;padding:12px 28px;text-decoration:none;font-weight:900;border-radius:8px;border:2px solid black;'>Track Your Order</a></p></div>" +
                    "<div style='background:#1e293b;color:white;padding:18px;text-align:center;'>" +
                    "<p style='margin:0;font-size:0.85rem;'>© 2026 Skypay Aircraft Shop</p></div></div>"
                let builder = new BodyBuilder()
                builder.HtmlBody <- html
                msg.Body <- builder.ToMessageBody()
                let client = new SmtpClient()
                do! client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls)
                do! client.AuthenticateAsync(cfgSender, cfgPassword)
                let! _ = client.SendAsync(msg)
                do! client.DisconnectAsync(true)
                client.Dispose()
                printfn "[Email] Sent confirmation to %s" toEmail
            with ex ->
                printfn "[Email] Failed: %s" ex.Message
    }

// ── Email: Welcome on Registration ────────────────────────────────────────
let sendWelcomeEmail (toEmail: string) (username: string) =
    task {
        if cfgSender = "" || cfgPassword = "" then
            printfn "[Email] Not configured — skipping welcome email."
        else
            try
                let msg = new MimeMessage()
                msg.From.Add(new MailboxAddress("Skypay Aircraft Shop", cfgSender))
                msg.To.Add(new MailboxAddress(username, toEmail))
                msg.Subject <- sprintf "Welcome to Skypay, %s! Your account is ready." username
                let html =
                    "<div style='font-family:Inter,Arial,sans-serif;max-width:600px;margin:0 auto;border:1px solid #e2e8f0;border-radius:12px;overflow:hidden;'>" +
                    "<div style='background:#0f2044;padding:28px;text-align:center;'>" +
                    "<h1 style='margin:0;font-size:1.8rem;font-weight:800;color:#c4a55a;letter-spacing:4px;'>SKYPAY</h1>" +
                    "<p style='margin:6px 0 0;color:rgba(255,255,255,0.7);font-size:0.9rem;'>Aircraft Sales Portal</p></div>" +
                    "<div style='background:white;padding:36px;'>" +
                    "<h2 style='margin-top:0;color:#0f2044;'>Welcome aboard, " + username + "!</h2>" +
                    "<p style='color:#475569;line-height:1.6;'>Your Skypay account has been created successfully. You now have access to our full inventory of commercial and regional aircraft.</p>" +
                    "<table style='width:100%;border-collapse:collapse;margin:24px 0;background:#f8fafc;border-radius:8px;overflow:hidden;border:1px solid #e2e8f0;'>" +
                    "<tr><td style='padding:12px 16px;font-weight:600;color:#64748b;font-size:0.85rem;text-transform:uppercase;letter-spacing:0.05em;'>Username</td><td style='padding:12px 16px;font-weight:700;color:#0f2044;'>" + username + "</td></tr>" +
                    "<tr style='background:white;'><td style='padding:12px 16px;font-weight:600;color:#64748b;font-size:0.85rem;text-transform:uppercase;letter-spacing:0.05em;'>Email</td><td style='padding:12px 16px;color:#0f2044;'>" + toEmail + "</td></tr>" +
                    "<tr><td style='padding:12px 16px;font-weight:600;color:#64748b;font-size:0.85rem;text-transform:uppercase;letter-spacing:0.05em;'>Status</td><td style='padding:12px 16px;'><span style='background:#dcfce7;color:#166534;padding:3px 10px;border-radius:20px;font-size:0.8rem;font-weight:600;'>Active</span></td></tr>" +
                    "</table>" +
                    "<p style='text-align:center;margin-top:28px;'>" +
                    "<a href='https://skypay-aircraft-shop.onrender.com/' style='background:#0f2044;color:white;padding:13px 32px;text-decoration:none;font-weight:700;border-radius:8px;display:inline-block;'>Browse Aircraft</a></p></div>" +
                    "<div style='background:#f0f4f8;color:#94a3b8;padding:20px;text-align:center;font-size:0.8rem;'>" +
                    "<p style='margin:0;'>© 2026 Skypay Aircraft Shop · This email confirms your registration.</p></div></div>"
                let builder = new BodyBuilder()
                builder.HtmlBody <- html
                msg.Body <- builder.ToMessageBody()
                let client = new SmtpClient()
                do! client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls)
                do! client.AuthenticateAsync(cfgSender, cfgPassword)
                let! _ = client.SendAsync(msg)
                do! client.DisconnectAsync(true)
                client.Dispose()
                printfn "[Email] Sent welcome email to %s" toEmail
            with ex ->
                printfn "[Email] Welcome email failed: %s" ex.Message
    }

// ── Page Handlers ──────────────────────────────────────────────────────────


let dashboardHandler : HttpHandler =
    fun next ctx ->
        let q = match ctx.TryGetQueryStringValue "q"     with Some v -> v.Trim() | None -> ""
        let b = match ctx.TryGetQueryStringValue "brand"  with Some v -> v        | None -> ""
        let filtered =
            aircraftDatabase |> List.filter (fun a ->
                (String.IsNullOrWhiteSpace q     || a.Model.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) &&
                (String.IsNullOrWhiteSpace b     || a.Manufacturer.Equals(b, StringComparison.OrdinalIgnoreCase)))
        htmlView (dashboardView filtered topSellingAircraft q b (getCartCount ctx) (getUserFromSession ctx)) next ctx

let checkoutHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        match aircraftDatabase |> List.tryFind (fun a -> a.Id = id) with
        | Some a -> htmlView (paymentView a (getCartCount ctx) (getUserFromSession ctx)) next ctx
        | None   -> (setStatusCode 404 >=> text "Aircraft not found") next ctx

let processPaymentWithIdHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        task {
            let! form      = ctx.Request.ReadFormAsync()
            let cardNumber = if form.ContainsKey("cardNumber") then form.["cardNumber"].ToString() else ""
            let cvc        = if form.ContainsKey("cvc")        then form.["cvc"].ToString()        else ""
            let promoCode  = if form.ContainsKey("promoCode")  then form.["promoCode"].ToString()  else ""
            match aircraftDatabase |> List.tryFind (fun a -> a.Id = id) with
            | Some a ->
                if String.IsNullOrWhiteSpace(cardNumber) || cardNumber.Replace(" ","").Length < 15 then
                    return! (setStatusCode 400 >=> text "Invalid Card Number") next ctx
                elif String.IsNullOrWhiteSpace(cvc) || cvc.Length < 3 then
                    return! (setStatusCode 400 >=> text "Invalid CVC") next ctx
                else
                    let finalPrice, discountMsg =
                        match getDiscountFn promoCode with
                        | Some (fn, msg) -> fn a.Price, msg
                        | None           -> a.Price, ""
                    let updatedCart = getCartItems ctx |> List.filter (fun c -> c.AircraftId <> a.Id.ToString())
                    saveCartItems ctx updatedCart
                    // Send confirmation email if user is logged in
                    match getUserFromSession ctx with
                    | Some username ->
                        match Database.findByUsername username with
                        | Some user -> do! sendConfirmationEmail user.Email username a finalPrice discountMsg
                        | None      -> ()
                    | None -> ()
                    return! htmlView (successView a finalPrice discountMsg updatedCart.Length (getUserFromSession ctx)) next ctx
            | None ->
                return! (setStatusCode 404 >=> text "Aircraft not found") next ctx
        }

let processPaymentHandler : HttpHandler =
    fun next ctx ->
        let dummy = { Id = Guid.Empty; Model = "Selected Aircraft"; Manufacturer = "Unknown"
                      Price = 0m<USD>; Description = ""; ImageUrl = ""; SalesCount = 0 }
        htmlView (successView dummy 0m<USD> "" (getCartCount ctx) (getUserFromSession ctx)) next ctx

let trackOrderHandler : HttpHandler =
    fun next ctx ->
        let q = match ctx.TryGetQueryStringValue "order" with Some v -> v.Trim().ToUpper() | None -> ""
        let result = if q = "" then None else Map.tryFind q mockOrderDatabase
        htmlView (orderTrackView q result (getCartCount ctx) (getUserFromSession ctx)) next ctx

// ── Lease Handlers ────────────────────────────────────────────────────────
let leaseListHandler : HttpHandler =
    fun next ctx ->
        htmlView (leaseListView aircraftDatabase mockLeaseRequests (getCartCount ctx) (getUserFromSession ctx)) next ctx

let leaseRequestGetHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        match aircraftDatabase |> List.tryFind (fun a -> a.Id = id) with
        | Some a -> htmlView (leaseRequestView a false (getCartCount ctx) (getUserFromSession ctx)) next ctx
        | None   -> (setStatusCode 404 >=> text "Aircraft not found") next ctx

let leaseRequestPostHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        match aircraftDatabase |> List.tryFind (fun a -> a.Id = id) with
        | Some a -> htmlView (leaseRequestView a true (getCartCount ctx) (getUserFromSession ctx)) next ctx
        | None   -> (setStatusCode 404 >=> text "Aircraft not found") next ctx

// ── Cart Handlers ──────────────────────────────────────────────────────────
let cartViewHandler : HttpHandler =
    fun next ctx ->
        let items = getCartItems ctx
        htmlView (cartView items items.Length (getUserFromSession ctx)) next ctx

let addToCartHandler (id: Guid) : HttpHandler =
    fun next ctx ->
        match aircraftDatabase |> List.tryFind (fun a -> a.Id = id) with
        | Some a ->
            let cart = getCartItems ctx
            if not (cart |> List.exists (fun c -> c.AircraftId = a.Id.ToString())) then
                let item = { AircraftId = a.Id.ToString(); Model = a.Model
                             Manufacturer = a.Manufacturer; PriceUsd = a.Price / 1m<USD>; ImageUrl = a.ImageUrl }
                saveCartItems ctx (cart @ [item])
            let back = let r = ctx.Request.Headers.["Referer"].ToString()
                       if String.IsNullOrWhiteSpace r then "/" else r
            (redirectTo false back) next ctx
        | None -> (setStatusCode 404 >=> text "Aircraft not found") next ctx

let removeFromCartHandler (id: string) : HttpHandler =
    fun next ctx ->
        getCartItems ctx |> List.filter (fun c -> c.AircraftId <> id) |> saveCartItems ctx
        (redirectTo false "/cart") next ctx

let clearCartHandler : HttpHandler =
    fun next ctx -> saveCartItems ctx []; (redirectTo false "/cart") next ctx

// ── Auth Handlers ──────────────────────────────────────────────────────────
let loginGetHandler : HttpHandler =
    fun next ctx -> htmlView (loginView None) next ctx

let loginPostHandler : HttpHandler =
    fun next ctx ->
        task {
            let! form     = ctx.Request.ReadFormAsync()
            let username  = if form.ContainsKey("username") then form.["username"].ToString().Trim() else ""
            let password  = if form.ContainsKey("password") then form.["password"].ToString()        else ""
            match Database.validateLogin username password with
            | Some _ ->
                ctx.Session.SetString(userKey, username)
                return! (redirectTo false "/") next ctx
            | None ->
                return! htmlView (loginView (Some "Invalid username or password.")) next ctx
        }

let registerGetHandler : HttpHandler =
    fun next ctx -> htmlView (registerView None) next ctx

let registerPostHandler : HttpHandler =
    fun next ctx ->
        task {
            let! form    = ctx.Request.ReadFormAsync()
            let username = if form.ContainsKey("username") then form.["username"].ToString().Trim() else ""
            let email    = if form.ContainsKey("email")    then form.["email"].ToString().Trim()    else ""
            let password = if form.ContainsKey("password") then form.["password"].ToString()        else ""
            if username.Length < 3 then
                return! htmlView (registerView (Some "Username must be at least 3 characters.")) next ctx
            elif password.Length < 6 then
                return! htmlView (registerView (Some "Password must be at least 6 characters.")) next ctx
            else
                match Database.registerUser username email password with
                | Ok _ ->
                    ctx.Session.SetString(userKey, username)
                    do! sendWelcomeEmail email username  // fire-and-forget welcome email
                    return! (redirectTo false "/") next ctx
                | Error msg ->
                    return! htmlView (registerView (Some msg)) next ctx
        }

let logoutHandler : HttpHandler =
    fun next ctx ->
        ctx.Session.Clear()
        (redirectTo false "/") next ctx

let contactGetHandler : HttpHandler =
    fun next ctx -> htmlView (contactView false (getCartCount ctx) (getUserFromSession ctx)) next ctx

let contactPostHandler : HttpHandler =
    fun next ctx ->
        // In a real application, you might save this to the DB or send an email.
        // For now, we'll just show the success screen.
        htmlView (contactView true (getCartCount ctx) (getUserFromSession ctx)) next ctx

// ── Router ─────────────────────────────────────────────────────────────────
let webApp =
    choose [
        GET  >=> route  "/"                >=> dashboardHandler
        GET  >=> routef "/checkout/%O"          (fun id -> requireAuth >=> checkoutHandler id)
        POST >=> routef "/process-payment/%O"   processPaymentWithIdHandler
        POST >=> route  "/process-payment"  >=> processPaymentHandler
        GET  >=> route  "/track"            >=> trackOrderHandler
        GET  >=> route  "/lease"            >=> leaseListHandler
        GET  >=> routef "/lease/request/%O"     (fun id -> requireAuth >=> leaseRequestGetHandler id)
        POST >=> routef "/lease/request/%O"     (fun id -> requireAuth >=> leaseRequestPostHandler id)
        GET  >=> route  "/cart"             >=> requireAuth >=> cartViewHandler
        POST >=> routef "/cart/add/%O"          (fun id -> requireAuth >=> addToCartHandler id)
        GET  >=> routef "/cart/remove/%s"       (fun id -> requireAuth >=> removeFromCartHandler id)
        GET  >=> route  "/cart/clear"       >=> requireAuth >=> clearCartHandler
        GET  >=> route  "/login"            >=> loginGetHandler
        POST >=> route  "/login"            >=> loginPostHandler
        GET  >=> route  "/register"         >=> registerGetHandler
        POST >=> route  "/register"         >=> registerPostHandler
        GET  >=> route  "/contact"          >=> contactGetHandler
        POST >=> route  "/contact"          >=> contactPostHandler
        GET  >=> route  "/logout"           >=> logoutHandler
    ]

// ── Entry Point ────────────────────────────────────────────────────────────
[<EntryPoint>]
let main args =
    if args |> Array.contains "--build-static" then
        let view = dashboardView aircraftDatabase topSellingAircraft "" "" 0 None
        System.IO.File.WriteAllText("dist/index.html", Giraffe.ViewEngine.RenderView.AsString.htmlDocument view)
        0
    else
        let builder = WebApplication.CreateBuilder(args)
        builder.Services.AddGiraffe() |> ignore
        builder.Services.AddDistributedMemoryCache() |> ignore
        builder.Services.AddSession(fun o ->
            o.Cookie.HttpOnly    <- true
            o.Cookie.IsEssential <- true
            o.IdleTimeout        <- TimeSpan.FromHours(2.0)) |> ignore

        let app = builder.Build()
        // Read email config: env vars take priority over appsettings.json
        // Set SKYPAY_SENDER_EMAIL and SKYPAY_APP_PASSWORD in Render's Environment Variables dashboard
        let config = app.Services.GetRequiredService<IConfiguration>()
        let envSender   = Environment.GetEnvironmentVariable("SKYPAY_SENDER_EMAIL")
        let envPassword = Environment.GetEnvironmentVariable("SKYPAY_APP_PASSWORD")
        cfgSender   <- if not (String.IsNullOrEmpty envSender)   then envSender
                       elif not (isNull config.["Email:SenderEmail"]) then config.["Email:SenderEmail"]
                       else ""
        cfgPassword <- if not (String.IsNullOrEmpty envPassword) then envPassword
                       elif not (isNull config.["Email:AppPassword"])  then config.["Email:AppPassword"]
                       else ""

        // Initialise SQLite database
        Database.initDb()

        if app.Environment.IsDevelopment() then app.UseDeveloperExceptionPage() |> ignore
        app.UseStaticFiles() |> ignore
        app.UseSession()     |> ignore
        app.UseGiraffe(webApp)
        app.Run()
        0
