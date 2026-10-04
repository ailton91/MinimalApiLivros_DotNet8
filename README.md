Olá meu nome é Ailton Xavier, sou desenvolvedor .Net C# / Angular 10+, e estou criando esse projeto para mostrar os conhecimentos adquiridos.

Email: ailton.xavier.junior@gmail.com

Telefone: (21) 97999-9817 | (21) 96689-4472

Linkedin: https://www.linkedin.com/in/ailton-xavier-da-silva-junior-2722bb86/

Repositórios: https://github.com/ailton91

CI/CD: Github

Azure Devops (Nativo): https://minimalapilivros-a4gcdxbegmecdmg4.centralus-01.azurewebsites.net/swagger/index.html

Oracle Cloud (Linux): http://132.145.205.35:5000/swagger/index.html

Nesse projeto eu vou usar as tecnologias:

• .NET 8

• Entity Framework Core 8 - Migration

• MySQL / InMemory

• Clean Architecture

• CQRS

• FluentValidation

• DDD

• SOLID

• Assíncrono - Async Task<>

• Serilog ( LogsErro salvos em arquivos.txt e banco de dados )

• Minimal API
	Muito usado em microserviços de pequenos e médios portes, deixando o código mais limpo.

• Bearer Token

• Azure Devops / Oracle Cloud

##############################
-- Instruções -- 
##############################

 1º Etapa
 
 Precisa instalar o banco de dados MySql 8.0.46
 
 https://dev.mysql.com/downloads/installer/
 https://dev.mysql.com/downloads/file/?id=552804
 
 ----------------------------------------------------------------------------------------------------------------------------------------------------------
 
 2º Etapa
 
 Download de um gerenciado de banco de dados MySql, estou usando o DBeaver 26.2.0 Community
 
 https://dbeaver.io/download/

 ----------------------------------------------------------------------------------------------------------------------------------------------------------

 3º Etapa

 Vai ser preciso rodar migration em seu computador. Executar o comando abaixo.

 Executar => dotnet ef migrations add inicial_DbLivros --project ..\MinimalApiLivros.Infrastructure --startup-project ..\MinimalApiLivros.API

 Logo em seguida executar o próximo comando abaixo.

 Executar => dotnet ef database update 0 --project ..\MinimalApiLivros.Infrastructure --startup-project ..\MinimalApiLivros


