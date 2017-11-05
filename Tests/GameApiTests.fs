namespace Tests

open Xunit
open ZooDrafto

module GameApiTests =

    let gameConfig = { NumberOfPlayers = 5; NumberOfCardsInPacks = 7; NumberOfRounds = 3 }
    let sut = GameApi()
    let gameState = sut.StartGame gameConfig

    [<Fact>]
    let ``startGame should init gameState with game config``() =
        let res = sut.StartGame gameConfig
        Assert.Equal(gameConfig, res.GameConfig)

    [<Fact>]
    let ``startGame should init players``() =
        let res = sut.StartGame gameConfig
        Assert.Equal(gameConfig.NumberOfPlayers, res.DraftState.Players.Length)

    [<Fact>]
    let ``startGame should init scores``() =
        let res = sut.StartGame gameConfig
        Assert.Equal(gameConfig.NumberOfPlayers, res.GameScores.Count)
        Map.map (fun _key value -> Assert.Equal(0,value)) res.GameScores |> ignore
        Map.map (fun _key value -> Assert.Equal(0,value)) res.RoundScores |> ignore
    
    [<Fact>]
    let ``startGame should init deck``() =
        let res = sut.StartGame gameConfig
        Assert.NotEqual<Card array>(Array.empty, res.DraftState.Deck)
    
    [<Fact>]
    let ``startGame should init gameStatus as playing``() =
        let res = sut.StartGame gameConfig
        Assert.Equal(Playing, res.GameStatus)

    [<Fact>]
    let ``startGame should init currentRound as 0``() =
        let res = sut.StartGame gameConfig
        Assert.Equal(0, res.CurrentRound)
    
    [<Fact>]
    let ``nextRound should create packs``() =
        let res = gameState |> sut.NextRound
        Array.map (fun x -> Assert.Equal(gameConfig.NumberOfCardsInPacks, x.Pack.Length)) res.DraftState.Players |> ignore
        Assert.Equal(gameState.DraftState.Deck.Length - (gameConfig.NumberOfCardsInPacks * gameConfig.NumberOfPlayers),res.DraftState.Deck.Length)
    
    [<Fact>]
    let ``nextRound should reset round scores``() =
        let res = 
            { gameState with RoundScores = Map.map (fun _key _value -> 2 ) gameState.RoundScores } 
            |> sut.NextRound
        Map.map (fun _key value -> Assert.Equal(0,value)) res.RoundScores |> ignore
    
    [<Fact>]
    let ``nextRound should incrementRound by 1``() =
        let res = gameState |> sut.NextRound
        Assert.Equal(1, res.CurrentRound)

    [<Fact>]
    let ``nextRound should switch pass direction right to left``() =
        let res = 
            { gameState with DraftState = { gameState.DraftState with PassDirection = Right } } 
            |> sut.NextRound
        Assert.Equal(Left, res.DraftState.PassDirection)

    [<Fact>]
    let ``nextRound should switch pass direction left to right``() =
        let res = 
            { gameState with DraftState = { gameState.DraftState with PassDirection = Left } } 
            |> sut.NextRound
        Assert.Equal(Right, res.DraftState.PassDirection)

    [<Fact>]
    let ``chooseCard should move card from pack to chosenCard``() =
        let gs = gameState |> sut.NextRound
        let p = gs.DraftState.Players.[0]
        let c = p.Pack.[0]
        let res = sut.ChooseCard p.PlayerId c.CardId gs
        Assert.Equal(c, res.DraftState.Players.[0].ChosenCard.Value)
        Assert.Equal(None,Array.tryFind (fun x -> x.CardId = c.CardId) res.DraftState.Players.[0].Pack)

    [<Fact>]
    let ``endTurn should move cards from chosenCard to picked card``() =
        let gameState = gameState |> sut.NextRound
        let _,gs = 
            Array.mapFold 
                (fun state x -> 
                    let s = sut.ChooseCard x.PlayerId x.Pack.[0].CardId state 
                    (x,s)) gameState gameState.DraftState.Players
        let res = sut.EndTurn gs
        Array.map (fun x -> 
            Assert.Equal(None, x.ChosenCard) |> ignore
            Assert.Equal(1 , x.Picked.Length) |> ignore) res.DraftState.Players |> ignore

    [<Fact>]
    let ``endTurn should update round scores``() =
        let gameState = gameState |> sut.NextRound
        let _,gs = 
            Array.mapFold 
                (fun state x -> 
                    let s = sut.ChooseCard x.PlayerId x.Pack.[0].CardId state 
                    (x,s)) gameState gameState.DraftState.Players
        let res = sut.EndTurn gs
        Map.map (fun key value -> Assert.NotEqual(0,value)) res.RoundScores
    
    [<Fact>]
    let ``endTurn should pass packs``() =
        let gameState = gameState |> sut.NextRound
        let res = sut.EndTurn gameState
        Array.mapi (fun i x -> 
            Assert.NotEqual<Card array>(gameState.DraftState.Players.[i].Pack ,x.Pack) |> ignore )
            res.DraftState.Players

    [<Fact>]
    let ``haveAllPlayersChosen should return true if all players have some chosenCard``() =
        let gameState = gameState |> sut.NextRound
        let _,gs = 
            Array.mapFold 
                (fun state x -> 
                    let s = sut.ChooseCard x.PlayerId x.Pack.[0].CardId state 
                    (x,s)) gameState gameState.DraftState.Players
        let res = sut.HaveAllPlayersChosen gs
        Assert.True(res)

    [<Fact>]
    let ``haveAllPlayersChosen should return false if not all players have some chosenCard``() =
        let gameState = gameState |> sut.NextRound
        let res = sut.HaveAllPlayersChosen gameState
        Assert.False(res)