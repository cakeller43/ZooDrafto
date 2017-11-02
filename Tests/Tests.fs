namespace Tests

open Xunit
open ZooDrafto

module Tests =


    [<Fact>]
    let foo() =
        let sut = GameApi()
        let result = sut.StartGame
        Assert.NotNull(result)