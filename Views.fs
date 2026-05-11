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

// Professional Aviation B2B portal design.
// Palette: deep navy (#0f2044), gold (#c4a55a), clean white, slate grays.
// Typography: Inter (Google Fonts). No cartoon borders or Comic Sans.
let css = """
:root {
    --navy:       #0f2044;
    --navy-mid:   #1e3a5f;
    --gold:       #c4a55a;
    --gold-light: #f0e0b0;
    --bg:         #f0f4f8;
    --card:       #ffffff;
    --text:       #1e293b;
    --muted:      #64748b;
    --border:     #e2e8f0;
    --radius:     10px;
    --radius-lg:  16px;
    --shadow:     0 2px 12px rgba(15,32,68,0.09);
    --shadow-lg:  0 8px 32px rgba(15,32,68,0.15);
}
*, *::before, *::after { box-sizing: border-box; }
body { font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif; color: var(--text); background: var(--bg); margin: 0; padding: 0; display: flex; flex-direction: column; min-height: 100vh; font-size: 15px; line-height: 1.6; }
h1, h2, h3 { font-weight: 700; letter-spacing: -0.02em; margin-top: 0; color: var(--navy); }
.navbar { background: var(--navy); padding: 0 40px; display: flex; justify-content: space-between; align-items: center; height: 64px; position: sticky; top: 0; z-index: 1000; box-shadow: 0 2px 8px rgba(0,0,0,0.25); }
.nav-brand { font-size: 1.35rem; font-weight: 800; color: var(--gold); text-decoration: none; letter-spacing: 4px; text-transform: uppercase; }
.nav-links { display: flex; align-items: center; gap: 4px; }
.nav-links a { color: rgba(255,255,255,0.78); text-decoration: none; font-weight: 500; font-size: 0.875rem; padding: 8px 14px; border-radius: 6px; transition: all 0.15s; }
.nav-links a:hover { background: rgba(255,255,255,0.10); color: white; }
.nav-user { color: var(--gold); font-weight: 600; font-size: 0.875rem; padding: 6px 14px; border: 1px solid var(--gold); border-radius: 20px; }
.nav-badge { background: var(--gold); color: var(--navy); font-size: 0.7rem; font-weight: 700; border-radius: 10px; padding: 1px 7px; margin-left: 5px; }
.container { max-width: 1200px; margin: 0 auto; padding: 32px 24px; flex: 1; }
.header { background: linear-gradient(135deg, var(--navy) 0%, var(--navy-mid) 100%); color: white; padding: 48px 40px; border-radius: var(--radius-lg); margin-bottom: 40px; }
.header h1 { font-size: 2.25rem; font-weight: 800; color: white; margin-bottom: 8px; letter-spacing: -0.03em; }
.header p { color: rgba(255,255,255,0.68); margin: 0; font-size: 1rem; }
.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 24px; }
.card { background: var(--card); border-radius: var(--radius-lg); box-shadow: var(--shadow); overflow: hidden; transition: transform 0.2s, box-shadow 0.2s; display: flex; flex-direction: column; border: 1px solid var(--border); }
.card:hover { transform: translateY(-4px); box-shadow: var(--shadow-lg); }
.card-img-placeholder { height: 200px; background-color: var(--navy); background-size: cover; background-position: center; }
.card-content { padding: 20px 24px 24px; display: flex; flex-direction: column; flex: 1; }
.card-title { font-size: 1.1rem; font-weight: 700; margin-bottom: 4px; color: var(--navy); }
.card-subtitle { font-size: 1.2rem; font-weight: 700; color: var(--gold); margin-bottom: 12px; }
.card-desc { font-size: 0.875rem; color: var(--muted); line-height: 1.6; margin-bottom: 16px; flex: 1; }
.tag { display: inline-block; padding: 3px 10px; border-radius: 20px; font-size: 0.72rem; font-weight: 600; margin-bottom: 14px; letter-spacing: 0.04em; text-transform: uppercase; }
.tag.Airbus { background: #dbeafe; color: #1e40af; }
.tag.Boeing { background: #dcfce7; color: #166534; }
.tag.Embraer { background: #fef9c3; color: #854d0e; }
.tag.Bombardier { background: #fee2e2; color: #991b1b; }
.tag.COMAC { background: #f3e8ff; color: #6b21a8; }
.btn { display: inline-block; background: var(--navy); color: white; text-decoration: none; padding: 10px 20px; font-weight: 600; font-size: 0.875rem; border: none; border-radius: 8px; cursor: pointer; transition: all 0.15s; text-align: center; letter-spacing: 0.02em; font-family: 'Inter', sans-serif; }
.btn:hover { background: var(--navy-mid); transform: translateY(-1px); box-shadow: 0 4px 12px rgba(15,32,68,0.25); }
.btn:active { transform: translateY(0); box-shadow: none; }
input, select { font-family: 'Inter', sans-serif; font-size: 0.9375rem; border: 1.5px solid var(--border) !important; border-radius: 8px; background: white; transition: border-color 0.15s, box-shadow 0.15s; box-shadow: none !important; }
input:focus, select:focus { outline: none; border-color: var(--navy) !important; box-shadow: 0 0 0 3px rgba(15,32,68,0.10) !important; }
.chart-container { background: var(--card); border: 1px solid var(--border); border-radius: var(--radius-lg); padding: 24px; margin-bottom: 36px; box-shadow: var(--shadow); }
.payment-container { max-width: 580px; margin: 40px auto; padding: 40px; background: var(--card); border: 1px solid var(--border); box-shadow: var(--shadow-lg); border-radius: var(--radius-lg); }
.form-group { margin-bottom: 22px; position: relative; }
.form-label { display: block; margin-bottom: 7px; font-weight: 600; font-size: 0.875rem; color: var(--text); }
.form-input { width: 100%; padding: 11px 14px; border-radius: 8px; font-size: 0.9375rem; border: 1.5px solid var(--border); box-sizing: border-box; font-family: 'Inter', sans-serif; transition: border-color 0.15s, box-shadow 0.15s; }
.form-input:focus { outline: none; border-color: var(--navy); box-shadow: 0 0 0 3px rgba(15,32,68,0.10); }
.card-logos { display: flex; gap: 10px; margin-top: 8px; }
.card-logo { border: 1.5px solid var(--border); border-radius: 8px; padding: 8px 18px; cursor: pointer; font-weight: 600; font-size: 0.875rem; transition: all 0.15s; background: white; color: var(--text); }
.card-logo:hover { border-color: var(--navy); }
.card-logo.selected { border: 2px solid var(--navy); background: #eff6ff; color: var(--navy); font-weight: 700; }
.empty-state { text-align: center; padding: 60px 20px; background: var(--card); border: 1px solid var(--border); border-radius: var(--radius-lg); box-shadow: var(--shadow); margin: 40px 0; }
.empty-state h2 { color: var(--navy); }
.empty-state p { color: var(--muted); margin-bottom: 24px; }
.footer { background: var(--navy); color: rgba(255,255,255,0.60); text-align: center; padding: 32px; margin-top: 60px; font-size: 0.875rem; }
.footer p { margin: 5px 0; }
.footer-links a { color: var(--gold); text-decoration: none; margin: 0 10px; font-weight: 500; }
.track-container { max-width: 700px; margin: 40px auto; padding: 40px; background: var(--card); border: 1px solid var(--border); box-shadow: var(--shadow-lg); border-radius: var(--radius-lg); }
.pipeline { display: flex; justify-content: space-between; align-items: flex-start; margin: 36px 0; position: relative; }
.pipeline::before { content: ''; position: absolute; top: 20px; left: 30px; right: 30px; height: 2px; background: var(--border); z-index: 0; }
.pipeline-step { display: flex; flex-direction: column; align-items: center; gap: 8px; z-index: 1; flex: 1; }
.step-circle { width: 42px; height: 42px; border-radius: 50%; border: 2px solid var(--border); display: flex; align-items: center; justify-content: center; font-size: 0.8rem; font-weight: 700; background: white; color: var(--muted); box-shadow: var(--shadow); }
.step-done .step-circle    { background: #dcfce7; border-color: #16a34a; color: #166534; }
.step-active .step-circle  { background: var(--gold-light); border-color: var(--gold); color: var(--navy); animation: pulse 2s infinite; }
.step-pending .step-circle { background: white; border-color: var(--border); color: var(--muted); }
@keyframes pulse { 0%, 100% { box-shadow: 0 0 0 0 rgba(196,165,90,0.35); } 50% { box-shadow: 0 0 0 8px rgba(196,165,90,0); } }
.step-label { font-size: 0.64rem; font-weight: 600; text-align: center; text-transform: uppercase; letter-spacing: 0.04em; max-width: 70px; line-height: 1.3; color: var(--muted); }
.step-done .step-label   { color: #166534; }
.step-active .step-label { color: var(--gold); }
.order-info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-top: 24px; }
.order-info-cell { background: var(--bg); border: 1px solid var(--border); border-radius: 8px; padding: 12px 16px; }
.order-info-cell .oi-label { font-size: 0.7rem; font-weight: 600; color: var(--muted); text-transform: uppercase; letter-spacing: 0.06em; margin-bottom: 4px; }
.order-info-cell .oi-value { font-size: 0.9375rem; font-weight: 600; color: var(--navy); }
"""

