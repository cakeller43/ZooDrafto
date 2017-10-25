namespace ZooDrafto

open DomainTypes
open DomainFunctions

module UseCases =
    let startGame = 
        GameState.empty
        |> createDeck
