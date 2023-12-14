import express from "express";
import { ValidatedRequest, validateToken } from "./auth";
import Database from "../database";

// ----------------------------------------------------------------------------

let router = express.Router();

/**
 * Route: /quest/save (POST)
 *
 * Save questionnaire to the database
 *
 * # Input (JSON):
 * {
 *    questionnaire: string
 * }
 */
router.post("/save", validateToken, (req: ValidatedRequest, res: any) => {
  let questionnaire = req.body.questionnaire;

  // Get the UserID that corresponds with the email.
  let db = Database.getInstance().userdb();
  db.get(
    `SELECT UserID FROM User WHERE Email = ?;`,
    [req.email],
    async (error: any, row: any) => {
      if (error) {
        console.error(error);
        res.status(500).send("");
        return;
      }

      // Save questionnaire to db
      db.run(
        "INSERT INTO Questionnaire (QuestionnaireID, UserID, Content) VALUES (NULL, ?, ?)",
        [row.UserID, questionnaire],
      );

      res.status(200).send("Questionnaire succesfully saved.");
    },
  );
});

export default router;
