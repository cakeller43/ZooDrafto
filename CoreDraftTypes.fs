[<AutoOpen>]
module CoreDraftTypes

type Direction = | Left | Right

type Pack<'card> = { Cards: 'card array }

type Deck<'card> = { Cards: 'card array }
    with
        static member internal init cards = 
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
        member internal this.CreatePack num : (Pack<'card> * Deck<'card>) =
            let takenCards,remainingCards = Array.splitAt num this.Cards
            ({ Cards = takenCards }, { this with Cards = remainingCards })

type Picked<'card> = { Cards: 'card array }

type Player<'card> = { Id: int; Pack: Pack<'card>; Picked: Picked<'card>; ChosenCard: 'card option; }
    with
        member internal this.ChooseCard cardId predicate =
            let card = Array.find (fun (x:'card) -> predicate x cardId) this.Pack.Cards
            let pack = { this.Pack with Cards = Array.filter (fun x -> true <> predicate x cardId) this.Pack.Cards }
            { this with ChosenCard = Some card; Pack = pack; }
        member internal this.PickChosenCard =
            match this.ChosenCard with
            | Some card -> { this with ChosenCard = None; Picked = {this.Picked with Cards = Array.append this.Picked.Cards [|card|]} }
            | None -> this

type Players<'card> = { Players: Player<'card> array }
    with
        static member internal init numPlayers =
            [| for x in 1..numPlayers do
                yield { Id = x; Pack = {Cards=Array.empty}; Picked = {Cards=Array.empty}; ChosenCard = None; }|]
        member internal this.GetPlayer playerId =
            Array.find (fun x -> x.Id = playerId) this.Players
        member internal this.UpdatePlayer player =
            { this with Players = Array.map (fun x -> if x.Id = player.Id then player else x) this.Players }
        member internal this.PickChosenCards =
            let players = Array.map (fun (x:Player<'card>) -> x.PickChosenCard) this.Players
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

type DraftState<'card> = { Deck: Deck<'card>; Players: Players<'card>; PassDirection: Direction }
    with
        member internal this.SwitchPassDirection =
            let dir =
                match this.PassDirection with
                | Left -> Right
                | Right -> Left
            { this with PassDirection = dir }