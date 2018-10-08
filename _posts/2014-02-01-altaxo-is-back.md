---
layout: post
title:  "Altaxo is back!"
date:   2014-02-01 23:00:00 +0200
categories: 
---

### After a long period of development, Altaxo is back! 

The most important news is that now you can embed your graphs 
into MS Word, PowerPoint etc. via COM. 

It took me two thick books and a lot of internet searching 
to implement this, and at first I thought it wouldn't be possible 
at all using .NET and WPF. Beside this feature, all graphical items 
can now be put in any z-order (this means you can bring them on top, 
or hide them below other items). 
Per default any graph now contains at least two layers: 
one root layer RL (the outer boundaries of the graph), 
and the x-y plot layer L0. 
You now are able to position your graphical items either in 
absolute coordinates as it was before), 
or relative to the parent layer, 
and you can choose the anchor point for both the graphical item and 
the parent... 

**Attention developers**: has anyone made a **successfull connection to a COM local server** 
running as **64 bit WPF** process? You? Then please mail me how to do it! I could only get a connection if WPF runs as 32 bit process.
