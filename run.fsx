#load "CommonTypes.fs"
open CommonTypes

#load "CoreDraftTypes.fs"
open CoreDraftTypes

#load "CoreDraftFunctions.fs"
open CoreDraftFunctions

#load "DomainTypes.fs"
open DomainTypes

#load "DomainFunctions.fs"
open DomainFunctions

#load "UseCases.fs"
open ZooDrafto

let api = GameApi()
let gs =  api.StartGame { NumberOfPlayers = 1; NumberOfCardsInPacks = 2; NumberOfRounds = 3 } |> api.NextRound
//let p = gs.Players.GetPlayer 5

//gs.Players.PassPacks Left