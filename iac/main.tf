
# 1. Terraform Settings Block
terraform {
  # 1. Required Version Terraform
  required_version = ">= 0.13"
  # 2. Required Terraform Providers  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }

  }

  # Terraform State Storage to Azure Storage Container
  # backend "azurerm" {
  #   resource_group_name = "fantasygaming-tf-rg"
  #   storage_account_name = "terraformstate-fgaming"
  #   container_name       = "tfstatefiles"
  #   key                  = "terraform.tfstate"
  # }

}

# 2. Terraform Provider Block for AzureRM
provider "azurerm" {
  features {

  }
}

# Common group
module "common" {
  resource_group_name = var.resource_group_name
  location            = var.location
  environment         = var.environment
  billing_code_tag    = var.billing_code_tag

  source = "./modules/common"
}

# Queue group
module "servicebus" {
  location    = var.location
  environment = var.environment

  # Output module common
  resource_group_name         = module.common.resource_group_name
  common_tags                 = module.common.common_tags
  services_bus_namespace_name = var.services_bus_namespace_name
  paymentsvc_messages_queue_name = var.paymentsvc_messages_queue_name
  paymentsvc_messages_error_queue_name   = var.paymentsvc_messages_error_queue_name
  gamesvc_messages_queue_name = var.gamesvc_messages_queue_name
  gamesvc_messages_error_queue_name = var.gamesvc_messages_error_queue_name
  sagareply_messages_queue_name = var.sagareply_messages_queue_name
  sagareply_messages_error_queue_name = var.sagareply_messages_error_queue_name
  source     = "./modules/servicebus"
  depends_on = [module.common]
}

module "cosmosdb" {
  location    = var.location
  environment = var.environment

  # Output module common
  resource_group_name         = module.common.resource_group_name
  common_tags                 = module.common.common_tags

  source     = "./modules/cosmosdb"
  depends_on = [module.common]
}

module "functionapp" {
  location    = var.location
  environment = var.environment
  storage_account_name = var.storage_account_name

  # Output module common
  resource_group_name         = module.common.resource_group_name
  common_tags                 = module.common.common_tags

  cosmosdb_account_endpoint = module.cosmosdb.cosmosdb_account_endpoint
  cosmosdb_account_primarykey = module.cosmosdb.cosmosdb_account_primarykey
  cosmosdb_databasename = module.cosmosdb.cosmosdb_databasename
  payment_collection  = module.cosmosdb.payment_collection
  game_collection  = module.cosmosdb.game_collection
  orchestration_collection = module.cosmosdb.orchestration_collection
  servicebus_connection = module.servicebus.servicebus_reader_writer_connection_string
  paymentsvc_messagequeue = module.servicebus.paymentsvc_messages_queue_name
  gamesvc_messagequeue = module.servicebus.gamesvc_messages_queue_name
  sagareply_messagequeue = module.servicebus.sagareply_messages_queue_name

  source     = "./modules/functionapp"
  depends_on = [module.common, module.cosmosdb, module.servicebus]
}


