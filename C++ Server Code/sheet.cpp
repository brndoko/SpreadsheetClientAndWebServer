// TODO

// ------ !!!!!!  Do we want an automatic save every update and undo??? !!! ----

// Need to make sure destructor is doing enough

#include "sheet.h"
#include "dependency_graph.h"
#include <sstream>
#include <string>
#include <iostream>
#include <fstream>
#include <stdexcept>
#include <set>

using namespace std;

sheet::sheet(string f)
{
	this->version = 0;
	this->sheet_state = new map<string, string>;
	this->changes = new stack<cell*>;
	this->file_name = f;
	this->graph = new cs3505::dependency_graph(); // Creates a new dependency graph

	// Get file
	ifstream streamer (f.c_str());

	if (streamer.is_open()) // file exists
		open();
	else // File does not yes exist -- new file
	{
		version = 1; // Initialize version at 1
		save("1"); // Create XML file for this new sheet
	}
		
}

sheet::~sheet()
{
    if (!changes->empty()) // Delete cells
	{
		cell * c = changes->top();
		changes->pop();
		delete c;
	}

    delete changes;
    delete sheet_state;
}

/* Updates sheet_state with edit/change
Returns false if there is a circular dependency
Return 1 for success 
Return 2 for circular dependency
Return 3 for formula error */
int sheet::update(string version_number, std::string c_name, std::string  c_content)
{
	// Before checking for circular dependency or updating the sheet, make sure this 
	// Formula is using defined cells -- it cannot use cells that are strings or empty.
	if(!defined_formula(c_name, c_content))
		return 3;
	
	// See if this cell is already filled in sheet
	bool new_cell = true;
	for (map<string,string>::iterator it = sheet_state->begin(); it !=  sheet_state->end(); it++)
    {
    	if (it->first == c_name) // If cell is currently full, save its content before change
    	{
    		new_cell = false;
    		cell * c = new cell();
    		c->cell_name = c_name;
    		c->cell_content = it->second;
    		changes->push(c); // Put "name\econtents"

    		// Update dependency graph-- remove dependencies that old content made
    		dependency_work(false, c_name, it->second);

    		// See if this update will cause a circular dependency. Return false if it will.
			// dependency_work will update graph and report a circular dependency, if there is one
			if (!dependency_work(true, c_name, c_content))
			{
				dependency_work(true, c_name, it->second); // Replace dependencies that were removed
				changes->pop(); // Undo update to changes
				return 2; // If dependency work finds a circular dependency, return false.
			}
				
    		// change contents 
    		if (c_content != "")
    			it->second = c_content;
    		else
    			sheet_state->erase(it);

    		break; // Don't look through the rest of the cells
    	}
    }

    // If cell wasn't already in sheet, add it
    if (new_cell) 
    {
    	// See if this update will cause a circular dependency. Return false if it will.
		// dependency_work will update graph and report a circular dependency, if there is one
		if (!dependency_work(true, c_name, c_content))
			return 2; // If dependency work finds a circular dependency, return false.

    	cell * c = new cell();
    	c->cell_name = c_name;
    	c->cell_content = ""; // There was nothing in the cell before
    	changes->push(c);

    	sheet_state->insert(std::pair<string,string>(c_name,c_content)); // New cell
    }

	// Increment version for this change
	version++;

	// Automatically save
	string version_temp;
    stringstream stream;
	stream << version;
	version_temp = stream.str();
	save(version_temp);

	// Return true if no circular dependency
	return 1;
}

