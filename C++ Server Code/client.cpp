/* This class represents a client who is connected to the server.
 * This class contains the bufer which client uses to write to server
 * along with methods that deals with client requests. 
 * 
 * Each client contains a socket and a buffer that is specifically used for that client.
 */

#include "client.h"

// Constructor
client::client(boost::asio::io_service & input, server * s):sock(input),
							    serv(s) 
{
}

// Destructor
client::~client()
{
}

// Method that is called once a server sees a client is connected
// This method is also used when client provides a badlogin
void client::connected()
{	
  // Begin receiving. Call the process login method once login has been received
  boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_login,  this, 
								  boost::asio::placeholders::error,
								  boost::asio::placeholders::bytes_transferred));
  return;
}

// Method that is called once the client sends us the login message
// This method confirms the login message with the server. Sends the user a confirmation
// and then waits for a open/create request if user enters a good login.
// If a user enters a bad login, it directs the user back to client:connected.
void client::process_login(const boost::system::error_code & e, size_t bytes_transferred)
{
  // If it didn't error out
  if(!e)
    {
      // Make a reader to read from response buffer
      std::istream reader(&response); 

      // Output the response to a string
      std::string request_string;
      std::getline(reader, request_string);

      std::string password;
      
      // Parse for the password
      // Make sure the message starts with PASSWORD\e
      if(request_string.find("PASSWORD\e") == 0)
	{
	  // Get the remaining characters until \n
	  std::istringstream ss(request_string);
	  std::getline(ss, password, '\e'); // Read til \e
	  std::getline(ss, password);
	}
      else
	{
	  // Send an error message and direct them back to appropriate place
	  std::string error("ERROR\eBad protocol\n");
	  send_message(error);
  	// Begin receiving. Call the process login method once login has been received
	  boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_login,  this, 
									  boost::asio::placeholders::error,
									  boost::asio::placeholders::bytes_transferred));
	  return;
	}

      // Check the password
      if(serv->authenticate_password(password))
      	{
	  // Authentication correct
	  // Send a reply to client containing a filelists
	  std::string message("FILELIST");
	  
	  // Get the vector of filelists
	  std::vector<std::string> * filenames = serv->get_filenames();
	  
	  // Loop through it and write the string
	  for(std::vector<std::string>::iterator it = filenames->begin(); it != filenames->end(); ++it)
	  {
	  	// Write the  file name - \efilename\efilename etc
	      message = message + '\e' + *it;
	  }

	  // Add end line character
	  message = message + '\n';
	  
	  // Finish sending the message
	  send_message(message);
	  
	  // Listen for open requests
	  boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_open_create,  this, 
									  boost::asio::placeholders::error,
									  boost::asio::placeholders::bytes_transferred));
	}
      else
	{
	  // Authentication incorrect
	  // Send reply to client
	  std::string message("INVALID\n");
	  send_message(message);
	  // Begin receiving. Call the process login method once login has been received
	  boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_login,  this, 
									  boost::asio::placeholders::error,
									  boost::asio::placeholders::bytes_transferred));
	}
    }
  else 
  {
    process_disconnect();
    delete this;
  }
    
  
  return;
}

