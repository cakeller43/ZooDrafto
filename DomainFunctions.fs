module DomainFunctions

let private createDeck gameState = 
    { gameState with Deck = Deck.init.Shuffle }
let private createPlayers (gameState:GameState) =
    { gameState with Players = { Players = Players.init } }
let internal startGame = createDeck >> createPlayers

let private createPacks gameState =
    let players = gameState.Players.Players
    Array.fold (fun state elem -> 
        let pack,deck = state.Deck.CreatePack 7 //TODO: configure number of drawn cards.
        { state with Deck = deck; Players = state.Players.UpdatePlayer { elem with Pack = pack } }) gameState players
let private resetRoundScores (gameState:GameState) =
    { gameState with Players = gameState.Players.ResetScores }
let private advanceRoundNumber (gameState:GameState) = gameState.IncrementCurrentRound
let internal nextRound = createPacks >> resetRoundScores >> advanceRoundNumber

let internal chooseCard playerId cardId gameState =
    let p = gameState.Players.GetPlayer playerId 
    let player = p.ChooseCard cardId
    { gameState with Players = gameState.Players.UpdatePlayer player }

let private pickChosenCards (gameState:GameState) =
    { gameState with Players = gameState.Players.PickChosenCards }
let private score player =
    { player with RoundScore = 1 } //TODO: implement scoring and card values.
let private scorePickedCards gameState =
    let players = Array.map score gameState.Players.Players
    { gameState with Players = { Players = players } }
let private passPacks gameState =
    gameState //TODO: pass packs, determine what direction.
let internal endTurn = pickChosenCards >> scorePickedCards >> passPacks
    // move chosen card to picked and score picked pile and update roundscore pass packs
    