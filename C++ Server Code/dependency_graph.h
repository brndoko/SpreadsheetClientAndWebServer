/*
 * Brandon Koch, U0621456, CS3505
 * 
 * Class header for a dependency graph.
 * A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
 * (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
 * (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
 * set, and the element is already in the set, the set remains unchanged.)
 * 
 * Given a DependencyGraph DG:
 * 
 *    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
 *        
 *    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
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

#ifndef DEPENDENCY_GRAPH_H

#define DEPENDENCY_GRAPH_H

#include <string>
#include <map>
#include <stack>
#include <set>

namespace cs3505
{
	/*
	 * Class header for big_number class which utilizes a string as its internal storage for positive integers.
	 * Gives the capability to perform basic arithmetic  with +, -, *, /, %, =
	 */
	class dependency_graph
	{
		private:
			std::map<std::string, std::set<std::string> *> * dependees;
			std::map<std::string, std::set<std::string> *> * dependents;

		public:
			dependency_graph (); // Creates an empty dependency_graph.
			~dependency_graph(); // Destructor for dependency_graph
			bool has_dependents (std::string s); // Reports whether dependent(s) is non-empty.
			bool has_dependees (std::string s); // Reports whether dependee(s) is non-empty.
			std::set<std::string> * get_dependents(std::string s); // Enumerates dependent(s)
			std::set<std::string> * get_dependees (std::string s); // Enumerates dependee (s)
			void add_dependency(std::string s, std::string t); // Adds the ordered pair (s,t), if it doesn't exist.
			void remove_dependency(std::string s, std::string t); // Removes the ordered pair (s,t), if it doesn't exist.
	};
}
#endif