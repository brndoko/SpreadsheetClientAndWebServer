

/*
 * Brandon Koch, U0621456, CS3505
 * 
 * Class definition for a dependency graph.
 * A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
 * (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
 * (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
 * set, and the element is already in the set, the set remains unchanged.)
 * 
 * Given a DependencyGraph DG:
 * 
 *    (1) In the dependency pair (s,t), s is called a dependee.
 *        
 *    (2) In the dependency pair (s,t), t is called a dependent.
 *
 * For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
 *     dependents("a") = {"b", "c"}
 *     dependents("b") = {"d"}
 *     dependents("c") = {}
 *     dependents("d") = {"d"}
 *     dependees("a") = {}
 *     dependees("b") = {"a"}
 *     dependees("c") = {"a"}
 *     dependees("d") = {"b", "d"}
 *
 */

#include <iostream>
#include <string>
#include <map>
#include <stack>
#include <sstream>
#include <set>
#include "dependency_graph.h"

using namespace std;


// Mandatory to have a main for any C++ function - leave commented out for tester.
//int main()
//{
  //return 0;
//}

namespace cs3505
{

  /*
   * Creates an empty dependency_graph.
   *
   */
  dependency_graph::dependency_graph ()
  {
    this->dependees = new std::map<std::string, std::set<string> *>; // Dependees with lists of their dependents
    this->dependents = new std::map<std::string, std::set<string> *>; // Dependents with lists of their dependees
  }

  dependency_graph::~dependency_graph()
  {
    delete dependees;
    delete dependents;
  }


  /*
   * Reports whether dependent(s) is non-empty.
   * In other words, is s a dependee?
   */
  bool dependency_graph::has_dependents (std::string s)
  {
    // Check to see if this cell has dependents by cycling through the strings in the dependents map
    std::map<std::string, std::set<string> *>::iterator it = dependees->find(s);
    
    // If iterator is at end of map, dependee s was never found and therefore has no dependents
    if (it == dependees->end()) 
      return false; 

    // If iterator is not at end of map, dependee s is in dependee map and therefore has dependents
    return true;

  }

  /*
   * Reports whether dependee(s) is non-empty.
   * Reports whether string s is a dependent (i.e. it had dependees)
   *
   */
  bool dependency_graph::has_dependees (std::string s)
  {
    // Check to see if this cell has dependees by cycling through the strings in the dependents map
    std::map<std::string,std::set<string> *>::iterator it = dependents->find(s);
    
    // If iterator is at end of map, s is not a dependent and therefore it has no dependees
    if (it == dependents->end()) 
      return false; 

    // If iterator is not at end of map, s a dependent and therefore it has dependees
    return true;
  }

  /*
   * Enumerates dependent(s)
   * Gets the list of dependents for the dependee s
   */
  std::set<string> * dependency_graph::get_dependents(std::string s)
  {
    // Cycle through the keys in the dependees map and return the set of dependents if one is found
    for (std::map<std::string,std::set<string> *>::iterator itr = dependees->begin(); itr !=  dependees->end(); itr++)
    {
      if (itr->first == s) // If dependee is found, return its dependents
      {
        return itr->second;
      }
    }

    // Return empty set if dependee does not exist
    std::set<string> * new_set = new set<string>();
    return new_set;
  }

  /*
   * Enumerates dependee(s)
   * Gets the list of dependees for the dependent s
   */
  std::set<string> * dependency_graph::get_dependees (std::string s)
  {
    // Cycle through the keys in the dependents map and return the set of dependees if one is found
    for (std::map<std::string,std::set<string> *>::iterator itr = dependents->begin(); itr !=  dependents->end(); itr++)
    {
      if (itr->first == s) // If dependent is found, return its dependees
      {
        return itr->second;
      }
    }

    // Return empty set if dependent does not exist
    std::set<string> * new_set = new std::set<string>();
    return new_set;
  }

  /*
   * Adds the ordered pair (s,t), if it doesn't exist.
   * s is dependee
   * t is dependent
   */
  void dependency_graph::add_dependency(std::string s, std::string t) 
  {
    std::cout << "IN DEPENDENCY GRAPH -- ADDING DEP PAIR -- " << s << " " << t << std::endl;

    // -------------------------------- First , update dependee map.

    if (!has_dependents(s)) // If s is not yet a dependee -- it does not already have a list of dependents
    {
        std::set<string> * new_lineset = new std::set<string>(); // Create new lineset for dependents of s
        new_lineset->insert(t); // Add dependent t to that lineset
        // Create a new dependency of s on t
        dependees->insert(std::pair<std::string,std::set<string> *>(s, new_lineset));
    }
    else // Otherwise, s is already a dependee with a list of dependents
    {
      // Get a pointer to the set of dependents of s
      std::set<string> * set_pointer = this->get_dependents(s);
      set_pointer->insert(t);
    }

    // -------------------------------- Next, update dependents map.

    if (!has_dependees(t)) // If t is not yet a dependent -- it does not already have a list of dependees
    {
        std::set<string> * new_lineset = new std::set<string>(); // Create new lineset for dependees of t
        new_lineset->insert(s); // Add dependee s to that lineset

        dependents->insert(std::pair<std::string,std::set<string> *>(t, new_lineset));
    }
    else // Otherwise, t is already a dependent with a list of dependees
    {
      // Get a pointer to the lineset of dependees of t
      std::set<string> * set_pointer = this->get_dependees(t);
      set_pointer->insert(s);
    }

  }

  /*
   * Removes the ordered pair (s,t), if it exists.
   */
  void dependency_graph::remove_dependency(std::string s, std::string t) 
  {
    std::cout << "IN DEPENDENCY GRAPH -- REMOVING DEP PAIR -- " << s << " " << t << std::endl;


    //-----------------------------------------First update dependees
    if(has_dependents(s)) // If dependee s exists (if it has dependents)
    {
      // Get reference list to dependents for dependee s
      std::set<string> * set = this->get_dependents(s);

      // Remove dependent t
      set->erase(t);

      // If there are no more dependents for dependee s, remove s from dependee list
      if (set->size() == 0)
        dependees->erase(s);
    }


    //-----------------------------------------Next update dependents
    if(has_dependees(t)) // If dependent t exists (if it has dependees)
    {
      // Get reference list to dependents for dependee s
      std::set<string> * set = this->get_dependees(t);

      // Remove dependee s
      set->erase(s);

      // If there are no more dependees for dependent t, remove t from dependent list
      if (set->size() == 0)
        dependents->erase(t);
    }

  }
}