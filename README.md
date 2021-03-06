![Build](https://github.com/Timoms/LibICAP/workflows/.NET%20Core/badge.svg)
# LibICAP - An easy to use C# ICAP library (WIP)

### When scam and fraud are more and more present these days we need a solution.
##### This library wont provide much features by now - but needs your help!

## What's that? 
LibICAP is a simple and easy to use ICAP library.  
ICAP is a protocol (Internet Content Adaption Protocol) which targets endpoint protection devices.  

Thus this server should be used with a device that understands ICAP.  
This library is currently developed with the **c-icap-client** and a **FortiGate 100D**.

## Network Setup

![Diagram](https://raw.githubusercontent.com/Timoms/LibICAP/master/Visuals/ICAP_Diagram.png?token=AGLLVNTSENEP63NKRPWOKZ27GA2UQ)

ICAP should be used as an intermediate between your endpoint protection and client device.  
The firewall will communicate with the LibICAP server and asks two simple "questions":
* REQMOD: Is this URL okay? The client wants to access example.com.
* RESPMOD: example.com has returned a weird message. Could you please check that?

This flow allows the firewall to "outsource" the network checks to a server with the LibICAP installed.  
 
## Features

Currently im working on getting the [IETF Implementation RFC3507](https://tools.ietf.org/html/rfc3507) to run without any problem.  
This includes the REQMOD and RESPMOD (and of course the OPTIONS) requests.  

The TCP server should also handle an unlimited* amount of clients so a threaded implementation will follow.
##### * unlimited is limited by the OPTIONS request or the endpoint protection unit you are using - and of course the hardware on which LibICAP is running.

## How you can help?

You can help by implementing the basic RFC3507.  
This will be a great step in the right direction.  

If LibICAP reaches the full-implementation we can go further and include WebFilter lists and an orchestrator.  
This will result into a full-functional WebFilter, Anti-Virus and maybe even Ad-Removing server solution! 