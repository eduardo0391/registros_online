CREATE TABLE [dbo].[usuarios] (
    [usuario]        NCHAR (40) NOT NULL,
    [clave]          NCHAR (40) NULL,
    [fechaCreacion]  DATETIME   NULL,
    [nombre]         NCHAR (40) NULL,
    [email]          NCHAR (50) NULL,
    [fechaEnvio]     DATETIME   NULL,
    [codiActivacion] CHAR (36)  NULL,
    [id_usuario]     INT        IDENTITY (1, 1) NOT NULL,
    [confirmado]     INT        NULL,
    [IsSuperUser]    INT        NULL,
    [expirationDate] DATETIME   NULL,
    PRIMARY KEY CLUSTERED ([id_usuario] ASC)
);

