[<AutoOpen>]
module DomainTypes

type Direction = | Left | Right
type CardType = | EggNigiri | SalmonNigiri | SquidNigiri
    with
        static member internal types = [|EggNigiri; SalmonNigiri; SquidNigiri|]

type Card = { Id: int; CardType: CardType }

type Pack = { Cards: Card array }

type Deck = { Cards: Card array }
    with
        static member internal init =
            let cards = 
                [for t in CardType.types do
                    yield [for _x in 1..20 do yield { Id = 0; CardType = t }]]
                |> Seq.collect id
                |> Seq.mapi (fun index elem -> { elem with Id = index })
                |> Array.ofSeq
            { Cards = cards }
        member internal this.Shuffle =
            let cards = this.Cards
            let rand = System.Random()
            let swap (arr:_[]) first second =
                let tmp = arr.[first]
                arr.[first] <- arr.[second]
                arr.[second] <- tmp
            Array.iteri (fun i _ -> swap cards i (rand.Next(i, Array.length cards))) cards
            { this with Cards = cards }
        member internal this.CreatePack num : (Pack * Deck) =
            let takenCards,remainingCards = Array.splitAt num this.Cards
            ({ Cards = takenCards }, { this with Cards = remainingCards })

type Picked = { Cards: Card array }
type Player = { Id: int; Pack: Pack; Picked: Picked; ChosenCard: Card option; GameScore: int; RoundScore: int }
    with
        member internal this.ChooseCard cardId =
            let card = Array.find (fun (x:Card) -> x.Id = cardId) this.Pack.Cards
            let pack = { this.Pack with Cards = Array.filter (fun x -> x.Id <> cardId) this.Pack.Cards }
            { this with ChosenCard = Some card; Pack = pack; }
        member internal this.PickChosenCard =
            match this.ChosenCard with
            | Some card -> { this with ChosenCard = None; Picked = {this.Picked with Cards = Array.append this.Picked.Cards [|card|]} }
            | None -> this

type Players = { Players: Player array }
    with
        static member internal init numPlayers =
            [| for x in 1..numPlayers do
                yield { Id = x; Pack = {Cards=Array.empty}; Picked = {Cards=Array.empty}; ChosenCard = None; GameScore = 0; RoundScore = 0 }|]
        member internal this.GetPlayer playerId =
            Array.find (fun x -> x.Id = playerId) this.Players
        member internal this.UpdatePlayer player =
            { this with Players = Array.map (fun x -> if x.Id = player.Id then player else x) this.Players }
        member internal this.PickChosenCards =
            let players = Array.map (fun (x:Player) -> x.PickChosenCard) this.Players
            { this with Players = players }
        member internal this.PassPacks direction =
            let players,_ = 
                match direction with
                | Left ->
                    let lastPack = (Array.last this.Players).Pack
                    Array.mapFold (fun state elem -> 
                        let tmp = elem.Pack
                        ({elem with Pack = state},tmp)) lastPack this.Players
                | Right -> 
                    let lastPack = (Array.head this.Players).Pack
                    Array.mapFoldBack (fun elem state -> 
                        let tmp = elem.Pack
                        ({elem with Pack = state},tmp)) this.Players lastPack
            { this with Players = players }                

type GameConfig = { NumberOfPlayers: int; NumberOfCardsInPacks: int; NumberOfRounds: int }
type GameStatus = | Playing | RoundEnding | GameOver
type GameState = { GameConfig: GameConfig; Deck: Deck; Players: Players; CurrentRound: int; PassDirection: Direction; GameStatus: GameStatus }
    with
        static member internal empty =
            { 
                GameConfig = { NumberOfPlayers = 0; NumberOfCardsInPacks = 0; NumberOfRounds = 0 };
                Deck = { Cards = Array.empty };
                Players = { Players = Array.empty };
                CurrentRound = 0;
                PassDirection = Right;
                GameStatus = Playing
            }
        member internal this.ApplyGameConfig gameConfig = 
            { this with GameConfig = gameConfig }
        member internal this.IncrementCurrentRound =
            { this with CurrentRound = this.CurrentRound + 1 }
        member internal this.SwitchPassDirection =
            let dir =
                match this.PassDirection with
                | Left -> Right
                | Right -> Left
            { this with PassDirection = dir }