# General purpose parameters
# variable "client_secret" {
#   type        = string
#   description = "The password for this user"
# }

# variable "subscription_id" {
#   type        = string
#   description = "The subscription to which the configurations must be applied"
# }

# variable "client_id" {
#   type        = string
#   description = "The user name"
# }

# variable "tenant_id" {
#   type        = string
#   description = "The identification for Microsoft AD"
# }

variable "environment" {
  type        = string
  description = "Target Environment"
  default     = "dev"
}

variable "location" {
  description = "Target Location"
  default     = "southindia"
}

variable "resource_group_name" {
  description = "Resource group name"
  default     = "fantasygaming"
}

# Variables for tags
variable "billing_code_tag" {
  type        = string
  default     = "fantasygaming"
  description = "The billing code to add as tag for resources"
}

variable "services_bus_namespace_name" {
  type        = string
  description = "Name for the queue namespace"
  default = "fantasygaming-bus"
}

variable "paymentsvc_messages_queue_name" {
  type        = string
  description = "Messages queue name"
  default = "paymentsvc-queue"
}

variable "paymentsvc_messages_error_queue_name" {
  type        = string
  description = "Error queue name"
  default = "paymentsvc-error-queue"
}

variable "gamesvc_messages_queue_name" {
  type        = string
  description = "Messages queue name"
  default = "gamesvc-queue"
}

variable "gamesvc_messages_error_queue_name" {
  type        = string
  description = "Error queue name"
  default = "gamesvc-error-queue"
}

variable "sagareply_messages_queue_name" {
  type        = string
  description = "Messages queue name"
  default = "sagareply-queue"
}

variable "sagareply_messages_error_queue_name" {
  type        = string
  description = "Error queue name"
  default = "sagareply-error-queue"
}

variable storage_account_name { 
  type = string
  description = "Name of the storage account"
  default = "fantasygamingstorage"
}