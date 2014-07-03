default:	spreadsheet_server.cpp client.o sheet.o cell.o dependency_graph.o
	g++ spreadsheet_server.cpp client.o sheet.o cell.o dependency_graph.o /usr/local/lib/libboost_*.a  -I/usr/local/include/ -lpthread -lboost_system -lrt

client.o:	client.cpp client.h sheet.o
	g++ -c client.cpp sheet.o

sheet.o:	sheet.cpp sheet.h cell.o dependency_graph.o
	g++ -c sheet.cpp cell.o dependency_graph.o

cell.o:	cell.cpp cell.h
	g++ -c cell.cpp

dependency_graph.o:	dependency_graph.cpp dependency_graph.h
	g++ -c  dependency_graph.cpp

