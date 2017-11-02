[<AutoOpen>]
module DomainTypes

type CardType = | EggNigiri | SalmonNigiri | SquidNigiri
    with
        static member types = [|EggNigiri; SalmonNigiri; SquidNigiri|]

type Card = { CardId: int; CardType: CardType }

type GameConfig = { NumberOfPlayers: int; NumberOfCardsInPacks: int; NumberOfRounds: int }

type GameStatus = | Playing | RoundEnding | GameOver

type GameState = internal { DraftState: DraftState<Card>; GameConfig: GameConfig; CurrentRound: int; GameStatus: GameStatus; RoundScores: Map<int,int>; GameScores: Map<int,int> }
    with
        static member empty =
            { 
                DraftState = { Deck = Array.empty<Card>; Players = Array.empty<Player<Card>>; PassDirection = Right }
                GameConfig = { NumberOfPlayers = 0; NumberOfCardsInPacks = 0; NumberOfRounds = 0 };
                CurrentRound = 0;
                GameStatus = Playing;
                RoundScores = Map.empty;
                GameScores = Map.empty;
            }