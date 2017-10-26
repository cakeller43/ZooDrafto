module DomainFunctions

let private createDeck gameState = 
    { gameState with Deck = Deck.init.Shuffle }
let private createPlayers (gameState:GameState) =
    { gameState with Players = { Players = Players.init } }
let internal initGame = createDeck >> createPlayers

let private createPacks gameState =
    let players = gameState.Players.Players
    Array.fold (fun state elem -> 
        let pack,deck = state.Deck.CreatePack 7 //TODO: configure number of drawn cards.
        { state with Deck = deck; Players = state.Players.UpdatePlayer { elem with Pack = pack } }) gameState players
let private resetRoundScores (gameState:GameState) =
    { gameState with Players = gameState.Players.ResetScores }
let private advanceRoundNumber (gameState:GameState) = gameState.IncrementCurrentRound
let internal startNextRound = createPacks >> resetRoundScores >> advanceRoundNumber

let internal chooseCard gameState playerId cardId =
    let p = gameState.Players.GetPlayer playerId 
    let player = p.ChooseCard cardId
    { gameState with Players = gameState.Players.UpdatePlayer player }