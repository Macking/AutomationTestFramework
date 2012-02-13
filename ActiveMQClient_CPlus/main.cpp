#include <activemq/library/ActiveMQCPP.h>
#include <activemq/core/ActiveMQConnectionFactory.h>
#include <cms/Connection.h>
#include <cms/Session.h>
#include <cms/TextMessage.h>
#include <cms/ExceptionListener.h>
#include <cms/MessageListener.h>
#include <stdlib.h>
#include <stdio.h>
#include <iostream>
#include <memory>
#include <vector>
#include <fstream>
#include <string>
#include <algorithm>
#include <iterator> 
#include <sstream>

using namespace activemq::core;
using namespace cms;
using namespace std;


class ConsumerSample : public ExceptionListener, public MessageListener {
private:
    Connection* _connection;
    Session* _session;
    string _brokerURI;
    vector<Destination*> destinations;
    vector<MessageConsumer*> consumers;
	//log the number of received activeMQ messages
	int numMessage;

public:
	ofstream oFile;


    ConsumerSample(const string& brokerURI_)
        :_connection(NULL), _session(NULL), _brokerURI(brokerURI_) {
        // empty
			numMessage=0;
    }

    virtual ~ConsumerSample() {
        cleanup();
    }

    void subscribe(const vector<string>& messageTypes_) {
        try {
        // Create a ConnectionFactory
        auto_ptr<ConnectionFactory> connectionFactory(
            ConnectionFactory::createCMSConnectionFactory(_brokerURI));
        // Create a Connection
        _connection = connectionFactory->createConnection();

        _connection->setExceptionListener(this);

        // Create a Session
         _session = _connection->createSession(Session::AUTO_ACKNOWLEDGE);

         // Create consumers
         for (vector<string>::const_iterator iter = messageTypes_.begin();
             iter != messageTypes_.end(); ++iter) {
            Destination* destination = _session->createTopic(*iter);
            MessageConsumer* consumer = _session->createConsumer(destination);
            consumer->setMessageListener(this);

            destinations.push_back(destination);
            consumers.push_back(consumer);
         }
// start the connection after subscribe all topics
		         _connection->start();

        } catch (CMSException& e) {
            // Log the error
            cerr << "Opps! A exception happened: " << e.getMessage() << endl;
        }
    }

    // Called from the consumer since this class is a registered MessageListener.
    virtual void onMessage( const Message* message ) {
        try
        {
            const TextMessage* textMessage =
                dynamic_cast< const TextMessage* >( message );
            string text = "";

            if( textMessage != NULL ) {
                text = textMessage->getText();
                const Topic* destination = dynamic_cast<const Topic*>(textMessage->getCMSDestination());
                destination->getTopicName();

                // Do something with the text, like:
                printf("Number of received message is %d\n",numMessage);
				printf( "Message Received, messageType: %s, message content: %s\n",destination->getTopicName().c_str(), text.c_str() );
				
				// output message into log file

				//open the log file
oFile.open("log_PAS.txt",ios::app);

	//move point to the end of the file, append information at the  end of the log file
	oFile.seekp(0,ios::end);

vector<string> logStr3;
logStr3.push_back("\nMessage Received, messageType: ");
logStr3.push_back(destination->getTopicName().c_str());
logStr3.push_back("\n message content: ");
logStr3.push_back(text.c_str());
logStr3.push_back("\n Number of received Message are : ");
std::stringstream strNum;
strNum << numMessage;
logStr3.push_back(strNum.str());
	copy(logStr3.begin(),logStr3.end(),ostream_iterator<string>(oFile));
//close the log file
	oFile.close();
	numMessage++;



            } else {
                // Log the error
                cerr << "Received a non-text message" << endl;
            }

        } catch (CMSException& e) {
            // Log the error
            cerr << "Opps! B exception happened: " << e.getMessage() << endl;
        }
    }

    // If something bad happens you see it here as this class is also been
    // registered as an ExceptionListener with the connection.
    virtual void onException( const CMSException& ex AMQCPP_UNUSED) {
        // Log the error
        cerr << "Opps! C exception happened: " << ex.getMessage() << endl;
    }

