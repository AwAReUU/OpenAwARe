-- Table Creation
CREATE TABLE IF NOT EXISTS Ingredient(
    IngredientID INTEGER,
    PrefName VARCHAR(30) NOT NULL,
    GramsPerML FLOAT,
    GramsPerPiece FLOAT,
    PRIMARY KEY (IngredientID)
);

CREATE TABLE IF NOT EXISTS Search(
    IngredientID INTEGER,
    AltName VARCHAR(30) NOT NULL,
    PRIMARY KEY (IngredientID, AltName),
    CONSTRAINT FK_siid FOREIGN KEY (IngredientID) REFERENCES Ingredient(IngredientID)
);

CREATE TABLE IF NOT EXISTS Requires(
    IngredientID INTEGER,
    ResourceID INTEGER,
    ResPerIngr INTEGER,
    PRIMARY KEY (IngredientID, ResourceID),
    CONSTRAINT  FK_rqiid FOREIGN KEY (IngredientID) REFERENCES Ingredient(IngredientID),
    CONSTRAINT FK_rqrsid FOREIGN KEY   (ResourceID) REFERENCES   Resource(ResourceID)
);

CREATE TABLE IF NOT EXISTS Resource(
    ResourceID INTEGER,
    Name VARCHAR(30),
    Type VARCHAR(14) NOT NULL,
    GramsPerModel INTEGER,
    ModelID INTEGER,
    PRIMARY KEY (ResourceID),
    CONSTRAINT FK_rsmid FOREIGN KEY (ModelID) REFERENCES Model(ModelID)
);

CREATE TABLE IF NOT EXISTS Model(
    ModelID INTEGER,
    Type VARCHAR(14) NOT NULL,
    PrefabPath VARCHAR NOT NULL,
    RealHeight INTEGER,
    PRIMARY KEY(ModelID)
);


-- Table Population
/* because of 'UNIQUE' and 'PRIMARY KEY' constraints, the following commands
might throw an error if the tuple has already been inserted before.
The 'OR IGNORE' part will make the program ignore the error in case inserting fails. */
INSERT OR IGNORE INTO Ingredient VALUES ( 1,     'Water',  1.0, NULL);
INSERT OR IGNORE INTO Ingredient VALUES ( 2,     'Apple', NULL,  100);
INSERT OR IGNORE INTO Ingredient VALUES ( 3,    'Banana', NULL,  200);
INSERT OR IGNORE INTO Ingredient VALUES ( 4,      'Pear', NULL,  150);
INSERT OR IGNORE INTO Ingredient VALUES ( 5,  'Mandarin', NULL,   60);
INSERT OR IGNORE INTO Ingredient VALUES ( 6,    'Orange', NULL,  100);
INSERT OR IGNORE INTO Ingredient VALUES ( 7,     'Grape', NULL,    8);
INSERT OR IGNORE INTO Ingredient VALUES ( 8,'Strawberry', NULL,    7);
INSERT OR IGNORE INTO Ingredient VALUES ( 9,'Kiwi Fruit', NULL,   60);
INSERT OR IGNORE INTO Ingredient VALUES (10, 'Pineapple', NULL, 1000);
INSERT OR IGNORE INTO Ingredient VALUES (11,     'Melon', NULL, 1000);
INSERT OR IGNORE INTO Ingredient VALUES (12,      'Beef', NULL,  250);
INSERT OR IGNORE INTO Ingredient VALUES (13,   'Chicken', NULL,  250);
INSERT OR IGNORE INTO Ingredient VALUES (14,      'Pork', NULL,  250);
INSERT OR IGNORE INTO Ingredient VALUES (15,      'Duck', NULL,  250);
INSERT OR IGNORE INTO Ingredient VALUES (16,      'Milk', 1.04, NULL);
INSERT OR IGNORE INTO Ingredient VALUES (17,      'Potato', NULL, 100);
INSERT OR IGNORE INTO Ingredient VALUES (18,      'Beet', NULL, 150);

