module DomainFunctions

open DomainTypes

let internal createDeck gameState = 
    { gameState with Deck = Deck.init.Shuffle }