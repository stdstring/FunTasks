USE master
GO

IF DB_ID (N'TicTacToe') IS NOT NULL
    DROP DATABASE TicTacToe
GO

CREATE DATABASE TicTacToe
GO

USE TicTacToe
GO

IF SCHEMA_ID(N'Impl') IS NOT NULL
    DROP SCHEMA Impl
GO

CREATE SCHEMA Impl
GO

IF OBJECT_ID(N'Impl.GameSession', N'U') IS NOT NULL
    DROP TABLE Impl.GameSession
GO

CREATE TABLE Impl.GameSession
(
GameID varchar(100) NOT NULL PRIMARY KEY,
/* 'U' = User, 'C' = Comp */
FirstPlayer char(1) NOT NULL CHECK (FirstPlayer = 'U' OR FirstPlayer = 'C'),
NextCompStepGenerator varchar(100) NOT NULL,
/* 'U' = User, 'C' = Comp, 'D' = Draw, NULL = game is not finished */
GameResult char(1) NULL CHECK (GameResult = 'U' OR GameResult = 'C' OR GameResult = 'D')
)
GO

IF OBJECT_ID(N'Impl.GameSessionLog', N'U') IS NOT NULL
    DROP TABLE Impl.GameSessionLog
GO

CREATE TABLE Impl.GameSessionLog
(
GameID varchar(100) NOT NULL,
Number int NOT NULL CHECK(Number >= 1 AND Number <= 9),
Step int NOT NULL CHECK(Step IN (11, 12, 13, 21, 22, 23, 31, 32, 33)),
Value char(1) NOT NULL CHECK (Value = 'X' OR Value = 'O'),
CONSTRAINT GameSessionLog_PK PRIMARY KEY (GameID, Number),
CONSTRAINT GameSession_FK FOREIGN KEY (GameID) REFERENCES Impl.GameSession(GameID) ON UPDATE CASCADE ON DELETE CASCADE
)
GO

