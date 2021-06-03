CREATE TABLE [dbo].[movimientos] (
    [id]             INT            IDENTITY (1, 1) NOT NULL,
    [movimiento]     NVARCHAR (40)  NULL,
    [descripcion]    NVARCHAR (500) NULL,
    [fecha]          DATE           NULL,
    [precioUnitario] REAL           NULL,
    [tipo]           INT            NOT NULL,
    [id_user]        INT            NULL,
    CONSTRAINT [clavePrimaria] PRIMARY KEY CLUSTERED ([id] ASC),
    FOREIGN KEY ([tipo]) REFERENCES [dbo].[tipo_gastos] ([id]),
    CONSTRAINT [FK_gastos_usuario] FOREIGN KEY ([id_user]) REFERENCES [dbo].[usuarios] ([id_usuario])
);

