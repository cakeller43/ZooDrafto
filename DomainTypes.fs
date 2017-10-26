[<AutoOpen>]
module DomainTypes

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
            | Some card -> { this with ChosenCard = None; Picked = { this.Picked with Cards = Array.append this.Picked.Cards [|card|]  }}
            | None -> this

type Players = { Players: Player array }
    with
        static member internal init =
            [| for x in 1..5 do
                yield { Id = x; Pack = {Cards=Array.empty}; Picked = {Cards=Array.empty}; ChosenCard = None; GameScore = 0; RoundScore = 0 }|]
        member internal this.GetPlayer playerId =
            Array.find (fun x -> x.Id = playerId) this.Players
        member internal this.UpdatePlayer player =
            { this with Players = Array.map (fun x -> if x.Id = player.Id then player else x) this.Players }
        member internal this.ResetScores =
            let players = Array.map (fun x -> { x with RoundScore = 0 }) this.Players
            { this with Players = players }

type GameState = { Deck: Deck; Players: Players; CurrentRound:int; }
    with
        static member internal empty =
            { Deck = { Cards = Array.empty }; Players = { Players = Array.empty }; CurrentRound = 0 }
        member internal this.IncrementCurrentRound =
            { this with CurrentRound = this.CurrentRound + 1 }