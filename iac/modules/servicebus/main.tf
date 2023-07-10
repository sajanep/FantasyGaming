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
  lock_duration         = "PT30S"
  max_size_in_megabytes = 1024
  enable_partitioning   = true
}

# Queue for messages that had errors
resource "azurerm_servicebus_queue" "paymentSvcMessagesErrorQueue" {
  name = var.paymentsvc_messages_error_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  max_size_in_megabytes                = 1024
  enable_partitioning                  = true
  default_message_ttl                  = lookup(var.message_expiration_time, var.environment)
  dead_lettering_on_message_expiration = true
  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.paymentSvcMessagesQueue.name
}

# Credentials for App1 message generator
resource "azurerm_servicebus_queue_authorization_rule" "paymentSvcMsgQueueWriter" {
  name     = "paymentSvcMsgQueueWriter"
  queue_id = azurerm_servicebus_queue.paymentSvcMessagesQueue.id
  listen = false
  send   = true
  manage = false
}

# Credentials for Azure Functions consumer
resource "azurerm_servicebus_queue_authorization_rule" "paymentSvcMsgQueueReader" {
  name     = "paymentSvcMsgQueueReader"
  queue_id = azurerm_servicebus_queue.paymentSvcMessagesQueue.id
  listen = true
  send   = false
  manage = false
}

# Credentials for App1 message generator
resource "azurerm_servicebus_queue_authorization_rule" "paymentSvcErrorMessagesWriter" {
  name     = "paymentSvcErrorMessagesWriter"
  queue_id = azurerm_servicebus_queue.paymentSvcMessagesErrorQueue.id
  listen = false
  send   = true
  manage = false
}

# Queue for regular messages
resource "azurerm_servicebus_queue" "gameSvcMessagesQueue" {
  name = var.gamesvc_messages_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  lock_duration         = "PT30S"
  max_size_in_megabytes = 1024
  enable_partitioning   = true
}

# Queue for messages that had errors
resource "azurerm_servicebus_queue" "gameSvcMessagesErrorQueue" {
  name = var.gamesvc_messages_error_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  max_size_in_megabytes                = 1024
  enable_partitioning                  = true
  default_message_ttl                  = lookup(var.message_expiration_time, var.environment)
  dead_lettering_on_message_expiration = true
  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.gameSvcMessagesQueue.name
}

# Credentials for App1 message generator
resource "azurerm_servicebus_queue_authorization_rule" "gameSvcMsgQueueWriter" {
  name     = "gameSvcMsgQueueWriter"
  queue_id = azurerm_servicebus_queue.gameSvcMessagesQueue.id
  listen = false
  send   = true
  manage = false
}

# Credentials for Azure Functions consumer
resource "azurerm_servicebus_queue_authorization_rule" "gameSvcMsgQueueReader" {
  name     = "gameSvcMsgQueueReader"
  queue_id = azurerm_servicebus_queue.gameSvcMessagesQueue.id
  listen = true
  send   = false
  manage = false
}

# Credentials for App1 message generator
resource "azurerm_servicebus_queue_authorization_rule" "gameSvcErrorMessagesWriter" {
  name     = "gameSvcErrorMessagesWriter"
  queue_id = azurerm_servicebus_queue.gameSvcMessagesErrorQueue.id
  listen = false
  send   = true
  manage = false
}

# Queue for regular messages
resource "azurerm_servicebus_queue" "sagaReplyMessagesQueue" {
  name = var.sagareply_messages_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  lock_duration         = "PT30S"
  max_size_in_megabytes = 1024
  enable_partitioning   = true
}

# Queue for messages that had errors
resource "azurerm_servicebus_queue" "sagaReplyMessagesErrorQueue" {
  name = var.sagareply_messages_error_queue_name
  namespace_id = azurerm_servicebus_namespace.busNamespace.id
  max_size_in_megabytes                = 1024
  enable_partitioning                  = true
  default_message_ttl                  = lookup(var.message_expiration_time, var.environment)
  dead_lettering_on_message_expiration = true
  forward_dead_lettered_messages_to    = azurerm_servicebus_queue.sagaReplyMessagesQueue.name
}

# Credentials for App1 message generator
resource "azurerm_servicebus_queue_authorization_rule" "sagaReplyMsgQueueWriter" {
  name     = "sagaReplyMsgQueueWriter"
  queue_id = azurerm_servicebus_queue.sagaReplyMessagesQueue.id
  listen = false
  send   = true
  manage = false
}

# Credentials for Azure Functions consumer
resource "azurerm_servicebus_queue_authorization_rule" "sagaReplyMsgQueueReader" {
  name     = "sagaReplyMsgQueueReader"
  queue_id = azurerm_servicebus_queue.sagaReplyMessagesQueue.id
  listen = true
  send   = false
  manage = false
}

# Credentials for App1 message generator
resource "azurerm_servicebus_queue_authorization_rule" "sagaReplyErrorMessagesWriter" {
  name     = "sagaReplyErrorMessagesWriter"
  queue_id = azurerm_servicebus_queue.sagaReplyMessagesErrorQueue.id
  listen = false
  send   = true
  manage = false
}