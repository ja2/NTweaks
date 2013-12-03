NTweaks 
=======

A .Net extension library: Additional classes to fill frequently found gaps in the main framework libraries and functionality to streamline tasks that commonly occur when working in .Net.

The latest version of NTweaks is available on [Nuget here](https://www.nuget.org/packages/NTweaks).


Notes
-----

_Namespaces_

The classes are arranged into Namespaces to match the namespacing in the core framework (so Generic collections are in NTweaks.Collections.Generic for example). The exception to this is NTweaks.Sys, which contains extensions related to functionality in the main System namespace in .Net. This is so that NTweaks doesn't get in the way when you're not using anything in the NTweaks.Sys namespace.

The same namespacing pattern is used in the NTweaks.Tests and NTweaks.Sample projects.


_Versions_

The versioning will be simplified a little from the main .Net versioning setup. A typical version number is 1.0.0.2. Only the first and last portions of the version number will be used. For minor changes and bug fixes, they will be point releases (so 1.0.0.3 next). For additional classes added to the library, the library will move up to the next major version.

This should make clear the main focus of versions as they are released.


_Documentation_

At present the documentation is in the form of the open source code, and unit tests, plus any samples as they are added. At some point there will be more full documentation written as the main JA2 website gets up to speed.


Classes included
----------------

At present the library includes the following (should be a fairly complete list):


- `NTweaks.Sys.EquatableBase`: Provides simplified complete implementation of IEquatable (including operators).
- `NTweaks.Sys.EquatableAuto`: Provides an automatic implementation of IEquatable using attributes to denote included properties.
- `NTweaks.Collections.Generic.Map`: A bi-directional dictionary implementation.
- `NTweaks.Collections.Generic.Lookup`: A one-many dictionary implementation.
- `NTweaks.Collections.Generic.LookupMap`: A bi-directional one-many dictionary implementation (so is effectively many-many).





