/* 
 0;136;0c* Header file for the server class
 */

#ifndef SERVER_H
#define SERVER_H

#include <cstdlib>
#include <stdio.h>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include "client.h"
#include <string>
#include <vector>
#include <iostream>
#include <fstream>
#include <sstream>
#include "sheet.h"
#include <map>

class client;
class server
{
 private:
  boost::asio::io_service & socket_input;
  boost::asio::ip::tcp::acceptor tcp_listener;
  std::string password;
  std::vector<std::string> * filenames;
  const char * filename_disk;
  std::map<std::string, sheet *> * filename_to_spreadsheet;
  std::map<std::string, std::vector<client *> *> * sheet_to_clients;
  
public:
  server(int port, boost::asio::io_service & input, const char * pass, const char * filename);
  int authenticate_password(std::string input);
  int contains_filename(std::string input);
  sheet * create_spreadsheet(client * c, std::string new_filename);
  std::vector<std::string> * get_filenames();
  sheet * open_spreadsheet(client * c, std::string filename);
  std::vector<client *> * get_active_client(std::string filename);
  void client_disconnected(client *c , std::string filename);
  void clean();
 private:
  void client_connected(client * new_client, const boost::system::error_code & e);
  void add_client_to_spreadsheet(client *, std::string s);
};

#endif
