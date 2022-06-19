# My little CI

An app that will listen to changes on user project, 
and automatically will build and test the project.


## What were we asked
1. A program will listen on changes on a project. Whenever a change happens (save), run the tests automatically,If tests pass commit the 
changes to Github. If not program notifies user.
Add tests to check that it works.

2. Same program on another computer checks if changes were made: 
When changes are made pull the changes and run the tests, if succeeded, 
write the result to a log file, Commit the file and tag.
If not, program notifies user.
Add tests to check that it works.

3. After a cycle, test that the changes were made on Github site. Check 
that the original commit was added (with changes), Check that the 
second commit was added (with changes), Check that the tag was made.

4. Generalize and stabilize


## How we did it

We assumed the project was on a fixed url path, and he was build 
using microsoft visual studio.

To listen to project changed we use FileSystemWatcher Class,
to build the project, we use MSBuild, to run the project`s test we use nunit3-console.


## Authors

- [@yosef550](https://www.github.com/yosef550)
- [@ZionTamano](https://www.github.com/ZionTamano)
- [@judithTamino](https://www.github.com/judithTamino)

