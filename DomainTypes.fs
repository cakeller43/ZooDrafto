[<AutoOpen>]
module DomainTypes

type CardType = | EggNigiri | SalmonNigiri | SquidNigiri
    with
        static member internal types = [|EggNigiri; SalmonNigiri; SquidNigiri|]

type Card = { Id: int; CardType: CardType }

type GameConfig = { NumberOfPlayers: int; NumberOfCardsInPacks: int; NumberOfRounds: int }
type GameStatus = | Playing | RoundEnding | GameOver
type GameState = { DraftState: DraftState<Card>; GameConfig: GameConfig; CurrentRound: int; GameStatus: GameStatus; RoundScores: Map<int,int>; GameScores: Map<int,int> }
    with
        static member internal empty =
            { 
                DraftState = { Deck = { Cards = Array.empty<Card> }; Players = { Players = Array.empty<Player<Card>> }; PassDirection = Right }
                GameConfig = { NumberOfPlayers = 0; NumberOfCardsInPacks = 0; NumberOfRounds = 0 };
                CurrentRound = 0;
                GameStatus = Playing;
                RoundScores = Map.empty;
                GameScores = Map.empty;
            }
        member internal this.ApplyGameConfig gameConfig = 
            { this with GameConfig = gameConfig }
        member internal this.InitScores =
            let roundScores = Array.fold (fun state (elem:Player<Card>) -> Map.add elem.Id 0 state) this.RoundScores this.DraftState.Players.Players
            let gameScores = Array.fold (fun state (elem:Player<Card>) -> Map.add elem.Id 0 state) this.GameScores this.DraftState.Players.Players
            { this with RoundScores = roundScores; GameScores = gameScores }
        member internal this.IncrementCurrentRound =
            { this with CurrentRound = this.CurrentRound + 1 }