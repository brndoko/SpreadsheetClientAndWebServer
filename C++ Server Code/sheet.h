#ifndef SHEET_H
#define SHEET_H

#include<map>
#include<stack>
#include "cell.h"
#include "dependency_graph.h"
#include <string>

class sheet
{	
private:
	int version; // The current version of this spreadsheet
	std::string file_name; // The file name of this spreadsheet
	std::map<std::string, std::string> * sheet_state; // Holds current sheet state (populated cells and their contents)
	std::stack<cell*> * changes; // Keeps track of cell changes by recording state of every cell before it is changed
	cs3505::dependency_graph * graph;
	void open(); // Populates sheet_state map from xml file
	bool dependency_work(bool add, std::string c_name, std::string c_content); // Updates graph and checks for dependencies
	bool circular_dependency(std::string l, std::string r); // Returns true if two strings will cause circular dependency
	bool defined_formula(std::string c_name, std::string c_content); // Returns true if a formula is comprised of cells with defined, quantifiable contents

public:
	sheet(std::string f);
	~sheet();
	int update(std::string version_number, std::string cell_name, std::string cell_content); // Updates version by changing cell and incrementing version member
	std::string undo(std::string version_number); // Undoes last change 
	void save(std::string version_number); // Saves current state of spreadsheet to xlm file
	std::map<std::string, std::string> * get_active_cells();
	std::string get_version();
};

#endif
