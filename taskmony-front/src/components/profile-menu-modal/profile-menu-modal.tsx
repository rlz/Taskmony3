import deleteI from "../../images/delete.svg";

type ModalPropsT = {
  close: Function;
};

export const ProfileMenuModal = ({ close }: ModalPropsT) => {
  return (
    <div className="w-full relative top-0 left-0 p-3 pb-2 bg-slate-50 rounded-lg drop-shadow-lg ">
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

export const Input = ({ label }) => {
  return (
    <input
      type="text"
      defaultValue={label}
      className="border w-full border-gray-300 rounded mt-2 mb-1 pl-1 pr-2"
    />
  );
};

export const Btn = ({ onClick, label }) => {
  return (
    <div
      className={"p-1 w-fit mt-2 mb-2 bg-blue-400 rounded-md"}
      onClick={() => onClick()}
    >
      <span className={"text-white"}>{label}</span>
    </div>
  );
};
