module Skypay.Views

// ==========================================
// VIEW LAYER (Views.fs)
// ==========================================
// This file is the "View". It takes the data from the Controller
// and turns it into HTML for the user's browser.
// I also put my custom CSS here to give it the "Cartoon" look.

open System
open Giraffe.ViewEngine
open Skypay.Models

// I decided to use a "Neo-Brutalism" / Cartoon style for the UI.
// This use thick black borders (3px) and hard shadows to make it pop.
// I picked bright colors like yellow and pink to make it fun, not like a boring corporate site.
let css = """
:root {
    --bg-color: #fffbeb;     /* Cream */
    --card-bg: #ffffff;
    --text-main: #000000;
    --border-color: #000000;
    --primary: #fbbf24;      /* Amber/Yellow */
    --secondary: #f472b6;    /* Pink */
    --accent: #3b82f6;       /* Blue */
    --border-width: 3px;
    --shadow-offset: 4px;
}

body {
    font-family: 'Comic Sans MS', 'Chalkboard SE', 'Marker Felt', 'Inter', sans-serif;
    color: var(--text-main);
    background-color: var(--bg-color);
    background-image: radial-gradient(#000 1px, transparent 1px);
    background-size: 20px 20px;
    margin: 0;
    padding: 0;
    display: flex;
    flex-direction: column;
    min-height: 100vh;
}

h1, h2, h3 {
    font-weight: 900;
    text-transform: uppercase;
    letter-spacing: 1px;
    margin-top: 0;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
    flex: 1;
}

/* Navbar */
.navbar {
    background-color: white;
    border-bottom: var(--border-width) solid black;
    padding: 15px 30px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    position: sticky;
    top: 0;
    z-index: 1000;
    box-shadow: 0px 4px 0px 0px rgba(0,0,0,1);
}

.nav-brand {
    font-size: 1.8rem;
    font-weight: 900;
    color: var(--primary);
    text-shadow: 2px 2px 0px black;
    text-decoration: none;
    letter-spacing: 2px;
}

.nav-links a {
    color: black;
    text-decoration: none;
    font-weight: bold;
    margin-left: 20px;
    padding: 8px 16px;
    border: 2px solid transparent;
    border-radius: 8px;
    transition: all 0.2s;
}

.nav-links a:hover {
    background-color: var(--primary);
    border: 2px solid black;
    box-shadow: 2px 2px 0px 0px black;
    transform: translateY(-2px);
}

/* Header */
.header {
    background-color: var(--primary);
    border: var(--border-width) solid black;
    box-shadow: var(--shadow-offset) var(--shadow-offset) 0px 0px black;
    padding: 30px;
    border-radius: 12px;
    margin-bottom: 40px;
    margin-top: 20px;
    text-align: center;
}

.header h1 {
    font-size: 3rem;
    color: black;
    text-shadow: 2px 2px 0px white;
    margin-bottom: 10px;
}

/* Grid & Cards */
.grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 30px;
}

.card {
    background-color: var(--card-bg);
    border: var(--border-width) solid black;
    border-radius: 12px;
    box-shadow: var(--shadow-offset) var(--shadow-offset) 0px 0px black;
    overflow: hidden;
    transition: transform 0.2s cubic-bezier(0.175, 0.885, 0.32, 1.275), box-shadow 0.2s;
    display: flex;
    flex-direction: column;
}

.card:hover {
    transform: translate(-4px, -4px) rotate(-1deg);
    box-shadow: 8px 8px 0px 0px black;
}

.card-img-placeholder {
    height: 200px; 
    background-color: #334155; 
    background-size: cover; 
    background-position: center;
    border-bottom: var(--border-width) solid black;
}

.card-content {
    padding: 20px;
    display: flex;
    flex-direction: column;
    flex: 1;
}

.card-title {
    font-size: 1.5rem;
    margin-bottom: 5px;
    color: black;
}

.card-subtitle {
    font-size: 1.3rem;
    font-weight: 900;
    color: var(--accent);
    margin-bottom: 15px;
}

.card-desc {
    font-size: 0.95rem;
    line-height: 1.5;
    margin-bottom: 20px;
    flex: 1;
}

/* Manufacturer Tags */
.tag {
    display: inline-block;
    padding: 4px 12px;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: bold;
    border: 2px solid black;
    margin-bottom: 15px;
    box-shadow: 2px 2px 0px 0px black;
}

.tag.Airbus { background-color: #93c5fd; }
.tag.Boeing { background-color: #86efac; }
.tag.Embraer { background-color: #fde047; }
.tag.Bombardier { background-color: #fca5a5; }
.tag.COMAC { background-color: #d8b4fe; }

/* Buttons */
.btn {
    display: inline-block;
    background-color: var(--secondary);
    color: black;
    text-decoration: none;
    padding: 12px 24px;
    font-weight: 900;
    text-transform: uppercase;
    border: var(--border-width) solid black;
    border-radius: 8px;
    box-shadow: 4px 4px 0px 0px black;
    cursor: pointer;
    transition: all 0.15s;
    text-align: center;
}

.btn:hover {
    background-color: #fbcfe8;
    transform: translate(-2px, -2px);
    box-shadow: 6px 6px 0px 0px black;
}

.btn:active {
    transform: translate(2px, 2px);
    box-shadow: 1px 1px 0px 0px black;
}

/* Forms */
input, select {
    border: var(--border-width) solid black !important;
    box-shadow: 3px 3px 0px 0px black !important;
    font-family: inherit;
    transition: transform 0.1s;
}

input:focus, select:focus {
    outline: none;
    background-color: #e0f2fe;
    transform: translate(-1px, -1px);
    box-shadow: 4px 4px 0px 0px black !important;
}

/* Chart Container */
.chart-container {
    background-color: white;
    border: var(--border-width) solid black;
    box-shadow: var(--shadow-offset) var(--shadow-offset) 0px 0px black;
    border-radius: 12px;
    padding: 20px;
    margin-bottom: 40px;
}

/* Checkout Page Styles */
.payment-container {
    max-width: 600px;
    margin: 50px auto;
    padding: 40px;
    background: white;
    border: var(--border-width) solid black;
    box-shadow: 10px 10px 0px 0px black;
    border-radius: 15px;
}

.form-group { margin-bottom: 25px; position: relative; }
.form-label { display: block; margin-bottom: 8px; font-weight: 900; font-size: 1.1rem; }
.form-input {
    width: 100%;
    padding: 14px 14px 14px 40px;
    border-radius: 8px;
    font-size: 16px;
    border: var(--border-width) solid black;
    box-sizing: border-box;
}

.input-icon {
    position: absolute;
    left: 14px;
    top: 42px;
    font-size: 1.2rem;
    color: #64748b;
}

.card-logos { display: flex; gap: 15px; margin-top: 10px; }
.card-logo {
    border: 2px solid #000;
    border-radius: 8px;
    padding: 10px 20px;
    cursor: pointer;
    font-weight: 900;
    transition: all 0.2s;
    background-color: white;
}
.card-logo:hover {
    transform: translateY(-2px);
    box-shadow: 2px 2px 0px 0px black;
}
.card-logo.selected {
    border: var(--border-width) solid black;
    background-color: var(--primary);
    box-shadow: 4px 4px 0px 0px black;
    transform: translateY(-2px);
}

/* Empty State */
.empty-state {
    text-align: center;
    padding: 50px 20px;
    background-color: white;
    border: var(--border-width) solid black;
    border-radius: 12px;
    box-shadow: 6px 6px 0px 0px black;
    margin: 40px 0;
}
.empty-state h2 { font-size: 2.5rem; margin-bottom: 10px; }
.empty-state p { font-size: 1.2rem; color: #64748b; margin-bottom: 25px; }
.empty-state .emoji { font-size: 5rem; margin-bottom: 20px; }

/* Footer */
.footer {
    background-color: #1e293b;
    color: white;
    text-align: center;
    padding: 30px;
    border-top: var(--border-width) solid black;
    margin-top: 60px;
}
.footer p { margin: 5px 0; font-weight: bold; }
.footer-links a { color: var(--primary); text-decoration: none; margin: 0 10px; font-weight: bold; }
.footer-links a:hover { text-decoration: underline; }
"""