let layout (title: string) (cartCount: int) (username: string option) (content: XmlNode list) =
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
                        a [ _href "/track" ] [ str "Track Order" ]
                        a [ _href "/cart"; _style "position:relative;" ] [
                            str "Cart"
                            if cartCount > 0 then
                                span [ _style "position:absolute;top:-8px;right:-10px;background:#f472b6;color:black;border:2px solid black;border-radius:50%;width:20px;height:20px;font-size:0.7rem;font-weight:900;display:flex;align-items:center;justify-content:center;box-shadow:1px 1px 0px black;" ] [ str (string cartCount) ]
                        ]
                        match username with
                        | Some name ->
                            span [ _style "font-weight:600;padding:6px 14px;border:1px solid var(--gold);border-radius:20px;color:var(--gold);" ] [ str name ]
                            a [ _href "/logout"; _style "color:#dc2626;font-weight:bold;padding:8px 12px;border:2px solid #dc2626;border-radius:8px;margin-left:8px;text-decoration:none;transition:all 0.2s;" ] [ str "Logout" ]
                        | None ->
                            a [ _href "/login"    ] [ str "Login" ]
                            a [ _href "/register"; _style "background:var(--gold);color:var(--navy);border-radius:6px;font-weight:600;" ] [ str "Register" ]
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

