import express, { Express, Request, Response } from "express";
import bodyParser from "body-parser";
import dotenv from "dotenv";

import auth from "./routes/authentication";

// ----------------------------------------------------------------------------

dotenv.config();
const port = process.env.PORT;

const app: Express = express();
app.use(bodyParser.json());

app.listen(port, () => {
    console.log(`⚡️[server]: Server is running at http://localhost:${port}`);
});

// ----------------------------------------------------------------------------
// Routes:

app.get("/", (_req: Request, res: Response) => {
    res.send("Hello world!");
});


app.use("/auth", auth);
