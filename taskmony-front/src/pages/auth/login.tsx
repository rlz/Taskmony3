import { NavLink, useNavigate } from "react-router-dom";
import React, { FormEvent, useEffect, useState } from "react";
import { TaskmonyTitle } from "../../components/taskmony-title";
import deleteI from "../../images/delete.svg";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { Btn } from "./btn";
import { Input } from "./input";
import { login } from "../../services/actions/auth/login";

export const Login = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [passwordVisible, setPasswordVisible] = React.useState<boolean>(false);
  const loading = useAppSelector((store) => store.auth.login_loading);
  const error = useAppSelector((store) => store.auth.login_error);
  const success = useAppSelector((store) => store.auth.login_success);

  useEffect(() => {
    if (!success) return;
    console.log("logging was successful");
    //go to home page
    navigate("/");
  }, [success]);

  const togglePasswordVisibility = () => {
    setPasswordVisible(!passwordVisible);
  };
  const loginUser = () => {
    console.log("login"+email+password)
    dispatch(login(email, password));
  };
  return (
    <>
      <TaskmonyTitle />
      <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
        <div className="w-1/3 m-auto pb-20">
          <h1 className="font-bold text-3xl">Sign in</h1>
          <Input
            label={"email"}
            type={"email"}
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <Input
            label={"password"}
            type={"password"}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          <NavLink to="/forgot-password">
            <p className="mt-2">forgot your password?</p>
          </NavLink>
          <div className="mt-10">
            <Btn label={"sign in"} onClick={loginUser} />
          </div>
          <p className="mt-2">
            or{" "}
            <NavLink to="/register" className="font-semibold underline">
              sign up
            </NavLink>
          </p>
        </div>
      </div>
    </>
  );
};
