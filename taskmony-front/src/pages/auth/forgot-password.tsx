import { NavLink, useNavigate } from "react-router-dom";
import { TaskmonyTitle } from "../../components/taskmony-title";
import deleteI from "../../images/delete.svg";
import { Btn } from "./btn";
import { Input } from "./input";
import React, { useState, useEffect, FormEvent } from "react";
import { resetPassword } from "../../services/actions/auth/resetPassword";
import ClipLoader from "react-spinners/ClipLoader";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";


export const ForgotPassword = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [email, setEmail] = React.useState<string>("");
  const loading = useAppSelector(
    (store) => store.resetPassword.reset_password_loading
  );
  const error = useAppSelector(
    (store) => store.resetPassword.reset_password_error
  );
  const success = useAppSelector(
    (store) => store.resetPassword.reset_password_success
  );
  const reset = () => {
    dispatch(resetPassword(email));
  };

  useEffect(() => {
    if (!success) return;
    console.log("reseting was successful");
    //go to changing password page
    navigate("/reset-password");
  }, [success]);
  return (
    <>
    <TaskmonyTitle/>
    <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
    <div className="w-1/3 m-auto pb-20">
    <h1 className="font-bold text-3xl">Reset password</h1>
    <Input
            label={"email"}
            type={"email"}
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
      <p className="mt-2">remembered your password? <NavLink to="/login" className="font-semibold underline">sign in</NavLink></p>
      <div className="mt-10">
      <Btn label={"recover password"} onClick={reset} />
      </div>
    </div>
    </div>

    </>
  );
};


  