/* Uses changes to undo last change to the spreadsheet
Returns string of form <version>\e<cell_name>\e<cell_content>\n for successfull undoes.
Returns null if there's nothing to undo */
string sheet::undo(string version_number)
{
	cell * c;

	// Get name and contents of previously changed cell
	if (!changes->empty())
	{
		c = changes->top();
	}
	else
		return ""; // Return null if there is no change to undo

    // Two cases: Cell used to be empty or it used to be full:
    bool was_empty = false;
    if (c->cell_content == "")
    	was_empty = true;

	// Change sheet state
	for (map<string,string>::iterator it = sheet_state->begin(); it !=  sheet_state->end(); it++)
    {
    	if (it->first == c->cell_name) // If cell is currently full, save its content before change
    	{
    		// First, remove dependencies that are there BEFORE undo
    		dependency_work(false, it->first, it->second);

    		if (was_empty)
    			sheet_state->erase(it); // Erase if undo causes cell to be empty
    		else
    		{
    			// add dependencies that are there as a result of undo
    			if(!dependency_work(true, it->first, c->cell_content)) 
    			{
    				dependency_work(true, it->first, it->second); // Put dependencies back
    				return ""; // If adding dependency failed, circular dependency and undo was not carried out
    			}
    			it->second = c->cell_content; // Change content if content was different
    		}
    			
    		
    		break; // Don't look through the rest of the cells
    	}
    }

    // Successful undo, pop from changes
    changes->pop();

    // Increment version for successful undo
    version++;

    string version_temp;
    stringstream stream;
	stream << version;
	version_temp = stream.str();
	save(version_temp);

	string return_me = version_temp + "\e" + c->cell_name + "\e" + c->cell_content + "\n";
	return return_me;

}

// Opens spreadsheet by populating sheet_state with xml file data
void sheet::open()
{
	// Open file
	ifstream streamer (file_name.c_str());

	// Variable for current character
	char c;
	string temp = "";
	string name = "";
	string contents = "";
	string vers = "";
	bool na = false;
	bool co = false;
	bool ve = false;

	if(streamer.is_open())
	{
		while (streamer.get(c))
		{
			if (c == '<')
			{
				if (na)
					na = false;
				else if (co)
					co = false;
				else if (ve)
					ve = false;

				temp = ""; // Reset for new fields
				continue;
			}
			
			if (c=='>') // Get current fields
			{
				if (temp == "name")
				{
					na = true; // Getting name
					co = false;
					ve = false;
				}
				else if (temp == "contents")
				{
					co = true; // Getting contents
					na = false;
					ve = false;
				}
				else if (temp == "/cell") // Reached end of cell
				{
					// Add name and contents 
					sheet_state->insert(std::pair<string,string>(name,contents));
					name = "";
					contents = "";
					na = false;
					co = false;
					ve = false;
				}
				else if (temp == "spreadsheet version")
				{
					stringstream streamer;
					streamer << vers;
					int v_temp;
					streamer >> v_temp;
					version = v_temp;
				}
				
				continue;
			}

			if (c == '=')
			{
				if (temp == "spreadsheet version") // confirm not part of a formula
				{
					ve = true;
					continue;
				}

			}

			if (na)
				name = name + c;
			else if (co)
				contents = contents + c;
			else if (ve)
				vers = vers + c;
			else
				temp = temp + c;

		}

	}
	else
		throw runtime_error("ERROR: XML file could not be read.");

}

// Saves spreadsheet by writing sheet_state to xml file
void sheet::save(string version_number)
{
	ofstream streamer (file_name.c_str());

	// Write version
	streamer << "<spreadsheet version=" << version_number << ">";

	// Write cells
	for (map<string,string>::iterator it = sheet_state->begin(); it !=  sheet_state->end(); it++)
    {
    	streamer << "<cell>";
    	streamer << "<name>" << it->first << "</name>";
		streamer << "<contents>" << it->second << "</contents>";
		streamer << "</cell>";
    }

    // End spreadsheet
    streamer << "</spreadsheet>";

    streamer.close();
}

/* Returns map of cell names and cell contents */
std::map<std::string, std::string> * sheet::get_active_cells()
{
  return sheet_state;
}

/* Returns current version of this spreadsheet */
string sheet::get_version()
{
	string version_temp;
	stringstream stream;
	stream << version;
	version_temp = stream.str();
	save(version_temp);
  	return version_temp;
}

/* 
 * Adds or removes dependencies between c_name and cell names in c_content
 * For adds, returns false if circular dependency found, true if not.
 * if add is true, adding dependency. Otherwise, removing one. */
