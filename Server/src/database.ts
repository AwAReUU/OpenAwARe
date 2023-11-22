import * as fs from "fs";
import sqlite3 from "sqlite3";

// ----------------------------------------------------------------------------

const UDB_SRC = "userdb.sqlite";
const IDB_SRC = "ingrdb.sqlite";

export default class Database {
    // ----------------------------------------------------------------------------
    // Singleton:

    private static instance: Database;

    public static getInstance(): Database {
        if (!Database.instance) Database.instance = new Database();

        return Database.instance;
    }

    // ----------------------------------------------------------------------------
    // instance:

    private userDatabase: sqlite3.Database;
    private ingrDatabase: sqlite3.Database;

    private constructor() {
        this.userDatabase = new sqlite3.Database(UDB_SRC, (err) => {
            if (err) {
                // Cannot open database
                console.error(err.message);
                throw err;
            } else {
                console.log("Connected to the user SQLite database.");
            }
        });

        this.ingrDatabase = new sqlite3.Database(IDB_SRC, (err) => {
            if (err) {
                // Cannot open database
                console.error(err.message);
                throw err;
            } else {
                console.log("Connected to the ingredient SQLite database.");
            }
        });

        let userSqlSetup = fs.readFileSync("./usersetup.sql").toString();
        this.userDatabase.exec(userSqlSetup);

        let ingrSqlSetup = fs.readFileSync("./ingrsetup.sql").toString();
        this.ingrDatabase.exec(ingrSqlSetup);
    }

    public userdb(): sqlite3.Database {
        return this.userDatabase;
    }

    public ingrdb(): sqlite3.Database {
        return this.ingrDatabase;
    }
}
