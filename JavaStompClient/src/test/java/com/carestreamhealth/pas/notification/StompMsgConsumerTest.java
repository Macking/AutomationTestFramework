package com.carestreamhealth.pas.notification;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import javax.security.auth.login.LoginException;

import net.ser1.stomp.Client;
import net.ser1.stomp.Listener;

import org.apache.log4j.Logger;
import org.junit.Test;


public class StompMsgConsumerTest {

	public static void main(String[] args) throws InterruptedException, LoginException, IOException {
		Client c = new Client("10.112.37.242",2077,null,null);
		c.addErrorListener(new Listener(){
			public void message (Map header,String message){
				//TO-DO;
			}
		});
		HashMap headers = new HashMap();
		headers.put( "CONNECTED", "my-receipt-123" );
		headers.put("CONNECT","");

Listener my_listener = new Listener(){
	public void message (Map header,String message){
		//TO-DO;
	}
};

		c.subscribe("APP_STATE",my_listener);
		
		//c.send( "channel", "Hello World", headers );

		
	}

}
