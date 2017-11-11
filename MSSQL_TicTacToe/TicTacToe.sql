USE master
GO

IF DB_ID (N'TicTacToe') IS NOT NULL
    DROP DATABASE TicTacToe
GO

CREATE DATABASE TicTacToe
GO

USE TicTacToe
GO

/* TODO (std_string) : use domain for 'U' = User and 'C' = Comp */
CREATE TABLE GameSession
(
GameID varchar(100)/*uniqueidentifier DEFAULT(NEWID())*/ PRIMARY KEY,
FirstPlayer char(1) not null CHECK (FirstPlayer = 'U' or FirstPlayer = 'C'),
/*Finished bit not null DEFAULT(0)*/
/*Winner char(1) null DEFAULT(null) CHECK (FirstPlayer = 'U' or FirstPlayer = 'C')*/
)
GO


/* TODO (std_string) : use domain for 'X' and 'O' */
CREATE TABLE GameSessionLog
(
GameID varchar(100)/*uniqueidentifier*/ not null,
Step int not null CHECK(Step >= 1 and Step <= 9),
[Row] int not null CHECK([Row] >= 1 and [Row] <= 3),
[Column] int not null CHECK([Column] >= 1 and [Column] <= 3),
Value char(1) not null CHECK (Value = 'X' or Value = 'O'),
CONSTRAINT GameSessionLog_PK PRIMARY KEY (GameID, Step),
CONSTRAINT GameSession_FK FOREIGN KEY (GameID) REFERENCES GameSession(GameID) ON UPDATE CASCADE ON DELETE CASCADE
)
GO

/* INTERNAL */
IF OBJECT_ID (N'GetCellValue', N'FN') IS NOT NULL
    DROP FUNCTION GetCellValue
GO

CREATE FUNCTION GetCellValue (@GameID varchar(100)/*uniqueidentifier*/, @Row int, @Column int)
RETURNS char(1)
AS
BEGIN
    DECLARE @Value char(1)
    SELECT @Value = Value FROM GameSessionLog WHERE (GameId = @GameID) and ([Row] = @Row) and ([Column] = @Column)
    RETURN ISNULL(@Value, ' ')
END
GO

IF OBJECT_ID(N'MakeStep', N'P') IS NOT NULL
    DROP PROCEDURE MakeStep
GO

CREATE PROCEDURE MakeStep
    @GameID varchar(100)/*uniqueidentifier*/,
    @Row int,
    @Column int,
    @Player char(1)
AS
    SET NOCOUNT ON
    DECLARE @FirstPlayer char(1)
    SELECT @FirstPlayer = FirstPlayer FROM GameSession WHERE GameID = @GameID
    DECLARE @LogCount int
    SELECT @LogCount = COUNT(Step) FROM GameSessionLog WHERE GameID = @GameID
    INSERT GameSessionLog(GameID, Step, [Row], [Column], Value) VALUES(@GameID, @LogCount + 1, @Row, @Column, IIF(@Player = @FirstPlayer, 'X', 'O'))
GO

IF OBJECT_ID (N'GetRowValue', N'FN') IS NOT NULL
    DROP FUNCTION GetRowValue
GO

CREATE FUNCTION GetRowValue (@GameID varchar(100)/*uniqueidentifier*/, @Row int)
RETURNS char(3)
AS
BEGIN
    RETURN dbo.GetCellValue(@GameID, @Row, 1) + dbo.GetCellValue(@GameID, @Row, 2) + dbo.GetCellValue(@GameID, @Row, 3)
END
GO

IF OBJECT_ID (N'GetColumnValue', N'FN') IS NOT NULL
    DROP FUNCTION GetColumnValue
GO

CREATE FUNCTION GetColumnValue (@GameID varchar(100)/*uniqueidentifier*/, @Column int)
RETURNS char(3)
AS
BEGIN
    RETURN dbo.GetCellValue(@GameID, 1, @Column) + dbo.GetCellValue(@GameID, 2, @Column) + dbo.GetCellValue(@GameID, 3, @Column)
END
GO

IF OBJECT_ID (N'GetDirectDiagonalValue', N'FN') IS NOT NULL
    DROP FUNCTION GetDirectDiagonalValue
GO

CREATE FUNCTION GetDirectDiagonalValue (@GameID varchar(100)/*uniqueidentifier*/)
RETURNS char(3)
AS
BEGIN
    RETURN dbo.GetCellValue(@GameID, 1, 1) + dbo.GetCellValue(@GameID, 2, 2) + dbo.GetCellValue(@GameID, 3, 3)
END
GO