let dashboardView (aircrafts: Aircraft list) (topStats: Aircraft list) (searchQuery: string) (brandFilter: string) (cartCount: int) (username: string option) =
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

    layout "Skypay" cartCount username [
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
                div [ _class "emoji" ] [ str "No results" ]
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
                            div [ _style "display:flex;gap:10px;margin-top:auto;flex-wrap:wrap;" ] [
                                a [ _href linkUrl; _class "btn"; _style "flex:1;text-align:center;" ] [ str "Buy Now" ]
                                tag "form" [ attr "action" (sprintf "/cart/add/%s" (item.Id.ToString())); attr "method" "POST"; attr "style" "flex:1;" ] [
                                    button [ _type "submit"; _class "btn"; _style "width:100%;background:#3b82f6;color:white;" ] [ str "Add to Cart" ]
                                ]
                            ]
                        ]
                    ]
                )
            )

        script [] [ rawText chartScript ]
    ]

let paymentView (aircraft: Aircraft) (cartCount: int) (username: string option) =
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

    layout "Checkout" cartCount username [
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
                        div [ _class "card-logo selected"; attr "onclick" "selectCard(this)" ] [ str "VISA" ]
                        div [ _class "card-logo"; attr "onclick" "selectCard(this)" ] [ str "MasterCard" ]
                    ]
                    input [ _type "hidden"; _name "cardType"; _id "cardType"; _value "VISA" ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "cardNumber" ] [ str "Card Number" ]
                    input [ _type "text"; _name "cardNumber"; _id "cardNumber"; _class "form-input"; _placeholder "0000 0000 0000 0000"; _required ]
                ]
                div [ _style "display: grid; grid-template-columns: 1fr 1fr; gap: 20px;" ] [
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "expiry" ] [ str "Expiry Date" ]
                        input [ _type "text"; _name "expiry"; _id "expiry"; _class "form-input"; _placeholder "MM/YY"; _required ]
                    ]
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "cvc" ] [ str "CVC" ]
                        input [ _type "text"; _name "cvc"; _id "cvc"; _class "form-input"; _placeholder "123"; _required ]
                    ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "airport" ] [ str "Delivery Airport Address" ]
                    input [ _type "text"; _name "airport"; _id "airport"; _class "form-input"; _placeholder "Full Address of Airport or ICAO Code"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "country" ] [ str "Country (Detailed)" ]
                    input [ _type "text"; _name "country"; _id "country"; _class "form-input"; _placeholder "Singapore, Cambodia, etc."; _required ]
                ]

                // --- LIVE PROMO CODE VALIDATOR ---
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "promoCode" ] [ str "Promo Code (Optional)" ]
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
let successView (aircraft: Aircraft) (finalPrice: decimal<USD>) (discountMsg: string) (cartCount: int) (username: string option) =
    let originalPriceText = sprintf "$%s USD" ((aircraft.Price / 1m<USD>).ToString("N0"))
    let finalPriceText    = sprintf "$%s USD" ((finalPrice / 1m<USD>).ToString("N0"))
    let hadDiscount       = discountMsg <> "" && finalPrice < aircraft.Price

    layout "Order Placed" cartCount username [
        div [ _class "container"; _style "text-align: center; padding-top: 50px;" ] [
            
            h1 [] [ str "Order Confirmed!" ]
            p [ _style "font-size: 1.2rem;" ] [ str (sprintf "We have received your order for the %s." aircraft.Model) ]

            // Discount Summary Box (only shows if a promo code was used)
            if hadDiscount then
                div [ _style "margin: 25px auto; max-width: 400px; background: #fef08a; border: 3px solid black; border-radius: 12px; padding: 20px; box-shadow: 4px 4px 0px black;" ] [
                    p [ _style "font-weight: 900; font-size: 1.1rem; margin: 0 0 10px 0;" ] [ str discountMsg ]
                    p [ _style "margin: 5px 0; text-decoration: line-through; color: #64748b;" ] [ str (sprintf "Original Price: %s" originalPriceText) ]
                    p [ _style "margin: 5px 0; font-size: 1.5rem; font-weight: 900; color: #16a34a;" ] [ str (sprintf "You Paid: %s" finalPriceText) ]
                    p [ _style "margin: 10px 0 0 0; font-size: 0.85rem; color: #475569;" ] [ str (sprintf "You saved: $%s USD" ((aircraft.Price - finalPrice) / 1m<USD> |> fun x -> x.ToString("N0"))) ]
                ]
            else
                div [ _style "margin: 25px auto; max-width: 400px; background: white; border: 3px solid black; border-radius: 12px; padding: 20px; box-shadow: 4px 4px 0px black;" ] [
                    p [ _style "font-size: 1.4rem; font-weight: 900; margin: 0;" ] [ str (sprintf "Total Paid: %s" finalPriceText) ]
                ]

            p [ _style "color: #64748b; margin-top: 15px;" ] [ str "We will contact you shortly about delivery." ]
            // Tracking hint box
            div [ _style "margin: 20px auto; max-width: 420px; background: #eff6ff; border: 3px solid black; border-radius: 12px; padding: 16px 20px; box-shadow: 4px 4px 0px black; text-align:left;" ] [
                p [ _style "margin:0 0 6px 0; font-weight:900; font-size:1rem;" ] [ str "Track Your Delivery" ]
                p [ _style "margin:0 0 10px 0; font-size:0.85rem; color:#475569;" ] [ str "Use one of these demo order numbers on the Track page:" ]
                p [ _style "margin:0; font-size:0.85rem; font-weight:700;" ] [
                    str "SKY-2026-001  •  SKY-2026-002  •  SKY-2026-003"
                ]
            ]
            div [ _style "margin-top:20px; display:flex; gap:12px; justify-content:center; flex-wrap:wrap;" ] [
                a [ _href "/"; _class "btn" ] [ str "← Back to Shop" ]
                a [ _href "/track"; _class "btn"; _style "background:var(--accent);color:white;" ] [ str "Track Order" ]
            ]
        ]
    ]

