namespace Sudoku.FSharp.Tests

open System;
open System.IO;
open NUnit.Framework;
open Sudoku.FSharp;

[<TestFixture>]
type ProjectEulerTests() =

    let readPortion (start: int) (lines: string[]) =
        let name = lines.[start]
        let grid = seq { 1 .. Common.GridSide } |> Seq.map (fun index -> lines.[start + index]) |> Seq.toArray |> GridConversion.convertStringsToGrid
        name, grid

    let readInput (filename: string) =
        let lines = filename |> File.ReadAllLines
        seq { 0 .. 1 + Common.GridSide .. (lines |> Array.length) - 1} |> Seq.map (fun index -> lines |> readPortion index) |> Seq.toList

    [<Test>]
    member public this.Problem096() =
        let inputFilename = "TestCases\\projecteuler\\problem_096.in"
        let expectedResult = 24702
        let solver = new Solver()
        let mapFun (name: string, grid: Grid) =
            Console.WriteLine("Processing of the \"{0}\" case", name)
            grid |> solver.Solve
        let result = inputFilename |> readInput |> List.map mapFun |> List.fold (fun result grid -> result + 100 * grid.[1, 1] + 10 * grid.[1, 2] + grid.[1, 3]) 0
        Console.WriteLine("Result = {0}", result);
        Assert.That(result, Is.EqualTo(expectedResult))