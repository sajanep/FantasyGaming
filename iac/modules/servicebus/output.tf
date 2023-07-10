output paymentsvc_messages_queue_reader_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.paymentSvcMsgQueueReader.primary_connection_string
}

output paymentsvc_messages_queue_writer_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.paymentSvcMsgQueueWriter.primary_connection_string
}

output paymentsvc_error_messages_queue_writer_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.paymentSvcErrorMessagesWriter.primary_connection_string
}

output paymentsvc_messages_queue_name {
  value = azurerm_servicebus_queue.paymentSvcMessagesQueue.name
}

output paymentsvc_error_message_queue_name {
  value = azurerm_servicebus_queue.paymentSvcMessagesErrorQueue.name
}


output gamesvc_messages_queue_reader_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.gameSvcMsgQueueReader.primary_connection_string
}

output gamesvc_messages_queue_writer_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.gameSvcMsgQueueWriter.primary_connection_string
}

output gamesvc_error_messages_queue_writer_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.gameSvcErrorMessagesWriter.primary_connection_string
}

output gamesvc_messages_queue_name {
  value = azurerm_servicebus_queue.gameSvcMessagesQueue.name
}

output gamesvc_error_message_queue_name {
  value = azurerm_servicebus_queue.gameSvcMessagesErrorQueue.name
}


output sagareply_messages_queue_reader_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.sagaReplyMsgQueueReader.primary_connection_string
}

output sagareply_messages_queue_writer_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.sagaReplyMsgQueueWriter.primary_connection_string
}

output sagareply_error_messages_queue_writer_connection_string {
  value = azurerm_servicebus_queue_authorization_rule.sagaReplyErrorMessagesWriter.primary_connection_string
}

output sagareply_messages_queue_name {
  value = azurerm_servicebus_queue.sagaReplyMessagesQueue.name
}

output sagareply_error_message_queue_name {
  value = azurerm_servicebus_queue.sagaReplyMessagesErrorQueue.name
}
