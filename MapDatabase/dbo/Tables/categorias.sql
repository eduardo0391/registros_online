CREATE TABLE [dbo].[categorias] (
    [id]          INT          IDENTITY (1, 1) NOT NULL,
    [id_user]     INT          NOT NULL,
    [id_tipo]     INT          NOT NULL,
    [descripcion] VARCHAR (30) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([id_tipo]) REFERENCES [dbo].[tipo_gastos] ([id]),
    FOREIGN KEY ([id_user]) REFERENCES [dbo].[usuarios] ([id_usuario])
);

