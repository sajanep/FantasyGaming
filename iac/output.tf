output "resource_group_name" {
  value = module.common.resource_group_name
}

output "cosmosdb_primary_connectionstring" {
   value = module.cosmosdb.cosmosdb_primary_connectionstring
   sensitive   = true
}

output "paymentsvc_messages_queue_name" {
  value = module.servicebus.paymentsvc_messages_queue_name
}

output "paymentsvc_error_message_queue_name" {
  value = module.servicebus.paymentsvc_error_message_queue_name
}

# output "paymentsvc_messages_queue_reader_writer_connection_string" {
#   value = module.servicebus.paymentsvc_messages_queue_reader_writer_connection_string
#   # sensitive = true
# }

# output "paymentsvc_error_messages_queue_writer_connection_string" {
#   value = module.servicebus.paymentsvc_error_messages_queue_writer_connection_string
#   # sensitive = true
# }

output "gamesvc_messages_queue_name" {
  value = module.servicebus.gamesvc_messages_queue_name
}

output "gamesvc_error_message_queue_name" {
  value = module.servicebus.gamesvc_error_message_queue_name
}

# output "gamesvc_messages_queue_reader_writer_connection_string" {
#   value = module.servicebus.gamesvc_messages_queue_reader_writer_connection_string
#   # sensitive = true
# }

# output "gamesvc_error_messages_queue_reader_writer_connection_string" {
#   value = module.servicebus.gamesvc_error_messages_queue_reader_writer_connection_string
#   # sensitive = true
# }

output "sagareply_messages_queue_name" {
  value = module.servicebus.sagareply_messages_queue_name
}

output "sagareply_error_message_queue_name" {
  value = module.servicebus.sagareply_error_message_queue_name
}

# output "sagareply_messages_queue_reader_writer_connection_string" {
#   value = module.servicebus.sagareply_messages_queue_reader_writer_connection_string
#   # sensitive = true
# }

# output "sagareply_error_messages_queue_reader_writer_connection_string" {
#   value = module.servicebus.sagareply_error_messages_queue_reader_writer_connection_string
#   # sensitive = true
# }

output "servicebus_reader_writer_connection_string" {
  value = module.servicebus.servicebus_reader_writer_connection_string
  sensitive = true
}

output "instrumentation_key" {
  value = module.functionapp.instrumentation_key
  sensitive = true
}

output "app_id" {
  value = module.functionapp.app_id
}