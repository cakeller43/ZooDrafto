module DomainTypes

type CardType = | EggNigiri | SalmonNigiri | SquidNigiri
type Card = { Id: int; CardType: CardType  }
type Deck = Card array
type GameState = { Deck: Deck }