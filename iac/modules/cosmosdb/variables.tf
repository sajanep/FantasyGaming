variable "collections" {
  description = "Create Tables in SQL"
  type        = map(string)
  default     = { "payment" = "id", "gameregistration" = "id", "saga" = "transactionId" }
}

variable "location" {
  description = "Target Location"
}

variable "resource_group_name" {
  description = "Resource group name"
}

variable common_tags {}

variable environment {}