bool sheet::dependency_work(bool add, std::string c_name, std::string c_content)
{
	// Parse the contents to determine the dependencies
	// If the contents has an equal sign, this is a formula
	// Step through the string
	// When a letter is hit, use regex to recognize that as a cell name and put it as a dependent
	// If a symbol (+, - , / , *) is hit, just continue

	std::set<string> cells_found;
	bool getting_cell = false;
	bool cont = false;
	std::string temp_cell = "";

	std::string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	std::string numbers = "0123456789";

	// Can loop through the cell contents
	for(int i = 0; i<c_content.size(); i++)
	{

		// If first character of content isn't '=', return true becuase this isn't a formula
		if ((i == 0) && (c_content[i] != '='))
			return true;
		else if (i == 0) // Skip =
			continue;
			
		// Check to see if this character is an alphabetical character
		for(int a = 0; a<alphabet.size(); a++)
		{
			if(c_content[i] == alphabet[a])
			{
				//If it has, we have reached a cell name.
				getting_cell = true;
				cont = true;
				temp_cell += c_content[i];
				break; // Break out of the alphabet loop and go to next character because you found letter
			}

		}

		// If this was a letter, continue,
		if (cont)
		{
			cont = false;
			continue;
		}
			
		// If we get to this point, and getting_cell is true, we will have a number as part of the 
		// cell name, or we will hit a symbol and will be done with this cell name

		// See if current character is a number
		if(getting_cell)
		{
			for(int n = 0; n<numbers.size(); n++)
			{
				if(c_content[i] == numbers[n]) // Number!
				{
					cont = true;
					temp_cell += c_content[i]; // Add to cell name
					break; // break out of number loop
				}

			}
		}

		// If this was a number, continue
		if (cont)
		{
			cont = false;
			continue;
		}

		// If we get to this point, and getting_cell is true, we have neither a character or a number,
		// which means we must have a symbol. Which means we have collected a whole cell name.

		// Check for dependency, and continue iterating through cell contents
		if (getting_cell)
		{
			if(add)
			{
				// record found cell just in case dependency is found in another pair for this equation
				cells_found.insert(temp_cell);

				// Update dependency graph
				graph->add_dependency(c_name,temp_cell);

				// See if update created circular dependency
				if(circular_dependency(c_name, temp_cell)) // Recursively check if this causes a circular dependecy. If so, return false
				{
					cout << "CIRCULAR DEPENDENCY !!!!!" << endl;
					// Remove all dependencies made in this equation
					for (std::set<string>::iterator itt=cells_found.begin(); itt!=cells_found.end(); ++itt)
						graph->remove_dependency(c_name, *itt); // If circular dependency was caused, remove pair from graph
					
					return false;  // Report circular dependency error
				}

			}
			else
			{
				graph->remove_dependency(c_name, temp_cell); // If circular dependency was caused, remove pair from graph
			}
			
			getting_cell = false;
		}

		temp_cell = ""; // Reset temp_cell for possibly finding another cell in these contents

		// If we got to this point, getting_cell is not true and we have not gotten a letter or a number
		// So we just continue iterating until we get another letter!


	}

	// SEE IF THERE WAS ONE LAST CELL NAME

	if (getting_cell && temp_cell != "")
	{
		if(add)
		{
			// record found cell just in case dependency is found in another pair for this equation
			cells_found.insert(temp_cell);

			// Update dependency graph
			graph->add_dependency(c_name,temp_cell);

			// See if update created circular dependency
			if(circular_dependency(c_name, temp_cell)) // Recursively check if this causes a circular dependecy. If so, return false
			{
				cout << "CIRCULAR DEPENDENCY !!!!!" << endl;
				// Remove all dependencies made in this equation
				for (std::set<string>::iterator itt=cells_found.begin(); itt!=cells_found.end(); ++itt)
					graph->remove_dependency(c_name, *itt); // If circular dependency was caused, remove pair from graph
				
				return false;  // Report circular dependency error
			}

		}
		else
			graph->remove_dependency(c_name, temp_cell); // If circular dependency was caused, remove pair from graph
		

	}

	return true;

}

/* Returns false if NO circular dependency.
 * Returns true if circular dependency. */
bool sheet::circular_dependency(std::string l, std::string r) 
{

	if (l == r) // If strings are the same, return true -- circular dependency
		return true;

	// Base case
	if(!graph->has_dependees(l))
		return false; // No circular dependency

	std::set<string> * temp_set = graph->get_dependees(l);  // Get dependees of r (should already include l)

 	std::set<string>::iterator it = temp_set->find(r); 
    if (it != temp_set->end())  // If iterator is not at end of set, r was found
    	return true; // If a dependee of l is r, circular dependency

	// recursively do this, for all dependees in temp_set
	for (std::set<string>::iterator itt=temp_set->begin(); itt!=temp_set->end(); ++itt)
	{
		if(circular_dependency(*itt, r))
	 		return true; // Circular dependency!
	}

	// If we made it through checking all dependees, and never found r,
	// Return false! :) -- no circular dependency
	return false;

}