// ==========================================
// ORDER TRACKING VIEW
// ==========================================
// This view uses pattern matching on the DeliveryStatus DU.
// The compiler guarantees we handle ALL 5 cases — no bugs from missed branches.
let orderTrackView (query: string) (result: OrderRecord option) (cartCount: int) (username: string option) =
    let queryDisplay = if isNull query then "" else query.ToUpper()

    // The 5 pipeline steps: (icon, label)
    let steps = [
        ("1", "Order Confirmed")
        ("2", "In Production")
        ("3", "Ready for Delivery")
        ("4", "In Transit")
        ("5", "Delivered")
    ]

    // Pattern match on DeliveryStatus to get the active step index
    let activeStep =
        match result with
        | Some { Status = OrderConfirmed }   -> 0
        | Some { Status = InProduction }     -> 1
        | Some { Status = ReadyForDelivery } -> 2
        | Some { Status = InTransit }        -> 3
        | Some { Status = Delivered }        -> 4
        | None                               -> -1

    // Pattern match to render the correct status badge
    let statusBadgeHtml =
        match result with
        | Some { Status = s } ->
            let text, color =
                match s with
                | OrderConfirmed   -> "Order Confirmed",      "#93c5fd"
                | InProduction     -> "In Production",       "#fde047"
                | ReadyForDelivery -> "Ready for Delivery",  "#86efac"
                | InTransit        -> "In Transit",          "#fbbf24"
                | Delivered        -> "Delivered",           "#4ade80"
            [ span [ _style (sprintf "display:inline-block;padding:6px 18px;background:%s;border:2px solid black;border-radius:20px;font-weight:900;box-shadow:2px 2px 0px black;font-size:1rem;" color) ] [ str text ] ]
        | None -> []

    // Build the pipeline nodes from the steps list
    let buildPipeline () =
        steps |> List.mapi (fun i (icon, label) ->
            let cls =
                if   i < activeStep then "pipeline-step step-done"
                elif i = activeStep then "pipeline-step step-active"
                else                     "pipeline-step step-pending"
            let circleIcon = if i < activeStep then "✓" else icon
            div [ _class cls ] [
                div [ _class "step-circle" ] [ str circleIcon ]
                span [ _class "step-label" ] [ str label ]
            ]
        )

    // Build the result HTML section
    let resultSection =
        match result with
        | None when queryDisplay <> "" ->
            [ div [ _style "text-align:center;padding:30px;background:#fee2e2;border:3px solid #dc2626;border-radius:12px;box-shadow:4px 4px 0px black;" ] [
                h2 [ _style "color:#dc2626;" ] [ str "Order Not Found" ]
                p [] [ str (sprintf "No order found matching \"%s\". Please double-check your order number." queryDisplay) ]
              ] ]
        | Some order ->
            [
                div [ _style "text-align:center;margin-bottom:18px;" ] statusBadgeHtml
                div [ _style "background:var(--primary);border:3px solid black;border-radius:12px;padding:18px 20px;text-align:center;box-shadow:4px 4px 0px black;margin-bottom:25px;" ] [
                    h2 [ _style "margin:0 0 4px 0;" ] [ str order.AircraftModel ]
                    p  [ _style "margin:0;font-weight:bold;" ] [ str (sprintf "Manufactured by %s" order.Manufacturer) ]
                ]
                div [ _class "pipeline" ] (buildPipeline ())
                div [ _class "order-info-grid" ] [
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Order Number" ]
                        div [ _class "oi-value" ] [ str order.OrderNumber ]
                    ]
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Order Date" ]
                        div [ _class "oi-value" ] [ str order.OrderDate ]
                    ]
                    div [ _class "order-info-cell"; _style "grid-column:span 2;" ] [
                        div [ _class "oi-label" ] [ str "Delivery Destination" ]
                        div [ _class "oi-value" ] [ str order.Destination ]
                    ]
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Est. Delivery" ]
                        div [ _class "oi-value" ] [ str order.EstimatedDate ]
                    ]
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Manufacturer" ]
                        div [ _class "oi-value" ] [ str order.Manufacturer ]
                    ]
                ]
                div [ _style "margin-top:28px;display:flex;gap:12px;justify-content:center;flex-wrap:wrap;" ] [
                    a [ _href "/"; _class "btn" ] [ str "← Back to Shop" ]
                    a [ _href "/track"; _class "btn"; _style "background:var(--accent);color:white;" ] [ str "Track Another" ]
                ]
            ]
        | _ -> []

    // Static top section — always shown
    let topSection = [
        div [ _style "text-align:center;margin-bottom:28px;" ] [
            h1 [] [ str "Track Your Order" ]
            p [ _style "color:#64748b;" ] [ str "Enter your Skypay order number to check your aircraft delivery status." ]
        ]
        form [ _action "/track"; _method "get"; _style "display:flex;gap:10px;margin-bottom:14px;" ] [
            input [
                _type "text"; _name "order"; _id "orderInput"
                _placeholder "e.g. SKY-2026-001"
                _value queryDisplay
                _style "flex:1;padding:12px 16px;border-radius:8px;font-size:16px;text-transform:uppercase;"
            ]
            button [ _type "submit"; _class "btn"; _style "white-space:nowrap;" ] [ str "Search" ]
        ]
        p [ _style "font-size:0.82rem;color:#64748b;margin-bottom:22px;" ] [
            str "Demo orders: "
            strong [] [ str "SKY-2026-001" ]; str "  •  "
            strong [] [ str "SKY-2026-002" ]; str "  •  "
            strong [] [ str "SKY-2026-003" ]; str "  •  "
            strong [] [ str "SKY-2026-004" ]; str "  •  "
            strong [] [ str "SKY-2026-005" ]
        ]
    ]

    layout "Track Your Order | Skypay" cartCount username [
        div [ _class "track-container" ] (topSection @ resultSection)
    ]

