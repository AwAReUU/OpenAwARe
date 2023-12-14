import * as fs from "fs";
import sqlite3 from "sqlite3";

// ----------------------------------------------------------------------------

const UDB_SRC = "userdb.sqlite";
const IDB_SRC = "ingrdb.sqlite";

/**
 * A singleton class to hold a connection to the user database and ingredients database
 */
export default class Database {
  // ----------------------------------------------------------------------------
  // Singleton:

  /*
   * Set this to true to use a mock database for testing.
   */
  public static testing: boolean = false;

  /*
   * The singleton instance
   */
  private static instance?: Database;

  /*
   * Access the singleton instance
   */
  public static getInstance(): Database {
    if (!Database.instance) Database.instance = new Database();

    return Database.instance;
  }

  /*
   * Delete existing databases. Use this at the end of testing.
   */
  public static delete() {
    this.getInstance().userDatabase.close();
    this.getInstance().ingrDatabase.close();
    Database.instance = undefined;
    let testing = "";
    if (Database.testing) {
      testing = "test_";
    }
    fs.unlinkSync(testing + UDB_SRC);
    fs.unlinkSync(testing + IDB_SRC);
  }

  // ----------------------------------------------------------------------------
  // instance:

  /*
   * The database that stores user information
   */
  private userDatabase: sqlite3.Database;
  /*
   * The database that stores the ingredient list
   */
  private ingrDatabase: sqlite3.Database;

  private constructor() {
    let testing = "";
    if (Database.testing) {
      testing = "test_";
    }
    this.userDatabase = new sqlite3.Database(testing + UDB_SRC, (err) => {
      if (err) {
        // Cannot open database
        console.error(err.message);
        throw err;
      }
    });

    this.ingrDatabase = new sqlite3.Database(testing + IDB_SRC, (err) => {
      if (err) {
        // Cannot open database
        console.error(err.message);
        throw err;
      }
    });

    let userSqlSetup = fs.readFileSync("./usersetup.sql").toString();
    this.userDatabase.exec(userSqlSetup);

    let ingrSqlSetup = fs.readFileSync("./ingrsetup.sql").toString();
    this.ingrDatabase.exec(ingrSqlSetup);
  }

  /*
   * Get the user database
   */
  public userdb(): sqlite3.Database {
    return this.userDatabase;
  }

  /*
   * Get the ingredients database
   */
  public ingrdb(): sqlite3.Database {
    return this.ingrDatabase;
  }
}
