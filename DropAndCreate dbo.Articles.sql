USE [ShopDatabase]
GO

/****** Object: Table [dbo].[Articles] Script Date: 19.01.2024 13:48:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Articles];


GO
CREATE TABLE [dbo].[Articles] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [BarCode]         BIGINT         NOT NULL,
    [ProductName]     NVARCHAR (40)  NOT NULL,
    [Price]           REAL           NOT NULL,
    [FileName]        NVARCHAR (MAX) NULL,
    [CategoryId]      INT            NOT NULL
);


