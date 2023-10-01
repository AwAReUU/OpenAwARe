import express from "express";
import { validateToken } from "./authentication";

// ----------------------------------------------------------------------------

let router = express.Router();

router.get("/", validateToken, (req, res) => {

});

export default router;
