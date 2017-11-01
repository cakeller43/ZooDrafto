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

let chooseCard player cardId predicate =
    let card = Array.find (fun x -> predicate x cardId) player.Pack
    let pack = Array.filter (fun x -> not (predicate x cardId)) player.Pack
    { player with ChosenCard = Some card; Pack = pack; }

let pickChosenCard player =
    match player.ChosenCard with
    | Some card -> { player with ChosenCard = None; Picked = Array.append player.Picked [|card|] } 
    | None -> player

let initPlayers numPlayers =
    [| for x in 1..numPlayers do
        yield { PlayerId = x; Pack = Array.empty; Picked = Array.empty; ChosenCard = None; }|]

let getPlayer players playerId =
    Array.find (fun x -> x.PlayerId = playerId) players

let updatePlayer players player =
    Array.map (fun x -> if x.PlayerId = player.PlayerId then player else x) players

let pickChosenCards players =
    Array.map pickChosenCard players

let passPacks players direction =
    match direction with
    | Left ->
        let lastPack = (Array.last players).Pack
        Array.mapFold (fun state elem -> 
            let tmp = elem.Pack
            ({elem with Pack = state},tmp)) lastPack players
    | Right -> 
        let lastPack = (Array.head players).Pack
        Array.mapFoldBack (fun elem state -> 
            let tmp = elem.Pack
            ({elem with Pack = state},tmp)) players lastPack

let switchPassDirection draftState =
    let dir =
        match draftState.PassDirection with
        | Left -> Right
        | Right -> Left
    { draftState with PassDirection = dir }