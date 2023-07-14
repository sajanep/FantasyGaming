variable "collections" {
  description = "Create Tables in SQL"
  type        = map(string)
  default     = { "payment" = "userid", "gameregistration" = "userid", "saga" = "Id" }
}

variable "location" {
  description = "Target Location"
}

variable "resource_group_name" {
  description = "Resource group name"
}

variable common_tags {}

variable environment {}