// ==========================================
// SHOPPING CART VIEW
// ==========================================
// Shows all items in the session cart, a live currency converter,
// a running total, and individual checkout links.
let cartView (items: CartItem list) (cartCount: int) (username: string option) =
    // Compute total in USD (using F# Units of Measure)
    let totalUsd = items |> List.sumBy (fun i -> i.PriceUsd)

    // Currency conversions using the HOFs defined in Models.fs
    // We embed the rates as JS constants so the client-side toggle is instant.
    let ratesJs =
        "const rates = { USD: 1.0, EUR: " + string usdToEurRate + ", SGD: " + string usdToSgdRate + ", GBP: " + string usdToGbpRate + " };\n" +
        "const symbols = { USD: '$', EUR: '€', SGD: 'S$', GBP: '£' };\n" +
        "let activeCurrency = 'USD';\n" +
        "function setCurrency(code) {\n" +
        "  activeCurrency = code;\n" +
        "  document.querySelectorAll('.cur-btn').forEach(b => b.classList.toggle('selected', b.dataset.cur === code));\n" +
        "  document.querySelectorAll('[data-usd]').forEach(el => {\n" +
        "    const usd = parseFloat(el.dataset.usd);\n" +
        "    const converted = usd * rates[code];\n" +
        "    el.textContent = symbols[code] + Math.round(converted).toLocaleString();\n" +
        "  });\n" +
        "}\n"

    layout "Shopping Cart | Skypay" cartCount username [
        div [ _class "track-container"; _style "max-width:800px;" ] [

            // ---- Header ----
            div [ _style "text-align:center;margin-bottom:28px;" ] [
                h1 [] [ str "Your Cart" ]
                p [ _style "color:#64748b;" ] [
                    str (sprintf "%d item(s) selected" items.Length)
                ]
            ]

            // ---- Currency Switcher ----
            // Uses convertFromUsd (partial application) — rates injected from F# into JS
            div [ _style "margin-bottom:24px;background:#f0fdf4;border:3px solid black;border-radius:12px;padding:16px 20px;box-shadow:4px 4px 0px black;" ] [
                p [ _style "margin:0 0 10px 0;font-weight:900;font-size:0.95rem;" ] [ str "Currency Converter (F# Units of Measure)" ]
                div [ _style "display:flex;gap:10px;flex-wrap:wrap;" ] [
                    for (code, flag) in [("USD","$");("EUR","€");("SGD","S$");("GBP","£")] do
                        button [
                            _class "btn cur-btn"
                            attr "data-cur" code
                            attr "onclick" (sprintf "setCurrency('%s')" code)
                            _style (if code = "USD" then "background:var(--primary);border:3px solid black;" else "background:white;")
                        ] [ str (sprintf "%s %s" flag code) ]
                ]
            ]

            // ---- Empty State ----
            if items.IsEmpty then
                div [ _class "empty-state" ] [
                    div [ _class "emoji" ] [ str "Empty" ]
                    h2 [] [ str "Your cart is empty!" ]
                    p [] [ str "Browse our inventory and add some aircraft." ]
                    a [ _href "/"; _class "btn" ] [ str "← Browse Aircraft" ]
                ]
            else
                // ---- Cart Items ----
                div [ _style "display:flex;flex-direction:column;gap:16px;margin-bottom:28px;" ] (
                    items |> List.map (fun item ->
                        let bgStyle = sprintf "background-image:url('%s');background-size:cover;background-position:center;" item.ImageUrl
                        div [ _style "display:flex;gap:16px;align-items:center;background:white;border:3px solid black;border-radius:12px;padding:16px;box-shadow:4px 4px 0px black;" ] [
                            div [ _style (sprintf "width:90px;height:65px;border-radius:8px;border:2px solid black;flex-shrink:0;%s" bgStyle) ] []
                            div [ _style "flex:1;" ] [
                                p [ _style "margin:0 0 2px 0;font-weight:900;font-size:1.1rem;" ] [ str item.Model ]
                                p [ _style "margin:0;font-size:0.85rem;color:#64748b;" ] [ str item.Manufacturer ]
                                p [ _style "margin:4px 0 0 0;font-weight:900;color:var(--accent);font-size:1.05rem;" ] [
                                    span [ attr "data-usd" (string item.PriceUsd) ] [
                                        str (sprintf "$%s" (item.PriceUsd.ToString("N0")))
                                    ]
                                    str " USD"
                                ]
                            ]
                            div [ _style "display:flex;flex-direction:column;gap:8px;" ] [
                                a [ _href (sprintf "/checkout/%s" item.AircraftId); _class "btn"; _style "font-size:0.8rem;padding:8px 14px;text-align:center;" ] [ str "Buy Now" ]
                                a [ _href (sprintf "/cart/remove/%s" item.AircraftId); _style "font-size:0.8rem;padding:8px 14px;text-align:center;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;font-weight:900;color:#dc2626;text-decoration:none;box-shadow:2px 2px 0px black;" ] [ str "Remove" ]
                            ]
                        ]
                    )
                )

                // ---- Total + Actions ----
                div [ _style "background:var(--primary);border:3px solid black;border-radius:12px;padding:20px 24px;box-shadow:6px 6px 0px black;text-align:center;" ] [
                    p [ _style "margin:0 0 4px 0;font-size:0.9rem;font-weight:700;text-transform:uppercase;letter-spacing:1px;" ] [ str "Cart Total" ]
                    p [ _style "margin:0 0 16px 0;font-size:2.5rem;font-weight:900;" ] [
                        span [ attr "data-usd" (string totalUsd) ] [
                            str (sprintf "$%s" (totalUsd.ToString("N0")))
                        ]
                        str " USD"
                    ]
                    div [ _style "display:flex;gap:12px;justify-content:center;flex-wrap:wrap;" ] [
                        a [ _href "/"; _class "btn"; _style "background:white;" ] [ str "← Continue Shopping" ]
                        a [ _href "/cart/clear"; _style "padding:12px 24px;background:#fee2e2;border:3px solid #dc2626;border-radius:8px;font-weight:900;color:#dc2626;text-decoration:none;box-shadow:4px 4px 0px black;" ] [ str "Clear Cart" ]
                    ]
                ]

            script [] [ rawText ratesJs ]
        ]
    ]

