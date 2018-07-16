# Facades Namespace #

Facades namespace, as the name sugest, contains API facades to a set of diferent cross funcionalities that are usually implemented in backend systems.

The Namespaces that will be make available will be: 

[x] Serialization
[ ] Messaging Patterns
[ ] Repository Patterns

## What is the need of this ? ## 

There are out there many implementations of many software patterns and design patterns, that despite they have the same concepts the do not share one common 
interface. 
The goal of this namespace is to provide a set of interfaces that make available APIs common to all patterns implementations, for instance, serialization pattern can be performed
with the API available on the ISerializer interface but the implementation is diferent per format or library. 