let layout (title: string) (content: XmlNode list) =
    html [] [
        head [] [
            meta [ _charset "UTF-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            tag "title" [] [ str title ]
            link [ _rel "stylesheet"; _href "https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" ]
            script [ _src "https://cdn.jsdelivr.net/npm/chart.js" ] []
            style [] [ str css ]
        ]
        body [] (
            [
                nav [ _class "navbar" ] [
                    a [ _href "/"; _class "nav-brand" ] [ str "SKYPAY" ]
                    div [ _class "nav-links" ] [
                        a [ _href "/" ] [ str "Home" ]
                        a [ _href "#" ] [ str "Orders" ]
                        a [ _href "#" ] [ str "Cart (0)" ]
                    ]
                ]
                div [ _class "container" ] content
                footer [ _class "footer" ] [
                    p [] [ str "© 2026 Skypay Aircraft Shop. All flights reserved." ]
                    div [ _class "footer-links" ] [
                        a [ _href "#" ] [ str "About Us" ]
                        a [ _href "#" ] [ str "Support" ]
                        a [ _href "#" ] [ str "Terms of Service" ]
                    ]
                ]
            ]
        )
    ]

// Moved template out to avoid parser confusion
let chartScriptTemplate = """
    const ctx = document.getElementById('salesChart');
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['%s'],
            datasets: [{
                label: '# of Aircraft Sold',
                data: [%s],
                backgroundColor: 'rgba(56, 189, 248, 0.6)',
                borderColor: 'rgba(56, 189, 248, 1)',
                borderWidth: 1,
                borderRadius: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    labels: { color: '#94a3b8' }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(255, 255, 255, 0.1)' },
                    ticks: { color: '#94a3b8' }
                },
                x: {
                    grid: { display: false },
                    ticks: { color: '#94a3b8' }
                }
            }
        }
    });
"""

let dashboardView (aircrafts: Aircraft list) (topStats: Aircraft list) (searchQuery: string) (brandFilter: string) =
    let searchVal = if isNull searchQuery then "" else searchQuery
    let brandVal = if isNull brandFilter then "" else brandFilter
    let manufacturers = [ "All"; "Airbus"; "Boeing"; "Embraer"; "Bombardier"; "COMAC" ]
    // Generate JS arrays manually to avoid sprintf %s conflicts
    let jsLabels = "[" + (topStats |> List.map (fun a -> sprintf "'%s'" a.Model) |> String.concat ",") + "]"
    let jsData = "[" + (topStats |> List.map (fun a -> string a.SalesCount) |> String.concat ",") + "]"

    // JS for the chart
    let chartScript = 
        String.concat "\n" [
            "const ctx = document.getElementById('salesChart');"
            "new Chart(ctx, {"
            "    type: 'bar',"
            "    data: {"
            "        labels: " + jsLabels + ","
            "        datasets: [{"
            "            label: '# of Aircraft Sold (Singapore Airshow 2026)',"
            "            data: " + jsData + ","
            "            backgroundColor: 'rgba(56, 189, 248, 0.6)',"
            "            borderColor: 'rgba(56, 189, 248, 1)',"
            "            borderWidth: 1,"
            "            borderRadius: 4"
            "        }]"
            "    },"
            "    options: {"
            "        responsive: true,"
            "        maintainAspectRatio: false,"
            "        plugins: {"
            "            legend: {"
            "                labels: { color: '#94a3b8' }"
            "            }"
            "        },"
            "        scales: {"
            "            y: {"
            "                beginAtZero: true,"
            "                grid: { color: 'rgba(255, 255, 255, 0.1)' },"
            "                ticks: { color: '#94a3b8' }"
            "            },"
            "            x: {"
            "                grid: { display: false },"
            "                ticks: { color: '#94a3b8' }"
            "            }"
            "        }"
            "    }"
            "});"
        ]

    layout "Skypay" [
        div [ _class "header" ] [
            h1 [] [ str "Aircraft Shop" ]
            p [] [ str "Browse our available inventory." ]
        ]

        // Search & Filter Section
        div [ _style "background: white; padding: 20px; border-radius: 12px; margin-bottom: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.05);" ] [
            form [ _action "/"; _method "get"; _style "display: flex; gap: 15px; align-items: end; flex-wrap: wrap;" ] [
                // Search Input
                div [ _style "flex-grow: 1;" ] [
                    label [ _for "q"; _style "display: block; margin-bottom: 5px; font-weight: 500; font-size: 0.9em; color: #64748b;" ] [ str "Search Model" ]
                    input [ 
                        _type "text"
                        _name "q"
                        _id "q"
                        _placeholder "e.g. 737, A320..."
                        _value searchVal
                        _style "width: 100%; padding: 10px 15px; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 16px;" 
                    ]
                ]

                // Brand Filter
                div [ _style "min-width: 200px;" ] [
                    label [ _for "brand"; _style "display: block; margin-bottom: 5px; font-weight: 500; font-size: 0.9em; color: #64748b;" ] [ str "Manufacturer" ]
                    select [ 
                        _name "brand"
                        _id "brand"
                        _style "width: 100%; padding: 10px 15px; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 16px; background-color: white;" 
                    ] [
                        for m in manufacturers do
                            let label = if m = "All" then "All Manufacturers" else m
                            let value = if m = "All" then "" else m
                            let isSelected = if value = brandVal then [ _selected ] else []
                            yield option ([ _value value ] @ isSelected) [ str label ]
                    ]
                ]

                // Submit Button
                button [ _type "submit"; _style "padding: 10px 25px; background-color: #0f172a; color: white; border: none; border-radius: 8px; font-weight: 600; cursor: pointer; height: 42px;" ] [ 
                    str "Filter" 
                ]
                
                // Clear Button (only show if filtering)
                if searchVal <> "" || brandVal <> "" then
                    a [ _href "/"; _style "padding: 10px 15px; color: #64748b; text-decoration: none; font-weight: 500; display: flex; align-items: center;" ] [ str "Clear" ]
            ]
        ]

        div [ _class "chart-container" ] [
            h2 [] [ str "Most Popular Models (Singapore Airshow 2026)" ]
            div [ _style "height: 300px; position: relative;" ] [
                canvas [ _id "salesChart" ] []
            ]
        ]

        div [] [ h2 [] [ str "Available Aircraft" ] ]

        if aircrafts.Length = 0 then
            div [ _class "empty-state" ] [
                div [ _class "emoji" ] [ str "🛩️💨" ]
                h2 [] [ str "No Aircraft Found!" ]
                p [] [ str "Looks like we don't have what you're looking for." ]
                a [ _href "/"; _class "btn" ] [ str "Clear Filters" ]
            ]
        else
            div [ _class "grid" ] (
                aircrafts |> List.map (fun item ->
                    // Simplified description logic
                    let descText = 
                        if item.Description.Length > 80 then 
                            item.Description.Substring(0, 80) + "..." 
                        else 
                            item.Description
                    
                    let bgStyle = sprintf "background-image: url('%s');" item.ImageUrl
                    // Strip <USD> unit for display formatting (decimal/1m<USD> removes the unit)
                    let priceText = sprintf "$%s USD" ((item.Price / 1m<USD>).ToString("N0"))
                    let linkUrl = sprintf "/checkout/%s" (item.Id.ToString())

                    div [ _class "card" ] [
                        // Placeholder image div
                        div [ _class "card-img-placeholder"; _style bgStyle ] []
                        div [ _class "card-content" ] [
                            h3 [ _class "card-title" ] [ str item.Model ]
                            div [ _class "card-subtitle" ] [ str priceText ]
                            p [ _class "card-desc" ] [ str descText ]
                            div [] [
                                span [ _class (sprintf "tag %s" item.Manufacturer) ] [ str item.Manufacturer ]
                            ]
                            a [ _href linkUrl; _class "btn" ] [ str "Buy Now" ]
                        ]
                    ]
                )
            )

        script [] [ rawText chartScript ]
    ]

let paymentView (aircraft: Aircraft) =
    let priceInfo = sprintf "$%s" ((aircraft.Price / 1m<USD>).ToString("N0"))
    let actionUrl = sprintf "/process-payment/%O" aircraft.Id
    // Build JS by concatenation to avoid sprintf %% conflicts with % in JS
    let priceJs   = string (int (aircraft.Price / 1m<USD>))
    let jsScript  =
        "const applyDiscount = rate => price => price * (1 - rate);\n" +
        "const vipDiscount   = applyDiscount(0.20);\n" +
        "const staffDiscount = applyDiscount(0.50);\n" +
        "const getDiscountFn = code => {\n" +
        "  switch (code.trim().toUpperCase()) {\n" +
        "    case 'VIP20':   return { fn: vipDiscount,   label: 'VIP20 - 20% Off!' };\n" +
        "    case 'STAFF50': return { fn: staffDiscount, label: 'STAFF50 - 50% Off!' };\n" +
        "    default:        return null;\n" +
        "  }\n" +
        "};\n" +
        "const originalPrice = " + priceJs + ";\n" +
        "function formatPrice(n) { return '$' + Math.round(n).toLocaleString('en-US') + ' USD'; }\n" +
        "function validatePromo(code) {\n" +
        "  const badge   = document.getElementById('promo-badge');\n" +
        "  const hint    = document.getElementById('promo-hint');\n" +
        "  const preview = document.getElementById('price-preview');\n" +
        "  if (code.trim() === '') {\n" +
        "    badge.innerHTML = '';\n" +
        "    if (hint) badge.appendChild(hint);\n" +
        "    preview.style.display = 'none'; return;\n" +
        "  }\n" +
        "  const result = getDiscountFn(code);\n" +
        "  if (result) {\n" +
        "    const finalPrice = result.fn(originalPrice);\n" +
        "    const saved      = originalPrice - finalPrice;\n" +
        "    badge.innerHTML = '<div style=\"display:flex;align-items:center;gap:10px;padding:10px 14px;background:#dcfce7;border:2px solid #16a34a;border-radius:8px;box-shadow:2px 2px 0px black;animation:popIn 0.2s ease;\"><span style=\"font-size:1.3rem;\">&#9989;</span><span style=\"font-weight:900;color:#15803d;\">' + result.label + '</span></div>';\n" +
        "    document.getElementById('original-price').textContent  = formatPrice(originalPrice);\n" +
        "    document.getElementById('discounted-price').textContent = formatPrice(finalPrice);\n" +
        "    document.getElementById('saved-amount').textContent     = formatPrice(saved);\n" +
        "    preview.style.display = 'block';\n" +
        "  } else {\n" +
        "    badge.innerHTML = '<div style=\"display:flex;align-items:center;gap:10px;padding:10px 14px;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;box-shadow:2px 2px 0px black;animation:popIn 0.2s ease;\"><span style=\"font-size:1.3rem;\">&#10060;</span><span style=\"font-weight:900;color:#dc2626;\">Invalid promo code</span></div>';\n" +
        "    preview.style.display = 'none';\n" +
        "  }\n" +
        "}\n" +
        "function selectCard(el) {\n" +
        "  document.querySelectorAll('.card-logo').forEach(e => e.classList.remove('selected'));\n" +
        "  el.classList.add('selected');\n" +
        "  document.getElementById('cardType').value = el.innerText;\n" +
        "}\n" +
        "const animStyle = document.createElement('style');\n" +
        "animStyle.textContent = '@keyframes popIn { from { transform: scale(0.9); opacity: 0; } to { transform: scale(1); opacity: 1; } }';\n" +
        "document.head.appendChild(animStyle);\n"

    layout "Checkout" [
        div [ _class "payment-container" ] [
            div [ _style "text-align: center; margin-bottom: 30px;" ] [
                h2 [] [ str "Checkout" ]
                p [] [ str (sprintf "Buying: %s" aircraft.Model) ]
                h3 [] [ str priceInfo ]
            ]
            
            tag "form" [ attr "action" actionUrl; attr "method" "POST" ] [
                div [ _class "form-group" ] [
                    label [ _class "form-label" ] [ str "Payment Method" ]
                    div [ _class "card-logos" ] [
                        div [ _class "card-logo selected"; attr "onclick" "selectCard(this)" ] [ str "💳 VISA" ]
                        div [ _class "card-logo"; attr "onclick" "selectCard(this)" ] [ str "💳 MasterCard" ]
                    ]
                    input [ _type "hidden"; _name "cardType"; _id "cardType"; _value "VISA" ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "cardNumber" ] [ str "Card Number" ]
                    div [ _class "input-icon" ] [ str "💳" ]
                    input [ _type "text"; _name "cardNumber"; _id "cardNumber"; _class "form-input"; _placeholder "0000 0000 0000 0000"; _required ]
                ]
                div [ _style "display: grid; grid-template-columns: 1fr 1fr; gap: 20px;" ] [
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "expiry" ] [ str "Expiry Date" ]
                        div [ _class "input-icon" ] [ str "📅" ]
                        input [ _type "text"; _name "expiry"; _id "expiry"; _class "form-input"; _placeholder "MM/YY"; _required ]
                    ]
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "cvc" ] [ str "CVC" ]
                        div [ _class "input-icon" ] [ str "🔒" ]
                        input [ _type "text"; _name "cvc"; _id "cvc"; _class "form-input"; _placeholder "123"; _required ]
                    ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "airport" ] [ str "Delivery Airport Address" ]
                    div [ _class "input-icon" ] [ str "📍" ]
                    input [ _type "text"; _name "airport"; _id "airport"; _class "form-input"; _placeholder "Full Address of Airport or ICAO Code"; _required ]
                ]
                div [ _class "form-group" ] [
                     label [ _class "form-label"; attr "for" "country" ] [ str "Country (Detailed)" ]
                     div [ _class "input-icon" ] [ str "🌍" ]
                     input [ _type "text"; _name "country"; _id "country"; _class "form-input"; _placeholder "Singapore, Cambodia, etc."; _required ]
                ]

                // --- LIVE PROMO CODE VALIDATOR ---
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "promoCode" ] [ str "🎟️ Promo Code (Optional)" ]
                    div [ _class "input-icon" ] [ str "🏷️" ]
                    input [
                        _type "text"; _name "promoCode"; _id "promoCode"; _class "form-input"
                        _placeholder "e.g. VIP20 or STAFF50"
                        attr "oninput" "validatePromo(this.value)"
                        attr "autocomplete" "off"
                        attr "style" "text-transform:uppercase;"
                    ]
                    div [ _id "promo-badge"; _style "margin-top: 10px; min-height: 40px;" ] [
                        p [ _id "promo-hint"; _style "font-size: 0.82rem; color: #64748b; margin: 0;" ] [
                            str "Try: "; strong [] [ str "VIP20" ]
                            str " (20% off)  |  "; strong [] [ str "STAFF50" ]
                            str " (50% off)"
                        ]
                    ]
                ]

                // Dynamic price preview box (shown when valid code entered)
                div [ _id "price-preview"; _style "display:none; margin-bottom: 20px; padding: 16px; background:#f0fdf4; border: 3px solid #16a34a; border-radius: 12px; box-shadow: 3px 3px 0px black; text-align:center;" ] [
                    p [ _style "margin: 0; font-size: 0.85rem; color: #64748b; text-decoration: line-through;" ] [
                        str "Original: "; span [ _id "original-price" ] []
                    ]
                    p [ _style "margin: 5px 0 0 0; font-size: 1.5rem; font-weight: 900; color: #16a34a;" ] [
                        str "You pay: "; span [ _id "discounted-price" ] []
                    ]
                    p [ _style "margin: 4px 0 0 0; font-size: 0.85rem; color: #475569;" ] [
                        str "You save: "; span [ _id "saved-amount" ] []
                    ]
                ]

                button [ _type "submit"; _class "btn"; _style "margin-top: 10px; width: 100%;" ] [ str "Complete Purchase" ]
            ]
            p [ _style "text-align: center; margin-top: 20px; font-size: 0.8rem; color: #64748b;" ] [ str "Secure 256-bit SSL Encrypted Transaction" ]
        ]
        script [] [ rawText jsScript ]
    ]



