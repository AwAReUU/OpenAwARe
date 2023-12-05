import { Response } from "express";

export function assert_res(
  res: Response,
  value: boolean,
  msg?: string,
  status: number = 400,
): boolean {
  if (!value) {
    if (msg) res.status(status).send(msg);
    else res.status(status).send("Something went wrong");
  }

  return value;
}

export function validName(name: string): boolean {
  let pat = new RegExp(/^[a-z ,.'-]+$/i);
  return pat.test(name);
}

export function validEmail(email: string): boolean {
  let pat = new RegExp(/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/i);
  return pat.test(email);
}

export function validPassword(password: string): boolean {
  let pat = new RegExp(/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/i);
  return pat.test(password);
}
