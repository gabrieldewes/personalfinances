# .NET Core 1.0
## MVC, Web API, Authorization, MySQL, ADO.NET

## App Personal Finances

# Install & Run
* **Requires** MySQL 5.6+, .NET Core SDK 1.0 (Windows only because of binaries of MySql Data Core), a bash.
* Create the db by the ER model in the docs folder
* Run in bash:
```{r, engine='bash', count_lines}
bower install && dotnet build && dotnet bundle && dotnet run
```