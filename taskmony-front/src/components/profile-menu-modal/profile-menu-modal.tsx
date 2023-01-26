import deleteI from "../../images/delete.svg";

type ModalPropsT = {
  close: Function;
};

export const ProfileMenuModal = ({ close }: ModalPropsT) => {
  return (
    <div className="w-1/4 absolute top-0 left-0 p-3 m-4 pb-2 bg-slate-50 rounded-lg drop-shadow-lg z-40">
      <img
        src={deleteI}
        className="cursor-pointer mr-0 ml-auto"
        onClick={(e) => close()}
      ></img>
      <Input label={"John Doe"} />
      <Input label={"johnd@gmail.com"} />
      <Btn label={"change password"} onClick={() => {}} />
      <Btn label={"sign out"} onClick={() => {}} />
    </div>
  );
};

type InputPropsT = {
    label:string
  };

export const Input = ({ label } : InputPropsT) => {
  return (
    <input
      type="text"
      defaultValue={label}
      className="border w-full border-gray-300 rounded mt-2 mb-1 pl-1 pr-2"
    />
  );
};

type BtnPropsT = {
    label:string,
    onClick: Function
  };

export const Btn = ({ onClick, label } : BtnPropsT) => {
  return (
    <div
      className={"p-1 w-fit mt-2 mb-2 bg-blue-400 rounded-md"}
      onClick={() => onClick()}
    >
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
