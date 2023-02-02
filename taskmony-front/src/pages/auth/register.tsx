import { NavLink, useNavigate } from "react-router-dom";
import { TaskmonyTitle } from "../../components/taskmony-title";
import deleteI from "../../images/delete.svg";
import { Btn } from "./btn";
import { Input } from "./input";

import React, { FormEvent, useEffect, useState } from "react";
import { register } from "../../services/actions/auth/register";
import ClipLoader from "react-spinners/ClipLoader";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";


export const Register = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [name, setName] = React.useState<string>("");
  const [email, setEmail] = React.useState<string>("");
  const [password, setPassword] = React.useState<string>("");
  const [passwordVisible, setPasswordVisible] = React.useState<boolean>(false);
  const loading = useAppSelector((store) => store.auth.register_loading);
  const error = useAppSelector((store) => store.auth.register_error);
  const success = useAppSelector((store) => store.auth.register_success);

  useEffect(() => {
    if (!success) return;
    console.log("registration was successful");
    //go to home page
    navigate("/login");
  }, [success]);

  const togglePasswordVisibility = () => {
    setPasswordVisible(!passwordVisible);
  };
  const registerUser = () => {
    console.log("registering"+email+password+name)
    dispatch(register(email, password, name));
  };

  return (
    <>
    <TaskmonyTitle/>
    <div className="w-full h-full absolute flex justify-center content-center bg-slate-50">
    <div className="w-1/3 m-auto pb-20">
    <h1 className="font-bold text-3xl">Sign up</h1>
      <Input label={"name"} type={"text"} value={name} onChange={(e) => setName(e.target.value)}/>
      <Input label={"email"} type={"email"} value={email} onChange={(e) => setEmail(e.target.value)}/>
      <Input label={"password"} type={"password"} value={password} onChange={(e) => setPassword(e.target.value)}/>
      <div className="mt-10">
      <Btn label={"sign up"} onClick={registerUser} />
      </div>
      <p>Or <NavLink to="/login" className="font-semibold underline">sign in</NavLink></p>
    </div>
    </div>

    </>
  );
};


  