INSERT OR IGNORE INTO Search VALUES ( 1,         'Water');
INSERT OR IGNORE INTO Search VALUES ( 2,         'Apple');
INSERT OR IGNORE INTO Search VALUES ( 2,     'Red Apple');
INSERT OR IGNORE INTO Search VALUES ( 2,   'Green Apple');
INSERT OR IGNORE INTO Search VALUES ( 2,    'Fuji Apple');
INSERT OR IGNORE INTO Search VALUES ( 2,  'Elstar Apple');
INSERT OR IGNORE INTO Search VALUES ( 2,     'Pink Lady');
INSERT OR IGNORE INTO Search VALUES ( 3,        'Banana');
INSERT OR IGNORE INTO Search VALUES ( 4,          'Pear');
INSERT OR IGNORE INTO Search VALUES ( 5,      'Mandarin');
INSERT OR IGNORE INTO Search VALUES ( 5,       'Satsuma');
INSERT OR IGNORE INTO Search VALUES ( 6,        'Orange');
INSERT OR IGNORE INTO Search VALUES ( 6,     'Tangerine');
INSERT OR IGNORE INTO Search VALUES ( 7,         'Grape');
INSERT OR IGNORE INTO Search VALUES ( 7,     'Red Grape');
INSERT OR IGNORE INTO Search VALUES ( 7,   'White Grape');
INSERT OR IGNORE INTO Search VALUES ( 8,    'Strawberry');
INSERT OR IGNORE INTO Search VALUES ( 9,    'Kiwi Fruit');
INSERT OR IGNORE INTO Search VALUES (10,     'Pineapple');
INSERT OR IGNORE INTO Search VALUES (10,        'Ananas');
INSERT OR IGNORE INTO Search VALUES (11,         'Melon');
INSERT OR IGNORE INTO Search VALUES (11,    'Watermelon');
INSERT OR IGNORE INTO Search VALUES (12,          'Beef');
INSERT OR IGNORE INTO Search VALUES (12,         'Steak');
INSERT OR IGNORE INTO Search VALUES (12,     'Hamburger');
INSERT OR IGNORE INTO Search VALUES (13,       'Chicken');
INSERT OR IGNORE INTO Search VALUES (13,  'Chicken Legs');
INSERT OR IGNORE INTO Search VALUES (13, 'Chicken Wings');
INSERT OR IGNORE INTO Search VALUES (13,    'Drumsticks');
INSERT OR IGNORE INTO Search VALUES (14,          'Pork');
INSERT OR IGNORE INTO Search VALUES (14,         'Bacon');
INSERT OR IGNORE INTO Search VALUES (14,           'Ham');
INSERT OR IGNORE INTO Search VALUES (15,          'Duck');
INSERT OR IGNORE INTO Search VALUES (16,          'Milk');

INSERT OR IGNORE INTO Model VALUES (1,  'Shapes',         'Cube.prefab', 1.0 );
INSERT OR IGNORE INTO Model VALUES (2, 'Animal',       'CowBIW.prefab', 1.5 );
INSERT OR IGNORE INTO Model VALUES (3, 'Animal', 'ChickenBrown.prefab', 0.5 );
INSERT OR IGNORE INTO Model VALUES (4, 'Animal',          'Pig.prefab', 0.94 );
INSERT OR IGNORE INTO Model VALUES (5, 'Animal',    'DuckWhite.prefab', 0.39 );
INSERT OR IGNORE INTO Model VALUES (6,   'Plant',            'grap.fbx', 1.0 );
INSERT OR IGNORE INTO Model VALUES (7,   'Plant',          'wheat1.fbx', 1.2 );

