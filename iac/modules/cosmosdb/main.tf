resource "random_integer" "sagalogic-ri" {
  min = 10000
  max = 99999
}

resource "azurerm_cosmosdb_account" "fantasygaming-db-account" {
  name                = "${var.environment}-cosmos-fantasygaming-db-${random_integer.sagalogic-ri.result}"
  resource_group_name = var.resource_group_name
  location            = var.location
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }

  enable_automatic_failover = false
 
  geo_location {
    location          = var.location
    failover_priority = 0
  }

  tags = var.common_tags
}

resource "azurerm_cosmosdb_sql_database" "fantasygaming-sql-database" {
  name                = "${var.environment}-cosmos-sql-db"
  resource_group_name = azurerm_cosmosdb_account.fantasygaming-db-account.resource_group_name
  account_name        = azurerm_cosmosdb_account.fantasygaming-db-account.name
  throughput          = 500
}

resource "azurerm_cosmosdb_sql_container" "fantasygaming-sql-container" {
  for_each            = tomap(var.collections)
  name                = each.key
  resource_group_name = azurerm_cosmosdb_account.fantasygaming-db-account.resource_group_name
  account_name        = azurerm_cosmosdb_account.fantasygaming-db-account.name
  database_name       = azurerm_cosmosdb_sql_database.fantasygaming-sql-database.name
  partition_key_path  = "/${each.value}"
}
