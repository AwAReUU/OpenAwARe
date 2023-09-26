import express, { Express, Request, Response } from "express";
import bodyParser from "body-parser";
import * as bcrypt from "bcrypt";
import * as jwt from "jsonwebtoken";
import dotenv from "dotenv";
import * as fs from "fs";

import sqlite3 from "sqlite3";

const DB_SRC = "db.sqlite";

let db = new sqlite3.Database(DB_SRC, (err) => {
    if (err) {
        // Cannot open database
        console.error(err.message);
        throw err;
    } else {
        console.log("Connected to the SQLite database.");
    }
});

let sqlSetup = fs.readFileSync("./setup.sql").toString();
db.run(sqlSetup);

dotenv.config();
const app: Express = express();
app.use(bodyParser.json());

const port = process.env.PORT;

app.get("/", (_req: Request, res: Response) => {
    res.send("Hello world!");
});

app.listen(port, () => {
    console.log(`⚡️[server]: Server is running at http://localhost:${port}`);
});

app.post("/auth/register", async (req: any, res: any) => {
    const firstName = req.body.first_name;
    const lastName = req.body.last_name;
    let password = req.body.password;
    const confirmPassword = req.body.confirm_password;
    const email = req.body.email;

    if (password != confirmPassword) {
        res.status(401).send("Passwords don't match");
        return;
    }

    password = await bcrypt.hash(confirmPassword, 10);

    db.get(
        "SELECT email FROM users WHERE email = ?",
        [email],
        async (error: any, row: any) => {
            if (error) {
                console.error(error);
            }
            if (row) {
                // User already exists
                res
                    .status(401)
                    .send("Email adress is already used for a different account");
                return;
            } else {
                db.run(
                    "INSERT INTO users (first_name, last_name, password, email) VALUES (?, ?, ?, ?)",
                    [firstName, lastName, password, email],
                );
                res.status(201).send("Registration succesfull");
            }
        },
    );
});

app.post("/auth/login", async (req: any, res: any) => {
    const email: string = req.body.email;
    const password: string = req.body.password;

    // Check login
    db.get(
        "SELECT password FROM users WHERE email = ?",
        [email],
        async (error: any, row: any) => {
            if (error) {
                console.error(error);
            }
            if (row) {
                console.log(password, row.password);
                if (await bcrypt.compare(password, row.password)) {
                    // Login succesfull
                    const accessToken = generateAccessToken(email);
                    const refreshToken = generateRefreshToken(email);

                    res.json({ accessToken: accessToken, refreshToken: refreshToken });
                } else {
                    res.status(401).send("Failed to login. Wrong email/password.");
                }
            } else {
                res.status(401).send("Failed to login. Wrong email/password.");
            }
        },
    );
});

function generateAccessToken(email: string): string {
    return jwt.sign({ email: email }, process.env.ACCESS_TOKEN_SECRET!, {
        expiresIn: "15m",
    });
}

let refreshTokens = [];
function generateRefreshToken(email: string): string {
    const refreshToken = jwt.sign(
        { email: email },
        process.env.REFRESH_TOKEN_SECRET!,
        {
            expiresIn: "20m",
        },
    );

    refreshTokens.push(refreshToken);

    return refreshToken;
}
