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
    ResPerIngr FLOAT NOT NULL,
    PRIMARY KEY (IngredientID, ResourceID),
    CONSTRAINT FK_rqiid FOREIGN KEY (IngredientID) REFERENCES Ingredient(IngredientID),
    CONSTRAINT FK_rqrsid FOREIGN KEY (ResourceID) REFERENCES Resource(ResourceID)
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
    RealHeight FLOAT NOT NULL,
    PRIMARY KEY(ModelID)
);

-- Table Population
/* because of 'UNIQUE' and 'PRIMARY KEY' constraints, the following commands
 might throw an error if the tuple has already been inserted before.
 The 'OR IGNORE' part will make the program ignore the error in case inserting fails. */
INSERT
    OR IGNORE INTO Ingredient
VALUES
    (1, 'Water', 1.0, NULL);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (2, 'Apple', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (3, 'Banana', NULL, 200);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (4, 'Pear', NULL, 150);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (5, 'Mandarin', NULL, 60);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (6, 'Orange', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (7, 'Grape', NULL, 8);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (8, 'Strawberry', NULL, 7);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (9, 'Kiwi Fruit', NULL, 60);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (10, 'Pineapple', NULL, 1000);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (11, 'Melon', NULL, 1000);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (12, 'Beef', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (13, 'Chicken', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (14, 'Pork', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (15, 'Duck', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (16, 'Milk', 1.04, NULL);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (17, 'Potato', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (18, 'Beet', NULL, 150);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (1, 'Water', 1.0, NULL);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (2, 'Apple', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (3, 'Banana', NULL, 200);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (4, 'Pear', NULL, 150);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (5, 'Mandarin', NULL, 60);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (6, 'Orange', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (7, 'Grape', NULL, 8);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (8, 'Strawberry', NULL, 7);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (9, 'Kiwi Fruit', NULL, 60);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (10, 'Pineapple', NULL, 1000);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (11, 'Melon', NULL, 1000);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (12, 'Beef', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (13, 'Chicken', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (14, 'Pork', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (15, 'Duck', NULL, 250);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (16, 'Cow Milk', 1.04, NULL);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (17, 'Potato', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (18, 'Beet', NULL, 150);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (19, 'Artichoke', NULL, 120);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (20, 'Broccoli', NULL, 300);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (21, 'Cabbage', NULL, 1000);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (22, 'Carrot', NULL, 60);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (23, 'Corn', NULL, 150);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (24, 'Cucumber', NULL, 300);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (25, 'Eggplant', NULL, 400);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (26, 'Garlic', NULL, 5);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (27, 'Onion', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (28, 'Pepper', NULL, 45);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (29, 'Poppy Seeds', NULL, 4);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (30, 'Pumpkin', NULL, 2000);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (31, 'Sunflower Seeds', NULL, 4);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (32, 'Tomato', NULL, 100);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (33, 'Wheat', NULL, 45);

INSERT
    OR IGNORE INTO Ingredient
VALUES
    (34, 'Pasta', NULL, NULL);

INSERT
    OR IGNORE INTO Search
VALUES
    (1, 'Water');

INSERT
    OR IGNORE INTO Search
VALUES
    (2, 'Apple');

INSERT
    OR IGNORE INTO Search
VALUES
    (2, 'Red Apple');

INSERT
    OR IGNORE INTO Search
VALUES
    (2, 'Green Apple');

INSERT
    OR IGNORE INTO Search
VALUES
    (2, 'Fuji Apple');

INSERT
    OR IGNORE INTO Search
VALUES
    (2, 'Elstar Apple');

INSERT
    OR IGNORE INTO Search
VALUES
    (2, 'Pink Lady');

INSERT
    OR IGNORE INTO Search
VALUES
    (3, 'Banana');

INSERT
    OR IGNORE INTO Search
VALUES
    (4, 'Pear');

INSERT
    OR IGNORE INTO Search
VALUES
    (5, 'Mandarin');

INSERT
    OR IGNORE INTO Search
VALUES
    (5, 'Satsuma');

INSERT
    OR IGNORE INTO Search
VALUES
    (6, 'Orange');

INSERT
    OR IGNORE INTO Search
VALUES
    (6, 'Tangerine');

INSERT
    OR IGNORE INTO Search
VALUES
    (7, 'Grape');

INSERT
    OR IGNORE INTO Search
VALUES
    (7, 'Red Grape');

INSERT
    OR IGNORE INTO Search
VALUES
    (7, 'White Grape');

INSERT
    OR IGNORE INTO Search
VALUES
    (8, 'Strawberry');

INSERT
    OR IGNORE INTO Search
VALUES
    (9, 'Kiwi Fruit');

INSERT
    OR IGNORE INTO Search
VALUES
    (10, 'Pineapple');

INSERT
    OR IGNORE INTO Search
VALUES
    (10, 'Ananas');

INSERT
    OR IGNORE INTO Search
VALUES
    (11, 'Melon');

INSERT
    OR IGNORE INTO Search
VALUES
    (11, 'Watermelon');

INSERT
    OR IGNORE INTO Search
VALUES
    (12, 'Beef');

INSERT
    OR IGNORE INTO Search
VALUES
    (12, 'Steak');

INSERT
    OR IGNORE INTO Search
VALUES
    (12, 'Hamburger');

INSERT
    OR IGNORE INTO Search
VALUES
    (13, 'Chicken');

INSERT
    OR IGNORE INTO Search
VALUES
    (13, 'Chicken Legs');

INSERT
    OR IGNORE INTO Search
VALUES
    (13, 'Chicken Wings');

INSERT
    OR IGNORE INTO Search
VALUES
    (13, 'Drumsticks');

INSERT
    OR IGNORE INTO Search
VALUES
    (14, 'Pork');

INSERT
    OR IGNORE INTO Search
VALUES
    (14, 'Bacon');

INSERT
    OR IGNORE INTO Search
VALUES
    (14, 'Ham');

INSERT
    OR IGNORE INTO Search
VALUES
    (15, 'Duck');

INSERT
    OR IGNORE INTO Search
VALUES
    (16, 'Cow Milk');

INSERT
    OR IGNORE INTO Search
VALUES
    (16, 'Milk');

INSERT
    OR IGNORE INTO Search
VALUES
    (17, 'Potato');

INSERT
    OR IGNORE INTO Search
VALUES
    (18, 'Beet');

INSERT
    OR IGNORE INTO Search
VALUES
    (19, 'Artichoke');

INSERT
    OR IGNORE INTO Search
VALUES
    (20, 'Broccoli');

INSERT
    OR IGNORE INTO Search
VALUES
    (21, 'Cabbage');

INSERT
    OR IGNORE INTO Search
VALUES
    (22, 'Carrot');

INSERT
    OR IGNORE INTO Search
VALUES
    (23, 'Corn');

INSERT
    OR IGNORE INTO Search
VALUES
    (24, 'Cucumber');

INSERT
    OR IGNORE INTO Search
VALUES
    (25, 'Eggplant');

INSERT
    OR IGNORE INTO Search
VALUES
    (26, 'Garlic');

INSERT
    OR IGNORE INTO Search
VALUES
    (27, 'Onion');

INSERT
    OR IGNORE INTO Search
VALUES
    (28, 'Pepper');

INSERT
    OR IGNORE INTO Search
VALUES
    (29, 'Poppy');

INSERT
    OR IGNORE INTO Search
VALUES
    (29, 'Poppy Seeds');

INSERT
    OR IGNORE INTO Search
VALUES
    (30, 'Pumpkin');

INSERT
    OR IGNORE INTO Search
VALUES
    (31, 'Sunflower');

INSERT
    OR IGNORE INTO Search
VALUES
    (33, 'Sunflower Seeds');

INSERT
    OR IGNORE INTO Search
VALUES
    (32, 'Tomato');

INSERT
    OR IGNORE INTO Search
VALUES
    (33, 'Wheat');

INSERT
    OR IGNORE INTO Search
VALUES
    (34, 'Pasta');

INSERT
    OR IGNORE INTO Search
VALUES
    (34, 'Macaroni');

INSERT
    OR IGNORE INTO Search
VALUES
    (34, 'Spaghetti');

INSERT
    OR IGNORE INTO Search
VALUES
    (34, 'Penne');

INSERT
    OR IGNORE INTO Model
VALUES
    (1, 'Water', 'Water/Water_bottle', 0.3);

INSERT
    OR IGNORE INTO Model
VALUES
    (2, 'Animal', 'Animals/CowBlW', 1.5);

INSERT
    OR IGNORE INTO Model
VALUES
    (3, 'Animal', 'Animals/ChickenBrown', 0.5);

INSERT
    OR IGNORE INTO Model
VALUES
    (4, 'Animal', 'Animals/Pig', 1);

INSERT
    OR IGNORE INTO Model
VALUES
    (5, 'Animal', 'Animals/DuckWhite', 0.39);

INSERT
    OR IGNORE INTO Model
VALUES
    (6, 'Plant', 'Plants/grap', 1);

INSERT
    OR IGNORE INTO Model
VALUES
    (7, 'Plant', 'Plants/wheat1', 1.2);

INSERT
    OR IGNORE INTO Model
VALUES
    (8, 'Animal', 'Water/Milk_carton', 0.2);

INSERT
    OR IGNORE INTO Model
VALUES
    (9, 'Plant', 'Plants/potato', 0.4);

INSERT
    OR IGNORE INTO Model
VALUES
    (10, 'Plant', 'Plants/beet', 0.2);

INSERT
    OR IGNORE INTO Model
VALUES
    (11, 'Plant', 'Plants/artichoke', 1.5);

INSERT
    OR IGNORE INTO Model
VALUES
    (12, 'Plant', 'Plants/brokoly', 0.4);

INSERT
    OR IGNORE INTO Model
VALUES
    (13, 'Plant', 'Plants/cabbage', 0.3);

INSERT
    OR IGNORE INTO Model
VALUES
    (14, 'Plant', 'Plants/carrot', 0.2);

INSERT
    OR IGNORE INTO Model
VALUES
    (15, 'Plant', 'Plants/corn', 1.8);

INSERT
    OR IGNORE INTO Model
VALUES
    (16, 'Plant', 'Plants/corn2', 1.8);

INSERT
    OR IGNORE INTO Model
VALUES
    (17, 'Plant', 'Plants/cucumber', 1.5);

INSERT
    OR IGNORE INTO Model
VALUES
    (18, 'Plant', 'Plants/eggplant', 0.3);

INSERT
    OR IGNORE INTO Model
VALUES
    (19, 'Plant', 'Plants/garlic', 0.2);

INSERT
    OR IGNORE INTO Model
VALUES
    (20, 'Plant', 'Plants/grap2', 2.1);

INSERT
    OR IGNORE INTO Model
VALUES
    (21, 'Plant', 'Plants/onion', 0.3);

INSERT
    OR IGNORE INTO Model
VALUES
    (22, 'Plant', 'Plants/pepper', 0.7);

INSERT
    OR IGNORE INTO Model
VALUES
    (23, 'Plant', 'Plants/poppy', 0.6);

INSERT
    OR IGNORE INTO Model
VALUES
    (24, 'Plant', 'Plants/pumpkin', 0.3);

INSERT
    OR IGNORE INTO Model
VALUES
    (25, 'Plant', 'Plants/sunflower', 2);

INSERT
    OR IGNORE INTO Model
VALUES
    (26, 'Plant', 'Plants/tomato', 0.6);

INSERT
    OR IGNORE INTO Model
VALUES
    (27, 'Plant', 'Plants/wheat2', 0.6);

INSERT
    OR IGNORE INTO Model
VALUES
    (28, 'Animal', 'Animals/SheepWhite', 1);

INSERT
    OR IGNORE INTO Resource
VALUES
    (1, 'Water', 'Water', NULL, 1);

INSERT
    OR IGNORE INTO Resource
VALUES
    (2, 'Apple', 'Plant', 10000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (3, 'Banana', 'Plant', 10000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (4, 'Pear', 'Plant', 10000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (5, 'Mandarin', 'Plant', 8000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (6, 'Orange', 'Plant', 10000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (7, 'Grape', 'Plant', 5000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (8, 'Strawberry', 'Plant', 100, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (9, 'Kiwi Fruit', 'Plant', 1000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (10, 'Pineapple', 'Plant', 1000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (11, 'Melon', 'Plant', 5000, 6);

INSERT
    OR IGNORE INTO Resource
VALUES
    (12, 'Beef', 'Animal', 200000, 2);

INSERT
    OR IGNORE INTO Resource
VALUES
    (13, 'Chicken', 'Animal', 2500, 3);

INSERT
    OR IGNORE INTO Resource
VALUES
    (14, 'Pork', 'Animal', 50000, 4);

INSERT
    OR IGNORE INTO Resource
VALUES
    (15, 'Duck', 'Animal', 2500, 5);

INSERT
    OR IGNORE INTO Resource
VALUES
    (16, 'Milk', 'Animal', 1000, 8);

INSERT
    OR IGNORE INTO Resource
VALUES
    (17, 'Potato', 'Plant', 1000, 9);

INSERT
    OR IGNORE INTO Resource
VALUES
    (18, 'Beet', 'Plant', 250, 10);

INSERT
    OR IGNORE INTO Resource
VALUES
    (19, 'Artichoke', 'Plant', 300, 11);

INSERT
    OR IGNORE INTO Resource
VALUES
    (20, 'Broccoli', 'Plant', 500, 12);

INSERT
    OR IGNORE INTO Resource
VALUES
    (21, 'Cabbage', 'Plant', 1000, 13);

INSERT
    OR IGNORE INTO Resource
VALUES
    (22, 'Carrot', 'Plant', 550, 14);

INSERT
    OR IGNORE INTO Resource
VALUES
    (23, 'Corn', 'Plant', 2000, 15);

INSERT
    OR IGNORE INTO Resource
VALUES
    (24, 'Cucumber', 'Plant', 800, 17);

INSERT
    OR IGNORE INTO Resource
VALUES
    (25, 'Eggplant', 'Plant', 1000, 18);

INSERT
    OR IGNORE INTO Resource
VALUES
    (26, 'Garlic', 'Plant', 150, 19);

INSERT
    OR IGNORE INTO Resource
VALUES
    (27, 'Onion', 'Plant', 250, 21);

INSERT
    OR IGNORE INTO Resource
VALUES
    (28, 'Pepper', 'Plant', 400, 22);

INSERT
    OR IGNORE INTO Resource
VALUES
    (29, 'Poppy', 'Plant', 100, 23);

INSERT
    OR IGNORE INTO Resource
VALUES
    (30, 'Pumpkin', 'Plant', 2000, 24);

INSERT
    OR IGNORE INTO Resource
VALUES
    (31, 'Sunflower', 'Plant', 1000, 25);

INSERT
    OR IGNORE INTO Resource
VALUES
    (32, 'Tomato', 'Plant', 800, 26);

INSERT
    OR IGNORE INTO Resource
VALUES
    (33, 'Wheat', 'Plant', 1000, 7);

INSERT
    OR IGNORE INTO Requires
VALUES
    (1, 1, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (2, 1, 495);

INSERT
    OR IGNORE INTO Requires
VALUES
    (3, 1, 594);

INSERT
    OR IGNORE INTO Requires
VALUES
    (4, 1, 495);

INSERT
    OR IGNORE INTO Requires
VALUES
    (5, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (6, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (7, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (8, 1, 495);

INSERT
    OR IGNORE INTO Requires
VALUES
    (9, 1, 495);

INSERT
    OR IGNORE INTO Requires
VALUES
    (10, 1, 594);

INSERT
    OR IGNORE INTO Requires
VALUES
    (11, 1, 495);

INSERT
    OR IGNORE INTO Requires
VALUES
    (12, 1, 15415);

INSERT
    OR IGNORE INTO Requires
VALUES
    (13, 1, 4325);

INSERT
    OR IGNORE INTO Requires
VALUES
    (14, 1, 5988);

INSERT
    OR IGNORE INTO Requires
VALUES
    (15, 1, 4325);

INSERT
    OR IGNORE INTO Requires
VALUES
    (16, 1, 4.32692307692);

INSERT
    OR IGNORE INTO Requires
VALUES
    (17, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (18, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (19, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (20, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (21, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (22, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (23, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (24, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (25, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (26, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (27, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (28, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (29, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (30, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (31, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (32, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (33, 1, 347);

INSERT
    OR IGNORE INTO Requires
VALUES
    (34, 1, 700);

INSERT
    OR IGNORE INTO Requires
VALUES
    (11, 17, 10);

INSERT
    OR IGNORE INTO Requires
VALUES
    (12, 17, 10);

INSERT
    OR IGNORE INTO Requires
VALUES
    (13, 17, 2.5);

INSERT
    OR IGNORE INTO Requires
VALUES
    (14, 17, 10);

INSERT
    OR IGNORE INTO Requires
VALUES
    (15, 17, 2.5);

INSERT
    OR IGNORE INTO Requires
VALUES
    (16, 17, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (2, 2, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (3, 3, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (4, 4, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (5, 5, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (6, 6, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (7, 7, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (8, 8, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (9, 9, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (10, 10, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (11, 11, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (12, 12, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (13, 13, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (14, 14, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (15, 15, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (16, 16, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (17, 17, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (18, 18, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (19, 19, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (20, 20, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (21, 21, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (22, 22, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (23, 23, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (24, 24, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (25, 25, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (26, 26, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (27, 27, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (28, 28, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (29, 29, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (30, 30, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (31, 31, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (32, 32, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (33, 33, 1);

INSERT
    OR IGNORE INTO Requires
VALUES
    (34, 33, 0.95);