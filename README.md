# OctopathII_Items
This is a two part project for one of my favorite JRPG games, Octopath II, items. The list will conatain Items and Equipment such as weapons from the game. This repository focuses on the API part of the project. The Front-End is yet to be determined in terms of either it will be in Razor views or in Angular.

## Overview
The Api Will retireve data information stored from the database. The database is populated by CSV files that are obtained by

`https://www.kaggle.com/datasets/mattop/octopath-traveler-ii-equipment`

`https://www.kaggle.com/datasets/mattop/octopath-traveler-ii-items`

These files last update was on 4/29/2024. There won't be future update on the list since these are set items and equipment in the game.

The following column and information will be used and focused on 
- Name
- Price
- Description
- Item Type
- sell price

The database is hidden, so you will need to create your own database or use the local method for your editor. Once created, Implement the migrations, update database, and populate the values into database with the seed controller via Swagger.

## Packages
The following packages was used in this project:

`Microsoft.EntityFrameworkCore 8.0.11`

`Microsoft.EntityFrameworkCore.Tools 8.0.11`

`Microsoft.EntityFrameworkCore.SqlServer 8.0.11`

`Swashbuckle.AspNetCore 7.2.0`