/* INTERNAL */
IF OBJECT_ID (N'Impl.GetCellValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetCellValue
GO

CREATE FUNCTION Impl.GetCellValue (@GameID varchar(100), @Step int)
RETURNS char(1)
AS
BEGIN
    DECLARE @Value char(1)
    SELECT @Value = Value FROM Impl.GameSessionLog WHERE (GameId = @GameID) AND Step = @Step
    RETURN ISNULL(@Value, ' ')
END
GO

IF OBJECT_ID(N'Impl.MakeStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.MakeStep
GO

CREATE PROCEDURE Impl.MakeStep
    @GameID varchar(100),
    @Step int,
    @Player char(1)
AS
    SET NOCOUNT ON
    DECLARE @FirstPlayer char(1)
    SELECT @FirstPlayer = FirstPlayer FROM Impl.GameSession WHERE GameID = @GameID
    DECLARE @StepCount int
    SELECT @StepCount = COUNT(Number) FROM Impl.GameSessionLog WHERE GameID = @GameID
    INSERT Impl.GameSessionLog(GameID, Number, Step, Value) VALUES(@GameID, @StepCount + 1, @Step, IIF(@Player = @FirstPlayer, 'X', 'O'))
GO

IF OBJECT_ID (N'Impl.GetRowValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetRowValue
GO

CREATE FUNCTION Impl.GetRowValue (@GameID varchar(100), @Row int)
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, 10 * @Row + 1) + Impl.GetCellValue(@GameID, 10 * @Row + 2) + Impl.GetCellValue(@GameID, 10 * @Row + 3)
END
GO

IF OBJECT_ID (N'Impl.GetColumnValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetColumnValue
GO

CREATE FUNCTION Impl.GetColumnValue (@GameID varchar(100), @Column int)
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, 10 + @Column) + Impl.GetCellValue(@GameID, 20 + @Column) + Impl.GetCellValue(@GameID, 30 + @Column)
END
GO

IF OBJECT_ID (N'Impl.GetDirectDiagonalValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetDirectDiagonalValue
GO

CREATE FUNCTION Impl.GetDirectDiagonalValue (@GameID varchar(100))
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, 11) + Impl.GetCellValue(@GameID, 22) + Impl.GetCellValue(@GameID, 33)
END
GO

IF OBJECT_ID (N'Impl.GetInverseDiagonalValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetInverseDiagonalValue
GO

CREATE FUNCTION Impl.GetInverseDiagonalValue (@GameID varchar(100))
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, 13) + Impl.GetCellValue(@GameID, 22) + Impl.GetCellValue(@GameID, 31)
END
GO

IF OBJECT_ID(N'Impl.ProcessUserStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.ProcessUserStep
GO

CREATE PROCEDURE Impl.ProcessUserStep
    @GameID varchar(100),
    @Step int
AS
    SET NOCOUNT ON
    DECLARE @CellValue char(1)
    SELECT @CellValue = Value FROM Impl.GameSessionLog WHERE (GameId = @GameID) AND Step = @Step
    IF (@CellValue IS NOT NULL)
    BEGIN
        RAISERROR (N'Bad cell', 16, 1)
        RETURN
    END
    EXECUTE Impl.MakeStep @GameID, @Step, 'U'
GO

IF OBJECT_ID(N'Impl.ProcessCompStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.ProcessCompStep
GO

CREATE PROCEDURE Impl.ProcessCompStep @GameID varchar(100)
AS
    SET NOCOUNT ON
    DECLARE @NextCompStep int
    DECLARE @NextCompStepGenerator varchar(100)
    SELECT @NextCompStepGenerator = NextCompStepGenerator FROM Impl.GameSession WHERE GameID = @GameID
    EXECUTE @NextCompStepGenerator @GameID, @NextCompStep OUTPUT
    EXECUTE Impl.MakeStep @GameID, @NextCompStep, 'C'
GO

IF OBJECT_ID (N'Impl.CalculateGameResult', N'P') IS NOT NULL
    DROP PROCEDURE Impl.CalculateGameResult
GO

CREATE PROCEDURE Impl.CalculateGameResult @GameID varchar(100)
AS
    IF EXISTS(SELECT GameResult FROM Impl.GameSession WHERE GameID = @GameID AND GameResult IS NOT NULL)
        RETURN
    DECLARE @Row1 char(3), @Row2 char(3), @Row3 char(3)
    DECLARE @Column1 char(3), @Column2 char(3), @Column3 char(3)
    DECLARE @DirectDiagonal char(3), @InverseDiagonal char(3)
    SET @Row1 = Impl.GetRowValue(@GameID, 1)
    SET @Row2 = Impl.GetRowValue(@GameID, 2)
    SET @Row3 = Impl.GetRowValue(@GameID, 3)
    SET @Column1 = Impl.GetColumnValue(@GameID, 1)
    SET @Column2 = Impl.GetColumnValue(@GameID, 2)
    SET @Column3 = Impl.GetColumnValue(@GameID, 3)
    SET @DirectDiagonal = Impl.GetDirectDiagonalValue(@GameID)
    SET @InverseDiagonal = Impl.GetInverseDiagonalValue(@GameID)
    IF (@Row1 = N'XXX' OR @Row2 = N'XXX' OR @Row3 = N'XXX' OR @Column1 = N'XXX' OR @Column2 = N'XXX' OR @Column3 = N'XXX' OR @DirectDiagonal = N'XXX' OR @InverseDiagonal = N'XXX')
    BEGIN
        UPDATE Impl.GameSession SET GameResult = FirstPlayer WHERE GameID = @GameID
        RETURN
    END
    IF (@Row1 = N'OOO' OR @Row2 = N'OOO' OR @Row3 = N'OOO' OR @Column1 = N'OOO' OR @Column2 = N'OOO' OR @Column3 = N'OOO' OR @DirectDiagonal = N'OOO' OR @InverseDiagonal = N'OOO')
    BEGIN
        UPDATE Impl.GameSession SET GameResult = IIF(FirstPlayer = 'U', 'C', 'U') WHERE GameID = @GameID
        RETURN
    END
    DECLARE @LogCount int
    SELECT @LogCount = COUNT(Number) FROM Impl.GameSessionLog WHERE GameID = @GameID
    IF (@LogCount = 9)
        UPDATE Impl.GameSession SET GameResult = 'D' WHERE GameID = @GameID
GO

IF OBJECT_ID (N'Impl.FindFreeCell', N'FN') IS NOT NULL
    DROP FUNCTION Impl.FindFreeCell
GO

CREATE FUNCTION Impl.FindFreeCell (@GameID varchar(100), @CellNumber int)
RETURNS int
AS
BEGIN
    DECLARE @Step int; /* ";" here only for CTE */
    WITH Board (Step)
    AS
    (
        SELECT 11 UNION ALL SELECT 12 UNION ALL SELECT 13
        UNION ALL
        SELECT 21 UNION ALL SELECT 22 UNION ALL SELECT 23
        UNION ALL
        SELECT 31 UNION ALL SELECT 32 UNION ALL SELECT 33
    ),
    Data (CellNumber, Step)
    AS
    (
        SELECT ROW_NUMBER() OVER(ORDER BY Board.Step ASC), Board.Step
        FROM Board LEFT OUTER JOIN Impl.GameSessionLog AS GameLog ON Board.Step = GameLog.Step AND GameLog.GameID = @GameID
        WHERE GameLog.Value IS NULL
    )
    SELECT @Step = Step FROM Data WHERE CellNumber = @CellNumber
    RETURN @Step
END
GO


IF OBJECT_ID(N'Impl.SimpleGenerateCompNextStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.SimpleGenerateCompNextStep
GO

CREATE PROCEDURE Impl.SimpleGenerateCompNextStep
    @GameID varchar(100),
    @NextStep int OUTPUT
AS
    SET NOCOUNT ON
    SET @NextStep = Impl.FindFreeCell(@GameID, 1)
GO

IF OBJECT_ID(N'Impl.RandomGenerateCompNextStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.RandomGenerateCompNextStep
GO

CREATE PROCEDURE Impl.RandomGenerateCompNextStep
    @GameID varchar(100),
    @NextStep int OUTPUT
AS
    DECLARE @FreeCellsCount int
    SELECT @FreeCellsCount = 9 - COUNT(Number) FROM Impl.GameSessionLog WHERE GameID = @GameID
    DECLARE @SelectedCell int
    SET @SelectedCell =  1 + FLOOR(@FreeCellsCount * RAND())
    SET @NextStep = Impl.FindFreeCell(@GameID, @SelectedCell)
GO

IF OBJECT_ID (N'Impl.FindMandatoryStep', N'FN') IS NOT NULL
    DROP FUNCTION Impl.FindMandatoryStep
GO

CREATE FUNCTION Impl.FindMandatoryStep (@GameID varchar(100), @Value char(1))
RETURNS int
AS
BEGIN
    DECLARE @Pattern1 char(3), @Pattern2 char(3), @Pattern3 char(3)
    SET @Pattern1 = ' ' + @Value + @Value
    SET @Pattern2 = @Value + ' ' + @Value
    SET @Pattern3 = @Value + @Value + ' '
    DECLARE @Result int
    SET @Result = CASE Impl.GetRowValue(@GameID, 1) WHEN @Pattern1 THEN 11 WHEN @Pattern2 THEN 12 WHEN @Pattern3 THEN 13 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    SET @Result = CASE Impl.GetRowValue(@GameID, 2) WHEN @Pattern1 THEN 21 WHEN @Pattern2 THEN 22 WHEN @Pattern3 THEN 23 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    SET @Result = CASE Impl.GetRowValue(@GameID, 3) WHEN @Pattern1 THEN 31 WHEN @Pattern2 THEN 32 WHEN @Pattern3 THEN 33 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    SET @Result = CASE Impl.GetColumnValue(@GameID, 1) WHEN @Pattern1 THEN 11 WHEN @Pattern2 THEN 21 WHEN @Pattern3 THEN 31 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    SET @Result = CASE Impl.GetColumnValue(@GameID, 2) WHEN @Pattern1 THEN 12 WHEN @Pattern2 THEN 22 WHEN @Pattern3 THEN 32 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    SET @Result = CASE Impl.GetColumnValue(@GameID, 3) WHEN @Pattern1 THEN 13 WHEN @Pattern2 THEN 23 WHEN @Pattern3 THEN 33 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    SET @Result = CASE Impl.GetDirectDiagonalValue(@GameID) WHEN @Pattern1 THEN 11 WHEN @Pattern2 THEN 22 WHEN @Pattern3 THEN 33 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    SET @Result = CASE Impl.GetInverseDiagonalValue(@GameID) WHEN @Pattern1 THEN 13 WHEN @Pattern2 THEN 22 WHEN @Pattern3 THEN 31 ELSE NULL END
    IF @Result IS NOT NULL
        RETURN @RESULT
    RETURN NULL
END
GO

IF OBJECT_ID (N'Impl.FindOppositeCornerStep', N'FN') IS NOT NULL
    DROP FUNCTION Impl.FindOppositeCornerStep
GO

CREATE FUNCTION Impl.FindOppositeCornerStep (@GameID varchar(100), @Step int/*@Row int, @Column int*/)
RETURNS int
AS
BEGIN
    IF @Step = 11
        RETURN IIF(Impl.GetCellValue(@GameID, 33) = ' ', 33, NULL)
    IF @Step = 13
        RETURN IIF(Impl.GetCellValue(@GameID, 31) = ' ', 31, NULL)
    IF @Step = 31
        RETURN IIF(Impl.GetCellValue(@GameID, 13) = ' ', 13, NULL)
    IF @Step = 33
        RETURN IIF(Impl.GetCellValue(@GameID, 11) = ' ', 11, NULL)
    IF @Step = 12
        RETURN IIF(Impl.GetCellValue(@GameID, 31) = ' ', 31, IIF(Impl.GetCellValue(@GameID, 33) = ' ', 33, NULL))
    IF @Step = 32
        RETURN IIF(Impl.GetCellValue(@GameID, 11) = ' ', 11, IIF(Impl.GetCellValue(@GameID, 13) = ' ', 13, NULL))
    IF @Step = 21
        RETURN IIF(Impl.GetCellValue(@GameID, 13) = ' ', 13, IIF(Impl.GetCellValue(@GameID, 33) = ' ', 33, NULL))
    IF @Step = 23
        RETURN IIF(Impl.GetCellValue(@GameID, 11) = ' ', 11, IIF(Impl.GetCellValue(@GameID, 31) = ' ', 31, NULL))
    RETURN NULL
END
GO

IF OBJECT_ID(N'Impl.SmartGenerateCompNextStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.SmartGenerateCompNextStep
GO

CREATE PROCEDURE Impl.SmartGenerateCompNextStep
    @GameID varchar(100),
    @NextStep int OUTPUT
AS
    SET NOCOUNT ON
    DECLARE @Value char(1)
    SET @Value = Impl.GetCellValue(@GameID, 22)
    IF (@Value IS NULL)
    BEGIN
        SET @NextStep = 22
        RETURN
    END
    DECLARE @Figure char(1), @OppositeFigure char(1)
    SELECT @Figure = IIF(FirstPlayer = 'C', 'X', 'O'), @OppositeFigure = IIF(FirstPlayer = 'U', 'X', 'O') FROM Impl.GameSession WHERE GameID = @GameID
    SET @NextStep = Impl.FindMandatoryStep(@GameID, @Figure)
    IF @NextStep IS NOT NULL
        RETURN
    SET @NextStep = Impl.FindMandatoryStep(@GameID, @OppositeFigure)
    IF @NextStep IS NOT NULL
        RETURN
    IF @Figure = 'X'
    BEGIN
        DECLARE @LastStep int
        SELECT TOP 1 @LastStep = Step FROM Impl.GameSessionLog WHERE GameID = @GameID ORDER BY Number DESC
        SET @NextStep = Impl.FindOppositeCornerStep(@GameID, @LastStep)
        IF @NextStep IS NULL
            SET @NextStep = Impl.FindFreeCell(@GameID, 1)
    END
    ELSE
    BEGIN
        RETURN
    END
GO


/* API */
IF OBJECT_ID(N'dbo.StartGame', N'P') IS NOT NULL
    DROP PROCEDURE dbo.StartGame
GO

CREATE PROCEDURE dbo.StartGame
    @GameID varchar(100),
    @IsUserFirst bit,
    @NextCompStepGenerator varchar(100)
AS
    SET NOCOUNT ON
    INSERT Impl.GameSession(GameID, FirstPlayer, NextCompStepGenerator) VALUES(@GameID, IIF(@IsUserFirst = 1, 'U', 'C'), @NextCompStepGenerator)
    IF (@IsUserFirst <> 1)
        INSERT Impl.GameSessionLog(GameID, Number, Step, Value) VALUES(@GameID, 1, 22, 'X')
GO

IF OBJECT_ID(N'dbo.StartSimpleCompGame', N'P') IS NOT NULL
    DROP PROCEDURE dbo.StartSimpleCompGame
GO

CREATE PROCEDURE dbo.StartSimpleCompGame
    @GameID varchar(100),
    @IsUserFirst bit
AS
    SET NOCOUNT ON
    EXECUTE dbo.StartGame @GameID, @IsUserFirst, 'Impl.SimpleGenerateCompNextStep'
GO

IF OBJECT_ID(N'dbo.StartRandomCompGame', N'P') IS NOT NULL
    DROP PROCEDURE dbo.StartRandomCompGame
GO

CREATE PROCEDURE dbo.StartRandomCompGame
    @GameID varchar(100),
    @IsUserFirst bit
AS
    SET NOCOUNT ON
    EXECUTE dbo.StartGame @GameID, @IsUserFirst, 'Impl.RandomGenerateCompNextStep'
GO

IF OBJECT_ID(N'dbo.StartSmartCompGame', N'P') IS NOT NULL
    DROP PROCEDURE dbo.StartSmartCompGame
GO

CREATE PROCEDURE dbo.StartSmartCompGame
    @GameID varchar(100),
    @IsUserFirst bit
AS
    SET NOCOUNT ON
    EXECUTE dbo.StartGame @GameID, @IsUserFirst, 'Impl.SmartGenerateCompNextStep'
GO

IF OBJECT_ID(N'dbo.FinishGame', N'P') IS NOT NULL
    DROP PROCEDURE dbo.FinishGame
GO

CREATE PROCEDURE dbo.FinishGame @GameID varchar(100)
AS
    SET NOCOUNT ON
    DELETE FROM Impl.GameSession WHERE GameID = @GameID
GO

IF OBJECT_ID(N'dbo.ShowGameLog', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ShowGameLog
GO

CREATE PROCEDURE dbo.ShowGameLog @GameID varchar(100)
AS
    SET NOCOUNT ON
    DECLARE @Number int
    DECLARE @Step int
    DECLARE @Value char(1)
    DECLARE LogCursor CURSOR READ_ONLY FOR SELECT Number, Step, Value FROM Impl.GameSessionLog WHERE GameID = @GameID ORDER BY Number ASC
    OPEN LogCursor
    FETCH NEXT FROM LogCursor INTO @Number, @Step, @Value
    PRINT N'GAME LOG:'
    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT N'Number = ' + STR(@Number, 1) + N', Row = ' + STR(@Step / 10, 1) + N', Column = ' + STR(@Step % 10, 1) + N', Value = ' + @Value
        FETCH NEXT FROM LogCursor INTO @Number, @Step, @Value
    END
    CLOSE LogCursor
    DEALLOCATE LogCursor
GO

IF OBJECT_ID(N'dbo.ShowBoard', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ShowBoard
GO

CREATE PROCEDURE dbo.ShowBoard @GameID varchar(100)
AS
    SET NOCOUNT ON
    PRINT REPLICATE(N'=', 13)
    PRINT N'|   |   |   |'
    PRINT N'| ' + Impl.GetCellValue(@GameID, 11) + N' | ' + Impl.GetCellValue(@GameID, 12) + N' | ' + Impl.GetCellValue(@GameID, 13) + N' |'
    PRINT N'|   |   |   |'
    PRINT REPLICATE(N'=', 13)
    PRINT N'|   |   |   |'
    PRINT N'| ' + Impl.GetCellValue(@GameID, 21) + N' | ' + Impl.GetCellValue(@GameID, 22) + N' | ' + Impl.GetCellValue(@GameID, 23) + N' |'
    PRINT N'|   |   |   |'
    PRINT REPLICATE(N'=', 13)
    PRINT N'|   |   |   |'
    PRINT N'| ' + Impl.GetCellValue(@GameID, 31) + N' | ' + Impl.GetCellValue(@GameID, 32) + N' | ' + Impl.GetCellValue(@GameID, 33) + N' |'
    PRINT N'|   |   |   |'
    PRINT REPLICATE(N'=', 13)
GO

IF OBJECT_ID (N'dbo.GetGameResult', N'FN') IS NOT NULL
    DROP FUNCTION dbo.GetGameResult
GO

CREATE FUNCTION dbo.GetGameResult (@GameID varchar(100))
RETURNS varchar(20)
AS
BEGIN
    DECLARE @GameResult char(1)
    SELECT @GameResult = GameResult FROM Impl.GameSession WHERE GameID = @GameID
    DECLARE @Result varchar(20)
    RETURN CASE @GameResult
               WHEN 'U' THEN 'User is winner'
               WHEN 'C' THEN 'Computer is winner'
               WHEN 'D' THEN 'Result is draw'
               ELSE NULL
           END
END
GO

IF OBJECT_ID('dbo.ProcessStep', 'P') IS NOT NULL
    DROP PROCEDURE dbo.ProcessStep
GO

CREATE PROCEDURE dbo.ProcessStep
    @GameID varchar(100),
    @UserStepRow int,
    @UserStepColumn int
AS
    SET NOCOUNT ON
    DECLARE @GameResult varchar(20)
    EXECUTE Impl.CalculateGameResult @GameID
    SET @GameResult = dbo.GetGameResult(@GameID)
    IF (@GameResult IS NOT NULL)
    BEGIN
        EXECUTE dbo.ShowBoard @GameID
        EXECUTE dbo.ShowGameLog @GameID
        PRINT N'Game is finished. ' + @GameResult
        RETURN
    END
    BEGIN TRY
        DECLARE @UserStep int
        SET @UserStep = 10 * @UserStepRow + @UserStepColumn
        EXECUTE Impl.ProcessUserStep @GameID, @UserStep
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage nvarchar(4000)
        DECLARE @ErrorSeverity int
        DECLARE @ErrorState int
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE()
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
        RETURN
    END CATCH
    EXECUTE Impl.CalculateGameResult @GameID
    SET @GameResult = dbo.GetGameResult(@GameID)
    IF (@GameResult IS NOT NULL)
    BEGIN
        EXECUTE dbo.ShowBoard @GameID
        EXECUTE dbo.ShowGameLog @GameID
        PRINT N'Game is finished. ' + @GameResult
        RETURN
    END
    EXECUTE Impl.ProcessCompStep @GameID
    EXECUTE dbo.ShowBoard @GameID
    EXECUTE dbo.ShowGameLog @GameID
    EXECUTE Impl.CalculateGameResult @GameID
    SET @GameResult = dbo.GetGameResult(@GameID)
    IF (@GameResult IS NOT NULL)
        PRINT N'Game is finished. ' + @GameResult
GO

IF OBJECT_ID(N'dbo.Cleanup', N'P') IS NOT NULL
    DROP PROCEDURE dbo.Cleanup
GO

CREATE PROCEDURE dbo.Cleanup
AS
    SET NOCOUNT ON
    DELETE FROM Impl.GameSession
GO

/*
EXECUTE StartSimpleCompGame N'AA', 1;
EXECUTE ShowBoard N'AA';
EXECUTE ProcessStep N'AA', 2, 2;
EXECUTE ProcessStep N'AA', 1, 1;
EXECUTE ProcessStep N'AA', 1, 3;
EXECUTE ProcessStep N'AA', 3, 1;
EXECUTE FinishGame N'AA';
*/

/*
EXECUTE StartSimpleCompGame N'BB', 0;
EXECUTE ShowBoard N'BB';
EXECUTE ProcessStep N'BB', 1, 2;
EXECUTE ProcessStep N'BB', 1, 3;
EXECUTE ProcessStep N'BB', 2, 3;
EXECUTE FinishGame N'BB';
*/

/*
EXECUTE StartSimpleCompGame N'CC', 0;
EXECUTE ShowBoard N'CC';
EXECUTE ProcessStep N'CC', 1, 2;
EXECUTE ProcessStep N'CC', 3, 3;
EXECUTE ProcessStep N'CC', 3, 1;
EXECUTE ProcessStep N'CC', 2, 3;
EXECUTE FinishGame N'CC';
*/

/*
EXECUTE StartRandomCompGame N'AA', 1;
EXECUTE ShowBoard N'AA';
EXECUTE ProcessStep N'AA', 2, 2;
EXECUTE ProcessStep N'AA', 1, 1;
EXECUTE ProcessStep N'AA', 3, 3;
EXECUTE FinishGame N'AA';
*/

/*
EXECUTE dbo.Cleanup
*/