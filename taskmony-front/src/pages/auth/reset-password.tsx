import { NavLink } from "react-router-dom";
import deleteI from "../../images/delete.svg";
import { Btn } from "./btn";
import { Input } from "./input";
import { TaskmonyTitle } from "../../components/taskmony-title";
import React, { useState, useEffect, FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { changePassword } from "../../services/actions/auth/resetPassword";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";

export const ResetPassword = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [password, setPassword] = React.useState<string>("");
  const [code, setCode] = React.useState<string>("");
  const [passwordVisible, setPasswordVisible] = React.useState<boolean>(false);
  const loading = useAppSelector(
    (store) => store.resetPassword.change_password_loading
  );
  const error = useAppSelector(
    (store) => store.resetPassword.change_password_error
  );
  const success = useAppSelector(
    (store) => store.resetPassword.change_password_success
  );
  const reset = () => {
    dispatch(changePassword(code, password));
  };

  useEffect(() => {
    if (!success) return;
    console.log("reseting was successful");
    //go to changing password page
    navigate("/login");
  }, [success]);

  return (
    <>
      <TaskmonyTitle />
      <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
        <div className="w-1/3 m-auto pb-20">
          <h1 className="font-bold text-3xl">Reset password</h1>
          <Input
            label={"new password"}
            type={"password"}
            value={password}
            onChange={setPassword}
          />
          <Input
            label={"code from email"}
            type={"text"}
            value={code}
            onChange={setCode}
          />
          <p className="mt-2">
            remembered your password?{" "}
            <NavLink to="/login" className="font-semibold underline">
              sign in
            </NavLink>
          </p>
          <div className="mt-10">
            <Btn label={"save changes"} onClick={reset} />
          </div>
        </div>
      </div>
    </>
  );
};
