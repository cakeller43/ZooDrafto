[<AutoOpen>]
module CoreDraftTypes

type Direction = | Left | Right

type Player<'card> = { PlayerId: int; Pack: 'card array; Picked: 'card array; ChosenCard: 'card option; }

type DraftState<'card> = { Deck: 'card array ; Players: Player<'card> array; PassDirection: Direction }