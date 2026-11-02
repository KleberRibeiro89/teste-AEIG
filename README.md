# teste-AEIG

API do teste prático da AEIG. O backend está em `apps/backend`.

## Tecnologias

- **.NET 10** e **ASP.NET Core**, em C#, para a API HTTP.
- **Entity Framework Core** para mapear as entidades e aplicar as migrations.
- **SQLite** como banco de dados.
- **JWT** para autenticação dos endpoints protegidos.
- **FluentValidation** para as regras de validação.
- **AutoMapper** para converter requisição e entidade em resposta.
- **Swagger** para consultar e testar a API no navegador.

É preciso ter o [.NET SDK 10](https://dotnet.microsoft.com/download) instalado. Não é necessário instalar servidor de banco.

## Banco de dados

O banco é **SQLite** para o recrutador ou avaliador conseguir rodar o projeto sem configurar PostgreSQL, SQL Server ou outro serviço. O arquivo `aeig.db` é criado em `apps/backend/AEIG.Api/Database` na primeira execução em Development, e a migration da entidade Usuário é aplicada nesse momento.

Na mesma subida, o seed cria dois usuários, os dois com a senha `123456`:

- **Kleber** — `kleber.ribeiro89@gmail.com`
- **Admin** — `admin@aeig.com.br`

## Como rodar

```powershell
dotnet run --project apps/backend/AEIG.Api
```

A API sobe em `http://localhost:5271` e o Swagger em `http://localhost:5271/swagger`.

## Worker

O projeto `apps/backend/AEIG.Api.Worker` varre as cobranças com status **Pendente**. Se a data de vencimento já passou, o status muda para **Em Atraso**. Outra varredura confere as cobranças que já têm pagamento (`DataPagamento` ou valor pago) e muda o status para **Pago**, ou para **Pago com Juros** quando o pagamento foi depois do vencimento. As duas rodam ao subir o worker e depois a cada 60 minutos. Ele usa o mesmo banco da API.

```powershell
dotnet run --project apps/backend/AEIG.Api.Worker
```
