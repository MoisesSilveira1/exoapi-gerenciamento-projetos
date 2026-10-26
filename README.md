# ExoApi - API de gerenciamento de projetos

API REST em **ASP.NET Core (.NET 10)** com **SQL Server**, desenvolvida para a empresa ficticia ExoApi armazenar e gerenciar seus projetos (titulo, status, data de inicio e tecnologias).

Projeto da unidade curricular **Desenvolvimento de APIs** (SENAI) - Aluno: Moisés Silveira.

## Recursos

| Metodo | Rota | Descricao | Acesso |
|--------|------|-----------|--------|
| GET | `/api/projetos` | Lista todos os projetos | Publico |
| GET | `/api/projetos/{id}` | Busca um projeto | Publico |
| POST | `/api/projetos` | Cria um projeto | Publico |
| PUT | `/api/projetos/{id}` | Atualiza todas as informacoes | Publico |
| PATCH | `/api/projetos/{id}` | Atualiza apenas os campos enviados | Publico |
| DELETE | `/api/projetos/{id}` | Exclui um projeto | **Somente autenticado (JWT)** |
| POST | `/api/auth/login` | Gera o token JWT | Publico |

Codigos de status: 200 OK, 201 Created, 204 No Content, 400 Bad Request (validacao), 401 Unauthorized, 404 Not Found, 500/503 erros internos. Os erros seguem o formato padronizado ProblemDetails (JSON), tratados por um middleware global.

## Estrutura em camadas

```
Controllers/    -> recebem as requisicoes HTTP
Services/       -> regras de negocio e autenticacao
Repositories/   -> acesso ao SQL Server (Dapper)
Models/         -> entidades e DTOs com validacao
Middleware/     -> tratamento global de excecoes
Exceptions/     -> excecoes de dominio
Database/db.sql -> script de criacao do banco
```

## Como executar

1. Instale o .NET SDK 10, o SQL Server e o SSMS.
2. Execute `Database/db.sql` no SSMS (cria o banco ExoApi, as tabelas e dados de exemplo).
3. Ajuste a connection string em `appsettings.json` se necessario.
4. No terminal, dentro da pasta do projeto:

```
dotnet run
```

5. Acesse a documentacao Swagger em `http://localhost:5000/swagger` (a porta aparece no terminal).

## Autenticacao (DELETE)

1. Faca `POST /api/auth/login` com `{"login": "admin", "senha": "Admin@123"}` (usuario criado automaticamente na primeira execucao).
2. Copie o `token` retornado.
3. No Swagger, clique em **Authorize** e informe o token; ou envie o cabecalho `Authorization: Bearer <token>`.
4. Sem token, `DELETE /api/projetos/{id}` retorna 401.

## Exemplo de projeto (JSON)

```json
{
  "titulo": "Aplicativo Mobile ExoApp",
  "status": "Em andamento",
  "dataInicio": "2026-09-01",
  "tecnologias": "C#, ASP.NET Core, SQL Server"
}
```
