import express from "express";
import Database from "../database";
import { validateToken } from "./auth";
import { validPassword, assert_res, validName, validEmail } from "../util";

// ----------------------------------------------------------------------------

let router = express.Router();

// Search
//
// # Body
// {
//      query:          string
// }
router.get("/search", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.query != null,
                "query is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

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
        }
    );
});

// Get Ingredient
//
// # Body
// {
//      id:          int
// }
router.get("/getIngredient", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.id != null,
                "id is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

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
        }
    );
});

// Get IngredientList
//
// # Body
// {
//      ids:          [int]
// }
router.get("/getIngredientList", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.ids != null,
                "ids is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

    const ids = req.body.ids;

    let db = Database.getInstance().ingrdb();

    db.all(
        `SELECT * FROM Ingredient WHERE IngredientID IN (` +
            ids
                .map(function () {
                    return "?";
                })
                .join(",") +
            `);`,
        ids,
        async (error: any, rows: any) => {
            if (error) {
                console.error(error);
            }
            res.status(200).json(rows);
        }
    );
});

// Get Resource Requirements
//
// # Body
// {
//      id:          int
// }
router.get("/getRequirements", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.id != null,
                "id is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

    const id = req.body.id;

    let db = Database.getInstance().ingrdb();

    db.all(
        `SELECT * FROM Requires WHERE IngredientID = ?;`,
        [id],
        async (error: any, rows: any) => {
            if (error) {
                console.error(error);
            }
            res.status(200).json(rows);
        }
    );
});

// Get Resource
//
// # Body
// {
//      id:          int
// }
router.get("/getResource", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.id != null,
                "id is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

    const id = req.body.id;

    let db = Database.getInstance().ingrdb();

    db.get(
        `SELECT * FROM Resource WHERE ResourceID = ?;`,
        [id],
        async (error: any, row: any) => {
            if (error) {
                console.error(error);
            }
            res.status(200).json(row);
        }
    );
});

// Get ResourceList
//
// # Body
// {
//      ids:          [int]
// }
router.get("/getResourceList", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.ids != null,
                "ids is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

    const ids = req.body.ids;

    let db = Database.getInstance().ingrdb();

    db.all(
        `SELECT * FROM Resource WHERE ResourceID IN (` +
            ids
                .map(function () {
                    return "?";
                })
                .join(",") +
            `);`,
        ids,
        async (error: any, rows: any) => {
            if (error) {
                console.error(error);
            }
            res.status(200).json(rows);
        }
    );
});

// Get Model
//
// # Body
// {
//      id:          int
// }
router.get("/getModel", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.id != null,
                "id is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

    const id = req.body.id;

    let db = Database.getInstance().ingrdb();

    db.get(
        `SELECT * FROM Model WHERE ModelID = ?;`,
        [id],
        async (error: any, row: any) => {
            if (error) {
                console.error(error);
            }
            res.status(200).json(row);
        }
    );
});

// Get ModelList
//
// # Body
// {
//      ids:          [int]
// }
router.get("/getModelList", validateToken, async (req: any, res: any) => {
    // Input sanitization
    if (
        [
            assert_res(
                res,
                req.body.ids != null,
                "ids is missing from the request body"
            ),
        ].some((x) => !x)
    )
        return;

    const ids = req.body.ids;

    let db = Database.getInstance().ingrdb();

    db.all(
        `SELECT * FROM Model WHERE ModelID IN (` +
            ids
                .map(function () {
                    return "?";
                })
                .join(",") +
            `);`,
        ids,
        async (error: any, rows: any) => {
            if (error) {
                console.error(error);
            }
            res.status(200).json(rows);
        }
    );
});

export default router;