// Method that is called once client provides correct login informations. This method parses
// the request user makes as open or create. If user chooses to open an existing file, it loads the files and sends 
// the corresponding protocol. If user chooses to create a new file and the file name exists already
// it will call read_until again with call back as itself after sending them a message. Otherwise, it creates a 
// blank spreadsheet and informs the user of it
void client::process_open_create(const boost::system::error_code & e, size_t bytes_transferred)
{
  if(!e)
    {
      // Make a reader to read from response buffer
      std::istream reader(&response); 

      // Output the response to a string
      std::string request_string;
      std::getline(reader, request_string);

      std::string filename; 

      // If command is CREATE

      if(request_string.find("CREATE\e") == 0)
	{
	  // Get the remaining characters until \n
	  std::istringstream ss(request_string);
	  std::getline(ss, filename, '\e'); 
	  std::getline(ss, filename);

	  // If name does not exists
	  if(!serv->contains_filename(filename))
	    {
	      spreadsheet_filename = filename;

	      // Send them an update with version 1 for a new spreadsheet
	      std::string message("UPDATE\e1\n");
	      send_message(message);
	      	      
	      // Add filename to appropriate structure
	      spreadsheet = serv->create_spreadsheet(this, filename);

	      // Wait for them to make a request
	      boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_general_request,  this, 
									      boost::asio::placeholders::error,
									      boost::asio::placeholders::bytes_transferred));
	    }
	  else
	    {
	      // File already exists
	      std::string message("ERROR\eFile already exists. Please enter a different filename.\n");
	      send_message(message);
	      // Wait for new open/create request
	      boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_open_create,  this, 
									      boost::asio::placeholders::error,
									      boost::asio::placeholders::bytes_transferred));
	    }
	}
      else if(request_string.find("OPEN\e") == 0)
	{
	  // Get the remaining characters until \n
	  std::istringstream ss(request_string);
	  std::getline(ss, filename, '\e'); // Read til \e
	  std::getline(ss, filename);

	  // Check if it is an existing spreadsheet
	  if(serv->contains_filename(filename))
	    {
	      spreadsheet_filename = filename;

	      // Open the spreadsheet
	      spreadsheet = serv->open_spreadsheet(this, filename);

	      std::string message("UPDATE");

	     std::string version = spreadsheet->get_version();

	      message = message + '\e' + version;

	      std::map<std::string, std::string> * active_cells = spreadsheet->get_active_cells();

	      // For each active cell, make a message
	      for(std::map<std::string, std::string>::iterator it = active_cells->begin(); it != active_cells->end(); it++)
		{
		  std::string cellname;
		  std::string cellcontent;
		  
		  cellname = it->first;
		  cellcontent = it->second;

		  message = message + '\e' + cellname; // ...\ecellname
		  message = message + '\e' + cellcontent;
		}

	      message = message + '\n';

	      // rWrite the command
	      send_message(message);

	      // Wait for request to that spreadsheet
	      boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_general_request,  this, 
									      boost::asio::placeholders::error,
									      boost::asio::placeholders::bytes_transferred));
	    }
	    // If it is not
	    else
	      {
		// File already exists
		std::string message( "ERROR\eFile does not exists. Please enter a different filename.\n");
	    
		send_message(message);

		// Wait for new open/create request
		boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_open_create,  this, 
									      boost::asio::placeholders::error,
									      boost::asio::placeholders::bytes_transferred));
	      
	      }
	}
      else
	{
	  // Send an error message and direct them back to appropriate place
	  std::string error("ERROR\eBad protocol");
	  send_message(error);
	  boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_open_create,  this, 
									  boost::asio::placeholders::error,
									  boost::asio::placeholders::bytes_transferred));
	}
    }
  else
    {
      process_disconnect();
      delete this;
    }

	  return;
}

// This method is called after the user opened a file. This method will parse the 
// The user input and then calls an appropriate method to take care of the request
void client::process_general_request(const boost::system::error_code & e, size_t bytes_transferred)
{
  if(!e)
    {
      // Make a reader to read from response buffer
      std::istream reader(&response); 

      // Output the response to a string
      std::string request_string;
      std::getline(reader, request_string);

      std::cout << request_string << std::endl;
      // Case on what the request starts with
      if(request_string.find("ENTER\e") == 0)
	{
	  std::string version;
	  std::string cell_name;
	  std::string cell_content;

	  std::istringstream ss(request_string);
	  std::getline(ss, version, '\e');
	  std::getline(ss, version, '\e');
	  std::getline(ss, cell_name, '\e');
	  std::getline(ss, cell_content);


	  int undo_result = spreadsheet->update(version, cell_name, cell_content);
	    if(undo_result == 1)
	    {
	      std::cout << "Circular Dependency Not Seen" << std::endl;
	      // Get a vector pointer that points to all the client that are connected
	      std::vector<client *> * coeditors = serv->get_active_client(spreadsheet_filename);

	      // Message to send
	      std::string message = "UPDATE\e";
	      message = message + spreadsheet->get_version() + '\e';
	      message = message + cell_name + '\e';
	      message = message + cell_content + '\n'; 
	      
	      std::cout << cell_name  << " " << cell_content << std::endl;

	      std::cout << cell_name << " " << cell_content << std::endl;
	      for(std::vector<client *>::iterator it = coeditors->begin(); it != coeditors->end(); it++)
		{
		  client * coeditor = *it;
		  coeditor->send_message(message);
		}
	    }
	    else if(undo_result == 2)
	    {
	      std::cout << "Circular Dependency Prevented" << std::endl;
	      // Send an error message and direct them back to appropriate place
	      std::string error("ERROR\eCircular logic has occurred! Your request was ignored.\n");
	      send_message(error);
	    }
	    else
	      {
		std::cout << "Formula reference prevented" << std::endl;
		// Send an error message and direct them back to appropriate place
		std::string error("ERROR\eInvalid formula reference\n");
		send_message(error);	
	      } 
	}
      else if(request_string.find("RESYNC") == 0)
	{
	  process_sync();
	}
      else if(request_string.find("UNDO\e") == 0)
	{
	  std::string version_update;
	  
	  std::istringstream ss(request_string);
	  std::getline(ss, version_update, '\e'); // Read UNDO\e
	  std::getline(ss, version_update); // Read version_number\e
	  
	  std::string update = spreadsheet->undo(version_update);

	  // Call helper method
	  if(!update.empty())
	    {
	      std::string version;
	      std::string cell_name;
	      std::string cell_content;

	      std::istringstream ss(update);

	      std::getline(ss, version, '\e');
	      std::getline(ss, cell_name, '\e');
	      std::getline(ss, cell_content);

	      std::cout << version << " " << cell_name << " " << cell_content << std::endl;
	      // Get a vector pointer that points to all the client that are connected
	      std::vector<client *> * coeditors = serv->get_active_client(spreadsheet_filename);

	      // Message to send
	      std::string message = "UPDATE\e";
	      message = message + version + '\e';
	      message = message + cell_name + '\e';
	      message = message + cell_content + '\n'; 

	      for(std::vector<client *>::iterator it = coeditors->begin(); it != coeditors->end(); it++)
		{
		  client * coeditor = *it;
		  coeditor->send_message(message);
		}
	    }
	  else
	    {
	      // Send an error message and direct them back to appropriate place
	      std::string error("ERROR\eNothing to undo\n");
	      send_message(error);
	    }
	}
      else if(request_string.find("SAVE\e") == 0)
	{
	  std::string version;
	  
	  std::istringstream ss(request_string);
	  std::getline(ss, version, '\e'); // Read SAVE\e
	  std::getline(ss, version, '\e'); // Read version_number\e

	  // Call helper method
	  spreadsheet->save(version);

	  std::string saved("SAVED\n");
	  send_message(saved);
	}
      else if(request_string.find("DISCONNECT") == 0)
	{
	  std::string version;

	  std::cout << "Client disconnected" << std::endl;
	  std::istringstream ss(request_string);
	  std::getline(ss, version, '\e'); // Read SAVE\e
	  std::getline(ss, version, '\e'); // Read version_number\e

	  // Call helper method
	  spreadsheet->save(version);
	  process_disconnect();
	  delete this;
	  return;
	}
      else
	{
	  std::cout << request_string << std::endl;
	  // Send an error message and direct them back to appropriate place
	  std::string error("ERROR\eBad protocol\n");
	  send_message(error);
	}

      // Wait for additional requests
      boost::asio::async_read_until(sock, response, '\n', boost::bind(&client::process_general_request,  this, 
								      boost::asio::placeholders::error,
								      boost::asio::placeholders::bytes_transferred));
    }
  else 
    {
      process_disconnect();
      delete this;
    }
  return;
}

