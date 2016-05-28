# NintenTools

This is a collection of tools to handle typical Nintendo file formats, wrapping them in object oriented libraries and add-ons for programs to handle them, closely trying to resemble what the original Nintendo tools might have provided to create the files.

Most specifically, the BFRES file format is focused by writing a C# library and a Blender add-on, based on the documentation of the Custom Mario Kart 8 Wiki and own research.

Usage
=====

Right now, the following usage cases are possible:
- Load and inspect BFRES files.
- Decompress Yaz0 files.
- C# "Test" tool: Validates BFRES with researched details. It prints any assumptions not met as a warning.

Contributing
============

You're welcome to contribute. Let it be just raw BFRES file information which I implement then, or offer your own pull requests.

Expect some bigger code shifting to happen in the future, especially when I implement modification / saving of some file formats, so don't rely on a solid code foundation yet.

About code style: All Microsoft C# and common Python guidelines, max. line width of 120 characters. For C#, document everything except private stuff (it's nice to add research notes to some basic documentation later on).

License
=======

<a href="http://www.wtfpl.net/"><img src="http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl.svg" height="20" alt="WTFPL" /></a> WTFPL

    Copyright © 2016 syroot.com <admin@syroot.com>
    This work is free. You can redistribute it and/or modify it under the
    terms of the Do What The Fuck You Want To Public License, Version 2,
    as published by Sam Hocevar. See the COPYING file for more details.