// ==========================================
// LOGIN VIEW
// ==========================================
let loginView (errorMsg: string option) =
    layout "Login | Skypay" 0 None [
        div [ _class "payment-container"; _style "margin-top:60px;" ] [
            div [ _style "text-align:center;margin-bottom:28px;" ] [
                h1 [ _style "margin:8px 0 4px;" ] [ str "Welcome Back" ]
                p [ _style "color:#64748b;margin:0;" ] [ str "Log in to your Skypay account" ]
            ]
            match errorMsg with
            | Some msg ->
                div [ _style "padding:12px 16px;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;box-shadow:2px 2px 0px black;margin-bottom:20px;font-weight:700;color:#dc2626;" ] [
                    str msg
                ]
            | None -> ()
            tag "form" [ attr "action" "/login"; attr "method" "POST" ] [
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "username" ] [ str "Username" ]
                    input [ _type "text"; _name "username"; _id "username"; _class "form-input"; _placeholder "Your username"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "password" ] [ str "Password" ]
                    input [ _type "password"; _name "password"; _id "password"; _class "form-input"; _placeholder "Your password"; _required ]
                ]
                button [ _type "submit"; _class "btn"; _style "width:100%;margin-top:8px;" ] [ str "Log In" ]
            ]
            p [ _style "text-align:center;margin-top:20px;color:#64748b;" ] [
                str "No account yet? "
                a [ _href "/register"; _style "font-weight:900;color:var(--accent);" ] [ str "Register here" ]
            ]
        ]
    ]

