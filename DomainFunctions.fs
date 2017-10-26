module DomainFunctions

open DomainTypes

let private createDeck gameState = 
    { gameState with Deck = Deck.init.Shuffle }
let private createPlayers (gameState:GameState) =
    { gameState with Players = { Players = Players.init } }

let private createPacks gameState =
    let players = gameState.Players.Players
    Array.fold (fun state elem -> 
        let pack,deck = state.Deck.CreatePack 7
        { state with Deck = deck; Players = state.Players.UpdatePlayer { elem with Pack = pack } }) gameState players

let initGame = createDeck >> createPlayers >> createPacks
