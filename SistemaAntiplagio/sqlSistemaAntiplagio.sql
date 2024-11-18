CREATE DATABASE SistemaAntiplagio;

USE SistemaAntiplagio;

CREATE TABLE Usuario (
    Id INT PRIMARY KEY IDENTITY,
    Nome NVARCHAR(250),
    Email NVARCHAR(250) UNIQUE,
    Senha NVARCHAR(MAX),
    DataCadastro DATETIME DEFAULT GETDATE(),
    [Status] BIT
);

CREATE TABLE Atividade (
    Id INT PRIMARY KEY IDENTITY,
    [Guid] UNIQUEIDENTIFIER NOT NULL,
    UsuarioId INT,
    Nome NVARCHAR(250),
    Descricao NVARCHAR(MAX),
    DataCriacao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
);

CREATE TABLE Arquivo (
    Id INT PRIMARY KEY IDENTITY,
    AtividadeId INT,
    [Guid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- Gera o GUID automaticamente
    NomeOriginal NVARCHAR(250),
    CaminhoArquivo NVARCHAR(500),
    DataUpload DATETIME DEFAULT GETDATE(),
    UsuarioId INT,
    FOREIGN KEY (AtividadeId) REFERENCES Atividade(Id),
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
);

CREATE TABLE Resultado (
    Id INT PRIMARY KEY IDENTITY,
    AtividadeId INT,
    Arquivo1Id INT,
    Arquivo2Id INT,
    Relatorio NVARCHAR(MAX),
    PercentualSimilaridade DECIMAL(5, 2),
    DataAnalise DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (AtividadeId) REFERENCES Atividade(Id),
    FOREIGN KEY (Arquivo1Id) REFERENCES Arquivo(Id),
    FOREIGN KEY (Arquivo2Id) REFERENCES Arquivo(Id)
);
