#### Migration Scripts

```bash
dotnet ef migrations add InitialCatalogMigration -o Shared\Data\Migrations\Catalogs -c CatalogsDbContext
dotnet ef database update -c CatalogsDbContext
```
