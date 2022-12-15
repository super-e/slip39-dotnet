WARNING! THIS IS A PRE-RELEASE VERSION, FULL OF BUGS TO SAY THE LEAST! IT DOES NOT EVEN ACCOMPLISH THE MOST BASIC TASK: DO NOT USE!

# slip39-dotnet

The aim of this small project is to create a SLIP-39 (as defined in https://github.com/satoshilabs/slips/blob/master/slip-0039.md) implementation in C#, build with target .NET Standard 2.0 for very wide compatibility with different version of the .NET ecosystem. This includes .NET Core 2.0 to .NET 7.0, .NET Framework 4.6.1+, Mono 5.4+, Xamarin.Android 8.0+, Xamarin.iOS 10.14+, UWP 10.0.16299+, Unity 2018.1+

I decided to write from scratch the code base instead of porting existing implementations available already for JavaScript, Dart, and more. My main focus is on adherence to the official SLIP39 specifications and code readibility/understandability. Performance is not high in my priorities so you are more likely to see high-level functions that abstract the mathematical concepts rather than bit-wise operations. This might change in the future if performance will have a meaningful negative impact on the usability of this library.

This is my first OpenSource project, any help or suggestion is highly appreciated.
