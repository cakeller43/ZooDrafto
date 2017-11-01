namespace ZooDrafto

open DomainFunctions

type GameApi() =
    ///Creates Players, Creates Deck
    member this.StartGame gameConfig = 
        startGame gameConfig

    ///Creates Packs, Resets Round Scores, Advances the Round Number, Switches the Pass Direction
    member this.NextRound gameState =
        gameState
        |> nextRound

    ///Moves a Card from the Players Pack to ChosenCard field.
    member this.ChooseCard playerId cardId gameState =
        gameState 
        |> chooseCard playerId cardId
    
    ///Moves all Players ChosenCards to Picked and updates the RoundScore
    member this.EndTurn gameState =
        gameState 
        |> endTurn
    
    member this.HaveAllPlayersChosen (gameState:GameState) =
        let x = 
            Array.filter 
                (fun x -> 
                    match x.ChosenCard with
                    | Some _ -> false
                    | None -> true) 
                gameState.DraftState.Players.Players
        Array.isEmpty x