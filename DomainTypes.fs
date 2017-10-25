module DomainTypes

type CardType = | EggNigiri | SalmonNigiri | SquidNigiri
    with
        static member types = [|EggNigiri; SalmonNigiri; SquidNigiri|]

type Card = { Id: int; CardType: CardType }

type Deck = { Cards: Card array }
    with
        static member init =
            let cards = 
                [for t in CardType.types do
                    yield [for _x in 1..5 do yield { Id = 0; CardType = t }]]
                |> Seq.collect id
                |> Seq.mapi (fun index elem -> { elem with Id = index })
                |> Array.ofSeq
            { Cards = cards }
        member this.Shuffle =
            let cards = this.Cards
            let rand = System.Random()
            let swap (arr:_[]) first second =
                let tmp = arr.[first]
                arr.[first] <- arr.[second]
                arr.[second] <- tmp
            Array.iteri (fun i _ -> swap cards i (rand.Next(i, Array.length cards))) cards
            { this with Cards = cards }

type GameState = { Deck: Deck }
    with
        static member empty =
            { Deck = { Cards = Array.empty }}