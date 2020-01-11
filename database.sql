CREATE DATABASE [LibraryWPF];
GO
USE [LibraryWPF];
GO

CREATE TABLE [Books] (
    id      BIGINT IDENTITY(1, 1) PRIMARY KEY,
    title   NVARCHAR(128) NOT NULL,
    author  NVARCHAR(64),
    isbn    VARCHAR(128) UNIQUE
);

CREATE TABLE [Students] (
    [id]          BIGINT IDENTITY(1, 1) PRIMARY KEY,
    [name]        NVARCHAR(32) NOT NULL,
    [surname]     NVARCHAR(32) NOT NULL,
    [albumNumber] BIGINT NOT NULL UNIQUE
);
GO;

CREATE TABLE [Rents] (
    [id]          BIGINT IDENTITY(1, 1),
    [startDate]   SMALLDATETIME DEFAULT GETDATE(),
    [endDate]     SMALLDATETIME,
    [bookId]      BIGINT NOT NULL,
    [studentId]   BIGINT NOT NULL,

    FOREIGN KEY (bookId) REFERENCES Books(id),
    FOREIGN KEY (studentId) REFERENCES Students(id)
);
