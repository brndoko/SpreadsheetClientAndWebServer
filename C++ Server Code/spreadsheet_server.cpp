/* This represents a class for the spreadsheet server. The
 * main method embedded in this file starts a spreadsheet server
 * given a port number. This spreadsheet server is capable of handling
 * multiple client accessing the same spreadsheet.
 *
 * The structuring of this program is similar to http://www.boost.org/doc
 * /libs/1_55_0/doc/html/boost_asio/example/cpp03/echo/async_tcp_echo_server.cpp
 * which was used as a tutorial. Although structuring is the same
 * All code written here is original.
 *
 * April 9, 2014
 * Group: The Cowboys
 */

#include "server.h"
#include <boost/thread.hpp>

/* Constructor for server. Wraps io_service into a sockets. 
 * io_service is needed by socket for input and output. 
 * One shared by all server connections. 
 */
server::server(int port, boost::asio::io_service & input, const char * pass, const char * filename)
  :socket_input(input),
   tcp_listener(input, boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), port)),
   password(pass),
   filename_disk(filename)
{
  // Make a new client
  client * new_client = new client(socket_input, this);

  // Populate our vector by reading from the filename
  filenames = new std::vector<std::string>();
  std::ifstream reader(filename);
  std::string line;

  // Initialize the two data structure on the heap
  filename_to_spreadsheet = new std::map<std::string, sheet *>();
  sheet_to_clients = new std::map<std::string, std::vector<client *>*>();

  // Read all the lines from the file and add them to list of filenames
  while(std::getline(reader, line))
    {
      std::istringstream in(line);
      std::string name;
      
      in >> name;
      
      if(name.find('\r') == 0)
	std::cout << "You added something bad in" << std::endl;

      filenames->push_back(name);
    }

  // Open up all the spreadsheets in the from the list of filenames
  for(std::vector<std::string>::iterator it = filenames->begin(); it != filenames->end(); ++it)
    {
      std::string to_be_opened = *it;
      sheet * to_be_added = new sheet(to_be_opened);
      (*filename_to_spreadsheet)[to_be_opened] = to_be_added;
      (*sheet_to_clients)[to_be_opened] = new std::vector<client *>();
    }

  // Listen for new connection with the socket in the client class. Once someone has connected, call the client_connected callback
  tcp_listener.async_accept(new_client->get_socket(), boost::bind(&server::client_connected, this, new_client, 
								      boost::asio::placeholders::error));

}

/*
* Receive Callback for when a new client connects to server.
*/
void server::client_connected(client * new_client, const boost::system::error_code & e)
{  
  // Check for errors  
  if(!e)
  {
    // Call method in client to start listening from this client
    std::cout << "Client connected" << std::endl;
    new_client->connected();

  }
  else
    // Client errored out, delete him
    delete new_client; 

  // Begin listening again.
  client * new_client_2 = new client(socket_input, this);
  
  // LI!!! Changed this to new_client_2
  tcp_listener.async_accept(new_client_2->get_socket(), boost::bind(&server::client_connected, this, new_client_2, 
								      boost::asio::placeholders::error));
  return;
}

/* This method is used to see if a filename exists. Returns
 * 1 if true and 0 if false.
 */
int server::contains_filename(std::string input)
{
  return std::find(filenames->begin(), filenames->end(), input) != filenames->end();
}

/*  This method adds a new file name to to the list of filenames
 *  tracked by the spreadsheet. This method will add the filename
 *  to the vector of filename and the file on disk.
 */
sheet * server::create_spreadsheet(client * c, std::string new_filename)
{
  // Write to the file on the disk
  std::ofstream writer;
  writer.open(filename_disk, std::ofstream::app);
  writer << new_filename << '\n';
  writer.close();

  // Add the new spreadsheet to the map
  sheet * to_be_added = new sheet(new_filename);
  (*filename_to_spreadsheet)[new_filename] = to_be_added;

  // Get the vector which for that spreadsheet
  (*sheet_to_clients)[new_filename] = new std::vector<client *>();
  std::vector<client *> * add_to_me = (*sheet_to_clients)[new_filename];
  (*add_to_me).push_back(c);

  // Also write to find to the vector
  filenames->push_back(new_filename);

   return to_be_added;
}

