#include "sheet.h"
#include <iostream>

using namespace std;

int main()
{
	cout << "Compiled." << endl;
	sheet s("sheet_1.txt");
	sheet l("sheet_2.txt");
	l.update("","C333","=1+1");
	l.update("","C334","happy=");
	l.update("","C25","go");
	l.undo("");
	l.save("");

	cout << "CIRCULAR DEPENDENCY CHECKS" << endl;
	cout << s.update("","H2","2") << endl;
	cout << s.update("","H6","=4") << endl;
	cout << s.update("","H3","5") << endl;
	cout << s.update("","H1","=H2+3+H6") << endl;
	cout << s.undo("") << endl;
	cout << s.update("","H2","=H3+4") << endl;
	cout << s.update("","H3","=H4") << endl;
	cout << s.update("", "H3", "=H10+H15+H1") << endl;

}