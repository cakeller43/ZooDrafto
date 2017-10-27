module DomainFunctions

let private createDeck gameState = 
    { gameState with Deck = Deck.init.Shuffle }
let private createPlayers (gameState:GameState) =
    { gameState with Players = { Players = Players.init } }
let internal startGame gameState = 
    logger {
        let! x = createDeck gameState
        let! y = createPlayers x
        return y
    }        

let private createPacks gameState =
    let players = gameState.Players.Players
    Array.fold (fun state elem -> 
        let pack,deck = state.Deck.CreatePack 7 //TODO: configure number of drawn cards.
        { state with Deck = deck; Players = state.Players.UpdatePlayer { elem with Pack = pack } }) gameState players
let private resetRoundScores (gameState:GameState) =
    { gameState with Players = gameState.Players.ResetScores }
let private advanceRoundNumber (gameState:GameState) = gameState.IncrementCurrentRound
let internal nextRound gameState = 
    logger {
        let! a = createPacks gameState
        let! b = resetRoundScores a
        let! c = advanceRoundNumber b
        return c
    }

let internal chooseCard playerId cardId gameState =
    logger {
        let p = gameState.Players.GetPlayer playerId 
        let player = p.ChooseCard cardId
        let! a = { gameState with Players = gameState.Players.UpdatePlayer player }
        return a
    }

let private pickChosenCards (gameState:GameState) =
    { gameState with Players = gameState.Players.PickChosenCards }
let private score player =
    { player with RoundScore = 1 } //TODO: implement scoring and card values.
let private scorePickedCards gameState =
    let players = Array.map score gameState.Players.Players
    { gameState with Players = { Players = players } }
let private passPacks gameState =
    gameState //TODO: pass packs, determine what direction.
let internal endTurn gameState = 
    logger {
        let! a = pickChosenCards gameState
        let! b = scorePickedCards a
        let! c = passPacks b
        return c
    }