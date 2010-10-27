# FreeDB Scanner

This is an exploration into multi threaded techniques using C# and C++.

## Why FreeDB?

FreeDB is an online database with CD information. You can download large
text archives and this is the sole reason I chose FreeDB. For testing I
have used an update archive (which is of reasonable size). You can find them
[here](http://ftp.freedb.org/pub/freedb). The one I used was
[freedb-update-20080201-20080301.tar.bz2](http://ftp.freedb.org/pub/freedb/freedb-update-20080201-20080301.tar.bz2).

## The Problem Statement

The problem statement is to find the year when the most number of CDs were
published. In order to do so you need to scan a large number of files for
lines that look like this:

    DYEAR=1992

Do this as quickly as possible, sum up the number of CDs per year, and finally
output the year that occurred most often.

## Sample Run

Here's a sample run from my C# version:

	freedb-cs.exe C:\Users\Daniel\Downloads\freedb-update
	Enumerating files...
	Found 32229 files
	Scanning files...
	...Done
	Year 2007: 5071
	Processing took 1.39 s

And here is the same run from the C++ version:

	freedbpp.exe C:\Users\Daniel\Downloads\freedb-update
	Enumerating files...
	0.88 s

	Found 32229 files
	Scanning files...
	Year 2007: 5071
	2.49 s

## Challenge

The challenge for you is to try and beat these times. This is an opportunity to
learn more about I/O and multicore processing. Let me know if you improve the above results!

Good luck!
