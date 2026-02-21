Bodoconsult.App.ReactiveUi
===========================

# Overview

## What does the library

Bodoconsult.App.ReactiveUi is a library with basic functionality for multilayered monolithic Avalonia based applications. 

## How to use the library

The source code contains NUnit test classes the following source code is extracted from. The samples below show the most helpful use cases for the library.

# App start infrastructure basics

See page [app start infrastructure](../Bodoconsult.App/AppStartInfrastructure.md) for details.

# Basic ideas for platform independent view models based on ReactiveUI

-   Instead of storing colors in color classes dependent on platforms (System.Drawing.Media, ...), store colors as HTML string (like #FFFFFF) and use converters to get the requested color class instance from.



# About us

Bodoconsult <http://www.bodoconsult.de> is a Munich based software company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.