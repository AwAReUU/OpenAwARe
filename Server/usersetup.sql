-- Table Creation
CREATE TABLE IF NOT EXISTS User (
    UserID INTEGER,
    FirstName varchar NOT NULL,
    LastName varchar NOT NULL,
    Email varchar NOT NULL UNIQUE,
    Password varchar NOT NULL,
    PRIMARY KEY (UserID)
);
