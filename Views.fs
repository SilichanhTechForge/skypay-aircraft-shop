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
    font-family: 'Comic Sans MS', 'Chalkboard SE', 'Marker Felt', sans-serif;
    color: var(--text-main);
    background-color: var(--bg-color);
    background-image: radial-gradient(#000 1px, transparent 1px);
    background-size: 20px 20px;
    margin: 0;
    padding: 20px;
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
}

/* Header */
.header {
    background-color: var(--primary);
    border: var(--border-width) solid black;
    box-shadow: var(--shadow-offset) var(--shadow-offset) 0px 0px black;
    padding: 30px;
    border-radius: 12px;
    margin-bottom: 40px;
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
    transition: transform 0.1s, box-shadow 0.1s;
}

.card:hover {
    transform: translate(-2px, -2px);
    box-shadow: 6px 6px 0px 0px black;
}

.card-content {
    padding: 20px;
}

.card-title {
    font-size: 1.5rem;
    margin-bottom: 5px;
    color: black;
}

.card-subtitle {
    font-size: 1.2rem;
    font-weight: bold;
    color: var(--accent);
    margin-bottom: 15px;
}

.card-desc {
    font-size: 0.95rem;
    line-height: 1.5;
    margin-bottom: 20px;
}

/* Buttons */
.btn {
    display: inline-block;
    background-color: var(--secondary);
    color: black;
    text-decoration: none;
    padding: 12px 24px;
    font-weight: bold;
    border: var(--border-width) solid black;
    border-radius: 8px;
    box-shadow: 3px 3px 0px 0px black;
    cursor: pointer;
    transition: all 0.1s;
}

.btn:hover {
    background-color: #fce7f3;
    transform: translate(-2px, -2px);
    box-shadow: 5px 5px 0px 0px black;
}

.btn:active {
    transform: translate(2px, 2px);
    box-shadow: 0px 0px 0px 0px black;
}

/* Forms */
input, select {
    border: var(--border-width) solid black !important;
    box-shadow: 3px 3px 0px 0px rgba(0,0,0,0.1);
    font-family: inherit;
}

input:focus, select:focus {
    outline: none;
    background-color: #e0f2fe;
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
    padding: 30px;
    background: white;
    border: var(--border-width) solid black;
    box-shadow: 8px 8px 0px 0px black;
    border-radius: 15px;
}

.form-group { margin-bottom: 20px; }
.form-label { display: block; margin-bottom: 8px; font-weight: bold; }
.form-input {
    width: 100%;
    padding: 12px;
    border-radius: 8px;
    font-size: 16px;
    border: var(--border-width) solid black;
}
.card-logos { display: flex; gap: 15px; margin-top: 10px; }
.card-logo {
    border: 2px solid #ddd;
    border-radius: 8px;
    padding: 10px 20px;
    cursor: pointer;
    font-weight: bold;
}
.card-logo.selected {
    border: var(--border-width) solid black;
    background-color: var(--primary);
    box-shadow: 3px 3px 0px 0px black;
}
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
                div [ _class "container" ] content
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

        div [ _class "grid" ] (
            aircrafts |> List.map (fun item ->
                // Simplified description logic
                let descText = 
                    if item.Description.Length > 80 then 
                        item.Description.Substring(0, 80) + "..." 
                    else 
                        item.Description
                
                let bgStyle = sprintf "height: 200px; background-color: #334155; background-image: url('%s'); background-size: cover; background-position: center;" item.ImageUrl
                let priceText = sprintf "$%s USD" (item.Price.ToString("N0"))
                let mfgText = sprintf "Manufacturer: %s" item.Manufacturer
                let linkUrl = sprintf "/checkout/%s" (item.Id.ToString())

                div [ _class "card" ] [
                    // Placeholder image div
                    div [ _style bgStyle ] []
                    div [ _class "card-content" ] [
                        h3 [ _class "card-title" ] [ str item.Model ]
                        div [ _class "card-subtitle" ] [ str priceText ]
                        p [ _class "card-desc" ] [ str descText ]
                        p [ _style "font-size: 0.8rem; color: #64748b; margin-bottom: 20px;" ] [ str mfgText ]
                        a [ _href linkUrl; _class "btn" ] [ str "Select Aircraft" ]
                    ]
                ]
            )
        )

        script [] [ rawText chartScript ]
    ]

let paymentView (aircraft: Aircraft) =
    let priceInfo = sprintf "$%s" (aircraft.Price.ToString("N0"))
    let actionUrl = sprintf "/process-payment/%O" aircraft.Id

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
                        div [ _class "card-logo selected"; attr "onclick" "selectCard(this)" ] [ str "VISA" ]
                        div [ _class "card-logo"; attr "onclick" "selectCard(this)" ] [ str "MasterCard" ]
                    ]
                    input [ _type "hidden"; _name "cardType"; _id "cardType"; _value "VISA" ]
                ]

                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "cardNumber" ] [ str "Card Number" ]
                    input [ _type "text"; _id "cardNumber"; _class "form-input"; _placeholder "0000 0000 0000 0000"; _required ]
                ]

                div [ _style "display: grid; grid-template-columns: 1fr 1fr; gap: 20px;" ] [
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "expiry" ] [ str "Expiry Date" ]
                        input [ _type "text"; _id "expiry"; _class "form-input"; _placeholder "MM/YY"; _required ]
                    ]
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "cvc" ] [ str "CVC" ]
                        input [ _type "text"; _id "cvc"; _class "form-input"; _placeholder "123"; _required ]
                    ]
                ]

                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "airport" ] [ str "Delivery Airport Address" ]
                    input [ _type "text"; _id "airport"; _class "form-input"; _placeholder "Full Address of Airport or ICAO Code"; _required ]
                ]
                
                div [ _class "form-group" ] [
                     label [ _class "form-label"; attr "for" "country" ] [ str "Country (Detailed)" ]
                     input [ _type "text"; _id "country"; _class "form-input"; _placeholder "Singapore, Cambodia, etc."; _required ]
                ]

                button [ _type "submit"; _class "btn"; _style "margin-top: 10px;" ] [ str "Complete Purchase ($)" ]
            ]
            
            p [ _style "text-align: center; margin-top: 20px; font-size: 0.8rem; color: #64748b;" ] [ str "Secure 256-bit SSL Encrypted Transaction" ]
        ]

        script [] [ rawText """
            function selectCard(element) {
                document.querySelectorAll('.card-logo').forEach(el => el.classList.remove('selected'));
                element.classList.add('selected');
                document.getElementById('cardType').value = element.innerText;
            }
        """ ]
    ]

let successView (aircraft: Aircraft) =
    layout "Order Placed" [
        div [ _class "container"; _style "text-align: center; padding-top: 50px;" ] [
            h1 [] [ str "Order Placed" ]
            p [] [ str (sprintf "We have received your order for the %s." aircraft.Model) ]
            p [] [ str "We will contact you shortly about delivery." ]
            a [ _href "/"; _class "btn"; _style "margin-top: 20px;" ] [ str "Back to Shop" ]
        ]
    ]
