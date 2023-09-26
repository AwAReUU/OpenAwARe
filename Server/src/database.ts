import * as fs from "fs";
import sqlite3 from "sqlite3";

// ----------------------------------------------------------------------------

const DB_SRC = "db.sqlite";

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

    private database: sqlite3.Database;

    private constructor() {
        this.database = new sqlite3.Database(DB_SRC, (err) => {
            if (err) {
                // Cannot open database
                console.error(err.message);
                throw err;
            } else {
                console.log("Connected to the SQLite database.");
            }
        });

        let sqlSetup = fs.readFileSync("./setup.sql").toString();
        this.database.run(sqlSetup);
    }

    public db(): sqlite3.Database {
        return this.database;
    }
}
