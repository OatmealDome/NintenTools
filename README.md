# NintenTools

C# Framework to load Nintendo file formats into an object-oriented hierarchy, closely trying to resemble what the original Nintendo tools might have provided to compile the files.

Most specifically, the BFRES file format is focussed, based on the documentation of the Custom Mario Kart 8 Wiki.

The upcoming release features loading of the BFRES file, while saving functionality will be added later (at least in limited ways, as some parts are not yet fully reversed to allow completely new BFRES files from scratch).

Usage
=====

Right now, the following usage cases are possible:
- Use it as a managed .NET 4.5 library to load and inspect Nintendo files (only BFRES right now). Add the assembly to your references, and start programming with it.
- The "Test" tool validates BFRES at the moment and if any researched details meet what is seen in the wild. It prints any assumptions not met as a warning.

Contributing
============

At first, you're welcome to contribute. But be warned: This is probably one of the libraries so uselessy over-engineered that only I like the weird way it's written. If you like this way or want to torture yourself a little, that should be no problem. 
If you want stuff to get merged, please follow the code style I used until now. A quick read over existing code mostly makes clear that it matches almost all the Microsoft C# guidelines, with a max. line width of 120 characters. Document everything except private stuff, even if the comments might seem repetitive. It's nice to add research notes to these later on.

Other than that, I mostly have to say:
- Expose only stuff required to modify a file or section.
- Put internal stuff into a subclass (this is probably the most ugly thing here, but it hides implementational boredom from users), but keep it public for those people who want to analyze the structures.
- Keep it as object-oriented as possible, yet simple (haha, sounds like one of these stupid things an employer forces from you).

Expect some big code shifting to happen in the next time, especially when I implement real modification / saving of some file formats, so don't rely on a solid code foundation yet (yeah, that's just the mentioned weird way I work).

License
=======

<a href="http://www.wtfpl.net/"><img src="http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl.svg" height="20" alt="WTFPL" /></a> WTFPL

    Copyright © 2015 vibeware.net <admin@vibeware.net>
    This work is free. You can redistribute it and/or modify it under the
    terms of the Do What The Fuck You Want To Public License, Version 2,
    as published by Sam Hocevar. See the COPYING file for more details.
