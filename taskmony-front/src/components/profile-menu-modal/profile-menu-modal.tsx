import { useState } from "react";
import { useNavigate } from "react-router-dom";
import deleteI from "../../images/delete.svg";
import {
  changeUserEmail,
  changeUserName,
  changeUserPassword,
} from "../../services/actions/userInfo";
import Cookies from 'js-cookie';
import { useAppDispatch, useAppSelector } from "../../utils/hooks";

type ModalPropsT = {
  close: Function;
};

export const ProfileMenuModal = ({ close }: ModalPropsT) => {
  const displayName = useAppSelector(
    (store) => store.userInfo.user.displayName
  );
  const [changePassword, setChangePassword] = useState(false);
  const email = useAppSelector((store) => store.userInfo.user.email);
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const logout = () => {
    Cookies.remove("refreshToken");
    Cookies.remove("accessToken");      
    navigate("/login")}
  return (
    <div className="w-1/4 absolute top-0 left-0 p-3 m-4 pb-2 bg-slate-50 rounded-lg drop-shadow-lg z-40">
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => close()}
      ></img>
      <Input
        label={displayName}
        onBlur={(val : string) => {
          dispatch(changeUserName(val));
        }}
      />
      <Input
        label={email}
        onBlur={(val : string) => {
          dispatch(changeUserEmail(val));
        }}
      />
      {changePassword && (
        <>
          <PasswordInput
          label={"old password"}
            value={oldPassword}
            onChange={setOldPassword}
          />
          <PasswordInput
          label={"new password"}
            value={newPassword}
            onChange={setNewPassword}
          />
        </>
      )}
      <Btn
        label={"Change password"}
        onClick={() => {
          if (!changePassword) {
            setChangePassword(true);
          } else {
            dispatch(changeUserPassword(oldPassword, newPassword));
            setChangePassword(false);
          }
        }}
      />
      <Btn label={"Sign out"} onClick={logout} />
    </div>
  );
};

type InputPropsT = {
  label: string;
  onBlur: Function;
};

export const Input = ({ label, onBlur }: InputPropsT) => {
  return (
    <input
      type="text"
      defaultValue={label}
      className="border w-full border-gray-300 rounded mt-2 mb-1 pl-1 pr-2 p-1"
      onBlur={(e) => onBlur(e.target.value)}
    />
  );
};

type PasswordInputProps = {
  label: string;
  value: string;
  onChange: Function;
};
export const PasswordInput = ({ label,value, onChange } : PasswordInputProps) => {
  return (
    <input
      type="password"
      value={value}
      placeholder={label}
      className="border w-full border-gray-300 rounded mt-2 mb-1 pl-1 pr-2 p-1"
      onChange={(e) => onChange(e.target.value)}
    />
  );
};

type BtnPropsT = {
  label: string;
  onClick: Function;
};

export const Btn = ({ onClick, label }: BtnPropsT) => {
  return (
    <div
      className={"p-1 w-fit mt-2 mb-2 pl-2 pr-2 bg-blue-500 rounded-md cursor-pointer"}
      onClick={() => onClick()}
    >
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
