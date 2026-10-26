-- =====================================================================
-- ExoApi - Estrutura do banco de dados (SQL Server)
-- Execute no SSMS antes de rodar a API.
-- =====================================================================

CREATE DATABASE ExoApi;
GO

USE ExoApi;
GO

-- Projetos realizados pela empresa (titulo, status, data de inicio e tecnologias)
CREATE TABLE Projetos
(
     Id          INT PRIMARY KEY IDENTITY
    ,Titulo      VARCHAR(120) NOT NULL
    ,Status      VARCHAR(30)  NOT NULL
    ,DataInicio  DATE         NOT NULL
    ,Tecnologias VARCHAR(255) NOT NULL
);
GO

-- Usuarios que podem se autenticar na API (senha armazenada como hash PBKDF2)
CREATE TABLE Usuarios
(
     Id        INT PRIMARY KEY IDENTITY
    ,Login     VARCHAR(50)  NOT NULL UNIQUE
    ,SenhaHash VARCHAR(255) NOT NULL
);
GO

-- Dados de exemplo
INSERT INTO Projetos (Titulo, Status, DataInicio, Tecnologias) VALUES
 ('Aplicativo Mobile ExoApp', 'Em andamento', '2026-09-01', 'C#, ASP.NET Core, SQL Server'),
 ('Portal de Integracao de Pedidos', 'Planejado', '2026-11-15', 'Node.js, REST, JSON'),
 ('Painel de Indicadores Internos', 'Concluido', '2026-03-10', 'Angular, TypeScript');
GO

SELECT * FROM Projetos;
GO
-- O usuario "admin" (senha Admin@123) e criado automaticamente pela API na primeira execucao.