INSERT OR IGNORE INTO Resource VALUES ( 1,      'Water',  'Water',   NULL, 1); -- model set to cube
INSERT OR IGNORE INTO Resource VALUES ( 2,      'Apple',  'Plant',  10000, 7); -- all fruits set to grape
INSERT OR IGNORE INTO Resource VALUES ( 3,     'Banana',  'Plant',  10000, 7);
INSERT OR IGNORE INTO Resource VALUES ( 4,       'Pear',  'Plant',  10000, 7);
INSERT OR IGNORE INTO Resource VALUES ( 5,   'Mandarin',  'Plant',   8000, 7);
INSERT OR IGNORE INTO Resource VALUES ( 6,     'Orange',  'Plant',  10000, 7);
INSERT OR IGNORE INTO Resource VALUES ( 7,      'Grape',  'Plant',   5000, 7);
INSERT OR IGNORE INTO Resource VALUES ( 8, 'Strawberry',  'Plant',    100, 7);
INSERT OR IGNORE INTO Resource VALUES ( 9, 'Kiwi Fruit',  'Plant',   1000, 7);
INSERT OR IGNORE INTO Resource VALUES (10,  'Pineapple',  'Plant',   1000, 7);
INSERT OR IGNORE INTO Resource VALUES (11,      'Melon',  'Plant',   5000, 7);
INSERT OR IGNORE INTO Resource VALUES (12,       'Beef', 'Animal', 200000, 2);
INSERT OR IGNORE INTO Resource VALUES (13,    'Chicken', 'Animal',   2500, 3);
INSERT OR IGNORE INTO Resource VALUES (14,       'Pork', 'Animal',  50000, 4);
INSERT OR IGNORE INTO Resource VALUES (15,       'Duck', 'Animal',   2500, 5);
INSERT OR IGNORE INTO Resource VALUES (16,       'Milk', 'Animal',   NULL, 2);
INSERT OR IGNORE INTO Resource VALUES (17,      'Wheat',  'Plant',     80, 6); -- wheat

-- water requirements
INSERT OR IGNORE INTO Requires VALUES ( 1,  1,    1);
INSERT OR IGNORE INTO Requires VALUES ( 2,  1,  495);
INSERT OR IGNORE INTO Requires VALUES ( 3,  1,  594);
INSERT OR IGNORE INTO Requires VALUES ( 4,  1,  495);
INSERT OR IGNORE INTO Requires VALUES ( 5,  1,  347);
INSERT OR IGNORE INTO Requires VALUES ( 6,  1,  347);
INSERT OR IGNORE INTO Requires VALUES ( 7,  1,  347);
INSERT OR IGNORE INTO Requires VALUES ( 8,  1,  495);
INSERT OR IGNORE INTO Requires VALUES ( 9,  1,  495);
INSERT OR IGNORE INTO Requires VALUES (10,  1,  594);
INSERT OR IGNORE INTO Requires VALUES (11,  1,  495);
INSERT OR IGNORE INTO Requires VALUES (12,  1, 1000);
INSERT OR IGNORE INTO Requires VALUES (13,  1, 1000);
INSERT OR IGNORE INTO Requires VALUES (14,  1, 1000);
INSERT OR IGNORE INTO Requires VALUES (15,  1, 1000);
INSERT OR IGNORE INTO Requires VALUES (16,  1, 1000);

-- plant/meat requirements
INSERT OR IGNORE INTO Requires VALUES ( 2,  2,    1);
INSERT OR IGNORE INTO Requires VALUES ( 3,  3,    1);
INSERT OR IGNORE INTO Requires VALUES ( 4,  4,    1);
INSERT OR IGNORE INTO Requires VALUES ( 5,  5,    1);
INSERT OR IGNORE INTO Requires VALUES ( 6,  6,    1);
INSERT OR IGNORE INTO Requires VALUES ( 7,  7,    1);
INSERT OR IGNORE INTO Requires VALUES ( 8,  8,    1);
INSERT OR IGNORE INTO Requires VALUES ( 9,  9,    1);
INSERT OR IGNORE INTO Requires VALUES (10, 10,    1);
INSERT OR IGNORE INTO Requires VALUES (11, 11,    1);
INSERT OR IGNORE INTO Requires VALUES (12, 12,    1);
INSERT OR IGNORE INTO Requires VALUES (13, 13,    1);
INSERT OR IGNORE INTO Requires VALUES (14, 14,    1);
INSERT OR IGNORE INTO Requires VALUES (15, 15,    1);
INSERT OR IGNORE INTO Requires VALUES (16, 16,    1);

-- fodder requirements
INSERT OR IGNORE INTO Requires VALUES (11, 17,   10);
INSERT OR IGNORE INTO Requires VALUES (12, 17,   10);
INSERT OR IGNORE INTO Requires VALUES (13, 17,   10);
INSERT OR IGNORE INTO Requires VALUES (14, 17,   10);
INSERT OR IGNORE INTO Requires VALUES (15, 17,   10);
INSERT OR IGNORE INTO Requires VALUES (16, 17,   10);

