namespace abc

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.MathJS

[<JavaScript>]
module Calculator =
    let newVar = Var.Create ""
    
module Client =

    open System.Data
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    [<SPAEntryPoint>]
    let Main () =
        let display = Var.Create "0"
        let wholeEq = Var.Create ""
        let lastEq = Var.Create ""
        let showingResult = Var.Create true

        let formatNum (number: float) =
            let scifcNotation = sprintf "%e" number

            let parts = scifcNotation.Split 'e'
            let coefficient = float parts.[0]
            let exponent = int parts.[1]

            let formattedCoefficient = sprintf "%.3f" coefficient 

            sprintf "%sE%d" formattedCoefficient exponent

        let eval (str:string) =
            let rslt = Math.Evaluate(str)
            let rsltFloat = float (rslt.ToString())
            let fr = Math.Round(rsltFloat, 8).ToString()
            match String.length fr with
            | len when len <= 8 ->
                fr
            | len when len > 8 ->
                formatNum rsltFloat

        IndexTemplate.Main()
            .WholeEq(wholeEq.View)
            .LastEq(lastEq.View)
            .ShowingResult(showingResult.Value.ToString())
            .Display(display.View)
            .Click(fun e ->
                match e.Target.ClassName with
                | "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" -> 
                    match showingResult.Value with
                    | true -> 
                        Var.Set display (e.Target.ClassName)
                        Var.Set lastEq (lastEq.Value.ToString() + e.Target.ClassName)
                        Var.Set showingResult false
                    | _ -> 
                        match String.length (display.Value.ToString()) with
                        | len when len <= 8 ->
                            Var.Set display (display.Value.ToString() + e.Target.ClassName)
                            Var.Set lastEq (lastEq.Value.ToString() + e.Target.ClassName)
                        | _ -> 
                            Var.Set display (display.Value.ToString())

                | "=" -> 
                     match wholeEq.Value with
                     | "" ->
                         Var.Set wholeEq (wholeEq.Value)
                     | _ ->
                        match lastEq.Value with
                        | "/" | "+" | "-" | "*" ->
                            Var.Set lastEq ""
                        | _ ->
                             Var.Set wholeEq (eval ((wholeEq.Value.ToString() + lastEq.Value.ToString()).ToString()))
                             Var.Set display (wholeEq.Value)
                             Var.Set showingResult true
                             Var.Set lastEq ""

                | "ac" ->
                    Var.Set lastEq ""
                    Var.Set wholeEq ""
                    Var.Set display "0"

                | _ ->
                    match lastEq.Value with
                    | "" ->
                        match wholeEq.Value with
                        | "" -> 
                            Var.Set lastEq ""
                        | _ ->
                            Var.Set wholeEq (wholeEq.Value)
                            Var.Set lastEq (e.Target.ClassName)

                    | "/" | "+" | "-" | "*" ->
                        Var.Set lastEq (e.Target.ClassName)

                    | _ ->
                        Var.Set wholeEq ((Math.Evaluate(wholeEq.Value.ToString() + lastEq.Value.ToString())).ToString())
                        Var.Set display (wholeEq.Value)
                        Var.Set showingResult true
                        Var.Set lastEq (e.Target.ClassName)
                )
            .Doc()
        |> Doc.RunById "main"
