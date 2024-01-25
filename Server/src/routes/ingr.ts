import express from "express";
import Database from "../database";
import { validateToken } from "./auth";

// ----------------------------------------------------------------------------

let router = express.Router();

/**
 * Route: /ingr/search (GET)
 *
 * Search for an ingredient in the database
 *
 * # Input (JSON):
 * {
 *      query:          string
 * }
 *
 * # Output (JSON):
 * Returns a list of ids.
 */
router.get("/search", validateToken, async (req: any, res: any) => {
  const query = req.body.query;

  let db = Database.getInstance().ingrdb();
  db.all(
    `SELECT DISTINCT
        x.*
        FROM Ingredient x
            JOIN
            (SELECT DISTINCT IngredientID, instr(lower(AltName), lower(?)) AS Pos
                FROM Search
                WHERE Pos > 0) y
            ON x.IngredientID = y.IngredientID
            ORDER BY Pos, x.PrefName;`,
    [query],
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
      }
      // send back JSON with rows
      res.status(200).json(rows);
    },
  );
});

/**
 * Route: /ingr/getIngredient (GET)
 *
 * Get a full row of the ingredient.
 *
 * # Input (JSON):
 * {
 *      id:          int
 * }
 *
 * # Output (JSON):
 * A full row from the ingredient database.
 */
router.get("/getIngredient", validateToken, async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().ingrdb();

  db.get(
    `SELECT * FROM Ingredient WHERE IngredientID = ?;`,
    [id],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }
      res.status(200).json(row);
    },
  );
});

/**
 * Route: /ingr/getIngredientList (GET)
 *
 * Returns a list of rows from the ingredient database.
 *
 * # Input (json):
 * {
 *      ids:          [int]
 * }
 */
router.get("/getIngredientList", validateToken, async (req: any, res: any) => {
  const ids = req.body.ids;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM Ingredient WHERE IngredientID IN (` +
    ids
      .map(function() {
        return "?";
      })
      .join(",") +
    `);`,
    ids,
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }
      res.status(200).json(rows);
    },
  );
});

/**
 * Route: /ingr/getRequirements (GET)
 *
 * Get a list of all resources that the ingredient with the given id requires.
 *
 * # Input (JSON):
 * {
 *      id:          int
 * }
 */
router.get("/getRequirements", validateToken, async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM Requires WHERE IngredientID = ?;`,
    [id],
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }
      res.status(200).json(rows);
    },
  );
});

/**
 * Route: /ingr/getResource (GET)
 *
 * Get a full row of the resource with this ID.
 *
 * # Input (JSON):
 * {
 *      id:          int
 * }
 */
router.get("/getResource", validateToken, async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().ingrdb();

  db.get(
    `SELECT * FROM Resource WHERE ResourceID = ?;`,
    [id],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }
      res.status(200).json(row);
    },
  );
});

/**
 * Route: /ingr/getResourceList (GET)
 *
 * Get full rows of the resources with the ids in the list.
 *
 * # Input (JSON):
 * {
 *      ids:          [int]
 * }
 */
router.get("/getResourceList", validateToken, async (req: any, res: any) => {
  const ids = req.body.ids;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM Resource WHERE ResourceID IN (` +
    ids
      .map(function() {
        return "?";
      })
      .join(",") +
    `);`,
    ids,
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }
      res.status(200).json(rows);
    },
  );
});

/**
 * Route: /ingr/getModel (GET)
 *
 * Get a fullrow of the model.
 *
 * # Input (json):
 * {
 *      id:          int
 * }
 */
router.get("/getModel", validateToken, async (req: any, res: any) => {
  const id = req.body.id;

  let db = Database.getInstance().ingrdb();

  db.get(
    `SELECT * FROM Model WHERE ModelID = ?;`,
    [id],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }
      res.status(200).json(row);
    },
  );
});

/**
 * Route: /ingr/getModelList (GET)
 *
 * Get a list of rows for each model.
 *
 * # Input (JSON):
 * {
 *      ids:          [int]
 * }
 */
router.get("/getModelList", validateToken, async (req: any, res: any) => {
  const ids = req.body.ids;

  let db = Database.getInstance().ingrdb();

  db.all(
    `SELECT * FROM Model WHERE ModelID IN (` +
    ids
      .map(function() {
        return "?";
      })
      .join(",") +
    `);`,
    ids,
    async (error: any, rows: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }
      res.status(200).json(rows);
    },
  );
});

export default router;
