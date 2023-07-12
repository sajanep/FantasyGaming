# output paymentsvc_messages_queue_reader_writer_connection_string {
#   value = azurerm_servicebus_queue_authorization_rule.paymentSvcMsgQueueReaderWriter.primary_connection_string
# }

# output paymentsvc_error_messages_queue_writer_connection_string {
#   value = azurerm_servicebus_queue_authorization_rule.paymentSvcErrorMessagesReaderWriter.primary_connection_string
# }

output paymentsvc_messages_queue_name {
  value = azurerm_servicebus_queue.paymentSvcMessagesQueue.name
}

output paymentsvc_error_message_queue_name {
  value = azurerm_servicebus_queue.paymentSvcMessagesErrorQueue.name
}

# output gamesvc_messages_queue_reader_writer_connection_string {
#   value = azurerm_servicebus_queue_authorization_rule.gameSvcMsgQueueReaderWriter.primary_connection_string
# }

# output gamesvc_error_messages_queue_reader_writer_connection_string {
#   value = azurerm_servicebus_queue_authorization_rule.gameSvcErrorMessagesReaderWriter.primary_connection_string
# }

output gamesvc_messages_queue_name {
  value = azurerm_servicebus_queue.gameSvcMessagesQueue.name
}

output gamesvc_error_message_queue_name {
  value = azurerm_servicebus_queue.gameSvcMessagesErrorQueue.name
}

# output sagareply_messages_queue_reader_writer_connection_string {
#   value = azurerm_servicebus_queue_authorization_rule.sagaReplyMsgQueueReaderWriter.primary_connection_string
# }

# output sagareply_error_messages_queue_reader_writer_connection_string {
#   value = azurerm_servicebus_queue_authorization_rule.sagaReplyErrorMessagesReaderWriter.primary_connection_string
# }

output sagareply_messages_queue_name {
  value = azurerm_servicebus_queue.sagaReplyMessagesQueue.name
}

output sagareply_error_message_queue_name {
  value = azurerm_servicebus_queue.sagaReplyMessagesErrorQueue.name
}

output servicebus_reader_writer_connection_string {
  value = azurerm_servicebus_namespace_authorization_rule.servicebusReaderWriter.primary_connection_string
}
