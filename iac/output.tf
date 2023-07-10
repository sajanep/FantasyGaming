output "resource_group_name" {
  value = module.common.resource_group_name
}

output "paymentsvc_messages_queue_name" {
  value = module.servicebus.paymentsvc_messages_queue_name
}

output "paymentsvc_error_message_queue_name" {
  value = module.servicebus.paymentsvc_error_message_queue_name
}

output "paymentsvc_messages_queue_reader_connection_string" {
  value = module.servicebus.paymentsvc_messages_queue_reader_connection_string
  sensitive = true
}

output "paymentsvc_messages_queue_writer_connection_string" {
  value = module.servicebus.paymentsvc_messages_queue_writer_connection_string
  sensitive = true
}

output "paymentsvc_error_messages_queue_writer_connection_string" {
  value = module.servicebus.paymentsvc_error_messages_queue_writer_connection_string
  sensitive = true
}

output "cosmosdb_connectionstrings" {
   value = module.cosmosdb.cosmosdb_connectionstrings
   sensitive   = true
}

output "gamesvc_messages_queue_name" {
  value = module.servicebus.gamesvc_messages_queue_name
}

output "gamesvc_error_message_queue_name" {
  value = module.servicebus.gamesvc_error_message_queue_name
}

output "gamesvc_messages_queue_reader_connection_string" {
  value = module.servicebus.gamesvc_messages_queue_reader_connection_string
  sensitive = true
}

output "gamesvc_messages_queue_writer_connection_string" {
  value = module.servicebus.gamesvc_messages_queue_writer_connection_string
  sensitive = true
}

output "gamesvc_error_messages_queue_writer_connection_string" {
  value = module.servicebus.gamesvc_error_messages_queue_writer_connection_string
  sensitive = true
}


output "sagareply_messages_queue_name" {
  value = module.servicebus.sagareply_messages_queue_name
}

output "sagareply_error_message_queue_name" {
  value = module.servicebus.sagareply_error_message_queue_name
}

output "sagareply_messages_queue_reader_connection_string" {
  value = module.servicebus.sagareply_messages_queue_reader_connection_string
  sensitive = true
}

output "sagareply_messages_queue_writer_connection_string" {
  value = module.servicebus.sagareply_messages_queue_writer_connection_string
  sensitive = true
}

output "sagareply_error_messages_queue_writer_connection_string" {
  value = module.servicebus.sagareply_error_messages_queue_writer_connection_string
  sensitive = true
}