/* 
 * Makes sure a formula is properly defined, by 
 * checking that each of the dependent cells in the
 * formula are not strings or empty--dependent cells must have quantifyable contents */
bool sheet::defined_formula(std::string c_name, std::string c_content)
{
	std::set<string> cells_found;
	bool getting_cell = false;
	bool cont = false;
	std::string temp_cell = "";

	std::string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	std::string numbers = "0123456789";

	// Can loop through the cell contents
	for(int i = 0; i<c_content.size(); i++)
	{

		// If first character of content isn't '=', return true becuase this isn't a formula
		if ((i == 0) && (c_content[i] != '='))
			return true;
		else if (i == 0) // Skip =
			continue;
			
		// Check to see if this character is an alphabetical character
		for(int a = 0; a<alphabet.size(); a++)
		{
			if(c_content[i] == alphabet[a])
			{
				//If it has, we have reached a cell name.
				getting_cell = true;
				cont = true;
				temp_cell += c_content[i];
				break; // Break out of the alphabet loop and go to next character because you found letter
			}

		}

		// If this was a letter, continue,
		if (cont)
		{
			cont = false;
			continue;
		}
			
		// If we get to this point, and getting_cell is true, we will have a number as part of the 
		// cell name, or we will hit a symbol and will be done with this cell name

		// See if current character is a number
		if(getting_cell)
		{
			for(int n = 0; n<numbers.size(); n++)
			{
				if(c_content[i] == numbers[n]) // Number!
				{
					cont = true;
					temp_cell += c_content[i]; // Add to cell name
					break; // break out of number loop
				}

			}
		}

		// If this was a number, continue
		if (cont)
		{
			cont = false;
			continue;
		}

		// If we get to this point, and getting_cell is true, we have neither a character or a number,
		// which means we must have a symbol. Which means we have collected a whole cell name.

		// Check that cell is quantifiable and continue iterating through cell contents
		if (getting_cell)
		{
			bool cell_found = false;

			for (map<string,string>::iterator it = sheet_state->begin(); it !=  sheet_state->end(); it++)
    		{
    			if (it->first == temp_cell) // cell is at least defined!
    			{
    				cell_found = true;
    				string cont = it->second; // get temp_cell content

    				
					for(int n = 0; n<cont.size(); n++) // For each character in contents...
					{
						bool number = false;
						for(int x = 0; x<numbers.size(); x++)
						{
							if(cont[n] == numbers[x]) // Number!
							{
								number = true;
								break; // break out of number loop
							}

						}
						if (!number && cont[0]!='=')
							return false; // This character of contents was not a number and this is not a formula!
						else 
							number = false; // reset for next character of contents
					}

    				
    			}
    		}

    		if (!cell_found)
    			return false; // Cell is not defined, not a valid formula!

			getting_cell = false;
		}

		temp_cell = ""; // Reset temp_cell for possibly finding another cell in these contents

		// If we got to this point, getting_cell is not true and we have not gotten a letter or a number
		// So we just continue iterating until we get another letter!


	}

	// SEE IF THERE WAS ONE LAST CELL NAME

	if (getting_cell && temp_cell != "")
	{
		bool cell_found = false;

		for (map<string,string>::iterator it = sheet_state->begin(); it !=  sheet_state->end(); it++)
		{
			if (it->first == temp_cell) // cell is at least defined!
			{
				cell_found = true;
				string cont = it->second; // get temp_cell content

				
				for(int n = 0; n<cont.size(); n++) // For each character in contents...
				{
					bool number = false;
					for(int x = 0; x<numbers.size(); x++)
					{
						if(cont[n] == numbers[x]) // Number!
						{
							number = true;
							break; // break out of number loop
						}

					}
					if (!number && cont[0]!='=')
						return false; // This character of contents was not a number and this is not a formula!
					else 
						number = false; // reset for next character of contents
				}

				
			}
		}

		if (!cell_found)
			return false; // Cell is not defined, not a valid formula!

	}

	return true;

}
