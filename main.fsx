
//#r "FSharp.Json"
open System.Diagnostics
open FSharp.Data

type RecordType = {
    area: string
    forecast: array[]
}
type RecordType2 = {
    time: string
    temperature: float
    humidity: float
    wind: float
    pressure: float
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
        let process = Process.Start(psi)
        let response = process.StandardOutput.ReadToEnd()
        printfn "Response %d: %s" (x+1) response
        x <- x + 1
        thisString = response
        if not (lastString=thisString) then 
            printfn "CHANGED!"
        printfn "%s" lastString
        printfn "%s" thisString
        lastString = response
        
    
        newLineSplit = response.Split "\n"
        dataSplit = newLineSplit.Split ","
        time = dataSplit[0]
        temperature = dataSplit[2]
        humidity = dataSplit[3]
        wind = dataSplit[4]
        pressure = dataSplit[5]
        //let data: RecordType2 = {time = time, temperature = temperature, humidity = humidity, wind = wind , pressure = pressure}

        let deserialized = Json.deserialize<RecordType> json

        let data2: RecordType = { area = "AREA"; forecast = [|1;2;3|] }
        let json = Json.serialize data2
        printfn "%s" json
        // json is """{ "stringMember": "The string", "intMember": 123 }"""

        // deserialize from JSON to record
        let deserialized = Json.deserialize<RecordType> json
        printfn "%A" deserialized

        stopWatch.Stop()
        printfn "%f" stopWatch.Elapsed.TotalMilliseconds

    printfn "API call completed %d times" x


main()
