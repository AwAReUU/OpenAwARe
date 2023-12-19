-- Table Creation
CREATE TABLE IF NOT EXISTS User (
    UserID INTEGER,
    FirstName varchar NOT NULL,
    LastName varchar NOT NULL,
    Email varchar NOT NULL UNIQUE,
    Password varchar NOT NULL,
    PRIMARY KEY (UserID)
);

CREATE TABLE IF NOT EXISTS Questionnaire (
    QuestionnaireID INTEGER,
    UserID INTEGER,
    Content TEXT,
    PRIMARY KEY (QuestionnaireID),
    CONSTRAINT FK_rsmid FOREIGN KEY (UserID) REFERENCES User(UserID) 
);