// ==========================================
// REGISTER VIEW
// ==========================================
let registerView (errorMsg: string option) =
    layout "Register | Skypay" 0 None [
        div [ _class "payment-container"; _style "margin-top:60px;" ] [
            div [ _style "text-align:center;margin-bottom:28px;" ] [
                h1 [ _style "margin:8px 0 4px;" ] [ str "Create Account" ]
                p [ _style "color:#64748b;margin:0;" ] [ str "Join Skypay and start buying aircraft" ]
            ]
            match errorMsg with
            | Some msg ->
                div [ _style "padding:12px 16px;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;box-shadow:2px 2px 0px black;margin-bottom:20px;font-weight:700;color:#dc2626;" ] [
                    str msg
                ]
            | None -> ()
            tag "form" [ attr "action" "/register"; attr "method" "POST" ] [
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "username" ] [ str "Username" ]
                    input [ _type "text"; _name "username"; _id "username"; _class "form-input"; _placeholder "min. 3 characters"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "email" ] [ str "Email Address" ]
                    input [ _type "email"; _name "email"; _id "email"; _class "form-input"; _placeholder "you@gmail.com"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "password" ] [ str "Password" ]
                    input [ _type "password"; _name "password"; _id "password"; _class "form-input"; _placeholder "min. 6 characters"; _required ]
                ]
                button [ _type "submit"; _class "btn"; _style "width:100%;margin-top:8px;background:#3b82f6;color:white;" ] [ str "Create Account" ]
            ]
            p [ _style "text-align:center;margin-top:20px;color:#64748b;" ] [
                str "Already have an account? "
                a [ _href "/login"; _style "font-weight:900;color:var(--accent);" ] [ str "Login here" ]
            ]
        ]
    ]
