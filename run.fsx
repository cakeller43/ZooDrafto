#load "DomainTypes.fs"
open DomainTypes

#load "DomainFunctions.fs"
open DomainFunctions

#load "UseCases.fs"
open ZooDrafto.UseCases

let gs = startGame |> nextRound
let p = gs.Players.GetPlayer 5