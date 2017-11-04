module DomainFunctions

open CoreDraftFunctions

// TODO: Compose valdiation oneach of the usecases.
let private applyGameConfig gameConfig gameState = 
    { gameState with GameConfig = gameConfig }
let private initGameStateWithConfig gameConfig =
    GameState.empty 
    |> applyGameConfig gameConfig
let private initBaseDeck =
    [for t in CardType.types do
        yield [for _x in 1..20 do yield { CardId = 0; CardType = t }]]
    |> Seq.collect id
    |> Seq.mapi (fun index elem -> { elem with CardId = index })
    |> Array.ofSeq
let private createDeck gameState = 
    let deck = initBaseDeck |> shuffle
    { gameState with DraftState = { gameState.DraftState with Deck = deck } }
let private createPlayers gameState =
    { gameState with DraftState = { gameState.DraftState with Players = initPlayers gameState.GameConfig.NumberOfPlayers } }
let private initScores gameState = 
    let roundScores = Array.fold (fun state elem -> Map.add elem.PlayerId 0 state) gameState.RoundScores gameState.DraftState.Players
    let gameScores = Array.fold (fun state elem -> Map.add elem.PlayerId 0 state) gameState.GameScores gameState.DraftState.Players
    { gameState with RoundScores = roundScores; GameScores = gameScores }
let startGame gameConfig = 
    logger {
        let! a = initGameStateWithConfig gameConfig
        let! b = createDeck a
        let! c = createPlayers b
        let! d = initScores c
        return d
    }        

let private createPacks gameState =
    let players = gameState.DraftState.Players
    let draftState = Array.fold (fun state elem -> 
        let pack,deck = createPack state.Deck gameState.GameConfig.NumberOfCardsInPacks
        { state with Deck = deck; Players = updatePlayer {elem with Pack = pack} state.Players }) gameState.DraftState players
    { gameState with DraftState = draftState }
let private resetRoundScores gameState =
    let scores = Map.map (fun _key _value -> 0 ) gameState.RoundScores
    { gameState with RoundScores = scores }
let private advanceRoundNumber gameState = { gameState with CurrentRound = gameState.CurrentRound + 1 }
let private switchPassDirection gameState = { gameState with DraftState = switchPassDirection gameState.DraftState }
let nextRound gameState = 
    // TODO: validate gamestate and return ending game state if all rounds have been played.
    logger {
        let! a = createPacks gameState
        let! b = resetRoundScores a
        let! c = advanceRoundNumber b
        let! d = switchPassDirection c
        return d
    }

let private findCard cardId card = 
    card.CardId = cardId
let chooseCard playerId cardId gameState =
    let ds = gameState.DraftState
    let predicate = findCard cardId
    logger {
        let p = getPlayer playerId ds.Players  
        let player = chooseCard p predicate
        let! a = { gameState with DraftState = { ds with Players = updatePlayer player ds.Players  }}
        return a
    }

let private pickChosenCards gameState =
    let ds = { gameState.DraftState with Players = pickChosenCards gameState.DraftState.Players }
    { gameState with DraftState = ds }
let private score key value =
    1 //TODO: implement scoring and card values.
let private scorePickedCards gameState =
    let scores = Map.map score gameState.RoundScores
    { gameState with RoundScores = scores }
let private passPacks gameState =
    let ds = gameState.DraftState
    passPacks ds.PassDirection ds.Players
let endTurn gameState = 
    logger {
        let! a = pickChosenCards gameState
        let! b = scorePickedCards a
        let! c = passPacks b
        return c
    }