# Sistema de Gestão de Consultas UVV

Sistema web desenvolvido em ASP.NET Core MVC com Entity Framework Core e SQL Server (LocalDB) para gerenciamento de consultas médicas.

---

## Vídeo Demonstrativo

* **Link do video:** [Clique aqui para assistir ao vídeo](https://streamable.com/iovy0c)

> O vídeo demonstra o fluxo completo da aplicação: cadastro de novo usuário, autenticação/login, listagem, criação, edição e exclusão de consultas médicas, além da navegação pela interface Swagger.

---

## Configuração e Execução do Projeto

### Pré-requisitos
* Visual Studio 2022 (com pacote ASP.NET e Web) ou .NET SDK
* SQL Server Express / LocalDB

### 1. Configurar a Conexão
Verifique a string de conexão no arquivo `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GestaoConsultasUVV;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