    void cleanup() {
        // Destroy resources.
        for (vector<Destination*>::iterator iter = destinations.begin();
            iter != destinations.end(); ++iter) {
                delete *iter;
                *iter = NULL;
        }
        for (vector<MessageConsumer*>::iterator iter = consumers.begin();
            iter != consumers.end(); ++iter) {
                delete *iter;
                *iter = NULL;
        }
        // Close open resources.
        try {
            if( _session != NULL ) _session->close();
            if( _connection != NULL ) _connection->close();
        } catch (CMSException& e) { 
            // Log the error
            cerr << "Opps! D exception happened: " << e.getMessage() << endl;
        }

        // Now Destroy them
        try {
            if( _session != NULL ) delete _session;
        } catch (CMSException& e) { 
            // Log the error
            cerr << "Opps! E exception happened: " << e.getMessage() << endl;
        }
        _session = NULL;

        try {
            if( _connection != NULL ) delete _connection;
        } catch (CMSException& e) {
            // Log the error
            cerr << "Opps! F exception happened: " << e.getMessage() << endl;
        }
        _connection = NULL;
    }
};


int main(char logFile,char URI, char* argv[]) {
    activemq::library::ActiveMQCPP::initializeLibrary();

    cout << "=====================================================\n";
    cout << "Starting the sample:" << endl;
    cout << "-----------------------------------------------------\n";
    


// set the stomp server destination

    string brokerURI = "tcp://10.112.39.246:61616";
    vector<string> messageTypes;

	//topic of appState
    messageTypes.push_back("topic.appStateClosed");
	messageTypes.push_back("topic.appStateCreated");
	messageTypes.push_back("topic.appStateMinimized");
	messageTypes.push_back("topic.appStateRestored");
	messageTypes.push_back("topic.appStateBackToDPMS");

	//topic of acquisition
    messageTypes.push_back("topic.acquisitionCompleted");
	messageTypes.push_back("topic.acquisitionFailed");

	//topic of image
    messageTypes.push_back("topic.imageCreated");
	messageTypes.push_back("topic.imageDeleted");

	//topic of presentation State
	messageTypes.push_back("topic.presentationStateModified");
	messageTypes.push_back("topic.presentationStateDeleted");

	//topic of Analysis
	messageTypes.push_back("topic.analysisCreated");
	messageTypes.push_back("topic.analysisModified");
	messageTypes.push_back("topic.analysisDeleted");

	//topic of FMS
	messageTypes.push_back("topic.fmsCreated");
	messageTypes.push_back("topic.fmsModified");
	messageTypes.push_back("topic.fmsDeleted");

	//topic of Volume
	messageTypes.push_back("topic.volumeCreated");

	//topic of Patient
	messageTypes.push_back("topic.patientDeleted");

	//topic of postImportCompleted
	messageTypes.push_back("topic.postImportCompleted");

	//topic of Report
	//messageTypes.push_back("topic.reportCreated");
	//messageTypes.push_back("topic.reportModified");
	//messageTypes.push_back("topic.reportDeleted");



    // Subscribe the concerned message types
    ConsumerSample consumer(brokerURI);

	//logFile = "E:\VistudioProjects\PAS\ActiveMQClient\ActiveMQClient_CPlus\Debug\log_PAS.txt";

	//sample try to write log into a file
	//ofstream oFile;
	//if log file exist, open the file
	//consumer.oFile.open("log_PAS.txt",ios::app);

	//move point to the end of the file, append information at the  end of the log file
	//consumer.oFile.seekp(0,ios::end);

    consumer.subscribe(messageTypes);


// Auto program test, use sleep time to control waiting duration and quit


	//Manual test to use quit input to let program quit
    char input = 0;
    while (input != 'q') {
        cin >> input;
    }

    cout << "-----------------------------------------------------\n";
    cout << "Finished with the example." << endl;
    cout << "=====================================================\n";

    activemq::library::ActiveMQCPP::shutdownLibrary();



    system("pause");
    return 0;
}