// Method that is called for disconnects
void client::process_disconnect()
{
  serv->client_disconnected(this, spreadsheet_filename);
  return;
}

// Method that process a sync command
void client::process_sync()
{
  std::string message("SYNC");

  std::string version = spreadsheet->get_version();
  
  message = message + '\e' + version;
	      
  std::map<std::string, std::string> * active_cells = spreadsheet->get_active_cells();
  
  // For each active cell, make a message
  for(std::map<std::string, std::string>::iterator it = active_cells->begin(); it != active_cells->end(); it++)
    {
      std::string cellname;
      std::string cellcontent;
		  
      cellname = it->first;
      cellcontent = it->second;
		  
      message = message + '\e' + cellname; // ...\ecellname
      message = message + '\e' + cellcontent;
    }
  
  message = message + '\n';
  
  // Write the command
  send_message(message);
  return;
}

// Getter for the socket
boost::asio::ip::tcp::socket & client::get_socket()
{
  return sock;
}


/* Please use this method to send messages at any time.
 * This method simply queues up your message request so that
 * at any moment, the socket is only sending one thing.
 */
void client::send_message(std::string message)
 {
   // CITE: Referencing structure in chat_server.cpp in the boost example for asio library
   bool writing = !to_be_sent.empty();
   to_be_sent.push_back(message);

   // If no writing operation is occuring current
   if(!writing)
     {
       // Response writer
      std::ostream writer(&request);
      
      // Put in the message
      writer << message;

      // Begin writing
      boost::asio::async_write(sock, request, boost::bind(&client::handle_write, this, boost::asio::placeholders::error));
     }

   return;
 }

 /*
  * Call back for send message. Simply loops until all writes are complete
  */ 
 void client::handle_write(const boost::system::error_code & e)
 {
   if(!e)
     {
       // Pop the message we just sent
       to_be_sent.pop_front();

       if(!to_be_sent.empty())
	 {
	   // Get the next message
	   std::string message(to_be_sent.front());
	   
	   // Response writer
	   std::ostream writer(&request);
	   
	   // Put in the message
	   writer << message;
	   
	   // If there are more messages to send, do so
	   boost::asio::async_write(sock, request, boost::bind(&client::handle_write, this, boost::asio::placeholders::error));
	 }
     }
   else
     {
       process_disconnect();
       delete this;
     }
   return;
 }