// Updated to accept finalPrice (after lambda discount) and a discountMsg
let successView (aircraft: Aircraft) (finalPrice: decimal<USD>) (discountMsg: string) =
    let originalPriceText = sprintf "$%s USD" ((aircraft.Price / 1m<USD>).ToString("N0"))
    let finalPriceText    = sprintf "$%s USD" ((finalPrice / 1m<USD>).ToString("N0"))
    let hadDiscount       = discountMsg <> "" && finalPrice < aircraft.Price

    layout "Order Placed" [
        div [ _class "container"; _style "text-align: center; padding-top: 50px;" ] [
            div [ _style "font-size: 4rem; margin-bottom: 10px;" ] [ str "✅" ]
            h1 [] [ str "Order Confirmed!" ]
            p [ _style "font-size: 1.2rem;" ] [ str (sprintf "We have received your order for the %s." aircraft.Model) ]

            // Discount Summary Box (only shows if a promo code was used)
            if hadDiscount then
                div [ _style "margin: 25px auto; max-width: 400px; background: #fef08a; border: 3px solid black; border-radius: 12px; padding: 20px; box-shadow: 4px 4px 0px black;" ] [
                    p [ _style "font-weight: 900; font-size: 1.1rem; margin: 0 0 10px 0;" ] [ str (sprintf "🎟️ %s" discountMsg) ]
                    p [ _style "margin: 5px 0; text-decoration: line-through; color: #64748b;" ] [ str (sprintf "Original Price: %s" originalPriceText) ]
                    p [ _style "margin: 5px 0; font-size: 1.5rem; font-weight: 900; color: #16a34a;" ] [ str (sprintf "You Paid: %s" finalPriceText) ]
                    p [ _style "margin: 10px 0 0 0; font-size: 0.85rem; color: #475569;" ] [ str (sprintf "You saved: $%s USD 🎉" ((aircraft.Price - finalPrice) / 1m<USD> |> fun x -> x.ToString("N0"))) ]
                ]
            else
                div [ _style "margin: 25px auto; max-width: 400px; background: white; border: 3px solid black; border-radius: 12px; padding: 20px; box-shadow: 4px 4px 0px black;" ] [
                    p [ _style "font-size: 1.4rem; font-weight: 900; margin: 0;" ] [ str (sprintf "Total Paid: %s" finalPriceText) ]
                ]

            p [ _style "color: #64748b; margin-top: 15px;" ] [ str "We will contact you shortly about delivery." ]
            a [ _href "/"; _class "btn"; _style "margin-top: 20px;" ] [ str "Back to Shop" ]
        ]
    ]
