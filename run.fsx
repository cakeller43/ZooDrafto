#load "CommonTypes.fs"
open CommonTypes

#load "DomainTypes.fs"
open DomainTypes

#load "DomainFunctions.fs"
open DomainFunctions

#load "UseCases.fs"
open ZooDrafto

let api = GameApi()
let gs = api.StartGame |> api.NextRound
//let p = gs.Players.GetPlayer 5

//gs.Players.PassPacks Left