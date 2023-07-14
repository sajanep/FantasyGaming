locals {
  azure_function_app = "Fantasy-Gaming-App"
}

resource "azurerm_windows_function_app" "fantasygaming-function" {
  name                       = "${var.environment}-${local.azure_function_app}"
  resource_group_name        = var.resource_group_name
  location                   = var.location
  service_plan_id            = azurerm_service_plan.primary.id
  storage_account_name       = azurerm_storage_account.fantasygaming-storage-account.name
  storage_account_access_key = azurerm_storage_account.fantasygaming-storage-account.primary_access_key
  functions_extension_version = "~4"

  app_settings = {
    "CosmosDbConnectionString" = "AccountEndpoint=${var.cosmosdb_account_endpoint};AccountKey=${var.cosmosdb_account_primarykey};",
    "CosmosDbDatabaseName"     = var.cosmosdb_databasename,
    "PaymentCollection"        = var.payment_collection,
    "GameCollection"           = var.game_collection,
    "OrchestratorCollection"   = var.orchestration_collection,

    "ServiceBusConnection" : var.servicebus_connection,
    "PaymentSvcMessageQueue" : var.paymentsvc_messagequeue
    "GameSvcMessageQueue" : var.gamesvc_messagequeue,
    "SagaReplyMessageQueue" : var.sagareply_messagequeue

    "FUNCTIONS_WORKER_RUNTIME"       = "dotnet",
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.fantasygaming-application-insights.instrumentation_key,
    "WEBSITE_RUN_FROM_PACKAGE"       = 1
  }

  site_config {
    application_stack {
      dotnet_version = "v6.0"
    }
  }

  tags = {
    environment = var.environment
  }
}

# Create Azure App Service Plan using Consumption pricing
# resource azurerm_app_service_plan "primary" {
#   name                = var.app_service_plan_name
#   location            = var.location
#   resource_group_name = var.resource_group_name
#   kind                = "Linux"
#   reserved            = true

#   sku {
#     tier = "Dynamic"
#     size = "Y1"
#   }
# }

resource "azurerm_service_plan" "primary" {
  name                = var.app_service_plan_name
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_log_analytics_workspace" "fantasygaming-log-workspace" {
  name                = "fantasygaming-log-workspace"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "fantasygaming-application-insights" {
  name                = "${var.environment}-app-insights"
  location            = var.location
  resource_group_name = var.resource_group_name
  workspace_id        = azurerm_log_analytics_workspace.fantasygaming-log-workspace.id
  application_type    = "web"
}

resource "azurerm_storage_account" "fantasygaming-storage-account" {
  name                     = "${var.environment}${var.storage_account_name}"
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = {
    environment = var.environment
  }
}
