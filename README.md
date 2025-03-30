requirements:
nodejs
npm
dotnet core 9.0
sqlserver running with server name MSSQLLocalDB

prerequisites:
adjust the connection string to sqlserver in appsettings.json (server name)



migration:
dotnet ef migrations add InitialCreate
dotnet ef database update


Steps to run:

docker-compose up --build


npm run build
npm run dev