IF OBJECT_ID (N'GetInverseDiagonalValue', N'FN') IS NOT NULL
    DROP FUNCTION GetInverseDiagonalValue
GO

CREATE FUNCTION GetInverseDiagonalValue (@GameID varchar(100)/*uniqueidentifier*/)
RETURNS char(3)
AS
BEGIN
    RETURN dbo.GetCellValue(@GameID, 1, 3) + dbo.GetCellValue(@GameID, 2, 2) + dbo.GetCellValue(@GameID, 3, 1)
END
GO

IF OBJECT_ID(N'ProcessUserStep', N'P') IS NOT NULL
    DROP PROCEDURE ProcessUserStep
GO

CREATE PROCEDURE ProcessUserStep
    @GameID varchar(100)/*uniqueidentifier*/,
    @Row int,
    @Column int
AS
    SET NOCOUNT ON
    DECLARE @CellValue char(1)
    SELECT @CellValue = Value FROM GameSessionLog WHERE (GameId = @GameID) and ([Row] = @Row) and ([Column] = @Column)
    IF (@CellValue IS NOT NULL)
    BEGIN
        RAISERROR (N'Bad cell', 16, 1)
        RETURN
    END
    EXECUTE MakeStep @GameID, @Row, @Column, 'U'
GO

IF OBJECT_ID(N'ProcessCompStep', N'P') IS NOT NULL
    DROP PROCEDURE ProcessCompStep
GO

CREATE PROCEDURE ProcessCompStep @GameID varchar(100)/*uniqueidentifier*/
AS
    SET NOCOUNT ON
    DECLARE @Row int
    DECLARE @Column int;
    WITH Board_CTE ([Row], [Column])
    AS
    (
        SELECT 1, 1 UNION ALL SELECT 1, 2 UNION ALL SELECT 1, 3
        UNION ALL
        SELECT 2, 1 UNION ALL SELECT 2, 2 UNION ALL SELECT 2, 3
        UNION ALL
        SELECT 3, 1 UNION ALL SELECT 3, 2 UNION ALL SELECT 3, 3
    )
    SELECT TOP 1 @Row = Board.[Row], @Column = Board.[Column] FROM Board_CTE AS Board LEFT OUTER JOIN GameSessionLog GameLog
                                                              ON Board.[Row] = GameLog.[Row] AND Board.[Column] = GameLog.[Column] AND GameLog.GameID = @GameID
                                                              WHERE GameLog.Value IS NULL
    /* TODO (std_string) : probably check on search success */
    EXECUTE MakeStep @GameID, @Row, @Column, 'C'
GO

/* API */

IF OBJECT_ID(N'StartGame', N'P') IS NOT NULL
    DROP PROCEDURE StartGame
GO

CREATE PROCEDURE StartGame
    @GameID varchar(100)/*uniqueidentifier*/,
    @IsUserFirst bit
AS
    SET NOCOUNT ON
    /*DECLARE @GameID uniqueidentifier
    SET @GameID = NEWID()*/
    INSERT GameSession(GameID, FirstPlayer/*, Finished*/) VALUES(@GameID, IIF(@IsUserFirst = 1, 'U', 'C')/*, 0*/)
    IF (@IsUserFirst <> 1)
        INSERT GameSessionLog(GameID, Step, [Row], [Column], Value) VALUES(@GameID, 1, 2, 2, 'X')
    /*SELECT @GameID AS GameID*/
GO

IF OBJECT_ID(N'FinishGame', N'P') IS NOT NULL
    DROP PROCEDURE FinishGame
GO

CREATE PROCEDURE FinishGame @GameID varchar(100)/*uniqueidentifier*/
AS
    SET NOCOUNT ON
    DELETE FROM GameSession WHERE GameID = @GameID
GO

IF OBJECT_ID(N'GetGameLog', N'P') IS NOT NULL
    DROP PROCEDURE GetGameLog
GO

CREATE PROCEDURE GetGameLog @GameID varchar(100)/*uniqueidentifier*/
AS
    SET NOCOUNT ON
    SELECT GameID, Step, [Row], [Column], Value FROM GameSessionLog WHERE GameID = @GameID ORDER BY Step ASC
GO

IF OBJECT_ID(N'ShowBoard', N'P') IS NOT NULL
    DROP PROCEDURE ShowBoard
GO

