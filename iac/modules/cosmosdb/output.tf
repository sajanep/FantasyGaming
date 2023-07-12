output "cosmosdb_primary_connectionstring" {
   value = azurerm_cosmosdb_account.fantasygaming-db-account.connection_strings
}