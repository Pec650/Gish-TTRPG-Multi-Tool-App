# Gish: A TTRPG Multi-Tool

## Overview

* description here

#### Commit Log

* Initiate Project \[04/23/2026]

  * Set up initial project repository and baseline files.
  * Defined core objectives and project scope for the Gish TTRPG Multi-Tool App.
* Name and Structure Refactoring \[04/29/2026]

  * Updated project naming conventions and reorganized directory structure.
  * Improved file layout for better maintainability and clarity.
* Implementing Local Database - SQLite \[04/30/2026]

  * Added a local database for sign up and login.
  * The database utilizes the UserAccount class as the table structure for the users.
* Initial Fixes with Home Page \[05/02/2026]

  * Added Recent Homebrew, Dice Roller, and Session Reminder tabs, not finished
  * Most of the design is fixed, needs readjustments
  * Fixed the button icons for the dice roller
  * Fixed theme of the header
  * Added "Commissioner" font for smaller text
  * Added Roll Log, needs improvement
* Profile Page Update \[05/08/2026]

  * Updated the tab to have icons
  * Added headers to Creations, Tools, and Rules Page
  * Added a new attribute for the UserAccount (ProfileImage) to store the profile picture of the user
  * Added Profile Page
  * Profile Page consists of Edit Profile, Change Password and Log Out
  * Added Edit Profile Page to edit the profile picture and username
  * Bugfix: Turn off all buttons when navigating to prevent overlapping of pages
* Profile Page Update II \[05/09/2026]

  * Added Edit Profile Page and Change Password Page
  * Backend functionalities (Update) are now implemented for both pages
  * Allow users to change their profile image
* Text Theme Change and Home Page Tweaks

  * Changed the text themes of section headers and cards
  * Fixed the Dice roller to make it in line with the prototype