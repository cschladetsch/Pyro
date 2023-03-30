# Networking

_Pyro_ networking is inherently peer-to-peer (P2P). Any _Peer_ can act as a client or a server to another _Peer_.

## Peers

There is no idea of Client or Server in the context of Pyro's Network system. Any _Peer_ can act as a client or a
server. A _Peer_ is a process that can communicate with other _Peers_.

## Proxies

_Proxies_ are local representations of remote objects. When you call a method on a _Proxy_, the _Proxy_ will send a
message to the remote object, and wait for a reply. The _Proxy_ will then return the result of the method call to you.

## Agents

_Agents_ are local representations of local objects. When you call a method on an _Agent_, the _Agent_ will send a
message to the local object, which can then be sent to the requesting _Proxy_ on a remote _Peer_.

## NetworkDomain

The _NetworkDomain_ is the conceptual central point of the network system. No _Peer_ is created by default. You must
create a _Peer_ yourself, and then use the _Peer_ to create _Proxies_ and _Agents_.

## NetworkKernel

A _NetworkKernel_ is a _Peer_ that can be used to provide a central point of coordination for a network of _Peers_.
Peers must register themselves with a NetworkKernel in order to be able to coordinate with each other.

Note that there is no need for a NetworkKernel to be used. You can create a network of _Peers_ without a
_NetworkKernel_. It is only useful if you want to use the _NetworkKernel_ to coordinate the _Peers_.

## NetworkGenerator

This is a commandline tool that can be used to generate base classes for specific _Proxies_ and _Agents_.

Tau is an Interface Definition Language (IDL) for the Pyro Network system.

It takes _Tau_ files as input, and generates C# code as output for both Proxies and Agents.

## Example

For the moment, the [UnitTests](../../Test/TestNetwork) are the best examples.

## TODO

Everything.


