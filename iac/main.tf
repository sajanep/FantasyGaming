
# 1. Terraform Settings Block
terraform {
  # 1. Required Version Terraform
  required_version = ">= 0.13"
  # 2. Required Terraform Providers  
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.0"
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

module "cosmosdb"{
  location    = var.location
  environment = var.environment

  # Output module common
  resource_group_name         = module.common.resource_group_name
  common_tags                 = module.common.common_tags

  source     = "./modules/cosmosdb"
  depends_on = [module.common]

}


