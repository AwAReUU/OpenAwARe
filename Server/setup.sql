-- Table Creation
CREATE TABLE IF NOT EXISTS users (
	contact_id INTEGER PRIMARY KEY,
	first_name TEXT NOT NULL,
	last_name TEXT NOT NULL,
	email TEXT NOT NULL UNIQUE,
	password TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Ingredient(
    IngredientID int NOT NULL UNIQUE,
    PrefName varchar(30) NOT NULL,
    FoodType varchar(14) NOT NULL,
    PRIMARY KEY (IngredientID)
);

CREATE TABLE IF NOT EXISTS Search(
    IngredientID int NOT NULL,
    PosName varchar(30) NOT NULL,
    PRIMARY KEY (IngredientID, PosName),
    CONSTRAINT FK_sid FOREIGN KEY (IngredientID) REFERENCES Ingredient(IngredientID)
);

CREATE TABLE IF NOT EXISTS Plant(
    IngredientID int NOT NULL UNIQUE,
	UnitType varchar(6) NOT NULL, 
    UnitsPerPiece int, 
    Yield int,
    Water int,
    SpacePlants int,
    SpaceRows int,
    PRIMARY KEY (IngredientID),
    CONSTRAINT FK_pid FOREIGN KEY (IngredientID) REFERENCES Ingredient(IngredientID)
);

CREATE TABLE IF NOT EXISTS Animal(
    IngredientID int NOT NULL UNIQUE,
	UnitType varchar(6) NOT NULL, 
    UnitsPerPiece int,
    Yield int,
    Water int, 
    Fodder int,
    PRIMARY KEY (IngredientID),
    CONSTRAINT FK_aid FOREIGN KEY (IngredientID) REFERENCES Ingredient(IngredientID)
);


-- Table Population
/* because of 'UNIQUE' and 'PRIMARY KEY' constraints, the following commands
might throw an error if the tuple has already been inserted before.
The 'OR IGNORE' part will make the program ignore the error in case inserting fails. */
INSERT OR IGNORE INTO Ingredient VALUES (1,      'Apple',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (2,     'Banana',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (3,       'Pear',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (4,   'Mandarin',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (5,     'Orange',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (6,      'Grape',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (7, 'Strawberry',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (8, 'Kiwi Fruit',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (9,  'Pineapple',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (10,     'Melon',  'fruit');
INSERT OR IGNORE INTO Ingredient VALUES (11,      'Beef', 'animal');
INSERT OR IGNORE INTO Ingredient VALUES (12,   'Chicken', 'animal');
INSERT OR IGNORE INTO Ingredient VALUES (13,      'Pork', 'animal');
INSERT OR IGNORE INTO Ingredient VALUES (14,      'Duck', 'animal');
INSERT OR IGNORE INTO Ingredient VALUES (15,      'Milk', 'animal');

INSERT OR IGNORE INTO Search VALUES ( 1,         'Apple');
INSERT OR IGNORE INTO Search VALUES ( 1,     'Red Apple');
INSERT OR IGNORE INTO Search VALUES ( 1,   'Green Apple');
INSERT OR IGNORE INTO Search VALUES ( 1,    'Fuji Apple');
INSERT OR IGNORE INTO Search VALUES ( 1,  'Elstar Apple');
INSERT OR IGNORE INTO Search VALUES ( 1,     'Pink Lady');
INSERT OR IGNORE INTO Search VALUES ( 2,        'Banana');
INSERT OR IGNORE INTO Search VALUES ( 3,          'Pear');
INSERT OR IGNORE INTO Search VALUES ( 4,      'Mandarin');
INSERT OR IGNORE INTO Search VALUES ( 4,       'Satsuma');
INSERT OR IGNORE INTO Search VALUES ( 5,        'Orange');
INSERT OR IGNORE INTO Search VALUES ( 5,     'Tangerine');
INSERT OR IGNORE INTO Search VALUES ( 6,         'Grape');
INSERT OR IGNORE INTO Search VALUES ( 6,     'Red Grape');
INSERT OR IGNORE INTO Search VALUES ( 6,   'White Grape');
INSERT OR IGNORE INTO Search VALUES ( 7,    'Strawberry');
INSERT OR IGNORE INTO Search VALUES ( 8,    'Kiwi Fruit');
INSERT OR IGNORE INTO Search VALUES ( 9,     'Pineapple');
INSERT OR IGNORE INTO Search VALUES ( 9,        'Ananas');
INSERT OR IGNORE INTO Search VALUES (10,         'Melon');
INSERT OR IGNORE INTO Search VALUES (10,    'Watermelon');
INSERT OR IGNORE INTO Search VALUES (11,          'Beef');
INSERT OR IGNORE INTO Search VALUES (11,         'Steak');
INSERT OR IGNORE INTO Search VALUES (11,     'Hamburger');
INSERT OR IGNORE INTO Search VALUES (12,       'Chicken');
INSERT OR IGNORE INTO Search VALUES (12,  'Chicken Legs');
INSERT OR IGNORE INTO Search VALUES (12, 'Chicken Wings');
INSERT OR IGNORE INTO Search VALUES (12,    'Drumsticks');
INSERT OR IGNORE INTO Search VALUES (13,          'Pork');
INSERT OR IGNORE INTO Search VALUES (13,         'Bacon');
INSERT OR IGNORE INTO Search VALUES (13,           'Ham');
INSERT OR IGNORE INTO Search VALUES (14,          'Duck');
INSERT OR IGNORE INTO Search VALUES (15,          'Milk');

INSERT OR IGNORE INTO  Plant VALUES ( 1, 'G',  100,  10000, 495, 1100, 1100);
INSERT OR IGNORE INTO  Plant VALUES ( 2, 'G',  200,  10000, 594, 3658, 3658);
INSERT OR IGNORE INTO  Plant VALUES ( 3, 'G',  150,  10000, 495,  600,  600);
INSERT OR IGNORE INTO  Plant VALUES ( 4, 'G',   60,   8000, 347, 3658, 3658);
INSERT OR IGNORE INTO  Plant VALUES ( 5, 'G',  100,  10000, 347, 3658, 3658);
INSERT OR IGNORE INTO  Plant VALUES ( 6, 'G',    8,   5000, 347, 1828, 2438);
INSERT OR IGNORE INTO  Plant VALUES ( 7, 'G',    7,    100, 495,  254,  254);
INSERT OR IGNORE INTO  Plant VALUES ( 8, 'G',   60,   1000, 495, 4572, 4572);
INSERT OR IGNORE INTO  Plant VALUES ( 9, 'G', 1000,   1000, 594,  300, 1500);
INSERT OR IGNORE INTO  Plant VALUES (10, 'G', 1000,   5000, 495,  900,  900);

INSERT OR IGNORE INTO Animal VALUES (11, 'G',  250, 200000, 100, 100);
INSERT OR IGNORE INTO Animal VALUES (12, 'G',  250,   2500, 100, 100);
INSERT OR IGNORE INTO Animal VALUES (13, 'G',  250,  50000, 100, 100);
INSERT OR IGNORE INTO Animal VALUES (14, 'G',  250,   2500, 100, 100);
INSERT OR IGNORE INTO Animal VALUES (15, 'L', NULL,   NULL, 100, 100);
