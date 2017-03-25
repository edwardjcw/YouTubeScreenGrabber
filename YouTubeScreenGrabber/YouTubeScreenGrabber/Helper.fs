namespace Helper 

open System.Drawing
open System.Web
open FSharp.Data
open System.Xml
open System.Xml.Linq
open System.Windows.Forms

type Link = 
    {original:string; embeded:string; image:Bitmap option}

type Helper() =

    static let extractNameAndTime (uri : string) =
        let slash = '\\'
        let times = [|"?t=";"?start="|]
        uri.Split([|slash|])
        |> Array.last
        |> (fun x -> x.Split(times, System.StringSplitOptions.None))
        |> (fun x -> @"https://www.youtube.com/embed/" + x.[0] + "?start=" + (x |> Array.last))

    static let transformToImage (uri : string) =
        let size = new Size(1920,1080)
        let point = new Point(0,0)

        let saveBuild (form : Form) (browser : WebBrowser) = async {
            do! Async.Sleep(2000)
            use tmpImg = new Bitmap(size.Width, size.Height)
            use g = Graphics.FromImage(tmpImg)
        
            g.CopyFromScreen(browser.PointToScreen(point), point, size)
            form.Close()
            return tmpImg
        }

        use browser = new WebBrowser()
        use form = new Form()
        form.Size<-size
        browser.Size<-size
        browser.Navigate(@"https://www.youtube.com/embed/TPLo-lnpUjY?start=303&autoplay=1")
        browser.Location<-point
        form.Controls.Add(browser)
        form.Show()

        let result = Async.RunSynchronously(saveBuild form browser)
        Some(result)
        
    static member BuildLink uri = 
        {original=uri; embeded=""; image=None}

    static member BuildEmbedLink (link : Link) = 
        {link with embeded = extractNameAndTime link.original}

    static member BuildImage (link : Link) =
        {link with image = transformToImage link.embeded}

    static member SaveImage savePath (link : Link) =
        match link.image with
        | Some(i) -> i.Save(savePath)
        | _ -> failwith "no image"