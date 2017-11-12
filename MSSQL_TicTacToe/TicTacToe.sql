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

/* 'U' = User, 'C' = Comp */
CREATE TABLE Impl.GameSession
(
GameID varchar(100) NOT NULL PRIMARY KEY,
FirstPlayer char(1) NOT NULL CHECK (FirstPlayer = 'U' OR FirstPlayer = 'C')/*,
Finished bit NOT NULL DEFAULT(0),
Winner char(1) NULL CHECK (FirstPlayer = 'U' or FirstPlayer = 'C')*/
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
    DECLARE @Row int
    DECLARE @Column int;
    WITH Board ([Row], [Column])
    AS
    (
        SELECT 1, 1 UNION ALL SELECT 1, 2 UNION ALL SELECT 1, 3
        UNION ALL
        SELECT 2, 1 UNION ALL SELECT 2, 2 UNION ALL SELECT 2, 3
        UNION ALL
        SELECT 3, 1 UNION ALL SELECT 3, 2 UNION ALL SELECT 3, 3
    )
    SELECT TOP 1 @Row = Board.[Row], @Column = Board.[Column]
    FROM Board LEFT OUTER JOIN Impl.GameSessionLog AS GameLog ON Board.[Row] = GameLog.[Row] AND Board.[Column] = GameLog.[Column] AND GameLog.GameID = @GameID
    WHERE GameLog.Value IS NULL
    EXECUTE Impl.MakeStep @GameID, @Row, @Column, 'C'
GO

/* API */
IF OBJECT_ID(N'dbo.StartGame', N'P') IS NOT NULL
    DROP PROCEDURE dbo.StartGame
GO

CREATE PROCEDURE dbo.StartGame
    @GameID varchar(100),
    @IsUserFirst bit
AS
    SET NOCOUNT ON
    INSERT Impl.GameSession(GameID, FirstPlayer) VALUES(@GameID, IIF(@IsUserFirst = 1, 'U', 'C'))
    IF (@IsUserFirst <> 1)
        INSERT Impl.GameSessionLog(GameID, Step, [Row], [Column], Value) VALUES(@GameID, 1, 2, 2, 'X')
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
    DECLARE @Row1 CHAR(3), @Row2 CHAR(3), @Row3 CHAR(3)
    DECLARE @Column1 CHAR(3), @Column2 CHAR(3), @Column3 CHAR(3)
    DECLARE @DirectDiagonal CHAR(3), @InverseDiagonal CHAR(3)
    SET @Row1 = Impl.GetRowValue(@GameID, 1)
    SET @Row2 = Impl.GetRowValue(@GameID, 2)
    SET @Row3 = Impl.GetRowValue(@GameID, 3)
    SET @Column1 = Impl.GetColumnValue(@GameID, 1)
    SET @Column2 = Impl.GetColumnValue(@GameID, 2)
    SET @Column3 = Impl.GetColumnValue(@GameID, 3)
    SET @DirectDiagonal = Impl.GetDirectDiagonalValue(@GameID)
    SET @InverseDiagonal = Impl.GetInverseDiagonalValue(@GameID)
    DECLARE @FirstPlayer char(1)
    SELECT @FirstPlayer = FirstPlayer FROM Impl.GameSession WHERE GameID = @GameID
    IF (@Row1 = N'XXX' OR @Row2 = N'XXX' OR @Row3 = N'XXX' OR @Column1 = N'XXX' OR @Column2 = N'XXX' OR @Column3 = N'XXX' OR @DirectDiagonal = N'XXX' OR @InverseDiagonal = N'XXX')
        RETURN IIF(@FirstPlayer = 'U', 'User', 'Comp') + ' is winner'
    IF (@Row1 = N'OOO' OR @Row2 = N'OOO' OR @Row3 = N'OOO' OR @Column1 = N'OOO' OR @Column2 = N'OOO' OR @Column3 = N'OOO' OR @DirectDiagonal = N'OOO' OR @InverseDiagonal = N'OOO')
        RETURN IIF(@FirstPlayer = 'U', 'Comp', 'User') + ' is winner'
    DECLARE @LogCount int
    SELECT @LogCount = COUNT(Step) FROM Impl.GameSessionLog WHERE GameID = @GameID
    IF (@LogCount = 9)
        RETURN 'Result is draw'
    RETURN NULL
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
    SET @GameResult = dbo.GetGameResult(@GameID)
    IF (@GameResult IS NOT NULL)
        PRINT N'Game is finished. ' + @GameResult
GO

/*
EXECUTE StartGame N'AA', 1;
EXECUTE ShowBoard N'AA';
EXECUTE ProcessStep N'AA', 2, 2;
EXECUTE ProcessStep N'AA', 1, 1;
EXECUTE ProcessStep N'AA', 1, 3;
EXECUTE ProcessStep N'AA', 3, 1;
EXECUTE FinishGame N'AA';
*/

/*
EXECUTE StartGame N'BB', 0;
EXECUTE ShowBoard N'BB';
EXECUTE ProcessStep N'BB', 1, 2;
EXECUTE ProcessStep N'BB', 1, 3;
EXECUTE ProcessStep N'BB', 2, 3;
EXECUTE FinishGame N'BB';
*/

/*
EXECUTE StartGame N'CC', 0;
EXECUTE ShowBoard N'CC';
EXECUTE ProcessStep N'CC', 1, 2;
EXECUTE ProcessStep N'CC', 3, 3;
EXECUTE ProcessStep N'CC', 3, 1;
EXECUTE ProcessStep N'CC', 2, 3;
EXECUTE FinishGame N'CC';
*/