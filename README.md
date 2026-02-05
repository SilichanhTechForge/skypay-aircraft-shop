# Skypay - Aircraft Sales Portal

This is my project for the Aircraft Sales Website. I built it using **F#** because I wanted to try functional programming for web development. It is a full-stack web application where users can browse different aircraft, view sales statistics, and "buy" planes (simulation).

## 🛠 Libraries I Used

I stuck to a few key libraries to keep things lightweight and fast:

1.  **Giraffe**: This is the main web framework I used. It's built on top of ASP.NET Core but lets me write code in a functional F# style. It handles all the routing (like `/dashboard` or `/checkout`).
2.  **Giraffe.ViewEngine**: Instead of writing separate HTML files, I used this to write HTML directly inside my F# code. This makes it easier to pass data (like price or model name) straight into the page.
3.  **Chart.js**: I used this for the "Most Popular Models" graph on the dashboard. It runs in the browser (JavaScript) but I calculate the data on the server first.
4.  **System.IO/ASP.NET Static Files**: Used to serve the local images from the `wwwroot` folder.

## 🏗 How I Structured the Code

I organized the project using a variation of the **MVC (Model-View-Controller)** pattern so the code is easy to read:

### 1. `Models.fs` (The Data)

This is where I define what an "Aircraft" actually is (Name, Price, Manufacturer, etc.).
Since I didn't set up a complex SQL database for this project, I created a **Mock Database** list right in this file. It holds all the data for the 22 aircraft we have in stock.

### 2. `Views.fs` (The User Interface)

This file handles everything the user sees.

- **HTML Generation**: I use functions to create the grid of aircraft cards.
- **CSS / Styling**: I put my custom CSS here. I went for a **"Cartoon/Neo-Brutalist" style** (thick black borders, pop colors like yellow and pink) because I wanted it to look fun and unique, not like a standard template.
- **Search Bar**: I added the form here that lets users type a name or filter by manufacturer.

### 3. `Program.fs` (The Controller)

This is the "brain" of the application.

- **Routing**: It decides what function to run when someone visits `/` or `/checkout`.
- **Filtering Logic**: This is where the "100 Point" feature lives. When a user searches for something, this file grabs the query from the URL (like `?q=Boeing`), filters the list from `Models.fs`, and sends only the matching planes to `Views.fs`.

## 🌟 Extra Features

To make the project stand out, I added:

- **Dynamic Search & Filter**: You can search by text AND filter by brand at the same time. The server handles the logic so it's robust.
- **Responsive Design**: The grid layout adjusts automatically if you resize the window.
- **Local Images**: I manually curated 20+ images and mapped them to the specific aircraft models so every listing looks real.

## Screen-shot
<img width="1214" height="867" alt="image" src="https://github.com/user-attachments/assets/9c1a0960-8032-4f73-b706-c40e21811b59" />

<img width="1206" height="916" alt="image" src="https://github.com/user-attachments/assets/46ceb207-66e7-4ff7-946d-e82c4e53ab7d" />


## How to Run

1.  Open the folder in a terminal.
2.  Run `dotnet run`.
3.  Go to `http://localhost:5016` (or https://skypay-aircraft-shop.onrender.com ).




