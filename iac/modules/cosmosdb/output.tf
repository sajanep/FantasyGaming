output "cosmosdb_primary_connectionstring" {
  value = azurerm_cosmosdb_account.fantasygaming-db-account.connection_strings
}

output "cosmosdb_account_endpoint" {
  value = azurerm_cosmosdb_account.fantasygaming-db-account.endpoint
}

output "cosmosdb_account_primarykey" {
  value = azurerm_cosmosdb_account.fantasygaming-db-account.primary_key
}

output "cosmosdb_databasename" {
  value = azurerm_cosmosdb_sql_database.fantasygaming-sql-database.name
}

output "payment_collection" {
  value = azurerm_cosmosdb_sql_container.fantasygaming-sql-container["payment"].name
}

output "game_collection" {
  value = azurerm_cosmosdb_sql_container.fantasygaming-sql-container["gameregistration"].name
}

output "orchestration_collection" {
  value = azurerm_cosmosdb_sql_container.fantasygaming-sql-container["saga"].name
}  