/* This method is used for getting the existing file name from the server
 */
std::vector<std::string> * server::get_filenames()
{
  return filenames;
}

/*  This method is used for getting the active clients related to a spreadsheet
 */ 
std::vector<client *> * server::get_active_client(std::string filename)
{
  return (*sheet_to_clients)[filename];
}

/* This method is used for getting a spreadsheet assigned to a client
 * This method returns a pointer to a spreadsheet which the client is assigned to.
 * The server updates which client has which spreadsheet open each time this is called
 */ 
sheet * server::open_spreadsheet(client * c, std::string filename)
{
  // Assign the client to that spreadsheet
  add_client_to_spreadsheet(c, filename);
  // Return them the spreadsheet we just opened
  return (*filename_to_spreadsheet)[filename]; 
}

// Method to mark client as an active user of a spreadsheet in our map
void server::add_client_to_spreadsheet(client * c, std::string filename)
{
  // Get the vector which for that spreadsheet
  std::vector<client *> * add_to_me = (*sheet_to_clients)[filename];

if(contains_filename(filename))
    std::cout << "Got the spreadsheet" << std::endl;

  (*add_to_me).push_back(c);

  
  return;
}

// Method to call when client disconnects
void server::client_disconnected(client * c, std::string filename)
{
  // Get the vector that contains that client pointer
  std::vector<client *> * v = (*sheet_to_clients)[filename];

  // Remove the item from the vector
  v->erase(std::remove(v->begin(), v->end(), c), v->end());

  return;
}


// Clean up the server
void server::clean()
{
  std::cout << "Server closed" << std::endl;

  // Call destructor of all spreadsheets
  for(std::map<std::string, sheet *>::iterator it = filename_to_spreadsheet->begin(); it != filename_to_spreadsheet->end();)
  {
    sheet * close_me = it->second;
    filename_to_spreadsheet->erase(it++);
    delete close_me;
  }

  delete filename_to_spreadsheet;

  std::cout << "Server Closed #2" << std::endl;

  
  //Call destructor of all remaining clients
  for(std::map<std::string, std::vector<client *> *>::iterator it = sheet_to_clients->begin(); it != sheet_to_clients->end();)
    {
      std::vector<client *> * client_vector = it->second;
      
      // For every client in that client
      for(std::vector<client *>::iterator it2 = client_vector-> begin(); it2 != client_vector->end();)
	{
	  client * to_be_removed = *it2;
	  client_vector->erase(it2++);
	  delete to_be_removed;
	}

      sheet_to_clients->erase(it++);
      delete client_vector;
    }

  std::cout << "All memory are cleared" << std::endl;

  delete sheet_to_clients;
  
  std::cout << "Server Closed #3" << std::endl;

  return;
}

/*
 * Method that authenticates the user
 * Returns an int(bool) that indicates whether or not the given password matched.
 * Correct Login - Returns 1
 */
int server::authenticate_password(std::string input)
{
  // Just use the compare function in string libary
  return (password.compare(input) == 0);
}
			      
/* 
 * SERVER ENTRY POINT
 * Main method that takes in a port and opens a server listening for that
 * port number.
 */
int main(int argc, const char* argv[])
{
  // Variables for main
  int port;
  boost::asio::io_service server_input;
  const char * password;
  const char * filename;

  // Check argument counts
  if(argc != 2)
    {
      printf("Invalid number of arguments");
      return 1;
    }

  // Parse port number
  port = std::atoi(argv[1]);  // Start the server with a given port

  // Hard coded password
  password = "password";

  // Filename of the file that contains all the spreadsheet filenames
  filename = "spreadsheet_files.txt";
  server s(port, server_input, password, filename);

  // Run the service 
  //  boost::thread server_bt(boost::bind(&boost::asio::io_service::run, &server_input));

  boost::thread bt(boost::bind(&boost::asio::io_service::run, &server_input));

  // Exit the program
  while(true)
    {
      // Accept new commands
      std::string server_command;
      std::cin >> server_command;

      // If the server gets an command to exit
      if(server_command.find("exit") != std::string::npos)
	{
	  exit(0);
	}
      std::cout << "Command not supported" << std::endl;
    }

  server_input.stop();

  return 0;
}
