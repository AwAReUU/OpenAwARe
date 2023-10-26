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

  let db = Database.getInstance().ingrdb();
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
//      id:          int
// }
router.get("/getIngredient", async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().userdb();

  db.get(
    "SELECT * FROM TABLE Ingredient WHERE IngredientID = ?",
    [id],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(row);
    },
  );
});

// Get IngredientList
//
// # Body
// {
//      id:          int
// }
router.get("/getIngredientList", async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().userdb();

  db.all(
    "SELECT * FROM TABLE Ingredient WHERE IngredientID = ?",
    [id],
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(rows);
    },
  );
});

export default router;
