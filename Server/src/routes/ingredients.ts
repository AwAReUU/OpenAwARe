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
                WHERE instr(lower(AltName), lower(?)) > 0
                ORDER BY instr(lower(s.AltName), lower(?))  
                LIMIT 10) y
            ON x.IngredientID = y.IngredientID;`,
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

  let db = Database.getInstance().ingrdb();

  db.get(
    `SELECT * FROM Ingredient WHERE IngredientID = ?;`,
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
//      ids:          [int]
// }
router.get("/getIngredientList", async (req: any, res: any) => {
  const ids = req.body.ids;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM Ingredient WHERE IngredientID IN (`+ ids.map(function(){ return '?' }).join(',') +`);`,
    ids,
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(rows);
    },
  );
});

// Get Resource Requirements
//
// # Body
// {
//      id:          int
// }
router.get("/getRequirements", async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM TABLE Requires WHERE IngredientID = ?;`,
    [id],
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(rows);
    },
  );
});


// Get Resource
//
// # Body
// {
//      id:          int
// }
router.get("/getResource", async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().ingrdb();

  db.get(
    `SELECT * FROM TABLE Resource WHERE ResourceID = ?;`,
    [id],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(row);
    },
  );
});

// Get ResourceList
//
// # Body
// {
//      ids:          [int]
// }
router.get("/getResourceList", async (req: any, res: any) => {
  const ids = req.body.ids;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM Resource WHERE ResourceID IN (`+ ids.map(function(){ return '?' }).join(',') +`);`,
    ids,
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(rows);
    },
  );
});

// Get Model
//
// # Body
// {
//      id:          int
// }
router.get("/getModel", async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().ingrdb();

  db.get(
    `SELECT * FROM TABLE Model WHERE ModelID = ?;`,
    [id],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(row);
    },
  );
});

// Get ModelList
//
// # Body
// {
//      ids:          [int]
// }
router.get("/getModelList", async (req: any, res: any) => {
  const ids = req.body.ids;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM Model WHERE ModelID IN (`+ ids.map(function(){ return '?' }).join(',') +`);`,
    ids,
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
      }
      res.status(200).json(rows);
    },
  );
});

export default router;
