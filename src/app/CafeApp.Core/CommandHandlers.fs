module CommandHandlers

open States
open Events
open System
open Domain
open Commands
open Chessie.ErrorHandling
open Errors

let handleOpenTab tab = function
  | ClosedTab _ -> [TabOpened tab] |> ok
  | _ -> TabAlreadyOpened |> fail

let handlePlaceOrder order = function
  | OpenedTab _ -> 
    if List.isEmpty order.Foods && List.isEmpty order.Drinks then
      fail CanNotPlaceEmptyOrder
    else
      [OrderPlaced order] |> ok
  | ClosedTab _ -> fail CanNotOrderWithClosedTab
  | _ -> failwith "ToDo"

let execute state command = 
  match command with
  | OpenTab tab -> handleOpenTab tab state
  | PlaceOrder order -> handlePlaceOrder order state
  | _ -> failwith "ToDo"
let evolve state command = 
  match execute state command with
  | Ok (events, _) ->
    let newState = List.fold States.apply state events
    (newState, events) |> ok
  | Bad err -> Bad err