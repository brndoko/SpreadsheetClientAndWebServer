#ifndef CLIENT_H
#define CLIENT_H

#include <cstdlib> 
#include <stdio.h>
#include <string>
#include <istream>
#include <ostream>
#include <iostream> 
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include "server.h"
#include <deque>
#include "sheet.h"

class server;
class client
{
 private:
  sheet * spreadsheet;
  server * serv;
  std::string spreadsheet_filename;
  boost::asio::ip::tcp::socket sock;
  boost::asio::streambuf request;
  boost::asio::streambuf response;
  std::deque<std::string> to_be_sent;
 public: 
  client(boost::asio::io_service & input, server * s);
  ~client();
  void connected();
  void send_message(std::string message);
  boost::asio::ip::tcp::socket & get_socket();
 private:
  void process_login(const boost::system::error_code & e, size_t bytes_transferred);
  void process_open_create(const boost::system::error_code & e, size_t bytes_transferred);
  void process_general_request(const boost::system::error_code & e, size_t bytes_transferred);
  void process_edit(int version_number, std::string cell_name, std::string cell_content);
  void process_undo(int version_number);
  void process_save(int version_number);
  void process_disconnect();
  void process_sync();
  void handle_write(const boost::system::error_code & e);
};
#endif
