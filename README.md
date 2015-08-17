Pedal Builder 2.0.0
=========

###A tool for DIY guitar pedal builders to help organize and order components.###

DIY guitar pedals are fun to build and use.  I found that I would often miss some components when placing an order, and I didn't have a nice way to organize the components I needed to order if I was building multiple pedals, so I built Pedal Builder.

Blog post [here](http://robotabot.github.io/articles/pedal-builder/)

The first version used WinForms and SQLite, while this version is built with WPF and SQL Server CE.  Version 2 also makes use of [Mahapps.Metro](http://mahapps.com/) for UI styling and behavior.

####Features####

* Add, edit, or remove pedals
* Add, edit or remove components
* Add components to a pedal, or remove them
* Add a pedal to a list of pedals you want to build, or remove them
* Search for a component to add to a pedal
* See the total number of each component required to build your pedals
* See the cost to build each pedal
* See the cost to build all the pedals you have in the list
* Double clicking an item in the Required Components list will open that url in your browser
* Checkboxes help you keep track of items you have added to your vendor's cart
* Seed the components list with common resistor values