CREATE PROCEDURE ShowBoard @GameID varchar(100)/*uniqueidentifier*/
AS
    SET NOCOUNT ON
    PRINT REPLICATE(N'=', 7)
    PRINT N'|' + dbo.GetCellValue(@GameID, 1, 1) + N'|' + dbo.GetCellValue(@GameID, 1, 2) + N'|' + dbo.GetCellValue(@GameID, 1, 3) + N'|'
    PRINT REPLICATE(N'=', 7)
    PRINT N'|' + dbo.GetCellValue(@GameID, 2, 1) + N'|' + dbo.GetCellValue(@GameID, 2, 2) + N'|' + dbo.GetCellValue(@GameID, 2, 3) + N'|'
    PRINT REPLICATE(N'=', 7)
    PRINT N'|' + dbo.GetCellValue(@GameID, 3, 1) + N'|' + dbo.GetCellValue(@GameID, 3, 2) + N'|' + dbo.GetCellValue(@GameID, 3, 3) + N'|'
    PRINT REPLICATE(N'=', 7)
GO

IF OBJECT_ID (N'GetGameWinner', N'FN') IS NOT NULL
    DROP FUNCTION GetGameWinner
GO

CREATE FUNCTION GetGameWinner (@GameID varchar(100)/*uniqueidentifier*/)
RETURNS varchar(10)
AS
BEGIN
    DECLARE @Row1 CHAR(3), @Row2 CHAR(3), @Row3 CHAR(3)
    DECLARE @Column1 CHAR(3), @Column2 CHAR(3), @Column3 CHAR(3)
    DECLARE @DirectDiagonal CHAR(3), @InverseDiagonal CHAR(3)
    SET @Row1 = dbo.GetRowValue(@GameID, 1)
    SET @Row2 = dbo.GetRowValue(@GameID, 2)
    SET @Row3 = dbo.GetRowValue(@GameID, 3)
    SET @Column1 = dbo.GetColumnValue(@GameID, 1)
    SET @Column2 = dbo.GetColumnValue(@GameID, 2)
    SET @Column3 = dbo.GetColumnValue(@GameID, 3)
    SET @DirectDiagonal = dbo.GetDirectDiagonalValue(@GameID)
    SET @InverseDiagonal = dbo.GetInverseDiagonalValue(@GameID)
    DECLARE @FirstPlayer char(1)
    SELECT @FirstPlayer = FirstPlayer FROM GameSession WHERE GameID = @GameID
    IF (@Row1 = N'XXX' OR @Row2 = N'XXX' OR @Row3 = N'XXX' OR @Column1 = N'XXX' OR @Column2 = N'XXX' OR @Column3 = N'XXX' OR @DirectDiagonal = N'XXX' OR @InverseDiagonal = N'XXX')
        RETURN IIF(@FirstPlayer = 'U', 'User', 'Comp')
    IF (@Row1 = N'OOO' OR @Row2 = N'OOO' OR @Row3 = N'OOO' OR @Column1 = N'OOO' OR @Column2 = N'OOO' OR @Column3 = N'OOO' OR @DirectDiagonal = N'OOO' OR @InverseDiagonal = N'OOO')
        RETURN IIF(@FirstPlayer = 'U', 'Comp', 'User')
    DECLARE @LogCount int
    SELECT @LogCount = COUNT(Step) FROM GameSessionLog WHERE GameID = @GameID
    IF (@LogCount = 9)
        RETURN 'Draw'
    RETURN NULL
END
GO

IF OBJECT_ID('ProcessStep', 'P') IS NOT NULL
    DROP PROCEDURE ProcessStep
GO

CREATE PROCEDURE ProcessStep
    @GameID varchar(100)/*uniqueidentifier*/,
    @UserStepRow int,
    @UserStepColumn int
AS
    SET NOCOUNT ON
    DECLARE @Winner varchar(10)
    SET @Winner = dbo.GetGameWinner(@GameID)
    IF (@Winner IS NOT NULL)
    BEGIN
        EXECUTE ShowBoard @GameID
        EXECUTE GetGameLog @GameID
        PRINT N'GAME IS FINISHED. WINNER = ' + @Winner
        RETURN
    END
    BEGIN TRY
        EXECUTE ProcessUserStep @GameID, @UserStepRow, @UserStepColumn
    END TRY
    BEGIN CATCH
        RETURN
    END CATCH
    SET @Winner = dbo.GetGameWinner(@GameID)
    IF (@Winner IS NOT NULL)
    BEGIN
        EXECUTE ShowBoard @GameID
        EXECUTE GetGameLog @GameID
        PRINT N'GAME IS FINISHED. WINNER = ' + @Winner
        RETURN
    END
    EXECUTE ProcessCompStep @GameID
    EXECUTE ShowBoard @GameID
    EXECUTE GetGameLog @GameID
    SET @Winner = dbo.GetGameWinner(@GameID)
    IF (@Winner IS NOT NULL)
        PRINT N'GAME IS FINISHED. WINNER = ' + @Winner
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