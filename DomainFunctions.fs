module DomainFunctions
// TODO: Compose valdiation oneach of the usecases.

let private initGameStateWithConfig gameConfig =
    (GameState.empty).ApplyGameConfig gameConfig
let private initialDeck =
    [for t in CardType.types do
        yield [for _x in 1..20 do yield { Id = 0; CardType = t }]]
    |> Seq.collect id
    |> Seq.mapi (fun index elem -> { elem with Id = index })
    |> Array.ofSeq
let private createDeck gameState = 
    { gameState with DraftState = { gameState.DraftState with Deck = (Deck<Card>.init initialDeck).Shuffle }}
let private createPlayers (gameState:GameState) =
    { gameState with DraftState = { gameState.DraftState with Players = { Players = Players<Card>.init gameState.GameConfig.NumberOfPlayers }}}
let private initScores (gameState:GameState) = 
    gameState.InitScores
let internal startGame gameConfig = 
    logger {
        let! a = initGameStateWithConfig gameConfig
        let! b = createDeck a
        let! c = createPlayers b
        let! d = initScores c
        return d
    }        

let private createPacks gameState =
    let players = gameState.DraftState.Players.Players
    let draftState = Array.fold (fun state elem -> 
        let pack,deck = state.Deck.CreatePack gameState.GameConfig.NumberOfCardsInPacks
        { state with Deck = deck; Players = state.Players.UpdatePlayer { elem with Pack = pack } }) gameState.DraftState players
    { gameState with DraftState = draftState }
let private resetRoundScores (gameState:GameState) =
    let scores = Map.map (fun key value -> 0 ) gameState.RoundScores
    { gameState with RoundScores = scores }
let private advanceRoundNumber (gameState:GameState) = gameState.IncrementCurrentRound
let private switchPassDirection (gameState:GameState) = { gameState with DraftState = gameState.DraftState.SwitchPassDirection }
let internal nextRound gameState = 
    // TODO: validate gamestate and return ending game state if all rounds have been played.
    logger {
        let! a = createPacks gameState
        let! b = resetRoundScores a
        let! c = advanceRoundNumber b
        let! d = switchPassDirection c
        return d
    }

let private findCard card cardId = 
    card.Id = cardId
let internal chooseCard playerId cardId gameState =
    logger {
        let p = gameState.DraftState.Players.GetPlayer playerId 
        let player = p.ChooseCard cardId findCard
        let! a = { gameState with DraftState = { gameState.DraftState with Players = gameState.DraftState.Players.UpdatePlayer player }}
        return a
    }

let private pickChosenCards (gameState:GameState) =
    let ds = { gameState.DraftState with Players = gameState.DraftState.Players.PickChosenCards }
    { gameState with DraftState = ds }
let private score key value =
    1 //TODO: implement scoring and card values.
let private scorePickedCards gameState =
    let scores = Map.map score gameState.RoundScores
    { gameState with RoundScores = scores }
let private passPacks gameState =
    gameState.DraftState.Players.PassPacks gameState.DraftState.PassDirection
let internal endTurn gameState = 
    logger {
        let! a = pickChosenCards gameState
        let! b = scorePickedCards a
        let! c = passPacks b
        return c
    }