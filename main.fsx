#r "nuget: Newtonsoft.Json, 13.0.3"
open System.Diagnostics
open FSharp.Data
open Newtonsoft.Json



type ForecastData = {
    time: string
    temperature: float
    humidity: float
    wind: float
    pressure: float
}
type Forecast = {
    area: string
    forecast: ForecastData[]  // important detail here is that we want a list of ForecastData
}

let apiUrls = [| "https://incommodities.io/a?area=Eagle%20River"
                 "https://incommodities.io/a?area=Kincaid%20Park"
                 "https://incommodities.io/a?area=Far%20North%20Bicentennial%20Park"
                 "https://incommodities.io/a?area=Bear%20Valley"
                 "https://incommodities.io/a?area=Fire%20Island" |]

let authToken = "6b0fb5dad1564780a6bb83a5491e9bc5"
let mutable lastString = ""
let mutable thisString = ""

let main () =
    let mutable x = 0

    for apiUrl in apiUrls do        
        let stopWatch = System.Diagnostics.Stopwatch.StartNew()


        let psi = new ProcessStartInfo("curl", sprintf "\"%s\" -X POST -H \"Authorization: Bearer %s\"" apiUrl authToken)
        psi.RedirectStandardOutput <- true
        let proces = Process.Start(psi)
        let response = proces.StandardOutput.ReadToEnd()
        printfn "Response %d: %s" (x+1) response
        x <- x + 1
        

        // From here we disect the response from the API.
        try
            let newLineSplit = Seq.toList(response.Split "\n")
            let mutable forecasts = Array.empty
            for i in 1.. newLineSplit.Length-1 do
                let dataSplit = newLineSplit.[i].Split ","

                let time = dataSplit.[0]
                let temperature = dataSplit.[2]
                let humidity = dataSplit.[3]
                let wind = dataSplit.[4]
                let pressure = dataSplit.[5]
                let somecastData = {time = time ; temperature =  float temperature ; humidity = float humidity; wind = float wind; pressure = float pressure} 
                forecasts <- Array.append forecasts [| somecastData |]
            
            let forecast = { area = "AREA"; 
            forecast = forecasts}

            let json = JsonConvert.SerializeObject(forecast)
            System.IO.File.WriteAllText($"forecast{x}.json", json)
            
            stopWatch.Stop()
            printfn "%f" stopWatch.Elapsed.TotalMilliseconds
        with
            | ex -> printfn "An error occurred: %s\nOriginal response: %s" ex.Message response

    printfn "API call completed %d times" x



main()
