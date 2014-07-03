using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNetworking;
using System.Net.Sockets;
using MessageMachine;

namespace SpreadsheetClientModel
{
    /// <summary>
    /// Model used to communicate with the Boggle Server
    /// </summary>
    public class Model
    {
        // The socket used to communicate with the server.  If no connection has been
        // made yet, this will be null.
        private StringSocket socket;

        #region Register for these events when a line of text arrives

        // Invalid Password
        public event Action InvalidLineEvent;

        // List of available files on the server
        public event Action<data> FileListLineEvent;

        // Update the current spreadsheet
        public event Action<data> UpdateLineEvent;

        // Spreadsheet saved on server
        public event Action<data> SavedLineEvent;

        // Error message
        public event Action<data> ErrorLineEvent;

        // Error message, Save Messages etc.
        public event Action GeneralLineEvent;

        // The server has disconnected
        public event Action ServerDisconnectEvent;

        bool sentDisconnect;

        #endregion

        /// <summary>
        /// Creates a not-yet-connected client model.
        /// </summary>
        public Model()
        {
            socket = null;
            sentDisconnect = false;
        }

        /// <summary>
        /// Connect to the server at the given hostname and port and with the given password
        /// to allow access to the server
        /// </summary>
        public bool Connect(string hostname, int port)
        {
            socket = null;

            if (socket == null)
            {
                try
                {
                    TcpClient client = new TcpClient(hostname, port);
                    socket = new StringSocket(client.Client, UTF8Encoding.Default);

                    socket.BeginReceive(LineReceived, null);

                    // Succesful connection
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }            
            }

            // Could not connect / Already connected
            return false;
        }

        /// <summary>
        /// Inform the user if the model is already connected
        /// </summary>
        /// <returns></returns>
        public bool isConnected()
        {
            return socket != null;
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        public void Disconnect()
        {
            if (socket != null)
            {
                this.SendMessage("DISCONNECT\n");
                socket.Close();
                socket = null;
                sentDisconnect = true;
            }
        }

        /// <summary>
        /// Send a line of text to the server.
        /// </summary>
        /// <param name="line"></param>
        public void SendMessage(String line)
        {
            if (socket != null)
            {
                // the \n is included in the line when it is passed in
                socket.BeginSend(line, (e, p) => { }, null);
            }
        }

        /// <summary>
        /// Deal with an arriving line of text.
        /// </summary>
        public void LineReceived(String s, Exception e, object p)
        {
            // Build a MessageMachine and data object
            Machine m = new Machine();
            data d = new data();

            // If the string is null keep listening and return
            if (s == null)
            {
                if (socket != null)
                {
                    // If we reach this point then the server has disconnected
                    ServerDisconnectEvent();
                    this.Disconnect();
                }
                    
                return;
            }

            m.read(d, s);

            // Send the InvalidLineEvent to the controller
            if (InvalidLineEvent != null && d.type == "INVALID")
            {
                InvalidLineEvent();
            }

            // Send the FileListLineEvent to the controller
            else if (FileListLineEvent != null && d.type == "FILELIST")
            {
                FileListLineEvent(d);
            }

            // Send the UpdateLineEvent to the controller in the event of an update, undo or a sync
            else if (UpdateLineEvent != null && (d.type == "UPDATE" || d.type == "SYNC"))
            {
                GeneralLineEvent();
                UpdateLineEvent(d);
            }

            // Send the SavedLineEvent to controller
            else if (SavedLineEvent != null && d.type == "SAVED")
            {          
                SavedLineEvent(d);
            }

            // Send the ErrorLineEvent
            else if (ErrorLineEvent != null && d.type == "ERROR")
            {
                ErrorLineEvent(d);
            }

            // Continue listening
            socket.BeginReceive(LineReceived, null);
        }

    }
}