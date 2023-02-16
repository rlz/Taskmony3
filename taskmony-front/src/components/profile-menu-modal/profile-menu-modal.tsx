import deleteI from "../../images/delete.svg";
import { useAppSelector } from "../../utils/hooks";

type ModalPropsT = {
  close: Function;
};

export const ProfileMenuModal = ({ close }: ModalPropsT) => {
  const displayName = useAppSelector(
    (store) => store.userInfo.user.displayName
  );
  const email = useAppSelector((store) => store.userInfo.user.email);
  return (
    <div className="w-1/4 absolute top-0 left-0 p-3 m-4 pb-2 bg-slate-50 rounded-lg drop-shadow-lg z-40">
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => close()}
      ></img>
      <Input label={displayName} />
      <Input label={email} />
      <Btn label={"Change password"} onClick={() => {}} />
      <Btn label={"Sign out"} onClick={() => {}} />
    </div>
  );
};

type InputPropsT = {
  label: string;
};

export const Input = ({ label }: InputPropsT) => {
  return (
    <input
      type="text"
      defaultValue={label}
      className="border w-full border-gray-300 rounded mt-2 mb-1 pl-1 pr-2 p-1"
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
      className={"p-1 w-fit mt-2 mb-2 pl-2 pr-2 bg-blue-500 rounded-md"}
      onClick={() => onClick()}
    >
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
