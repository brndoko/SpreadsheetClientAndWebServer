/*
 * This class can create and parse all protocol messages.
 * There is a build function for every type of message.
 * There is a single read function which parses messages,
 * returning a data object with the appropriate fields filled
 * for the message read.
 * 
 * Team: Cowboys
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageMachine
{
    public class Machine
    {
        char esc;

        /// <summary>
        /// Constructor
        /// </summary>
        public Machine()
        {
            int x = 27;
            esc = (char)x;
        }

        // -----------------------------------------------------------------------
        // Methods for Message Building
        // -----------------------------------------------------------------------

        /// <summary>
        /// Builds authentication messages, for authenticating clients to the server.
        /// Client --> Server
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string build_password(string password)
        {
            return "PASSWORD" + esc + password + "\n";
        }

        /// <summary>
        /// Builds open messages, for opening spreadsheets.
        /// Client --> Server
        /// </summary>
        /// <param name="sheet_name"></param>
        /// <returns></returns>
        public string build_open(string sheet_name)
        {
            return "OPEN" + esc + sheet_name + "\n";
        }

        /// <summary>
        /// Builds create messages, for creating new spreadsheets.
        /// Client --> Server
        /// </summary>
        /// <param name="sheet_name"></param>
        /// <returns></returns>
        public string build_create(string sheet_name)
        {
            return "CREATE" + esc + sheet_name + "\n";
        }

        /// <summary>
        /// Builds edit messages, for submitting edits to the server.
        /// Client --> Server
        /// </summary>
        /// <param name="version"></param>
        /// <param name="cell_name"></param>
        /// <param name="cell_content"></param>
        /// <returns></returns>
        public string build_enter(int version, string cell_name, string cell_content)
        {
            return "ENTER" + esc + version + esc + cell_name + esc + cell_content + "\n";
        }

        /// <summary>
        /// For when client is out of sync with server
        /// Client --> Server
        /// </summary>
        /// <returns></returns>
        public string build_resync()
        {
            return "RESYNC\n";
        }

        /// <summary>
        /// Builds undo messages, for submitting undo commands to the server.
        /// Client --> Server
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string build_undo(int version)
        {
            return "UNDO" + esc + version + "\n";
        }

        /// <summary>
        /// Builds save messages, for saving spreadsheets.
        /// Client --> Server
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string build_save(int version)
        {
            return "SAVE" + esc  + version + "\n";
        }

        /// <summary>
        /// Builds diosconnect messages, for the case that a client is 
        /// disconnecting from the server.
        /// Client --> Server
        /// </summary>
        /// <returns></returns>
        public string build_disconnect()
        {
            return "DISCONNECT\n";
        }


        // -----------------------------------------------------------------------
        // Methods for Message Parsing
        // -----------------------------------------------------------------------

        /// <summary>
        /// Takes a message and parses the message. Resulting data is contained in data
        /// object. Simply access appropriate members of data object to access message data.
        /// </summary>
        /// <param name="return_data"></param>
        /// <param name="message"></param>
        public void read(data return_data, string message)
        {
            return_data.message = message;
           
            if (message.StartsWith("FILELIST"))
            {
                return_data.type = "FILELIST";
                read_filelist(return_data);
            }
            else if (message.StartsWith("INVALID"))
            {
                return_data.type = "INVALID";
            }
            else if (message.StartsWith("UPDATE"))
            {
                return_data.type = "UPDATE";
                read_update_sync(return_data);
            }
            else if (message.StartsWith("SAVED"))
            {
                return_data.type = "SAVED";
            }
            else if (message.StartsWith("SYNC"))
            {
                return_data.type = "SYNC";
                read_update_sync(return_data);
            }
            else if (message.StartsWith("ERROR"))
            {
                return_data.type = "ERROR";
                read_error(return_data);
            }
            else
            {
                throw new Exception("MessageBuilder cannot parse message: " + message);
            }

            //return return_data;
        }


        /// <summary>
        /// Parses filelist messages
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private data read_filelist(data d)
        {
            string message = d.message;
            string temp = "";
            char[] m = message.ToCharArray();
            d.file_names = new List<string>(); // initialize list of file names

            for (int i = 9; i < message.Length; i++) // Skip "FILELIST\e"
            {
                if (i == (message.Length - 1)) // If reached last character
                {
                    if (i != '\n') // If last character is not a new line, add to temp.
                        temp = temp + m[i]; 
                    break;
                }

                if (m[i] == esc) //esc 
                {
                    if (temp != "") // Add cell name to list.
                        d.file_names.Add(temp);
                    temp = ""; // reset for next file name
                    continue;
                }

                temp = temp + m[i];
            }

            if (temp != "") // Add last cell name to list
                d.file_names.Add(temp);
 
            return d;
        }

        /// <summary>
        /// Parses update messages
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private data read_update_sync(data d)
        {
            string message = d.message;
            string temp = "";
            char[] m = message.ToCharArray();

            int index;

            // Find starting index
            if (d.type == "UPDATE") // For UPDATE messages
                index = 7;
            else   // For SYNC messages
                index = 5;
            
            // First get version-- all messages will have version
            for (int i = index; i < message.Length; i++) // Skip "UPDATE\e" or "SYNC\e"
            {
                // If end of message is reached during this stage, version is only 
                // data this message contains. Save in data object and return.
                if (i == (message.Length - 1)) 
                {
                    temp = temp + m[i]; 
                    d.version = temp;
                    return d;
                }

                // If we reach escape, done getting version.
                if (m[i] == esc) // \e
                    break;

                // If not version or new line character, keep concantenating temp to get version.
                temp = temp + m[i];
                index++; // Keep overall index in message.
            }

            d.version = temp; // Save version in data object

            temp = ""; // Reset temp for getting cell names and contents
            index++; // skip esc for next phase pf parsing

            // Get cell names and contents

            bool getting_name = true; // intially, getting name
            for (int i = index; i < message.Length; i++) 
            {
                
               // If escape, you've gotten a whole name or contents string
                if (m[i] == esc) //esc 
                {
                    if (getting_name)
                    {
                        d.cell_names.Add(temp);
                        if (i == (message.Length - 1)) // If reached last character and no contents for this cell
                        {
                            // add final piece of data -- cell content
                            d.cell_contents.Add("");
                            break;
                        }
                        else
                            getting_name = false;
                    }
                    else
                    {
                        d.cell_contents.Add(temp);
                        getting_name = true;
                    }
                            
                    temp = ""; // reset for next 
                    continue;
                }

                if (i == (message.Length - 1))
                {
                    temp = temp + m[i];
                    d.cell_contents.Add(temp);
                    break;
                }

               
                // If not esc or new line, continue building name or contents string
                temp = temp + m[i];
            }

            
            return d;
        }


        /// <summary>
        /// Parses error messages
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private data read_error(data d)
        {
            string message = d.message;
            string temp = "";
            char[] m = message.ToCharArray();

            int start_index = 6;

            for (int i = start_index; i < message.Length; i++) // Skip "ERROR\e"
            {
                if (i == (message.Length - 1)) // If reached last character
                {
                    if (i != '\n') // If last character is not a new line, add to temp.
                        temp = temp + m[i];
                    break;
                }

                temp = temp + m[i];
            }

            d.error_message = temp;
            return d;
        }

    }

    // -----------------------------------------------------------------------
    // Struct for Message Parsing
    // -----------------------------------------------------------------------

    public class data
    {
        public string type = "";
        public string message = "";
        public string password = "";
        public string sheet_name = "";
        public string error_message = "";
        public string version = "";
        public List<string> file_names = new List<string>();
        public List<string> cell_names = new List<string>();
        public List<string> cell_contents = new List<string>();

    }
}
