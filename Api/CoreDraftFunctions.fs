module CoreDraftFunctions

let shuffle deck =
    let rand = System.Random()
    let swap (arr:_[]) first second =
        let tmp = arr.[first]
        arr.[first] <- arr.[second]
        arr.[second] <- tmp
    Array.iteri (fun i _ -> swap deck i (rand.Next(i, Array.length deck))) deck
    deck

/// returns (Pack,Deck)
let createPack deck numToDraw= 
    let takenCards,remainingCards = Array.splitAt numToDraw deck
    (takenCards,  remainingCards)

let chooseCard player predicate =
    let card = Array.tryFind predicate player.Pack
    let pack = Array.filter (predicate >> not) player.Pack
    { player with ChosenCard = card; Pack = pack; }

let pickChosenCard player =
    match player.ChosenCard with
    | Some card -> { player with ChosenCard = None; Picked = Array.append player.Picked [|card|] } 
    | None -> player

let initPlayers numPlayers =
    [| for x in 1..numPlayers do
        yield { PlayerId = x; Pack = Array.empty; Picked = Array.empty; ChosenCard = None; }|]

let getPlayer playerId players  =
    Array.find (fun x -> x.PlayerId = playerId) players

let updatePlayer player players  =
    Array.map (fun x -> if x.PlayerId = player.PlayerId then player else x) players

let pickChosenCards players =
    Array.map pickChosenCard players

let passPacks direction players=
    match direction with
    | Left ->
        let lastPack = (Array.last players).Pack
        let res,_ = Array.mapFold (fun state elem -> 
            let tmp = elem.Pack
            ({elem with Pack = state},tmp)) lastPack players
        res
    | Right -> 
        let lastPack = (Array.head players).Pack
        let res,_ = Array.mapFoldBack (fun elem state -> 
            let tmp = elem.Pack
            ({elem with Pack = state},tmp)) players lastPack
        res

let switchPassDirection draftState =
    let dir =
        match draftState.PassDirection with
        | Left -> Right
        | Right -> Left
    { draftState with PassDirection = dir }