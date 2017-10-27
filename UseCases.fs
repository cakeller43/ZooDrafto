namespace ZooDrafto

open DomainFunctions

type GameApi() =
    member this.StartGame = 
        GameState.empty
        |> startGame

    member this.NextRound gameState =
        gameState
        |> nextRound

    member this.ChooseCard playerId cardId gameState =
        gameState 
        |> chooseCard playerId cardId
    
    member this.EndTurn gameState =
        gameState 
        |> endTurn