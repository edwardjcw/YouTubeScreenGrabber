// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Helper
open System.IO

[<EntryPoint>]
let main argv = 

    let extractLinks text =
        match text |> File.Exists with
        | true -> text |> File.ReadAllLines
        | false -> Array.empty

    match argv.[0].ToLower() with
    | "-h" -> //show help
        let help =
            "YouTubeScreenGrabber:\n\
            USAGE: YouTubeScreenGrabber -x [params], where x is the use code\n\
            NOTE: All links MUST include a time marker or the program skips it.\n\
            The program saves all images into the subdirectory Images. If the\n\
            program converts more than one link, it saves a csv file in\n\
            the root directory that maps the link to the saved image name.\n\
            Convert one link to image: \n\
            \t YouTubeScreenGrabber -o [link] [directory to save image]\n\
            Convert multiple links to image: \n\
            \t YouTubeScreenGranner -m [file path of new line delimited links] [save directory]"
        help |> printfn "%A" |> ignore
    | "-o" ->
        let link = argv.[1]
        let savePath = argv.[2]
        link
        |> Helper.BuildLink
        |> Helper.BuildEmbedLink
        |> Helper.BuildImage
        |> Helper.SaveImage savePath
        |> ignore
        printfn "save complete" 
        |> ignore
    | "-m" ->
        let links = extractLinks argv.[1]
        let savePath = argv.[2]
        links
        |> Array.Parallel.map (Helper.BuildLink >> Helper.BuildEmbedLink >> Helper.BuildImage >>
            (fun i -> i |> Helper.SaveImage savePath |> ignore))
        |> ignore
        printfn "save multiple images done"
        |> ignore 
    | _ -> printfn "unknown args" |> ignore

    0 // return an integer exit code
