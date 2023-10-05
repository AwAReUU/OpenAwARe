import express, { Response, Request } from "express";
import Database from "../database";

// ----------------------------------------------------------------------------

let router = express.Router();

// Search
//
// # Body
// {
//      query:          string
// }
router.get("/search", async (req: any, res: any) => {
  const query = req.body.query;

  let db = Database.getInstance().db();
  db.all(
    `SELECT
        x.*
        FROM Ingredient x
            JOIN
            (SELECT DISTINCT IngredientID
                FROM Search s
                WHERE instr(lower(PosName), lower(?)) > 0
                ORDER BY instr(lower(s.PosName), lower(?))  
                LIMIT 10) y
            ON x.IngredientID = y.IngredientID;
        `,
    [query, query],
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
      }
      // send back JSON with rows
      res.status(200).json(rows);
    },
  );
});

// Get Ingredient
//
// # Body
// {
//      id:          int,
//      type:        string
// }
router.get("/getIngredient", async (req: any, res: any) => {
  const id = req.body.id;
  const type = req.body.type;

  let db = Database.getInstance().db();

  // we need to query different tables depending on the type of ingredient
  // at the moment the type is specified in the request, we could also query Ingredient table to get this
  // quite a bit of code duplication rn, could maybe just set the query string instead and (if not null) run db.get after...
  switch (type) {
    case "fruit":
      db.get(
        "SELECT * FROM TABLE Fruit WHERE IngredientID = ?",
        [id],
        async (error: any, row: any) => {
          if (error) {
            console.error(error);
          }
          res.status(200).json(row);
        },
      );
      break;
    case "animal":
      db.get(
        "SELECT * FROM TABLE Animal WHERE IngredientID = ?",
        [id],
        async (error: any, row: any) => {
          if (error) {
            console.error(error);
          }
          res.status(200).json(row);
        },
      );
      break;
    default:
      // type does not exist
      res.status(400).send("Invalid type.");
      break;
  }
});

export default router;
