# Creates the queue
resource "azurerm_servicebus_namespace" "busNamespace" {
  name                = var.services_bus_namespace_name
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "Basic"
  tags = var.common_tags
}

# Queue for regular messages
resource "azurerm_servicebus_queue" "paymentSvcMessagesQueue" {
  name = var.paymentsvc_messages_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  lock_duration         = "PT3M"
  max_delivery_count = 1
  max_size_in_megabytes = 1024
  enable_partitioning   = true
  dead_lettering_on_message_expiration = true
  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.paymentSvcMessagesErrorQueue.name
}

# Queue for messages that had errors
resource "azurerm_servicebus_queue" "paymentSvcMessagesErrorQueue" {
  name = var.paymentsvc_messages_error_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  max_size_in_megabytes                = 1024
  enable_partitioning                  = true
  default_message_ttl                  = lookup(var.message_expiration_time, var.environment)
}

resource "azurerm_servicebus_namespace_authorization_rule" "servicebusReaderWriter" {
  name         = "servicebusReaderWriter"
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  listen = true
  send   = true
  manage = false
}

# Queue for regular messages
resource "azurerm_servicebus_queue" "gameSvcMessagesQueue" {
  name = var.gamesvc_messages_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  lock_duration         = "PT3M"
  max_size_in_megabytes = 1024
  max_delivery_count = 1
  enable_partitioning   = true
  dead_lettering_on_message_expiration = true
  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.gameSvcMessagesErrorQueue.name
}

# Queue for messages that had errors
resource "azurerm_servicebus_queue" "gameSvcMessagesErrorQueue" {
  name = var.gamesvc_messages_error_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  max_size_in_megabytes                = 1024
  enable_partitioning                  = true
  default_message_ttl                  = lookup(var.message_expiration_time, var.environment)
}

# Queue for regular messages
resource "azurerm_servicebus_queue" "sagaReplyMessagesQueue" {
  name = var.sagareply_messages_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  lock_duration         = "PT3M"
  max_delivery_count = 1
  max_size_in_megabytes = 1024
  enable_partitioning   = true
  dead_lettering_on_message_expiration = true
  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.sagaReplyMessagesErrorQueue.name
}

# Queue for messages that had errors
resource "azurerm_servicebus_queue" "sagaReplyMessagesErrorQueue" {
  name = var.sagareply_messages_error_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  max_size_in_megabytes                = 1024
  enable_partitioning                  = true
  default_message_ttl                  = lookup(var.message_expiration_time, var.environment)
}
