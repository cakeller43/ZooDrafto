namespace ZooDrafto

module UseCases =

    open DomainFunctions

    let startGame = 
        GameState.empty
        |> initGame

    let nextRound gameState =
        gameState
        |> startNextRound

    let pickCard gameState playerId cardId =
         chooseCard gameState playerId cardId
    // PickCard
    // EndTurn