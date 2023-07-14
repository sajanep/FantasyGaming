
variable "location" {
  description = "Target Location"
}

variable "resource_group_name" {
  description = "Resource group name"
}

variable app_service_plan_name{
  default = "functionapp_consumption_plan"
}

variable common_tags {}

variable environment {}

variable storage_account_name{}

variable cosmosdb_account_endpoint {}

variable cosmosdb_account_primarykey {}

variable cosmosdb_databasename {}

variable payment_collection {}

variable game_collection {}

variable orchestration_collection {}

variable servicebus_connection {}
 
variable paymentsvc_messagequeue{}

variable gamesvc_messagequeue{}

variable sagareply_messagequeue {}