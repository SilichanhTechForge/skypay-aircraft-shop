open System.IO
open Skypay.Models
open Skypay.Views
open Giraffe.ViewEngine

let renderStatic () =
    let allAircraft = aircraftDatabase
    let topStats = topSellingAircraft
    let view = dashboardView allAircraft topStats "" ""
    
    // Render the view to an HTML string
    let htmlString = RenderView.AsString.htmlDocument view
    
    // Create dist directory if not exists
    if not (Directory.Exists("dist")) then
        Directory.CreateDirectory("dist") |> ignore
        
    File.WriteAllText("dist/index.html", htmlString)
    
    // Copy wwwroot/images
    if not (Directory.Exists("dist/images")) then
        Directory.CreateDirectory("dist/images") |> ignore
    
    for file in Directory.GetFiles("wwwroot/images") do
        let fileName = Path.GetFileName(file)
        File.Copy(file, Path.Combine("dist/images", fileName), true)
        
    printfn "Static site generated in dist/"

[<EntryPoint>]
let main argv =
    renderStatic()
    0
