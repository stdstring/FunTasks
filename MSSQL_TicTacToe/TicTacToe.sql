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
Step int NOT NULL CHECK(Step >= 1 AND Step <= 9),
[Row] int NOT NULL CHECK([Row] >= 1 AND [Row] <= 3),
[Column] int NOT NULL CHECK([Column] >= 1 AND [Column] <= 3),
Value char(1) NOT NULL CHECK (Value = 'X' OR Value = 'O'),
CONSTRAINT GameSessionLog_PK PRIMARY KEY (GameID, Step),
CONSTRAINT GameSession_FK FOREIGN KEY (GameID) REFERENCES Impl.GameSession(GameID) ON UPDATE CASCADE ON DELETE CASCADE
)
GO

/* INTERNAL */
IF OBJECT_ID (N'Impl.GetCellValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetCellValue
GO

CREATE FUNCTION Impl.GetCellValue (@GameID varchar(100), @Row int, @Column int)
RETURNS char(1)
AS
BEGIN
    DECLARE @Value char(1)
    SELECT @Value = Value FROM Impl.GameSessionLog WHERE (GameId = @GameID) AND ([Row] = @Row) AND ([Column] = @Column)
    RETURN ISNULL(@Value, ' ')
END
GO

IF OBJECT_ID(N'Impl.MakeStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.MakeStep
GO

CREATE PROCEDURE Impl.MakeStep
    @GameID varchar(100),
    @Row int,
    @Column int,
    @Player char(1)
AS
    SET NOCOUNT ON
    DECLARE @FirstPlayer char(1)
    SELECT @FirstPlayer = FirstPlayer FROM Impl.GameSession WHERE GameID = @GameID
    DECLARE @LogCount int
    SELECT @LogCount = COUNT(Step) FROM Impl.GameSessionLog WHERE GameID = @GameID
    INSERT Impl.GameSessionLog(GameID, Step, [Row], [Column], Value) VALUES(@GameID, @LogCount + 1, @Row, @Column, IIF(@Player = @FirstPlayer, 'X', 'O'))
GO

IF OBJECT_ID (N'Impl.GetRowValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetRowValue
GO

CREATE FUNCTION Impl.GetRowValue (@GameID varchar(100), @Row int)
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, @Row, 1) + Impl.GetCellValue(@GameID, @Row, 2) + Impl.GetCellValue(@GameID, @Row, 3)
END
GO

IF OBJECT_ID (N'Impl.GetColumnValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetColumnValue
GO

CREATE FUNCTION Impl.GetColumnValue (@GameID varchar(100), @Column int)
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, 1, @Column) + Impl.GetCellValue(@GameID, 2, @Column) + Impl.GetCellValue(@GameID, 3, @Column)
END
GO

IF OBJECT_ID (N'Impl.GetDirectDiagonalValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetDirectDiagonalValue
GO

CREATE FUNCTION Impl.GetDirectDiagonalValue (@GameID varchar(100))
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, 1, 1) + Impl.GetCellValue(@GameID, 2, 2) + Impl.GetCellValue(@GameID, 3, 3)
END
GO

IF OBJECT_ID (N'Impl.GetInverseDiagonalValue', N'FN') IS NOT NULL
    DROP FUNCTION Impl.GetInverseDiagonalValue
GO

CREATE FUNCTION Impl.GetInverseDiagonalValue (@GameID varchar(100))
RETURNS char(3)
AS
BEGIN
    RETURN Impl.GetCellValue(@GameID, 1, 3) + Impl.GetCellValue(@GameID, 2, 2) + Impl.GetCellValue(@GameID, 3, 1)
END
GO

IF OBJECT_ID(N'Impl.ProcessUserStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.ProcessUserStep
GO

CREATE PROCEDURE Impl.ProcessUserStep
    @GameID varchar(100),
    @Row int,
    @Column int
AS
    SET NOCOUNT ON
    DECLARE @CellValue char(1)
    SELECT @CellValue = Value FROM Impl.GameSessionLog WHERE (GameId = @GameID) AND ([Row] = @Row) AND ([Column] = @Column)
    IF (@CellValue IS NOT NULL)
    BEGIN
        RAISERROR (N'Bad cell', 16, 1)
        RETURN
    END
    EXECUTE Impl.MakeStep @GameID, @Row, @Column, 'U'
GO

IF OBJECT_ID(N'Impl.ProcessCompStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.ProcessCompStep
GO

CREATE PROCEDURE Impl.ProcessCompStep @GameID varchar(100)
AS
    SET NOCOUNT ON
    DECLARE @NextCompStepRow int
    DECLARE @NextCompStepColumn int
    DECLARE @NextCompStepGenerator varchar(100)
    SELECT @NextCompStepGenerator = NextCompStepGenerator FROM Impl.GameSession WHERE GameID = @GameID
    EXECUTE @NextCompStepGenerator @GameID, @NextCompStepRow OUTPUT, @NextCompStepColumn OUTPUT
    EXECUTE Impl.MakeStep @GameID, @NextCompStepRow, @NextCompStepColumn, 'C'
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
    SELECT @LogCount = COUNT(Step) FROM Impl.GameSessionLog WHERE GameID = @GameID
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
    DECLARE @Row int
    DECLARE @Column int; /* ";" here only for CTE */
    WITH Board ([Row], [Column])
    AS
    (
        SELECT 1, 1 UNION ALL SELECT 1, 2 UNION ALL SELECT 1, 3
        UNION ALL
        SELECT 2, 1 UNION ALL SELECT 2, 2 UNION ALL SELECT 2, 3
        UNION ALL
        SELECT 3, 1 UNION ALL SELECT 3, 2 UNION ALL SELECT 3, 3
    ),
    Data (CellNumber, [Row], [Column])
    AS
    (
        SELECT ROW_NUMBER() OVER(ORDER BY Board.[Row] ASC, Board.[Column] ASC), Board.[Row], Board.[Column]
        FROM Board LEFT OUTER JOIN Impl.GameSessionLog AS GameLog ON Board.[Row] = GameLog.[Row] AND Board.[Column] = GameLog.[Column] AND GameLog.GameID = @GameID
        WHERE GameLog.Value IS NULL
    )
    SELECT @Row = [Row], @Column = [Column] FROM Data WHERE CellNumber = @CellNumber
    RETURN 10 * @Row + @Column
END
GO


IF OBJECT_ID(N'Impl.SimpleGenerateCompNextStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.SimpleGenerateCompNextStep
GO

CREATE PROCEDURE Impl.SimpleGenerateCompNextStep
    @GameID varchar(100),
    @NextStepRow int OUTPUT,
    @NextStepColumn int OUTPUT
AS
    SET NOCOUNT ON
    DECLARE @Result int
    SET @Result = Impl.FindFreeCell(@GameID, 1)
    SET @NextStepRow = @Result / 10
    SET @NextStepColumn = @Result % 10
GO

IF OBJECT_ID(N'Impl.RandomGenerateCompNextStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.RandomGenerateCompNextStep
GO

CREATE PROCEDURE Impl.RandomGenerateCompNextStep
    @GameID varchar(100),
    @NextStepRow int OUTPUT,
    @NextStepColumn int OUTPUT
AS
    DECLARE @FreeCellsCount int
    SELECT @FreeCellsCount = 9 - COUNT(Step) FROM Impl.GameSessionLog WHERE GameID = @GameID
    DECLARE @SelectedCell int
    SET @SelectedCell =  1 + FLOOR(@FreeCellsCount * RAND())
    DECLARE @Result int
    SET @Result = Impl.FindFreeCell(@GameID, @SelectedCell)
    SET @NextStepRow = @Result / 10
    SET @NextStepColumn = @Result % 10
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

CREATE FUNCTION Impl.FindOppositeCornerStep (@GameID varchar(100), @Row int, @Column int)
RETURNS int
AS
BEGIN
    IF @Row = 1 AND @Column = 1
        RETURN IIF(Impl.GetCellValue(@GameID, 3, 3) = ' ', 33, NULL)
    IF @Row = 1 AND @Column = 3
        RETURN IIF(Impl.GetCellValue(@GameID, 3, 1) = ' ', 31, NULL)
    IF @Row = 3 AND @Column = 1
        RETURN IIF(Impl.GetCellValue(@GameID, 1, 3) = ' ', 13, NULL)
    IF @Row = 3 AND @Column = 3
        RETURN IIF(Impl.GetCellValue(@GameID, 1, 1) = ' ', 11, NULL)
    IF @Row = 1 AND @Column = 2
        RETURN IIF(Impl.GetCellValue(@GameID, 3, 1) = ' ', 31, IIF(Impl.GetCellValue(@GameID, 3, 3) = ' ', 33, NULL))
    IF @Row = 3 AND @Column = 2
        RETURN IIF(Impl.GetCellValue(@GameID, 1, 1) = ' ', 11, IIF(Impl.GetCellValue(@GameID, 1, 3) = ' ', 13, NULL))
    IF @Row = 2 AND @Column = 1
        RETURN IIF(Impl.GetCellValue(@GameID, 1, 3) = ' ', 13, IIF(Impl.GetCellValue(@GameID, 3, 3) = ' ', 33, NULL))
    IF @Row = 2 AND @Column = 3
        RETURN IIF(Impl.GetCellValue(@GameID, 1, 1) = ' ', 13, IIF(Impl.GetCellValue(@GameID, 3, 1) = ' ', 33, NULL))
    RETURN NULL
END
GO

IF OBJECT_ID(N'Impl.SmartGenerateCompNextStep', N'P') IS NOT NULL
    DROP PROCEDURE Impl.SmartGenerateCompNextStep
GO

CREATE PROCEDURE Impl.SmartGenerateCompNextStep
    @GameID varchar(100),
    @NextStepRow int OUTPUT,
    @NextStepColumn int OUTPUT
AS
    SET NOCOUNT ON
    DECLARE @Value char(1)
    SET @Value = Impl.GetCellValue(@GameID, 2, 2)
    IF (@Value IS NULL)
    BEGIN
        SET @NextStepRow = 2
        SET @NextStepColumn = 2
        RETURN
    END
    DECLARE @Figure char(1), @OppositeFigure char(1)
    SELECT @Figure = IIF(FirstPlayer = 'C', 'X', 'O'), @OppositeFigure = IIF(FirstPlayer = 'U', 'X', 'O') FROM Impl.GameSession WHERE GameID = @GameID
    DECLARE @MandatoryStep int
    SET @MandatoryStep = Impl.FindMandatoryStep(@GameID, @Figure)
    IF @MandatoryStep IS NOT NULL
    BEGIN
        SET @NextStepRow = @MandatoryStep / 10
        SET @NextStepColumn = @MandatoryStep % 10
        RETURN
    END
    SET @MandatoryStep = Impl.FindMandatoryStep(@GameID, @OppositeFigure)
    IF @MandatoryStep IS NOT NULL
    BEGIN
        SET @NextStepRow = @MandatoryStep / 10
        SET @NextStepColumn = @MandatoryStep % 10
        RETURN
    END
    IF @Figure = 'X'
    BEGIN
        DECLARE @LastStepRow int
        DECLARE @LastStepColumn int
        SELECT TOP 1 @LastStepRow = [Row], @LastStepColumn = [Column] FROM Impl.GameSessionLog WHERE GameID = @GameID ORDER BY Step DESC
        DECLARE @Result int
        SET @Result = Impl.FindOppositeCornerStep(@GameID, @LastStepRow, @LastStepColumn)
        IF @Result IS NULL
            SET @Result = Impl.FindFreeCell(@GameID, 1)
        SET @NextStepRow = @Result / 10
        SET @NextStepColumn = @Result % 10
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
        INSERT Impl.GameSessionLog(GameID, Step, [Row], [Column], Value) VALUES(@GameID, 1, 2, 2, 'X')
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
    DECLARE @Step int
    DECLARE @Row int
    DECLARE @Column int
    DECLARE @Value char(1)
    DECLARE LogCursor CURSOR READ_ONLY FOR SELECT Step, [Row], [Column], Value FROM Impl.GameSessionLog WHERE GameID = @GameID ORDER BY Step ASC
    OPEN LogCursor
    FETCH NEXT FROM LogCursor INTO @Step, @Row, @Column, @Value
    PRINT N'GAME LOG:'
    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT N'Step = ' + STR(@Step, 1) + N', Row = ' + STR(@Row, 1) + N', Column = ' + STR(@Column, 1) + N', Value = ' + @Value
        FETCH NEXT FROM LogCursor INTO @Step, @Row, @Column, @Value
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
    PRINT N'| ' + Impl.GetCellValue(@GameID, 1, 1) + N' | ' + Impl.GetCellValue(@GameID, 1, 2) + N' | ' + Impl.GetCellValue(@GameID, 1, 3) + N' |'
    PRINT N'|   |   |   |'
    PRINT REPLICATE(N'=', 13)
    PRINT N'|   |   |   |'
    PRINT N'| ' + Impl.GetCellValue(@GameID, 2, 1) + N' | ' + Impl.GetCellValue(@GameID, 2, 2) + N' | ' + Impl.GetCellValue(@GameID, 2, 3) + N' |'
    PRINT N'|   |   |   |'
    PRINT REPLICATE(N'=', 13)
    PRINT N'|   |   |   |'
    PRINT N'| ' + Impl.GetCellValue(@GameID, 3, 1) + N' | ' + Impl.GetCellValue(@GameID, 3, 2) + N' | ' + Impl.GetCellValue(@GameID, 3, 3) + N' |'
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
        EXECUTE Impl.ProcessUserStep @GameID, @UserStepRow, @UserStepColumn
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