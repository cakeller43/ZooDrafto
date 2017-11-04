namespace Tests

open Xunit
open CoreDraftFunctions

module CoreDraftTests =


    [<Fact>]
    let ``shuffle should not return list in same order``() =
        let res = [|1..100|] |> shuffle
        Assert.NotEqual<int array>([|1..100|],res)

    [<Fact>]
    let ``createPack should return same number of elements``() =
        let arr = [|1..100|]
        let resPack,resDeck = createPack arr 7
        Assert.Equal<int>(arr.Length,resPack.Length + resDeck.Length)

    [<Fact>]
    let ``createPack should return the first x elements``() =
        let arr = [|1..100|]
        let resPack,_resDeck = createPack arr 3
        Assert.Equal<int array>(arr.[0..2],resPack)

    [<Fact>]
    let ``createPack should return the last y elements``() =
        let arr = [|1..9|]
        let _resPack,resDeck = createPack arr 2
        Assert.Equal<int array>(arr.[2..arr.Length-1],resDeck)
    
    [<Fact>]
    let ``chooseCard should move elem from pack to chosen card``() =
        let arr = [|1..9|]
        let cardId = 4
        let player = { PlayerId = 1; Pack = arr; Picked = Array.empty; ChosenCard = None }
        let resPlayer = chooseCard player (fun x -> x = cardId)
        Assert.Equal(cardId,resPlayer.ChosenCard.Value)
        Assert.False(Array.contains cardId resPlayer.Pack)

    [<Fact>]
    let ``chooseCard should not move elem from pack to chosen card if not found``() =
        let arr = [|1..9|]
        let cardId = 10
        let player = { PlayerId = 1; Pack = arr; Picked = Array.empty; ChosenCard = None }
        let resPlayer = chooseCard player (fun x -> x = cardId)
        Assert.True(resPlayer.ChosenCard.IsNone)
        Assert.Equal<int array>(arr,resPlayer.Pack)
    
    [<Fact>]
    let ``pickChosenCard should move chosen card to picked``() =
        let arr = [|1..9|]
        let pickedId = Some 10
        let player = { PlayerId = 1; Pack = arr; Picked = Array.empty; ChosenCard = pickedId }
        let resPlayer = pickChosenCard player
        Assert.Equal(Array.head resPlayer.Picked, pickedId.Value)
        Assert.Equal(resPlayer.ChosenCard, None)

    [<Fact>]
    let ``pickChosenCard should do nothing when chosen card is None``() =
        let arr = [|1..9|]
        let player = { PlayerId = 1; Pack = arr; Picked = Array.empty; ChosenCard = None }
        let resPlayer = pickChosenCard player
        Assert.Equal(resPlayer, player)
    
    [<Fact>]
    let ``initPlayers should create array with x players``() =
        let x = 5
        let res = initPlayers 5
        Assert.Equal(x,res.Length)
    
    [<Fact>]
    let ``initPlayers should create array with empty players``() =
        let x = 5
        let resArr = initPlayers x
        let res = Array.filter (fun x -> x.Pack <> Array.empty || x.ChosenCard <> None) resArr
        Assert.Equal<Player<int> array>(Array.empty,res)

    [<Fact>]
    let ``getPlayer should return player with id equal to x``() =
        let x = 5
        let arr = initPlayers x
        let res = getPlayer x arr
        Assert.Equal(x,res.PlayerId)

    [<Fact>]
    let ``updatePlayer should return same player with updated fields``() =
        let x = 5
        let arr = initPlayers x
        let player = getPlayer x arr
        let updatedPlayer = { player with ChosenCard = Some x }
        let resArr = updatePlayer updatedPlayer arr 
        let resPlayer = getPlayer x resArr
        Assert.Equal(updatedPlayer, resPlayer)
    
    [<Fact>]
    let ``passPacks should rotate packs left``() =
        let x = 5
        let arr = 
            initPlayers x
            |> Array.mapi (fun i x -> { x with Pack = [|0..i|]})
        let resArr = passPacks Left arr
        Array.mapi (fun i e -> 
            let l = if i+1 = x then resArr.[0].Pack.Length else resArr.[i+1].Pack.Length        
            Assert.Equal(e.Pack.Length,l)) arr

    [<Fact>]
    let ``passPacks should rotate packs right``() =
        let x = 5
        let arr = 
            initPlayers x
            |> Array.mapi (fun i x -> { x with Pack = [|0..i|]})
        let resArr = passPacks Right arr
        Array.mapi (fun i e -> 
            let l = if i = 0 then resArr.[x-1].Pack.Length else resArr.[i-1].Pack.Length        
            Assert.Equal(e.Pack.Length,l)) arr
    
    [<Fact>]
    let ``switchPassDirection should return left when given right``() =
        let res = switchPassDirection { Deck = Array.empty; Players = Array.empty; PassDirection = Right}
        Assert.Equal(Left,res.PassDirection)
    
    [<Fact>]
    let ``switchPassDirection should return right when given left``() =
        let res = switchPassDirection { Deck = Array.empty; Players = Array.empty; PassDirection = Left}
        Assert.Equal(Right,res.PassDirection)