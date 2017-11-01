[<AutoOpen>]
module CommonTypes

// TODO: make gamestate monad once we have a reason to.
type LoggerBuilder() =
    let log p = printfn "%A" p

    member this.Bind(x, f) = 
        log x
        f x
    member this.Return(x) =
        x
let logger = LoggerBuilder()