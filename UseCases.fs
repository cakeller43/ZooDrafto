namespace ZooDrafto

module UseCases =

    open DomainTypes
    open DomainFunctions

    let startGame = 
        GameState.empty
        |> initGame

    //let nextRound gameState =
        
        //create packs
        //assign packs

    // PickCard
    // EndTurn